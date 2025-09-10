using Laneful;
using Laneful.Models;
using Laneful.Exceptions;

namespace Laneful.Examples;

/// <summary>
/// Template email example based on CSharpSDK.tsx documentation
/// </summary>
public class TemplateEmailExample
{
    public static async Task RunExample(string[] args)
    {
        Console.WriteLine("📋 Template Email Example");
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
        var templateId = Environment.GetEnvironmentVariable("LANEFUL_TEMPLATE_ID") 
            ?? "welcome-template"; // Use a default template ID

        var toEmailList = toEmails.Split(',').Select(email => email.Trim()).ToList();
        var primaryToEmail = toEmailList.First();

        try
        {
            // Create client
            var client = new LanefulClient(baseUrl, authToken);
            Console.WriteLine("✅ Client created successfully");

            var email = new Email.Builder()
                .From(new Address(fromEmail, "Your Name"))
                .To(new Address(primaryToEmail, "Recipient Name"))
                .Subject("Welcome to Our Service!")
                .TemplateId(templateId)
                .TemplateData(new Dictionary<string, object>
                {
                    ["name"] = "John Doe",
                    ["company"] = "Acme Corporation",
                    ["activation_link"] = "https://example.com/activate"
                })
                .Build();

            // Debug: Print the email object
            Console.WriteLine($"Email object: {email}");
            Console.WriteLine($"Template ID: {templateId}");

            var response = await client.SendEmailAsync(email);
            Console.WriteLine("✅ Template email sent successfully!");
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
