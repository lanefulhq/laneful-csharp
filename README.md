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
- Visible `from_header` and request-level `mail_settings`
- Scheduled sending
- Webhook signature verification
- Domain management (list, create, verify, update email track, delete)
- Unsubscribe groups
- Deliverability analytics (spam-ratio radar, Google Postmaster, Microsoft SNDS)
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

### Visible From header

```csharp
var email = new Email.Builder()
    .From(new Address("sender@example.com", "Your Name"))
    .To(new Address("user@example.com"))
    .Subject("Hello")
    .TextContent("Hello")
    .FromHeader(new Address("newsletter@example.com", "Newsletter"))
    .Build();
```

### Mail settings (sandbox and message IDs)

```csharp
var response = await client.SendEmailAsync(
    email,
    new MailSettings(sandboxMode: true, returnMessageIds: true)
);
// response["message_ids"] is present when returnMessageIds is true
```

### Tracking with an unsubscribe group

```csharp
var tracking = new TrackingSettings(
    opens: true,
    clicks: true,
    unsubscribes: true,
    unsubscribeGroupId: 123,
    // ignored if unsubscribeGroupId is set
    unsubscribeGroupName: "Newsletters"
);
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
    var webhookData = WebhookVerifier.ParseWebhookPayload(payload);
    foreach (var evt in webhookData.Events)
    {
        switch (evt.GetValueOrDefault("event")?.ToString())
        {
            case "request":
                // Handle send request accepted
                break;
            case "delivery":
                // Handle email delivered
                break;
            // Add other event types as needed
        }
    }
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

Send host (`https://your-endpoint.send.laneful.net`):

- `Task<Dictionary<string, object>> SendEmailAsync(Email email, MailSettings? settings = null)` - Sends a single email
- `Task<Dictionary<string, object>> SendEmailsAsync(IEnumerable<Email> emails, MailSettings? settings = null)` - Sends multiple emails

Organization API host (`https://api.laneful.net`):

- `Task<ListUnsubscribeGroupsResponse> ListUnsubscribeGroupsAsync(long workspaceId, ListUnsubscribeGroupsParams? parameters = null)`
- `Task<UnsubscribeGroup> CreateUnsubscribeGroupAsync(long workspaceId, string name)`
- `Task<UnsubscribeGroup> UpdateUnsubscribeGroupAsync(long workspaceId, long unsubscribeGroupId, string name)`
- `Task<ListDomainsResponse> ListDomainsAsync(long workspaceId, ListDomainsParams? parameters = null)`
- `Task<Domain> GetDomainAsync(long workspaceId, string domain)`
- `Task<Domain> CreateDomainAsync(long workspaceId, CreateDomainRequest request)`
- `Task<Domain> UpdateDomainAsync(long workspaceId, string domain, UpdateDomainRequest request)`
- `Task<Domain> VerifyDomainAsync(long workspaceId, string domain)`
- `Task<SuccessResponse> DeleteDomainAsync(long workspaceId, string domain)`
- `Task<ListDomainSpamRatioRadarResponse> ListDomainSpamRatioRadarAsync(ListDomainSpamRatioRadarParams? parameters = null)`
- `Task<ListGooglePostmasterSpamReportsResponse> ListGooglePostmasterSpamReportsAsync(ListGooglePostmasterSpamReportsParams? parameters = null)`
- `Task<ListSndsReportsResponse> ListSndsReportsAsync(ListSndsReportsParams? parameters = null)`

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
- `FromHeader(Address? fromHeader)` / `FromHeader(string email, string? name)` - Visible From header

### Address

- `Address(string email, string? name)` - Creates address with email and optional name

### Attachment

- `Attachment.FromFile(string filePath)` - Creates attachment from file
- `Attachment(string filename, string contentType, string content)` - Creates attachment from raw data

### TrackingSettings

- `TrackingSettings(bool opens, bool clicks, bool unsubscribes, long? unsubscribeGroupId = null, string? unsubscribeGroupName = null)` - Creates tracking settings

### MailSettings

- `MailSettings(bool? sandboxMode = null, bool? returnMessageIds = null)` - Request-level send options

## Domain, unsubscribe groups, and analytics

These endpoints live on the organization API host. Point the client at it:

```csharp
var client = new LanefulClient(
    "https://api.laneful.net",
    "your-auth-token"
);
```

### Unsubscribe groups

```csharp
var groups = await client.ListUnsubscribeGroupsAsync(42, new ListUnsubscribeGroupsParams { Limit = 50 });
var created = await client.CreateUnsubscribeGroupAsync(42, "Newsletters");
var updated = await client.UpdateUnsubscribeGroupAsync(42, created.UnsubscribeGroupId, "Weekly Newsletters");
```

### Domains

```csharp
var list = await client.ListDomainsAsync(42, new ListDomainsParams { Limit = 50 });
var domain = await client.CreateDomainAsync(42, new CreateDomainRequest(
    "mydomain.com",
    "tracking",
    "return-path"));
domain = await client.GetDomainAsync(42, "mydomain.com");
domain = await client.VerifyDomainAsync(42, "mydomain.com");

// Set the email track; pass "" to clear it, or omit EmailTrackId to leave it unchanged
domain = await client.UpdateDomainAsync(
    42,
    "mydomain.com",
    new UpdateDomainRequest("e59f0a35-05bc-4516-b585-c06f69c3e67e"));

await client.DeleteDomainAsync(42, "mydomain.com");
```

### Deliverability analytics

```csharp
var radar = await client.ListDomainSpamRatioRadarAsync(new ListDomainSpamRatioRadarParams
{
    WorkspaceIds = new[] { 1L, 2L },
    Domain = "example.com",
    StartDate = "2026-09-01",
    EndDate = "2026-09-08"
});

var postmaster = await client.ListGooglePostmasterSpamReportsAsync(
    new ListGooglePostmasterSpamReportsParams { Domain = "example.com" });

var snds = await client.ListSndsReportsAsync(new ListSndsReportsParams { Ip = "203.0.113.5" });
```

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
