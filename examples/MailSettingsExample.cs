using Laneful;
using Laneful.Models;
using Laneful.Exceptions;

namespace Laneful.Examples;

/// <summary>
/// Email with a visible From header and request-level mail settings.
/// </summary>
public class MailSettingsExample
{
    public static async Task RunExample(string[] args)
    {
        Console.WriteLine("🧪 Mail Settings Example");
        Console.WriteLine("========================\n");

        var baseUrl = Environment.GetEnvironmentVariable("LANEFUL_BASE_URL")
            ?? throw new InvalidOperationException("LANEFUL_BASE_URL environment variable is required");
        var authToken = Environment.GetEnvironmentVariable("LANEFUL_AUTH_TOKEN")
            ?? throw new InvalidOperationException("LANEFUL_AUTH_TOKEN environment variable is required");
        var fromEmail = Environment.GetEnvironmentVariable("LANEFUL_FROM_EMAIL")
            ?? throw new InvalidOperationException("LANEFUL_FROM_EMAIL environment variable is required");
        var toEmails = Environment.GetEnvironmentVariable("LANEFUL_TO_EMAILS")
            ?? throw new InvalidOperationException("LANEFUL_TO_EMAILS environment variable is required");

        var primaryToEmail = toEmails.Split(',').Select(email => email.Trim()).First();

        try
        {
            var client = new LanefulClient(baseUrl, authToken);

            var email = new Email.Builder()
                .From(new Address(fromEmail, "Your Name"))
                .To(new Address(primaryToEmail, "Recipient Name"))
                .Subject("Sandbox email")
                .TextContent("This email is sent with sandbox mode and returns message IDs.")
                .FromHeader(new Address(fromEmail, "Newsletter"))
                .Tracking(new TrackingSettings(opens: true, clicks: true, unsubscribeGroupName: "Newsletters"))
                .Build();

            var response = await client.SendEmailAsync(
                email,
                new MailSettings(sandboxMode: true, returnMessageIds: true));

            Console.WriteLine("✅ Email sent successfully!");
            Console.WriteLine($"Response: {response}");
        }
        catch (ValidationException ex)
        {
            Console.WriteLine($"❌ Validation error: {ex.Message}");
        }
        catch (ApiException ex)
        {
            Console.WriteLine($"❌ API error: {ex.Message}");
        }
        catch (HttpException ex)
        {
            Console.WriteLine($"❌ HTTP error: {ex.Message}");
        }
    }
}
