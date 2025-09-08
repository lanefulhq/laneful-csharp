using System.Text.Json;
using Laneful.Webhooks;

namespace Laneful.Examples;

/// <summary>
/// Laneful Webhook Handler Example
/// 
/// This example demonstrates how to properly handle Laneful webhooks
/// according to the documentation in Webhooks.tsx
/// </summary>
public class WebhookExample
{
    private readonly string _webhookSecret;

    public WebhookExample(string webhookSecret)
    {
        _webhookSecret = webhookSecret ?? throw new ArgumentNullException(nameof(webhookSecret));
    }

    /// <summary>
    /// Complete webhook verification and processing workflow
    /// following the documentation exactly
    /// </summary>
    /// <param name="payload">The raw webhook payload</param>
    /// <param name="headers">HTTP headers dictionary</param>
    /// <returns>Processing result</returns>
    public async Task<WebhookResult> HandleWebhookAsync(string payload, IDictionary<string, string> headers)
    {
        try
        {
            // Step 1: Validate payload
            if (string.IsNullOrWhiteSpace(payload))
                throw new InvalidOperationException("Empty payload received");

            // Step 2: Extract signature from headers (as documented)
            var signature = WebhookVerifier.ExtractSignatureFromHeaders(headers);
            if (string.IsNullOrEmpty(signature))
                throw new InvalidOperationException("Missing webhook signature header");

            // Step 3: Verify signature (supports sha256= prefix as documented)
            if (!WebhookVerifier.VerifySignature(_webhookSecret, payload, signature))
                throw new InvalidOperationException("Invalid webhook signature");

            // Step 4: Parse and validate payload structure
            var webhookData = WebhookVerifier.ParseWebhookPayload(payload);

            // Step 5: Process events (handles both batch and single event formats)
            var processedCount = 0;
            foreach (var eventData in webhookData.Events)
            {
                await ProcessWebhookEventAsync(eventData);
                processedCount++;
            }

            // Log successful processing
            Console.WriteLine($"Successfully processed {processedCount} webhook event(s) in {(webhookData.IsBatch ? "batch" : "single")} mode");

            return new WebhookResult
            {
                Success = true,
                ProcessedCount = processedCount,
                IsBatch = webhookData.IsBatch
            };
        }
        catch (ArgumentException ex)
        {
            // Payload validation error
            Console.WriteLine($"Webhook payload validation error: {ex.Message}");
            return new WebhookResult
            {
                Success = false,
                Error = $"Invalid payload: {ex.Message}",
                StatusCode = 400
            };
        }
        catch (Exception ex)
        {
            // Other errors (signature, missing data, etc.)
            Console.WriteLine($"Webhook processing error: {ex.Message}");
            return new WebhookResult
            {
                Success = false,
                Error = ex.Message,
                StatusCode = 401
            };
        }
    }

    /// <summary>
    /// Process individual webhook events according to documentation
    /// </summary>
    /// <param name="eventData">The webhook event data</param>
    private async Task ProcessWebhookEventAsync(Dictionary<string, object?> eventData)
    {
        var eventType = eventData["event"]?.ToString() ?? "unknown";
        var email = eventData["email"]?.ToString() ?? "unknown";
        var messageId = eventData["message_id"]?.ToString() ?? "unknown";
        var timestamp = eventData["timestamp"];

        // Log basic event info
        Console.WriteLine($"Processing {eventType} event for {email} (Message ID: {messageId})");

        // Process based on event type (all types from documentation)
        switch (eventType)
        {
            case "delivery":
                await HandleDeliveryEventAsync(eventData);
                break;

            case "open":
                await HandleOpenEventAsync(eventData);
                break;

            case "click":
                await HandleClickEventAsync(eventData);
                break;

            case "bounce":
                await HandleBounceEventAsync(eventData);
                break;

            case "drop":
                await HandleDropEventAsync(eventData);
                break;

            case "spam_complaint":
                await HandleSpamComplaintEventAsync(eventData);
                break;

            case "unsubscribe":
                await HandleUnsubscribeEventAsync(eventData);
                break;

            default:
                Console.WriteLine($"Unknown event type: {eventType}");
                break;
        }
    }

    /// <summary>
    /// Handle delivery events
    /// </summary>
    private async Task HandleDeliveryEventAsync(Dictionary<string, object?> eventData)
    {
        var email = eventData["email"]?.ToString() ?? "unknown";
        
        // Update delivery status in your database
        // Example: await MarkEmailAsDeliveredAsync(eventData["message_id"]?.ToString(), eventData["timestamp"]);
        
        Console.WriteLine($"Email delivered successfully to {email}");
        await Task.CompletedTask; // Placeholder for async operations
    }

    /// <summary>
    /// Handle open events
    /// </summary>
    private async Task HandleOpenEventAsync(Dictionary<string, object?> eventData)
    {
        var email = eventData["email"]?.ToString() ?? "unknown";
        
        // Track email opens
        var clientInfo = new
        {
            Device = eventData["client_device"]?.ToString() ?? "Unknown",
            OS = eventData["client_os"]?.ToString() ?? "Unknown",
            IP = eventData["client_ip"]?.ToString() ?? "Unknown"
        };

        Console.WriteLine($"Email opened by {email} on {clientInfo.Device} ({clientInfo.OS})");

        // Example: await TrackEmailOpenAsync(eventData["message_id"]?.ToString(), clientInfo, eventData["timestamp"]);
        await Task.CompletedTask; // Placeholder for async operations
    }

    /// <summary>
    /// Handle click events
    /// </summary>
    private async Task HandleClickEventAsync(Dictionary<string, object?> eventData)
    {
        var email = eventData["email"]?.ToString() ?? "unknown";
        var url = eventData["url"]?.ToString() ?? "Unknown URL";

        Console.WriteLine($"Link clicked in email to {email}: {url}");

        // Example: await TrackLinkClickAsync(eventData["message_id"]?.ToString(), url, eventData["timestamp"]);
        await Task.CompletedTask; // Placeholder for async operations
    }

    /// <summary>
    /// Handle bounce events
    /// </summary>
    private async Task HandleBounceEventAsync(Dictionary<string, object?> eventData)
    {
        var email = eventData["email"]?.ToString() ?? "unknown";
        var isHard = eventData["is_hard"] as bool? ?? false;
        var bounceType = isHard ? "hard" : "soft";
        var reason = eventData["text"]?.ToString() ?? "Unknown reason";

        Console.WriteLine($"Email bounced ({bounceType}) for {email}: {reason}");

        // Handle hard bounces by suppressing the email
        if (isHard)
        {
            // Example: await SuppressEmailAsync(email, "hard_bounce");
        }

        await Task.CompletedTask; // Placeholder for async operations
    }

    /// <summary>
    /// Handle drop events
    /// </summary>
    private async Task HandleDropEventAsync(Dictionary<string, object?> eventData)
    {
        var email = eventData["email"]?.ToString() ?? "unknown";
        var reason = eventData["reason"]?.ToString() ?? "Unknown reason";

        Console.WriteLine($"Email dropped for {email}: {reason}");

        // Example: await HandleEmailDropAsync(eventData["message_id"]?.ToString(), reason);
        await Task.CompletedTask; // Placeholder for async operations
    }

    /// <summary>
    /// Handle spam complaint events
    /// </summary>
    private async Task HandleSpamComplaintEventAsync(Dictionary<string, object?> eventData)
    {
        var email = eventData["email"]?.ToString() ?? "unknown";

        Console.WriteLine($"Spam complaint received for {email}");

        // Automatically unsubscribe users who mark emails as spam
        // Example: await UnsubscribeEmailAsync(email, "spam_complaint");
        await Task.CompletedTask; // Placeholder for async operations
    }

    /// <summary>
    /// Handle unsubscribe events
    /// </summary>
    private async Task HandleUnsubscribeEventAsync(Dictionary<string, object?> eventData)
    {
        var email = eventData["email"]?.ToString() ?? "unknown";
        var groupId = eventData["unsubscribe_group_id"]?.ToString();

        Console.WriteLine($"Unsubscribe event for {email}" + (groupId != null ? $" (Group: {groupId})" : ""));

        // Example: await ProcessUnsubscribeAsync(email, groupId);
        await Task.CompletedTask; // Placeholder for async operations
    }

    /// <summary>
    /// Generate a test webhook payload for demonstration
    /// </summary>
    /// <returns>Test webhook payload JSON</returns>
    public static string GenerateTestPayload()
    {
        var testEvent = new
        {
            @event = "delivery",
            email = "test@example.com",
            lane_id = "5805dd85-ed8c-44db-91a7-1d53a41c86a5",
            message_id = "test-message-id",
            timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            tag = "test-webhook"
        };

        return JsonSerializer.Serialize(testEvent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        });
    }

    /// <summary>
    /// Generate a test batch webhook payload for demonstration
    /// </summary>
    /// <returns>Test batch webhook payload JSON</returns>
    public static string GenerateTestBatchPayload()
    {
        var testEvents = new[]
        {
            new
            {
                @event = "delivery",
                email = "user1@example.com",
                lane_id = "5805dd85-ed8c-44db-91a7-1d53a41c86a5",
                message_id = "test-message-1",
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                tag = "batch-test-1"
            },
            new
            {
                @event = "open",
                email = "user2@example.com",
                lane_id = "5805dd85-ed8c-44db-91a7-1d53a41c86a5",
                message_id = "test-message-2",
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                client_device = "Desktop",
                client_os = "Windows",
                client_ip = "192.168.1.1",
                tag = "batch-test-2"
            }
        };

        return JsonSerializer.Serialize(testEvents, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        });
    }
}

/// <summary>
/// Result of webhook processing
/// </summary>
public class WebhookResult
{
    /// <summary>
    /// Whether the webhook was processed successfully
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Number of events processed
    /// </summary>
    public int ProcessedCount { get; set; }

    /// <summary>
    /// Whether the payload was in batch format
    /// </summary>
    public bool IsBatch { get; set; }

    /// <summary>
    /// Error message if processing failed
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// HTTP status code for the response
    /// </summary>
    public int StatusCode { get; set; } = 200;

    /// <summary>
    /// Returns a JSON response for the webhook endpoint
    /// </summary>
    /// <returns>JSON response string</returns>
    public string ToJsonResponse()
    {
        var response = new
        {
            status = Success ? "success" : "error",
            processed = ProcessedCount,
            mode = IsBatch ? "batch" : "single",
            error = Error
        };

        return JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        });
    }
}
