using Microsoft.AspNetCore.Mvc;
using Laneful.Webhooks;
using Laneful.Examples;

namespace Laneful.Examples;

/// <summary>
/// ASP.NET Core controller example for handling Laneful webhooks
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class WebhookController : ControllerBase
{
    private readonly WebhookExample _webhookHandler;
    private readonly ILogger<WebhookController> _logger;

    public WebhookController(ILogger<WebhookController> logger)
    {
        _logger = logger;
        
        // Get webhook secret from configuration
        var webhookSecret = Environment.GetEnvironmentVariable("LANEFUL_WEBHOOK_SECRET") 
            ?? throw new InvalidOperationException("LANEFUL_WEBHOOK_SECRET environment variable is required");
        
        _webhookHandler = new WebhookExample(webhookSecret);
    }

    /// <summary>
    /// Handle incoming Laneful webhook events
    /// </summary>
    /// <returns>Webhook processing result</returns>
    [HttpPost("laneful")]
    public async Task<IActionResult> HandleLanefulWebhook()
    {
        try
        {
            // Read the raw payload
            using var reader = new StreamReader(Request.Body);
            var payload = await reader.ReadToEndAsync();
            
            if (string.IsNullOrWhiteSpace(payload))
            {
                _logger.LogWarning("Empty webhook payload received");
                return BadRequest(new { error = "Empty payload received" });
            }

            // Convert headers to dictionary
            var headers = Request.Headers.ToDictionary(
                h => h.Key,
                h => h.Value.ToString(),
                StringComparer.OrdinalIgnoreCase
            );

            // Process the webhook
            var result = await _webhookHandler.HandleWebhookAsync(payload, headers);

            if (result.Success)
            {
                _logger.LogInformation("Webhook processed successfully: {ProcessedCount} events in {Mode} mode", 
                    result.ProcessedCount, result.IsBatch ? "batch" : "single");
                
                return Ok(new
                {
                    status = "success",
                    processed = result.ProcessedCount,
                    mode = result.IsBatch ? "batch" : "single"
                });
            }
            else
            {
                _logger.LogWarning("Webhook processing failed: {Error}", result.Error);
                return StatusCode(result.StatusCode, new { error = result.Error });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error processing webhook");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Health check endpoint for webhook service
    /// </summary>
    /// <returns>Service status</returns>
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new
        {
            status = "healthy",
            service = "Laneful Webhook Handler",
            timestamp = DateTimeOffset.UtcNow,
            version = "1.0.0"
        });
    }

    /// <summary>
    /// Test endpoint for webhook verification (GET request)
    /// </summary>
    /// <returns>Webhook configuration information</returns>
    [HttpGet("laneful")]
    public IActionResult GetWebhookInfo()
    {
        var testPayload = WebhookExample.GenerateTestPayload();
        var testSignature = WebhookVerifier.GenerateSignature(
            Environment.GetEnvironmentVariable("LANEFUL_WEBHOOK_SECRET") ?? "test-secret",
            testPayload,
            includePrefix: true
        );

        return Ok(new
        {
            service = "Laneful Webhook Endpoint",
            status = "ready",
            configuration = new
            {
                headerName = WebhookVerifier.GetSignatureHeaderName(),
                supportedEvents = new[] { "delivery", "open", "click", "bounce", "drop", "spam_complaint", "unsubscribe" },
                payloadFormats = new[] { "Single event (object)", "Batch mode (array)" }
            },
            testExample = new
            {
                curl = $"curl -X POST {Request.Scheme}://{Request.Host}{Request.Path} \\\n" +
                       $"  -H \"Content-Type: application/json\" \\\n" +
                       $"  -H \"{WebhookVerifier.GetSignatureHeaderName()}: {testSignature}\" \\\n" +
                       $"  -d '{testPayload}'",
                payload = testPayload,
                signature = testSignature
            },
            features = new[]
            {
                "✅ Signature verification with sha256= prefix support",
                "✅ Batch and single event mode detection",
                "✅ Payload structure validation",
                "✅ All documented event types supported",
                "✅ Header extraction with fallback formats",
                "✅ Comprehensive error handling"
            }
        });
    }
}

/// <summary>
/// Program class for ASP.NET Core webhook service
/// </summary>
public class WebhookProgram
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services
        builder.Services.AddControllers();
        builder.Services.AddLogging();

        // Configure logging
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.AddDebug();

        var app = builder.Build();

        // Configure pipeline
        app.UseRouting();
        app.MapControllers();

        // Add health check endpoint
        app.MapGet("/health", () => new
        {
            status = "healthy",
            service = "Laneful Webhook Service",
            timestamp = DateTimeOffset.UtcNow
        });

        // Start the service
        var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
        app.Run($"http://localhost:{port}");
    }
}
