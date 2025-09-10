using Laneful;
using Laneful.Models;
using Laneful.Exceptions;

namespace Laneful.Examples;

/// <summary>
/// Scheduled email example based on CSharpSDK.tsx documentation
/// </summary>
public class ScheduledEmailExample
{
    public static async Task RunExample(string[] args)
    {
        Console.WriteLine("⏰ Scheduled Email Example");
        Console.WriteLine("==========================\n");

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

            // Schedule for 24 hours from now
            var sendTime = DateTimeOffset.UtcNow.AddHours(24).ToUnixTimeSeconds();

            var email = new Email.Builder()
                .From(new Address(fromEmail))
                .To(new Address(primaryToEmail))
                .Subject("Scheduled Email")
                .TextContent("This email was scheduled.")
                .SendTime(sendTime)
                .Build();

            var response = await client.SendEmailAsync(email);
            Console.WriteLine("✅ Email scheduled successfully!");
            Console.WriteLine($"Response: {response}");
            Console.WriteLine($"Scheduled for: {DateTimeOffset.FromUnixTimeSeconds(sendTime):yyyy-MM-dd HH:mm:ss} UTC");
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
