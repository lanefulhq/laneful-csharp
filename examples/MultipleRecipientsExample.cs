using Laneful;
using Laneful.Models;
using Laneful.Exceptions;

namespace Laneful.Examples;

/// <summary>
/// Multiple recipients with reply-to example based on CSharpSDK.tsx documentation
/// </summary>
public class MultipleRecipientsExample
{
    public static async Task RunExample(string[] args)
    {
        Console.WriteLine("👥 Multiple Recipients Example");
        Console.WriteLine("==============================\n");

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

        try
        {
            // Create client
            var client = new LanefulClient(baseUrl, authToken);
            Console.WriteLine("✅ Client created successfully");

            var email = new Email.Builder()
                .From(new Address(fromEmail, "Your Name"))
                .To(new Address(toEmailList[0], "User One"))
                .To(new Address(toEmailList.Count > 1 ? toEmailList[1] : "user2@example.com", "User Two"))
                .Cc(new Address("cc@example.com", "CC Recipient"))
                .Bcc(new Address("bcc@example.com", "BCC Recipient"))
                .ReplyTo(new Address("reply@example.com", "Reply To"))
                .Subject("Email to Multiple Recipients")
                .TextContent("This email is being sent to multiple recipients.")
                .Build();

            var response = await client.SendEmailAsync(email);
            Console.WriteLine("✅ Multiple recipients email sent successfully!");
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
