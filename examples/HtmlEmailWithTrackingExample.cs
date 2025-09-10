using Laneful;
using Laneful.Models;
using Laneful.Exceptions;

namespace Laneful.Examples;

/// <summary>
/// HTML email with tracking example based on CSharpSDK.tsx documentation
/// </summary>
public class HtmlEmailWithTrackingExample
{
    public static async Task RunExample(string[] args)
    {
        Console.WriteLine("🎨 HTML Email with Tracking Example");
        Console.WriteLine("===================================\n");

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

            // Create tracking settings
            var tracking = new TrackingSettings(opens: true, clicks: true, unsubscribes: true);

            var email = new Email.Builder()
                .From(new Address(fromEmail, "Your Name"))
                .To(new Address(primaryToEmail, "Recipient Name"))
                .Subject("HTML Email with Tracking")
                .HtmlContent("<h1>Welcome!</h1><p>This is an <strong>HTML email</strong> with tracking enabled.</p><p><a href=\"https://example.com\">Click here</a> to test click tracking.</p>")
                .TextContent("Welcome! This is an HTML email with tracking enabled. Visit https://example.com to test click tracking.")
                .Tracking(tracking)
                .Tag("welcome-email")
                .Build();

            var response = await client.SendEmailAsync(email);
            Console.WriteLine("✅ HTML email with tracking sent successfully!");
            Console.WriteLine($"Response: {response}");
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
