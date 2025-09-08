# Laneful C# SDK - Webhook Handling

This document describes how to handle Laneful webhooks using the C# SDK, following the same patterns and features as the other language implementations.

## Features

✅ **Signature verification with sha256= prefix support**  
✅ **Batch and single event mode detection**  
✅ **Payload structure validation**  
✅ **All documented event types supported**  
✅ **Header extraction with fallback formats**  
✅ **Comprehensive error handling**  
✅ **Modern C# async/await patterns**  
✅ **Strong typing with nullable reference types**  

## Quick Start

### 1. Basic Webhook Verification

```csharp
using Laneful.Webhooks;

// Verify a webhook signature
var secret = "your-webhook-secret";
var payload = "{\"event\":\"delivery\",\"email\":\"user@example.com\",...}";
var signature = "sha256=abc123..."; // From x-webhook-signature header

var isValid = WebhookVerifier.VerifySignature(secret, payload, signature);
```

### 2. Parse Webhook Payload

```csharp
// Parse and validate webhook payload
var webhookData = WebhookVerifier.ParseWebhookPayload(payload);

Console.WriteLine($"Is batch: {webhookData.IsBatch}");
Console.WriteLine($"Events count: {webhookData.Events.Count}");

foreach (var eventData in webhookData.Events)
{
    var eventType = eventData["event"]?.ToString();
    var email = eventData["email"]?.ToString();
    Console.WriteLine($"Event: {eventType} for {email}");
}
```

### 3. Extract Signature from Headers

```csharp
// Extract signature from HTTP headers (supports multiple formats)
var headers = new Dictionary<string, string>
{
    ["x-webhook-signature"] = "sha256=abc123...",
    // Also supports: "X_WEBHOOK_SIGNATURE", "HTTP_X_WEBHOOK_SIGNATURE"
};

var signature = WebhookVerifier.ExtractSignatureFromHeaders(headers);
```

## Complete Webhook Handler

### WebhookExample Class

The `WebhookExample` class provides a complete webhook handling solution:

```csharp
using Laneful.Examples;

var webhookHandler = new WebhookExample("your-webhook-secret");

// Process webhook with headers
var result = await webhookHandler.HandleWebhookAsync(payload, headers);

if (result.Success)
{
    Console.WriteLine($"Processed {result.ProcessedCount} events");
}
else
{
    Console.WriteLine($"Error: {result.Error}");
}
```

### Event Processing

The handler automatically processes all supported event types:

- **delivery** - Email delivered successfully
- **open** - Email opened by recipient
- **click** - Link clicked in email
- **bounce** - Email bounced (hard/soft)
- **drop** - Email dropped by provider
- **spam_complaint** - Recipient marked as spam
- **unsubscribe** - Recipient unsubscribed

## ASP.NET Core Integration

### WebhookController

```csharp
[ApiController]
[Route("api/[controller]")]
public class WebhookController : ControllerBase
{
    [HttpPost("laneful")]
    public async Task<IActionResult> HandleLanefulWebhook()
    {
        // Read payload
        using var reader = new StreamReader(Request.Body);
        var payload = await reader.ReadToEndAsync();
        
        // Convert headers
        var headers = Request.Headers.ToDictionary(
            h => h.Key, h => h.Value.ToString(), StringComparer.OrdinalIgnoreCase);
        
        // Process webhook
        var result = await _webhookHandler.HandleWebhookAsync(payload, headers);
        
        return result.Success ? Ok(result) : StatusCode(result.StatusCode, result);
    }
}
```

## Supported Event Types

### Delivery Event
```json
{
  "event": "delivery",
  "email": "user@example.com",
  "lane_id": "5805dd85-ed8c-44db-91a7-1d53a41c86a5",
  "message_id": "msg-123",
  "timestamp": 1640995200,
  "tag": "welcome-email"
}
```

### Open Event
```json
{
  "event": "open",
  "email": "user@example.com",
  "lane_id": "5805dd85-ed8c-44db-91a7-1d53a41c86a5",
  "message_id": "msg-123",
  "timestamp": 1640995200,
  "client_device": "Desktop",
  "client_os": "Windows",
  "client_ip": "192.168.1.1"
}
```

### Click Event
```json
{
  "event": "click",
  "email": "user@example.com",
  "lane_id": "5805dd85-ed8c-44db-91a7-1d53a41c86a5",
  "message_id": "msg-123",
  "timestamp": 1640995200,
  "url": "https://example.com/link"
}
```

### Bounce Event
```json
{
  "event": "bounce",
  "email": "user@example.com",
  "lane_id": "5805dd85-ed8c-44db-91a7-1d53a41c86a5",
  "message_id": "msg-123",
  "timestamp": 1640995200,
  "is_hard": true,
  "text": "Mailbox does not exist"
}
```

## Batch Mode

Webhooks can be sent in batch mode (array of events) or single mode (single event object):

### Single Event
```json
{
  "event": "delivery",
  "email": "user@example.com",
  "lane_id": "5805dd85-ed8c-44db-91a7-1d53a41c86a5",
  "message_id": "msg-123",
  "timestamp": 1640995200
}
```

### Batch Events
```json
[
  {
    "event": "delivery",
    "email": "user1@example.com",
    "lane_id": "5805dd85-ed8c-44db-91a7-1d53a41c86a5",
    "message_id": "msg-123",
    "timestamp": 1640995200
  },
  {
    "event": "open",
    "email": "user2@example.com",
    "lane_id": "5805dd85-ed8c-44db-91a7-1d53a41c86a5",
    "message_id": "msg-124",
    "timestamp": 1640995200
  }
]
```

## Security

### Signature Verification

All webhooks are signed using HMAC-SHA256. The signature is sent in the `x-webhook-signature` header:

```
x-webhook-signature: sha256=abc123def456...
```

### Header Formats Supported

The SDK supports multiple header formats for compatibility:

- `x-webhook-signature` (documented format)
- `X_WEBHOOK_SIGNATURE` (uppercase with underscores)
- `HTTP_X_WEBHOOK_SIGNATURE` (with HTTP_ prefix)

### Constant Time Comparison

Signature verification uses constant-time comparison to prevent timing attacks.

## Error Handling

The webhook handler provides comprehensive error handling:

```csharp
try
{
    var result = await webhookHandler.HandleWebhookAsync(payload, headers);
    
    if (!result.Success)
    {
        // Handle specific error types
        switch (result.StatusCode)
        {
            case 400:
                // Payload validation error
                break;
            case 401:
                // Signature verification error
                break;
        }
    }
}
catch (ArgumentException ex)
{
    // Invalid payload structure
}
catch (Exception ex)
{
    // Unexpected error
}
```

## Examples

### Console Application
See `WebhookConsoleExample.cs` for a complete console application demonstrating all webhook features.

### ASP.NET Core Web API
See `WebhookControllerExample.cs` for a complete ASP.NET Core webhook endpoint.

### Integration Example
See the updated `Program.cs` for webhook verification integration with email sending examples.

## Testing

### Generate Test Signatures

```csharp
var testPayload = WebhookExample.GenerateTestPayload();
var testSignature = WebhookVerifier.GenerateSignature(secret, testPayload, includePrefix: true);
```

### Test with cURL

```bash
curl -X POST http://localhost:5000/api/webhook/laneful \
  -H "Content-Type: application/json" \
  -H "x-webhook-signature: sha256=abc123..." \
  -d '{"event":"delivery","email":"test@example.com",...}'
```

## Configuration

Set the webhook secret as an environment variable:

```bash
export LANEFUL_WEBHOOK_SECRET="your-webhook-secret-here"
```

Or in your application configuration:

```csharp
var webhookSecret = Environment.GetEnvironmentVariable("LANEFUL_WEBHOOK_SECRET") 
    ?? throw new InvalidOperationException("LANEFUL_WEBHOOK_SECRET is required");
```

## Comparison with Other SDKs

The C# implementation provides feature parity with the PHP, Java, and Ruby SDKs:

| Feature | PHP | Java | Ruby | C# |
|---------|-----|------|------|----|
| SHA256 prefix support | ✅ | ✅ | ✅ | ✅ |
| Payload parsing | ✅ | ✅ | ✅ | ✅ |
| Header extraction | ✅ | ✅ | ✅ | ✅ |
| Event validation | ✅ | ✅ | ✅ | ✅ |
| Batch/single detection | ✅ | ✅ | ✅ | ✅ |
| Error handling | ✅ | ✅ | ✅ | ✅ |
| Examples | ✅ | ✅ | ✅ | ✅ |

## Migration from Other SDKs

If you're migrating from another Laneful SDK, the C# implementation follows the same patterns:

1. **Signature verification** - Same algorithm and prefix handling
2. **Payload structure** - Same JSON structure and validation
3. **Event types** - Same event types and fields
4. **Error handling** - Similar error responses and status codes

The main differences are C#-specific features like async/await, nullable reference types, and modern C# syntax.
