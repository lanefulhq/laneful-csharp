using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Laneful.Webhooks;

/// <summary>
/// Utility class for verifying webhook signatures and processing webhook payloads.
/// </summary>
public static class WebhookVerifier
{
    private const string Algorithm = "HmacSHA256";
    private const string SignaturePrefix = "sha256=";
    private const string SignatureHeaderName = "x-webhook-signature";
    
    // Valid event types as documented
    private static readonly HashSet<string> ValidEventTypes = new()
    {
        "delivery", "open", "click", "drop", "spam_complaint", "unsubscribe", "bounce"
    };
    
    // UUID pattern for lane_id validation
    private static readonly Regex UuidPattern = new(
        @"^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );

    /// <summary>
    /// Verifies a webhook signature with support for sha256= prefix.
    /// </summary>
    /// <param name="secret">The webhook secret</param>
    /// <param name="payload">The webhook payload</param>
    /// <param name="signature">The signature to verify (may include 'sha256=' prefix)</param>
    /// <returns>true if the signature is valid, false otherwise</returns>
    public static bool VerifySignature(string secret, string payload, string signature)
    {
        if (string.IsNullOrWhiteSpace(secret))
            throw new ArgumentException("Secret cannot be empty", nameof(secret));

        if (payload == null)
            throw new ArgumentException("Payload cannot be null", nameof(payload));

        if (string.IsNullOrWhiteSpace(signature))
            return false;

        try
        {
            // Handle sha256= prefix as documented
            var cleanSignature = signature.StartsWith(SignaturePrefix, StringComparison.OrdinalIgnoreCase)
                ? signature[SignaturePrefix.Length..]
                : signature;

            var expectedSignature = GenerateSignature(secret, payload);
            return ConstantTimeEquals(expectedSignature, cleanSignature);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Generates a signature for the given payload.
    /// </summary>
    /// <param name="secret">The webhook secret</param>
    /// <param name="payload">The payload to sign</param>
    /// <param name="includePrefix">Whether to include the 'sha256=' prefix</param>
    /// <returns>The generated signature</returns>
    public static string GenerateSignature(string secret, string payload, bool includePrefix = false)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        var signature = Convert.ToHexString(hash).ToLowerInvariant();
        return includePrefix ? SignaturePrefix + signature : signature;
    }

    /// <summary>
    /// Parse and validate webhook payload structure.
    /// </summary>
    /// <param name="payload">The raw webhook payload JSON</param>
    /// <returns>WebhookData containing parsed events and batch mode flag</returns>
    /// <exception cref="ArgumentException">If payload is invalid JSON or structure</exception>
    public static WebhookData ParseWebhookPayload(string payload)
    {
        if (string.IsNullOrWhiteSpace(payload))
            throw new ArgumentException("Payload cannot be empty", nameof(payload));

        try
        {
            using var document = JsonDocument.Parse(payload);
            var root = document.RootElement;

            if (root.ValueKind == JsonValueKind.Array)
            {
                // Batch mode: array of events
                var events = new List<Dictionary<string, object?>>();
                foreach (var element in root.EnumerateArray())
                {
                    var eventDict = ValidateAndParseEvent(element);
                    events.Add(eventDict);
                }
                return new WebhookData(true, events);
            }
            else if (root.ValueKind == JsonValueKind.Object)
            {
                // Single event mode
                var eventDict = ValidateAndParseEvent(root);
                return new WebhookData(false, new List<Dictionary<string, object?>> { eventDict });
            }
            else
            {
                throw new ArgumentException("Invalid webhook payload structure");
            }
        }
        catch (JsonException ex)
        {
            throw new ArgumentException($"Invalid JSON payload: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Validate individual event structure according to documentation.
    /// </summary>
    /// <param name="eventElement">The event JSON element</param>
    /// <returns>Parsed event as Dictionary</returns>
    /// <exception cref="ArgumentException">If event structure is invalid</exception>
    private static Dictionary<string, object?> ValidateAndParseEvent(JsonElement eventElement)
    {
        if (eventElement.ValueKind != JsonValueKind.Object)
            throw new ArgumentException("Event must be an object");

        var eventDict = new Dictionary<string, object?>();

        // Required fields
        var requiredFields = new[] { "event", "email", "lane_id", "message_id", "timestamp" };
        foreach (var field in requiredFields)
        {
            if (!eventElement.TryGetProperty(field, out var property))
                throw new ArgumentException($"Missing required field: {field}");
            
            eventDict[field] = property.ValueKind switch
            {
                JsonValueKind.String => property.GetString(),
                JsonValueKind.Number => property.GetInt64(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                _ => property.GetRawText()
            };
        }

        // Validate event type
        var eventType = eventDict["event"]?.ToString();
        if (string.IsNullOrEmpty(eventType) || !ValidEventTypes.Contains(eventType))
            throw new ArgumentException($"Invalid event type: {eventType}");

        // Validate email format
        var email = eventDict["email"]?.ToString();
        if (string.IsNullOrEmpty(email) || !IsValidEmail(email))
            throw new ArgumentException($"Invalid email format: {email}");

        // Validate timestamp is numeric
        var timestamp = eventDict["timestamp"];
        if (timestamp is not long and not int)
            throw new ArgumentException("Invalid timestamp format");

        // Validate lane_id is a valid UUID format
        var laneId = eventDict["lane_id"]?.ToString();
        if (string.IsNullOrEmpty(laneId) || !UuidPattern.IsMatch(laneId))
            throw new ArgumentException($"Invalid lane_id format: {laneId}");

        // Add optional fields
        var optionalFields = new[]
        {
            "metadata", "tag", "url", "is_hard", "text", "reason", 
            "unsubscribe_group_id", "client_device", "client_os", "client_ip"
        };

        foreach (var field in optionalFields)
        {
            if (eventElement.TryGetProperty(field, out var property))
            {
                eventDict[field] = property.ValueKind switch
                {
                    JsonValueKind.String => property.GetString(),
                    JsonValueKind.Number => property.GetInt64(),
                    JsonValueKind.True => true,
                    JsonValueKind.False => false,
                    JsonValueKind.Object => JsonSerializer.Deserialize<Dictionary<string, object>>(property.GetRawText()),
                    JsonValueKind.Array => JsonSerializer.Deserialize<object[]>(property.GetRawText()),
                    _ => property.GetRawText()
                };
            }
        }

        return eventDict;
    }

    /// <summary>
    /// Simple email validation.
    /// </summary>
    /// <param name="email">The email to validate</param>
    /// <returns>true if email format is valid</returns>
    private static bool IsValidEmail(string email)
    {
        return !string.IsNullOrEmpty(email) && email.Contains('@') && email.Contains('.');
    }

    /// <summary>
    /// Get the webhook header name as documented.
    /// </summary>
    /// <returns>The correct header name for webhook signatures</returns>
    public static string GetSignatureHeaderName()
    {
        return SignatureHeaderName;
    }

    /// <summary>
    /// Extract webhook signature from HTTP headers (supports multiple formats).
    /// </summary>
    /// <param name="headers">HTTP headers dictionary</param>
    /// <returns>The webhook signature or null if not found</returns>
    public static string? ExtractSignatureFromHeaders(IDictionary<string, string> headers)
    {
        if (headers == null)
            return null;

        // Try documented header name first
        if (headers.TryGetValue(SignatureHeaderName, out var signature))
            return signature;

        // Try uppercase version
        var upperHeader = SignatureHeaderName.ToUpperInvariant().Replace("-", "_");
        if (headers.TryGetValue(upperHeader, out signature))
            return signature;

        // Try with HTTP_ prefix (common in web server environments)
        var serverHeader = "HTTP_" + upperHeader;
        if (headers.TryGetValue(serverHeader, out signature))
            return signature;

        return null;
    }

    /// <summary>
    /// Compares two strings in constant time to prevent timing attacks.
    /// </summary>
    /// <param name="a">First string</param>
    /// <param name="b">Second string</param>
    /// <returns>true if strings are equal, false otherwise</returns>
    private static bool ConstantTimeEquals(string a, string b)
    {
        if (a.Length != b.Length)
            return false;

        var result = 0;
        for (var i = 0; i < a.Length; i++)
        {
            result |= a[i] ^ b[i];
        }
        return result == 0;
    }
}

/// <summary>
/// Data class representing parsed webhook payload.
/// </summary>
public class WebhookData
{
    /// <summary>
    /// Whether this is a batch of events (array) or single event (object).
    /// </summary>
    public bool IsBatch { get; }

    /// <summary>
    /// List of webhook events.
    /// </summary>
    public IReadOnlyList<Dictionary<string, object?>> Events { get; }

    /// <summary>
    /// Initializes a new instance of the WebhookData class.
    /// </summary>
    /// <param name="isBatch">Whether this is a batch of events</param>
    /// <param name="events">List of webhook events</param>
    public WebhookData(bool isBatch, IReadOnlyList<Dictionary<string, object?>> events)
    {
        IsBatch = isBatch;
        Events = events;
    }

    /// <summary>
    /// Returns a string representation of the WebhookData.
    /// </summary>
    /// <returns>String representation</returns>
    public override string ToString()
    {
        return $"WebhookData{{IsBatch={IsBatch}, Events={Events.Count}}}";
    }
}

