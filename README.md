# Laneful C# SDK

A C# client library for the Laneful email API, built with modern C# features and .NET 9.

## Requirements

- .NET 8.0 or higher
- C# 12.0 or higher

## Installation

### NuGet Package

```bash
dotnet add package Laneful.CSharp
```

### Building from Source

```bash
git clone https://github.com/lanefulhq/laneful-csharp.git
cd laneful-csharp
dotnet build
```

## Quick Start

```csharp
using Laneful;
using Laneful.Models;

// Create client
var client = new LanefulClient(
    "https://your-endpoint.send.laneful.net",
    "your-auth-token"
);

// Create email
var email = new Email.Builder()
    .From(new Address("sender@example.com", "Your Name"))
    .To(new Address("recipient@example.com", "Recipient Name"))
    .Subject("Hello from Laneful C# SDK")
    .TextContent("This is a test email.")
    .HtmlContent("<h1>This is a test email.</h1>")
    .Build();

// Send email
try
{
    var response = await client.SendEmailAsync(email);
    Console.WriteLine("Email sent successfully");
}
catch (Exception ex)
{
    Console.WriteLine($"Failed to send email: {ex.Message}");
}
```

## Features

- Send single or multiple emails
- Plain text and HTML content
- Email templates with dynamic data
- File attachments
- Email tracking (opens, clicks, unsubscribes)
- Custom headers and reply-to addresses
- Scheduled sending
- Webhook signature verification
- Modern .NET 8 features (records, pattern matching, etc.)
- Async/await support
- Comprehensive error handling
- Logging support

## Examples

### Template Email

```csharp
var email = new Email.Builder()
    .From(new Address("sender@example.com"))
    .To(new Address("user@example.com"))
    .TemplateId("welcome-template")
    .TemplateData(new Dictionary<string, object>
    {
        ["name"] = "John Doe",
        ["company"] = "Acme Corp"
    })
    .Build();

var response = await client.SendEmailAsync(email);
```

### Email with Attachments

```csharp
// Create attachment from file
var attachment = Attachment.FromFile("/path/to/document.pdf");

var email = new Email.Builder()
    .From(new Address("sender@example.com"))
    .To(new Address("user@example.com"))
    .Subject("Document Attached")
    .TextContent("Please find the document attached.")
    .Attachment(attachment)
    .Build();

var response = await client.SendEmailAsync(email);
```

### Email with Tracking

```csharp
var tracking = new TrackingSettings(opens: true, clicks: true, unsubscribes: true);

var email = new Email.Builder()
    .From(new Address("sender@example.com"))
    .To(new Address("user@example.com"))
    .Subject("Tracked Email")
    .HtmlContent("<p>This email is tracked.</p>")
    .Tracking(tracking)
    .Build();

var response = await client.SendEmailAsync(email);
```

### Multiple Recipients

```csharp
var email = new Email.Builder()
    .From(new Address("sender@example.com"))
    .To(new Address("user1@example.com"))
    .To(new Address("user2@example.com", "User Two"))
    .Cc(new Address("cc@example.com"))
    .Bcc(new Address("bcc@example.com"))
    .Subject("Multiple Recipients")
    .TextContent("This email has multiple recipients.")
    .Build();

var response = await client.SendEmailAsync(email);
```

### Scheduled Email

```csharp
// Schedule for 24 hours from now
var sendTime = DateTimeOffset.UtcNow.AddHours(24).ToUnixTimeSeconds();

var email = new Email.Builder()
    .From(new Address("sender@example.com"))
    .To(new Address("user@example.com"))
    .Subject("Scheduled Email")
    .TextContent("This email was scheduled.")
    .SendTime(sendTime)
    .Build();

var response = await client.SendEmailAsync(email);
```

### Multiple Emails

```csharp
var emails = new[]
{
    new Email.Builder()
        .From(new Address("sender@example.com"))
        .To(new Address("user1@example.com"))
        .Subject("Email 1")
        .TextContent("First email content.")
        .Build(),
    new Email.Builder()
        .From(new Address("sender@example.com"))
        .To(new Address("user2@example.com"))
        .Subject("Email 2")
        .TextContent("Second email content.")
        .Build()
};

var response = await client.SendEmailsAsync(emails);
```

### Custom Timeout

```csharp
var client = new LanefulClient(
    "https://your-endpoint.send.laneful.net",
    "your-auth-token",
    TimeSpan.FromSeconds(60) // 60 second timeout
);
```

### With Logging

```csharp
using Microsoft.Extensions.Logging;

var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
var logger = loggerFactory.CreateLogger<LanefulClient>();

var client = new LanefulClient(
    "https://your-endpoint.send.laneful.net",
    "your-auth-token",
    logger: logger
);
```

## Webhook Verification

```csharp
using Laneful.Webhooks;

// In your webhook handler
var payload = await request.Body.ReadAsStringAsync(); // Get the raw request body
var signature = request.Headers["x-webhook-signature"].FirstOrDefault();
var secret = "your-webhook-secret";

if (WebhookVerifier.VerifySignature(secret, payload, signature))
{
    // Process webhook data
    var data = JsonSerializer.Deserialize<Dictionary<string, object>>(payload);
    // Handle webhook event
}
else
{
    // Invalid signature
    return BadRequest();
}
```

## Error Handling

```csharp
try
{
    var response = await client.SendEmailAsync(email);
    Console.WriteLine("Email sent successfully");
}
catch (ValidationException ex)
{
    // Invalid input data
    Console.WriteLine($"Validation error: {ex.Message}");
}
catch (ApiException ex)
{
    // API returned an error
    Console.WriteLine($"API error: {ex.Message}");
    Console.WriteLine($"Status code: {ex.StatusCode}");
    Console.WriteLine($"Error message: {ex.ErrorMessage}");
}
catch (HttpException ex)
{
    // Network or HTTP-level error
    Console.WriteLine($"HTTP error: {ex.Message}");
    Console.WriteLine($"Status code: {ex.StatusCode}");
}
catch (Exception ex)
{
    // Other unexpected errors
    Console.WriteLine($"Unexpected error: {ex.Message}");
}
```

## API Reference

### LanefulClient

#### Constructors

- `LanefulClient(string baseUrl, string authToken)` - Creates client with default timeout (30 seconds)
- `LanefulClient(string baseUrl, string authToken, TimeSpan timeout)` - Creates client with custom timeout
- `LanefulClient(string baseUrl, string authToken, HttpClient httpClient)` - Creates client with custom HTTP client
- `LanefulClient(string baseUrl, string authToken, ILogger<LanefulClient> logger)` - Creates client with logger

#### Methods

- `Task<Dictionary<string, object>> SendEmailAsync(Email email)` - Sends a single email
- `Task<Dictionary<string, object>> SendEmailsAsync(IEnumerable<Email> emails)` - Sends multiple emails
- `void Dispose()` - Disposes the HTTP client

### Email.Builder

#### Required Fields

- `From(Address from)` - Sender address

#### Optional Fields

- `To(Address to)` / `To(string email, string? name)` - Recipient addresses
- `Cc(Address cc)` / `Cc(string email, string? name)` - CC addresses
- `Bcc(Address bcc)` / `Bcc(string email, string? name)` - BCC addresses
- `Subject(string subject)` - Email subject
- `TextContent(string? textContent)` - Plain text content
- `HtmlContent(string? htmlContent)` - HTML content
- `TemplateId(string? templateId)` - Template ID
- `TemplateData(Dictionary<string, object>? templateData)` - Template data
- `Attachment(Attachment attachment)` - File attachments
- `Headers(Dictionary<string, string>? headers)` - Custom headers
- `ReplyTo(Address? replyTo)` / `ReplyTo(string email, string? name)` - Reply-to address
- `SendTime(long? sendTime)` - Scheduled send time (Unix timestamp)
- `WebhookData(Dictionary<string, string>? webhookData)` - Webhook data
- `Tag(string? tag)` - Email tag
- `Tracking(TrackingSettings? tracking)` - Tracking settings

### Address

- `Address(string email, string? name)` - Creates address with email and optional name

### Attachment

- `Attachment.FromFile(string filePath)` - Creates attachment from file
- `Attachment(string filename, string contentType, string content)` - Creates attachment from raw data

### TrackingSettings

- `TrackingSettings(bool opens, bool clicks, bool unsubscribes)` - Creates tracking settings

### WebhookVerifier

- `bool VerifySignature(string secret, string payload, string signature)` - Verifies webhook signature
- `string GenerateSignature(string secret, string payload)` - Generates webhook signature

## Exception Types

- `ValidationException` - Thrown when input validation fails
- `ApiException` - Thrown when the API returns an error response
- `HttpException` - Thrown when HTTP communication fails
- `LanefulException` - Base exception class for all SDK exceptions

## .NET 9 Features Used

- **Records** - Immutable data classes with automatic methods
- **Pattern Matching** - Switch expressions and enhanced control flow
- **Nullable Reference Types** - Compile-time null safety
- **Async/Await** - Modern asynchronous programming
- **System.Text.Json** - High-performance JSON serialization
- **Top-level Statements** - Simplified program entry points
- **File Scoped Namespaces** - Cleaner namespace declarations
- **Global Using** - Simplified imports

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
