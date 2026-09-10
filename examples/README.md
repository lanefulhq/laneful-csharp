# Laneful C# SDK Examples

This directory contains comprehensive examples demonstrating how to use the Laneful C# SDK for sending emails, handling webhooks, and managing various email features.

## Quick Start with Docker

The easiest way to run these examples is using Docker:

```bash
# Build the Docker image
docker build -f examples/Dockerfile .

# Run a specific example
docker run --env-file examples/.env laneful-csharp-examples BasicEmailExample

# Run all examples
docker run --env-file examples/.env laneful-csharp-examples all
```

## Local Development Setup

### Prerequisites

- .NET 8.0 or higher
- C# 12.0 or higher
- A Laneful account with API credentials

### Setup

1. **Clone the repository and navigate to examples:**
   ```bash
   cd laneful-csharp/examples
   ```

2. **Copy environment template:**
   ```bash
   cp env.example .env
   ```

3. **Edit `.env` with your credentials:**
   ```bash
   # Required: Laneful API Configuration
   LANEFUL_BASE_URL=https://your-endpoint.send.laneful.net
   LANEFUL_AUTH_TOKEN=your-auth-token
   
   # Required: Email Configuration
   LANEFUL_FROM_EMAIL=sender@yourdomain.com
   LANEFUL_TO_EMAILS=recipient1@example.com,recipient2@example.com
   
   # Required: Template Configuration
   LANEFUL_TEMPLATE_ID=your-template-id
   
   # Required: Webhook Configuration
   LANEFUL_WEBHOOK_SECRET=your-webhook-secret
   LANEFUL_WEBHOOK_URL=https://your-domain.com/webhook
   ```

4. **Build the project:**
   ```bash
   dotnet build
   ```

5. **Run examples:**
   ```bash
   # Run a specific example
   dotnet run --project Examples.csproj -- BasicEmailExample
   
   # Or run directly
   dotnet run --project Examples.csproj --project BasicEmailExample.cs
   ```

## Available Examples

| Example | Description | Features Demonstrated |
|---------|-------------|----------------------|
| `BasicEmailExample` | Simple text email | Basic email sending, error handling |
| `HtmlEmailWithTrackingExample` | HTML email with tracking | HTML content, click/open tracking |
| `TemplateEmailExample` | Template-based email | Template usage, dynamic data |
| `AttachmentEmailExample` | Email with attachments | File attachments, base64 content |
| `MultipleRecipientsExample` | Multiple recipients | TO, CC, BCC, Reply-To |
| `ScheduledEmailExample` | Scheduled email delivery | Future delivery scheduling |
| `BatchEmailExample` | Batch email sending | Multiple emails in one request |
| `WebhookHandlerExample` | Webhook processing | Signature verification, event handling |
| `ErrorHandlingExample` | Comprehensive error handling | Exception types, best practices |
| `ComprehensiveExample` | All features combined | Complete feature demonstration |
| `MailSettingsExample` | Sandbox send + From header | `mail_settings`, `from_header`, unsubscribe group name |
| `DomainsExample` | Domain management | List, create, verify, update email track |
| `UnsubscribeGroupsExample` | Unsubscribe groups | Create, update, list |
| `AnalyticsExample` | Deliverability analytics | Spam-ratio radar, Google Postmaster, Microsoft SNDS |

## Environment Variables

All examples use environment variables for configuration:

| Variable | Required | Description |
|----------|----------|-------------|
| `LANEFUL_BASE_URL` | ✅ | Your Laneful API endpoint |
| `LANEFUL_AUTH_TOKEN` | ✅ | Your API authentication token |
| `LANEFUL_FROM_EMAIL` | ✅ | Sender email address |
| `LANEFUL_TO_EMAILS` | ✅ | Comma-separated recipient emails |
| `LANEFUL_TEMPLATE_ID` | ✅ | Template ID for template examples |
| `LANEFUL_WEBHOOK_SECRET` | ✅ | Webhook signature verification secret |
| `LANEFUL_WEBHOOK_URL` | ✅ | Your webhook endpoint URL |
| `LANEFUL_ORG_BASE_URL` | ⚪ | Organization API host (`https://api.laneful.net`) for domain, group, and analytics examples |
| `LANEFUL_WORKSPACE_ID` | ⚪ | Workspace ID for domain and unsubscribe-group examples |

## Docker Usage

### Building the Image

```bash
# From the laneful-csharp root directory
docker build -f examples/Dockerfile -t laneful-csharp-examples .
```

### Running Examples

```bash
# Run a specific example
docker run --env-file examples/.env laneful-csharp-examples BasicEmailExample

# Run all examples
docker run --env-file examples/.env laneful-csharp-examples all

# Run with custom environment variables
docker run \
  -e LANEFUL_BASE_URL=https://your-endpoint.send.laneful.net \
  -e LANEFUL_AUTH_TOKEN=your-token \
  -e LANEFUL_FROM_EMAIL=sender@yourdomain.com \
  -e LANEFUL_TO_EMAILS=recipient@example.com \
  laneful-csharp-examples BasicEmailExample
```

### Docker Troubleshooting

**Build Issues:**
- Ensure you're running from the `laneful-csharp` root directory
- Check that the SDK builds successfully: `dotnet build`

**Runtime Issues:**
- Verify all required environment variables are set
- Check that your API credentials are valid
- Ensure your domain is verified in Laneful

**Network Issues:**
- Verify your `LANEFUL_BASE_URL` is accessible
- Check firewall settings if running in restricted environments

## Features Demonstrated

### Email Sending
- ✅ Simple text emails
- ✅ HTML emails with CSS styling
- ✅ Template-based emails with dynamic data
- ✅ Emails with file attachments
- ✅ Multiple recipients (TO, CC, BCC)
- ✅ Reply-to functionality
- ✅ Scheduled email delivery
- ✅ Batch email sending
- ✅ Sandbox mode and return message IDs
- ✅ Visible From header

### Tracking & Analytics
- ✅ Open tracking
- ✅ Click tracking
- ✅ Unsubscribe tracking
- ✅ Custom email tags
- ✅ Unsubscribe groups
- ✅ Domain management
- ✅ Spam-ratio radar, Google Postmaster, and Microsoft SNDS

### Webhook Handling
- ✅ Signature verification (HMAC-SHA256)
- ✅ Payload parsing and validation
- ✅ Event type handling
- ✅ Batch event processing
- ✅ Error handling and logging

### Error Handling
- ✅ Validation exceptions
- ✅ API exceptions with status codes
- ✅ HTTP exceptions
- ✅ Comprehensive error messages
- ✅ Best practices demonstration

## API Reference

For detailed API documentation, see the [C# SDK Documentation](../../web-admin/src/pages/docs/CSharpSDK.tsx).

## Support

- 📚 [Documentation](https://docs.laneful.com)
- 🐛 [Issue Tracker](https://github.com/lanefulhq/laneful-csharp/issues)
- 💬 [Community Support](https://github.com/lanefulhq/laneful-csharp/discussions)
