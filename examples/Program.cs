using Laneful;
using Laneful.Models;
using Laneful.Exceptions;
using Laneful.Webhooks;
using Laneful.Examples;

// Example: Simple email sending with the Laneful C# SDK
Console.WriteLine("📧 Laneful C# SDK Example");
Console.WriteLine("==========================\n");

// Configuration - Replace with your actual Laneful API credentials
var baseUrl = "https://your-endpoint.send.laneful.net";
var authToken = "your-auth-token";

// Email addresses - Replace with your actual addresses
var senderEmail = "sender@yourdomain.com";
var senderName = "Your Name";
var recipientEmail = "recipient@example.com";
var recipientName = "Recipient Name";

try
{
    // Create client
    var client = new LanefulClient(baseUrl, authToken);
    Console.WriteLine("✅ Client created successfully");

    // Example 1: Simple text email
    Console.WriteLine("\n📝 Example 1: Sending Simple Text Email");
    Console.WriteLine("----------------------------------------");

    var simpleEmail = new Email.Builder()
        .From(new Address(senderEmail, senderName))
        .To(new Address(recipientEmail, recipientName))
        .Subject("Hello from Laneful .NET SDK!")
        .TextContent("This is a simple text email sent using the Laneful .NET SDK with modern C# features.")
        .Tag("simple-email")
        .Build();

    var response1 = await client.SendEmailAsync(simpleEmail);
    Console.WriteLine("✅ Simple email sent successfully!");
    Console.WriteLine($"Response: {response1}");

    // Example 2: HTML email with tracking
    Console.WriteLine("\n🎨 Example 2: Sending HTML Email with Tracking");
    Console.WriteLine("----------------------------------------------");

    var tracking = new TrackingSettings(opens: true, clicks: true, unsubscribes: false);

    var htmlEmail = new Email.Builder()
        .From(new Address(senderEmail, senderName))
        .To(new Address(recipientEmail, recipientName))
        .Subject("HTML Email with Tracking - C# SDK Demo")
        .HtmlContent("""
            <!DOCTYPE html>
            <html>
            <head>
                <title>.NET SDK Demo</title>
            </head>
            <body>
                <h1 style="color: #2c3e50;">Welcome to .NET 8!</h1>
                <p>This email was sent using the <strong>Laneful .NET SDK</strong> with modern C# features:</p>
                <ul>
                    <li>Records for immutable data classes</li>
                    <li>Pattern matching with switch expressions</li>
                    <li>Nullable reference types</li>
                    <li>Async/await support</li>
                </ul>
                <p style="color: #7f8c8d;">This email has tracking enabled for opens and clicks.</p>
            </body>
            </html>
            """)
        .TextContent("Welcome to .NET 8! This email was sent using the Laneful .NET SDK with modern C# features including records, pattern matching, nullable reference types, and async/await support. This email has tracking enabled for opens and clicks.")
        .Tracking(tracking)
        .Tag("html-tracking-demo")
        .Build();

    var response2 = await client.SendEmailAsync(htmlEmail);
    Console.WriteLine("✅ HTML email with tracking sent successfully!");
    Console.WriteLine($"Response: {response2}");

    // Example 3: Multiple recipients
    Console.WriteLine("\n👥 Example 3: Sending Email to Multiple Recipients");
    Console.WriteLine("------------------------------------------------");

    var multiEmail = new Email.Builder()
        .From(new Address(senderEmail, senderName))
        .To(new Address(recipientEmail, recipientName))
        .To(new Address("user2@example.com", "User Two"))
        .To(new Address("user3@example.com", "User Three"))
        .Subject("Email to Multiple Recipients - C# SDK Demo")
        .TextContent("This email demonstrates sending to multiple recipients using the Laneful C# SDK.")
        .ReplyTo(new Address("reply@yourdomain.com", "Support Team"))
        .Tag("multi-recipient-demo")
        .Build();

    var response3 = await client.SendEmailAsync(multiEmail);
    Console.WriteLine("✅ Multi-recipient email sent successfully!");
    Console.WriteLine($"Response: {response3}");

    // Example 4: Scheduled email
    Console.WriteLine("\n⏰ Example 4: Scheduling Email for Future Delivery");
    Console.WriteLine("-----------------------------------------------");

    // Schedule email for 2 minutes from now
    var sendTime = DateTimeOffset.UtcNow.AddMinutes(2).ToUnixTimeSeconds();

    var scheduledEmail = new Email.Builder()
        .From(new Address(senderEmail, senderName))
        .To(new Address(recipientEmail, recipientName))
        .Subject("Scheduled Email - C# SDK Demo")
        .TextContent("This email was scheduled to be sent at a specific time using the Laneful C# SDK.")
        .SendTime(sendTime)
        .Tag("scheduled-demo")
        .Build();

    var response4 = await client.SendEmailAsync(scheduledEmail);
    Console.WriteLine("✅ Email scheduled successfully!");
    Console.WriteLine($"Response: {response4}");

    // Example 5: Multiple emails in batch
    Console.WriteLine("\n📦 Example 5: Sending Multiple Emails in Batch");
    Console.WriteLine("---------------------------------------------");

    var batchEmails = new[]
    {
        new Email.Builder()
            .From(new Address(senderEmail, senderName))
            .To(new Address(recipientEmail, recipientName))
            .Subject("Batch Email 1 - C# SDK Demo")
            .TextContent("First email in the batch sent using .NET 9 features.")
            .Tag("batch-1")
            .Build(),
        new Email.Builder()
            .From(new Address(senderEmail, senderName))
            .To(new Address("user2@example.com", "User Two"))
            .Subject("Batch Email 2 - C# SDK Demo")
            .TextContent("Second email in the batch sent using .NET 9 features.")
            .Tag("batch-2")
            .Build(),
        new Email.Builder()
            .From(new Address(senderEmail, senderName))
            .To(new Address("user3@example.com", "User Three"))
            .Subject("Batch Email 3 - C# SDK Demo")
            .TextContent("Third email in the batch sent using .NET 9 features.")
            .Tag("batch-3")
            .Build()
    };

    var response5 = await client.SendEmailsAsync(batchEmails);
    Console.WriteLine("✅ Batch emails sent successfully!");
    Console.WriteLine($"Response: {response5}");

    // Example 6: Webhook verification demonstration
    Console.WriteLine("\n🔗 Example 6: Webhook Verification Demo");
    Console.WriteLine("-------------------------------------");

    var webhookSecret = "test-webhook-secret";
    var testPayload = WebhookExample.GenerateTestPayload();
    var testSignature = WebhookVerifier.GenerateSignature(webhookSecret, testPayload, includePrefix: true);
    
    Console.WriteLine($"✅ Generated test webhook signature: {testSignature}");
    Console.WriteLine($"✅ Test payload: {testPayload}");
    
    // Verify the signature
    var isValid = WebhookVerifier.VerifySignature(webhookSecret, testPayload, testSignature);
    Console.WriteLine($"✅ Signature verification: {(isValid ? "VALID" : "INVALID")}");
    
    // Parse the payload
    var webhookData = WebhookVerifier.ParseWebhookPayload(testPayload);
    Console.WriteLine($"✅ Parsed webhook data: {webhookData}");
    Console.WriteLine($"✅ Event type: {webhookData.Events[0]["event"]}");
    Console.WriteLine($"✅ Email: {webhookData.Events[0]["email"]}");

    Console.WriteLine("\n🎉 All examples completed successfully!");
    Console.WriteLine("\n📋 Summary of .NET 8 features used:");
    Console.WriteLine("   • Records for Address and TrackingSettings");
    Console.WriteLine("   • Pattern matching in exception handling");
    Console.WriteLine("   • Nullable reference types");
    Console.WriteLine("   • Async/await support");
    Console.WriteLine("   • Modern C# syntax throughout");
    Console.WriteLine("   • Webhook signature verification");
    Console.WriteLine("   • JSON payload parsing and validation");
    Console.WriteLine("\n💡 Configuration:");
    Console.WriteLine("   • API endpoint: " + baseUrl);
    Console.WriteLine("   • Sender: " + senderEmail);
    Console.WriteLine("   • Recipient: " + recipientEmail);
    Console.WriteLine("   • Webhook secret: " + webhookSecret);
    Console.WriteLine("\n💡 To send real emails:");
    Console.WriteLine("   1. Replace example values with your actual Laneful API credentials");
    Console.WriteLine("   2. Update email addresses with real recipients");
    Console.WriteLine("   3. Run this example again");
    Console.WriteLine("\n💡 Webhook handling:");
    Console.WriteLine("   • See WebhookExample.cs for complete webhook handling");
    Console.WriteLine("   • See WebhookConsoleExample.cs for console demo");
    Console.WriteLine("   • See WebhookControllerExample.cs for ASP.NET Core integration");

}
catch (ValidationException ex)
{
    Console.WriteLine($"❌ Validation error: {ex.Message}");
}
catch (ApiException ex)
{
    Console.WriteLine($"❌ API error: {ex.Message}");
    Console.WriteLine($"Status code: {ex.StatusCode}");
    Console.WriteLine($"Error message: {ex.ErrorMessage}");
}
catch (HttpException ex)
{
    Console.WriteLine($"❌ HTTP error: {ex.Message}");
    Console.WriteLine($"Status code: {ex.StatusCode}");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Unexpected error: {ex.Message}");
}
