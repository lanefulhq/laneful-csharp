using Laneful;
using Laneful.Models;
using Laneful.Exceptions;

namespace Laneful.Examples;

/// <summary>
/// Email with attachments example based on CSharpSDK.tsx documentation
/// </summary>
public class AttachmentEmailExample
{
    public static async Task RunExample(string[] args)
    {
        Console.WriteLine("📎 Email with Attachments Example");
        Console.WriteLine("==================================\n");

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

            // Create attachment from base64 content (useful for testing)
            var attachment = new Attachment(
                "test-document.txt",
                "text/plain",
                "VGhpcyBpcyBhIHRlc3QgZG9jdW1lbnQgYXR0YWNobWVudC4=" // Base64 encoded content
            );

            // Alternative: Create attachment from file (uncomment to use)
            // var attachment = Attachment.FromFile("/path/to/document.pdf");

            var email = new Email.Builder()
                .From(new Address(fromEmail, "Your Name"))
                .To(new Address(primaryToEmail, "Recipient Name"))
                .Subject("Document Attached")
                .TextContent("Please find the document attached.")
                .Attachment(attachment)
                .Build();

            // Debug: Print the email object
            Console.WriteLine($"Email object: {email}");
            Console.WriteLine($"Attachment: {attachment}");

            var response = await client.SendEmailAsync(email);
            Console.WriteLine("✅ Email with attachment sent successfully!");
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
            Console.WriteLine($"Error message: {ex.ErrorMessage}");
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
