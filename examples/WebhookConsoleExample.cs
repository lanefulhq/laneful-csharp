using System.Text.Json;
using Laneful.Webhooks;
using Laneful.Examples;

namespace Laneful.Examples;

/// <summary>
/// Console application demonstrating Laneful webhook handling
/// </summary>
public class WebhookConsoleExample
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("🚀 Laneful C# Webhook Handler Example");
        Console.WriteLine("=====================================\n");

        // Configuration - Replace with your actual webhook secret
        var webhookSecret = Environment.GetEnvironmentVariable("LANEFUL_WEBHOOK_SECRET") ?? "your-webhook-secret-here";
        
        var webhookHandler = new WebhookExample(webhookSecret);

        // Example 1: Test single event webhook
        Console.WriteLine("📝 Example 1: Single Event Webhook");
        Console.WriteLine("----------------------------------");

        var singleEventPayload = WebhookExample.GenerateTestPayload();
        var singleEventSignature = WebhookVerifier.GenerateSignature(webhookSecret, singleEventPayload, includePrefix: true);
        
        var singleEventHeaders = new Dictionary<string, string>
        {
            [WebhookVerifier.GetSignatureHeaderName()] = singleEventSignature
        };

        var singleResult = await webhookHandler.HandleWebhookAsync(singleEventPayload, singleEventHeaders);
        Console.WriteLine($"✅ Single event result: {singleResult.ToJsonResponse()}\n");

        // Example 2: Test batch event webhook
        Console.WriteLine("📦 Example 2: Batch Event Webhook");
        Console.WriteLine("---------------------------------");

        var batchEventPayload = WebhookExample.GenerateTestBatchPayload();
        var batchEventSignature = WebhookVerifier.GenerateSignature(webhookSecret, batchEventPayload, includePrefix: true);
        
        var batchEventHeaders = new Dictionary<string, string>
        {
            [WebhookVerifier.GetSignatureHeaderName()] = batchEventSignature
        };

        var batchResult = await webhookHandler.HandleWebhookAsync(batchEventPayload, batchEventHeaders);
        Console.WriteLine($"✅ Batch event result: {batchResult.ToJsonResponse()}\n");

        // Example 3: Test invalid signature
        Console.WriteLine("❌ Example 3: Invalid Signature Test");
        Console.WriteLine("------------------------------------");

        var invalidSignature = "invalid-signature";
        var invalidHeaders = new Dictionary<string, string>
        {
            [WebhookVerifier.GetSignatureHeaderName()] = invalidSignature
        };

        var invalidResult = await webhookHandler.HandleWebhookAsync(singleEventPayload, invalidHeaders);
        Console.WriteLine($"❌ Invalid signature result: {invalidResult.ToJsonResponse()}\n");

        // Example 4: Test missing header
        Console.WriteLine("❌ Example 4: Missing Header Test");
        Console.WriteLine("---------------------------------");

        var missingHeaderResult = await webhookHandler.HandleWebhookAsync(singleEventPayload, new Dictionary<string, string>());
        Console.WriteLine($"❌ Missing header result: {missingHeaderResult.ToJsonResponse()}\n");

        // Example 5: Test invalid payload
        Console.WriteLine("❌ Example 5: Invalid Payload Test");
        Console.WriteLine("----------------------------------");

        var invalidPayload = "invalid-json-payload";
        var invalidPayloadSignature = WebhookVerifier.GenerateSignature(webhookSecret, invalidPayload, includePrefix: true);
        
        var invalidPayloadHeaders = new Dictionary<string, string>
        {
            [WebhookVerifier.GetSignatureHeaderName()] = invalidPayloadSignature
        };

        var invalidPayloadResult = await webhookHandler.HandleWebhookAsync(invalidPayload, invalidPayloadHeaders);
        Console.WriteLine($"❌ Invalid payload result: {invalidPayloadResult.ToJsonResponse()}\n");

        // Example 6: Test different header formats
        Console.WriteLine("🔍 Example 6: Different Header Formats");
        Console.WriteLine("--------------------------------------");

        var testPayload = WebhookExample.GenerateTestPayload();
        var testSignature = WebhookVerifier.GenerateSignature(webhookSecret, testPayload, includePrefix: true);

        // Test uppercase header
        var upperHeaders = new Dictionary<string, string>
        {
            ["X_WEBHOOK_SIGNATURE"] = testSignature
        };

        var upperResult = await webhookHandler.HandleWebhookAsync(testPayload, upperHeaders);
        Console.WriteLine($"✅ Uppercase header result: {upperResult.ToJsonResponse()}");

        // Test HTTP_ prefix header
        var httpHeaders = new Dictionary<string, string>
        {
            ["HTTP_X_WEBHOOK_SIGNATURE"] = testSignature
        };

        var httpResult = await webhookHandler.HandleWebhookAsync(testPayload, httpHeaders);
        Console.WriteLine($"✅ HTTP_ prefix header result: {httpResult.ToJsonResponse()}\n");

        Console.WriteLine("🎉 All webhook examples completed!");
        Console.WriteLine("\n📋 Summary of C# webhook features:");
        Console.WriteLine("   • ✅ Signature verification with sha256= prefix support");
        Console.WriteLine("   • ✅ Batch and single event mode detection");
        Console.WriteLine("   • ✅ Payload structure validation");
        Console.WriteLine("   • ✅ All documented event types supported");
        Console.WriteLine("   • ✅ Header extraction with fallback formats");
        Console.WriteLine("   • ✅ Comprehensive error handling");
        Console.WriteLine("   • ✅ Modern C# async/await patterns");
        Console.WriteLine("   • ✅ Strong typing with nullable reference types");
        Console.WriteLine("\n💡 Configuration:");
        Console.WriteLine("   • Webhook secret: " + (webhookSecret == "your-webhook-secret-here" ? "Not configured (using default)" : "Configured"));
        Console.WriteLine("   • Header name: " + WebhookVerifier.GetSignatureHeaderName());
        Console.WriteLine("   • Supported events: delivery, open, click, bounce, drop, spam_complaint, unsubscribe");
        Console.WriteLine("\n💡 To use with real webhooks:");
        Console.WriteLine("   1. Set LANEFUL_WEBHOOK_SECRET environment variable");
        Console.WriteLine("   2. Implement your webhook endpoint using WebhookExample class");
        Console.WriteLine("   3. Handle the WebhookResult response appropriately");
    }
}
