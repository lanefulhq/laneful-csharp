using Laneful;
using Laneful.Models;
using Laneful.Exceptions;

namespace Laneful.Examples;

/// <summary>
/// Error handling example based on CSharpSDK.tsx documentation
/// </summary>
public class ErrorHandlingExample
{
    public static async Task RunExample(string[] args)
    {
        Console.WriteLine("⚠️ Error Handling Example");
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

            // Create email with potential validation issues
            var email = new Email.Builder()
                .From(new Address(fromEmail, "Your Name"))
                .To(new Address(primaryToEmail, "Recipient Name"))
                .Subject("Error Handling Demo")
                .TextContent("This email demonstrates comprehensive error handling.")
                .Build();

            // Send email
            var response = await client.SendEmailAsync(email);
            Console.WriteLine("✅ Email sent successfully!");
            Console.WriteLine($"Response: {response}");
        }
        catch (ValidationException ex)
        {
            // Invalid input data
            Console.WriteLine($"❌ Validation error: {ex.Message}");
            Console.WriteLine("Please check your email configuration");
        }
        catch (ApiException ex)
        {
            // API returned an error
            Console.WriteLine($"❌ API error: {ex.Message}");
            Console.WriteLine($"  Status code: {ex.StatusCode}");
            Console.WriteLine($"  Error message: {ex.ErrorMessage}");
            Console.WriteLine("Please check your API credentials and endpoint");
        }
        catch (HttpException ex)
        {
            // Network or HTTP-level error
            Console.WriteLine($"❌ HTTP error: {ex.Message}");
            Console.WriteLine($"  Status code: {ex.StatusCode}");
            Console.WriteLine("Please check your network connection and endpoint URL");
        }
        catch (Exception ex)
        {
            // Other unexpected errors
            Console.WriteLine($"❌ Unexpected error: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }

        Console.WriteLine("\n📋 Error Handling Summary:");
        Console.WriteLine("• ValidationException - Input validation fails");
        Console.WriteLine("• ApiException - API returns error response (Properties: StatusCode, ErrorMessage)");
        Console.WriteLine("• HttpException - HTTP communication fails (Properties: StatusCode)");
        Console.WriteLine("• LanefulException - Base exception class");
        Console.WriteLine("\n💡 Best Practices:");
        Console.WriteLine("• Always wrap API calls in try-catch");
        Console.WriteLine("• Handle specific exception types first");
        Console.WriteLine("• Log errors with context information");
        Console.WriteLine("• Implement retry logic for transient failures");
    }
}
