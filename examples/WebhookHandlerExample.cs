using System.Text.Json;
using Laneful.Webhooks;
using Laneful.Exceptions;

namespace Laneful.Examples;

/// <summary>
/// Webhook handler example based on CSharpSDK.tsx documentation
/// </summary>
public class WebhookHandlerExample
{
    public static async Task RunExample(string[] args)
    {
        Console.WriteLine("🔗 Webhook Handler Example");
        Console.WriteLine("===========================\n");

        // Get configuration from environment variables
        var webhookSecret = Environment.GetEnvironmentVariable("LANEFUL_WEBHOOK_SECRET") 
            ?? throw new InvalidOperationException("LANEFUL_WEBHOOK_SECRET environment variable is required");

        try
        {
            // Generate sample payload for testing
            var samplePayload = JsonSerializer.Serialize(new[]
            {
                new
                {
                    @event = "delivery",
                    email = "user@example.com",
                    timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    lane_id = Guid.NewGuid().ToString()
                }
            });

            // Generate valid signature
            var validSignature = WebhookVerifier.GenerateSignature(webhookSecret, samplePayload, includePrefix: true);
            
            Console.WriteLine("✅ Generated test webhook payload and signature");
            Console.WriteLine($"Payload: {samplePayload}");
            Console.WriteLine($"Signature: {validSignature}");

            // Simulate webhook processing
            Console.WriteLine("\n🔍 Processing webhook...");

            // Convert headers to dictionary (simulating HTTP request headers)
            var headers = new Dictionary<string, string>
            {
                ["x-webhook-signature"] = validSignature
            };

            // Extract signature from headers
            var signature = WebhookVerifier.ExtractSignatureFromHeaders(headers);
            if (string.IsNullOrEmpty(signature))
            {
                Console.WriteLine("❌ Missing signature in headers");
                return;
            }

            // Verify signature
            if (!WebhookVerifier.VerifySignature(webhookSecret, samplePayload, signature))
            {
                Console.WriteLine("❌ Invalid signature");
                return;
            }

            Console.WriteLine("✅ Signature verified successfully");

            // Parse and validate payload
            var webhookData = WebhookVerifier.ParseWebhookPayload(samplePayload);

            // Process events
            foreach (var eventData in webhookData.Events)
            {
                var eventType = eventData["event"]?.ToString();
                var email = eventData["email"]?.ToString();

                switch (eventType)
                {
                    case "delivery":
                        Console.WriteLine($"✅ Email delivered to: {email}");
                        break;
                    case "open":
                        Console.WriteLine($"✅ Email opened by: {email}");
                        break;
                    case "click":
                        var url = eventData["url"]?.ToString() ?? "Unknown URL";
                        Console.WriteLine($"✅ Link clicked by {email}: {url}");
                        break;
                    case "bounce":
                        var isHard = eventData["is_hard"] as bool? ?? false;
                        Console.WriteLine($"✅ Email bounced ({(isHard ? "hard" : "soft")} type) for: {email}");
                        break;
                    default:
                        Console.WriteLine($"✅ Unknown event type: {eventType} for: {email}");
                        break;
                }
            }

            Console.WriteLine($"\n✅ Webhook processed successfully!");
            Console.WriteLine($"Processed {webhookData.Events.Count} events");
            Console.WriteLine($"Mode: {(webhookData.IsBatch ? "batch" : "single")}");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"❌ Invalid payload: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error processing webhook: {ex.Message}");
        }
    }
}
