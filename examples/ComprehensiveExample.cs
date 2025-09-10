using Laneful;
using Laneful.Models;
using Laneful.Exceptions;

namespace Laneful.Examples;

/// <summary>
/// Comprehensive example combining multiple features
/// </summary>
public class ComprehensiveExample
{
    public static async Task RunExample(string[] args)
    {
        Console.WriteLine("🚀 Comprehensive Laneful C# SDK Example");
        Console.WriteLine("========================================\n");

        // Get configuration from environment variables
        var baseUrl = Environment.GetEnvironmentVariable("LANEFUL_BASE_URL") 
            ?? throw new InvalidOperationException("LANEFUL_BASE_URL environment variable is required");
        var authToken = Environment.GetEnvironmentVariable("LANEFUL_AUTH_TOKEN") 
            ?? throw new InvalidOperationException("LANEFUL_AUTH_TOKEN environment variable is required");
        var fromEmail = Environment.GetEnvironmentVariable("LANEFUL_FROM_EMAIL") 
            ?? throw new InvalidOperationException("LANEFUL_FROM_EMAIL environment variable is required");
        var toEmails = Environment.GetEnvironmentVariable("LANEFUL_TO_EMAILS") 
            ?? throw new InvalidOperationException("LANEFUL_TO_EMAILS environment variable is required");

        var toEmailList = toEmails.Split(',').Select(email => email.Trim()).ToList();
        var primaryToEmail = toEmailList.First();

        try
        {
            // Create client
            var client = new LanefulClient(baseUrl, authToken);
            Console.WriteLine("✅ Client created successfully");

            // Example 1: HTML email with tracking and attachments
            Console.WriteLine("\n📧 Example 1: HTML Email with Tracking and Attachments");
            Console.WriteLine("------------------------------------------------------");

            var tracking = new TrackingSettings(opens: true, clicks: true, unsubscribes: false);
            var attachment = new Attachment(
                "welcome-guide.pdf",
                "application/pdf",
                "JVBERi0xLjQKJcOkw7zDtsO8CjIgMCBvYmoKPDwKL0xlbmd0aCAzIDAgUgovVHlwZSAvQ29udGVudAovRmlsdGVyIC9GbGF0ZURlY29kZQo+PgpzdHJlYW0K" // Base64 PDF content
            );

            var comprehensiveEmail = new Email.Builder()
                .From(new Address(fromEmail, "Laneful Team"))
                .To(new Address(primaryToEmail, "Valued Customer"))
                .Subject("Welcome to Laneful - Complete Feature Demo")
                .HtmlContent("""
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <title>Welcome to Laneful</title>
                    </head>
                    <body style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;">
                        <h1 style="color: #2c3e50;">Welcome to Laneful!</h1>
                        <p>Thank you for choosing our email service. This email demonstrates:</p>
                        <ul>
                            <li>HTML content with styling</li>
                            <li>Click tracking (try clicking the link below)</li>
                            <li>File attachments</li>
                            <li>Custom headers and tags</li>
                        </ul>
                        <p><a href="https://laneful.com" style="color: #3498db;">Visit our website</a> to learn more.</p>
                        <p>Best regards,<br>The Laneful Team</p>
                    </body>
                    </html>
                    """)
                .TextContent("Welcome to Laneful! Thank you for choosing our email service. This email demonstrates HTML content, click tracking, file attachments, and custom headers. Visit https://laneful.com to learn more. Best regards, The Laneful Team")
                .Tracking(tracking)
                .Attachment(attachment)
                .Tag("welcome-comprehensive")
                .Headers(new Dictionary<string, string>
                {
                    ["X-Custom-Header"] = "Comprehensive Example",
                    ["X-Priority"] = "1"
                })
                .Build();

            var response1 = await client.SendEmailAsync(comprehensiveEmail);
            Console.WriteLine("✅ Comprehensive email sent successfully!");
            Console.WriteLine($"Response: {response1}");

            // Example 2: Multiple recipients with different content
            Console.WriteLine("\n👥 Example 2: Multiple Recipients with Reply-To");
            Console.WriteLine("-----------------------------------------------");

            var multiRecipientEmail = new Email.Builder()
                .From(new Address(fromEmail, "Support Team"))
                .To(new Address(toEmailList[0], "Primary User"))
                .To(new Address(toEmailList.Count > 1 ? toEmailList[1] : "user2@example.com", "Secondary User"))
                .Cc(new Address("manager@example.com", "Manager"))
                .ReplyTo(new Address("support@example.com", "Support Team"))
                .Subject("Team Update - Multiple Recipients")
                .TextContent("This is a team update sent to multiple recipients with CC and reply-to functionality.")
                .Tag("team-update")
                .Build();

            var response2 = await client.SendEmailAsync(multiRecipientEmail);
            Console.WriteLine("✅ Multi-recipient email sent successfully!");
            Console.WriteLine($"Response: {response2}");

            // Example 3: Batch sending with different templates
            Console.WriteLine("\n📦 Example 3: Batch Email Sending");
            Console.WriteLine("----------------------------------");

            var batchEmails = new[]
            {
                new Email.Builder()
                    .From(new Address(fromEmail, "Marketing Team"))
                    .To(new Address(toEmailList[0], "Customer 1"))
                    .Subject("Special Offer - Customer 1")
                    .TextContent("Dear Customer 1, here's your special offer!")
                    .Tag("batch-offer-1")
                    .Build(),
                new Email.Builder()
                    .From(new Address(fromEmail, "Marketing Team"))
                    .To(new Address(toEmailList.Count > 1 ? toEmailList[1] : "customer2@example.com", "Customer 2"))
                    .Subject("Special Offer - Customer 2")
                    .TextContent("Dear Customer 2, here's your special offer!")
                    .Tag("batch-offer-2")
                    .Build()
            };

            var response3 = await client.SendEmailsAsync(batchEmails);
            Console.WriteLine("✅ Batch emails sent successfully!");
            Console.WriteLine($"Response: {response3}");

            Console.WriteLine("\n🎉 All comprehensive examples completed successfully!");
            Console.WriteLine("\n📋 Features Demonstrated:");
            Console.WriteLine("• HTML content with CSS styling");
            Console.WriteLine("• Click and open tracking");
            Console.WriteLine("• File attachments (base64 encoded)");
            Console.WriteLine("• Multiple recipients (TO, CC)");
            Console.WriteLine("• Reply-to functionality");
            Console.WriteLine("• Custom headers");
            Console.WriteLine("• Email tagging");
            Console.WriteLine("• Batch email sending");
            Console.WriteLine("• Comprehensive error handling");
        }
        catch (ValidationException ex)
        {
            Console.WriteLine($"❌ Validation error: {ex.Message}");
        }
        catch (ApiException ex)
        {
            Console.WriteLine($"❌ API error: {ex.Message}");
            Console.WriteLine($"Status code: {ex.StatusCode}");
        }
        catch (HttpException ex)
        {
            Console.WriteLine($"❌ HTTP error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Unexpected error: {ex.Message}");
        }
    }
}
