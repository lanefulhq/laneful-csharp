using Laneful.Examples;

namespace Laneful.Examples;

/// <summary>
/// Main program to run different examples
/// </summary>
public class Program
{
    public static async Task Main(string[] args)
    {
        var exampleName = args.Length > 0 ? args[0] : "BasicEmailExample";
        
        Console.WriteLine($"🚀 Running Laneful C# SDK Example: {exampleName}");
        Console.WriteLine("===============================================\n");

        try
        {
            switch (exampleName.ToLowerInvariant())
            {
                case "basicemail":
                case "basicemailexample":
                    await BasicEmailExample.RunExample(args);
                    break;
                case "htmlemail":
                case "htmlemailwithtrackingexample":
                    await HtmlEmailWithTrackingExample.RunExample(args);
                    break;
                case "template":
                case "templateemailexample":
                    await TemplateEmailExample.RunExample(args);
                    break;
                case "attachment":
                case "attachmentemailexample":
                    await AttachmentEmailExample.RunExample(args);
                    break;
                case "multiple":
                case "multiplerecipientsexample":
                    await MultipleRecipientsExample.RunExample(args);
                    break;
                case "scheduled":
                case "scheduledemailexample":
                    await ScheduledEmailExample.RunExample(args);
                    break;
                case "batch":
                case "batchemailexample":
                    await BatchEmailExample.RunExample(args);
                    break;
                case "webhook":
                case "webhookhandlerexample":
                    await WebhookHandlerExample.RunExample(args);
                    break;
                case "error":
                case "errorhandlingexample":
                    await ErrorHandlingExample.RunExample(args);
                    break;
                case "comprehensive":
                case "comprehensiveexample":
                    await ComprehensiveExample.RunExample(args);
                    break;
                case "all":
                    await RunAllExamples();
                    break;
                default:
                    Console.WriteLine($"❌ Unknown example: {exampleName}");
                    Console.WriteLine("Available examples:");
                    Console.WriteLine("  - BasicEmailExample");
                    Console.WriteLine("  - HtmlEmailWithTrackingExample");
                    Console.WriteLine("  - TemplateEmailExample");
                    Console.WriteLine("  - AttachmentEmailExample");
                    Console.WriteLine("  - MultipleRecipientsExample");
                    Console.WriteLine("  - ScheduledEmailExample");
                    Console.WriteLine("  - BatchEmailExample");
                    Console.WriteLine("  - WebhookHandlerExample");
                    Console.WriteLine("  - ErrorHandlingExample");
                    Console.WriteLine("  - ComprehensiveExample");
                    Console.WriteLine("  - all (runs all examples)");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error running example: {ex.Message}");
        }
    }

    private static async Task RunAllExamples()
    {
        var examples = new (string Name, Func<Task> Action)[]
        {
            ("BasicEmailExample", () => BasicEmailExample.RunExample(Array.Empty<string>())),
            ("HtmlEmailWithTrackingExample", () => HtmlEmailWithTrackingExample.RunExample(Array.Empty<string>())),
            ("TemplateEmailExample", () => TemplateEmailExample.RunExample(Array.Empty<string>())),
            ("AttachmentEmailExample", () => AttachmentEmailExample.RunExample(Array.Empty<string>())),
            ("MultipleRecipientsExample", () => MultipleRecipientsExample.RunExample(Array.Empty<string>())),
            ("ScheduledEmailExample", () => ScheduledEmailExample.RunExample(Array.Empty<string>())),
            ("BatchEmailExample", () => BatchEmailExample.RunExample(Array.Empty<string>())),
            ("WebhookHandlerExample", () => WebhookHandlerExample.RunExample(Array.Empty<string>())),
            ("ErrorHandlingExample", () => ErrorHandlingExample.RunExample(Array.Empty<string>())),
            ("ComprehensiveExample", () => ComprehensiveExample.RunExample(Array.Empty<string>()))
        };

        foreach (var example in examples)
        {
            try
            {
                Console.WriteLine($"\n{new string('=', 60)}");
                Console.WriteLine($"Running: {example.Name}");
                Console.WriteLine($"{new string('=', 60)}");
                await example.Action();
                Console.WriteLine($"✅ {example.Name} completed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ {example.Name} failed: {ex.Message}");
            }
        }
    }
}
