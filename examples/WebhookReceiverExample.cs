using System.Net;
using System.Text;
using System.Text.Json;
using Laneful.Webhooks;

namespace Laneful.Examples;

/// <summary>
/// Receives Laneful webhooks over HTTP: verifies the signature, then processes single or batch events.
/// Runs until stopped (Ctrl-C).
/// </summary>
public class WebhookReceiverExample
{
    public static async Task RunExample(string[] args)
    {
        var webhookSecret = Environment.GetEnvironmentVariable("LANEFUL_WEBHOOK_SECRET");
        var port = Environment.GetEnvironmentVariable("PORT") ?? "8000";

        if (string.IsNullOrEmpty(webhookSecret))
        {
            Console.WriteLine("❌ Missing required environment variable: LANEFUL_WEBHOOK_SECRET");
            return;
        }

        using var listener = new HttpListener();
        // The wildcard accepts any Host header (needed behind a proxy or tunnel) and listens
        // on all interfaces, so the receiver is reachable from your network.
        listener.Prefixes.Add($"http://*:{port}/");
        listener.Start();
        Console.WriteLine($"Webhook example listening on http://127.0.0.1:{port}/");

        while (true)
        {
            var context = await listener.GetContextAsync();
            try
            {
                await HandleAsync(context, webhookSecret);
            }
            catch (Exception ex)
            {
                // One bad request must not stop the receiver.
                Console.WriteLine($"Request failed: {ex.Message}");
            }
        }
    }

    private static async Task HandleAsync(HttpListenerContext context, string webhookSecret)
    {
        // Verify the raw body: re-serializing parsed JSON can change the bytes.
        using var reader = new StreamReader(context.Request.InputStream, Encoding.UTF8);
        var payload = await reader.ReadToEndAsync();
        var signature = context.Request.Headers[WebhookVerifier.GetSignatureHeaderName()];

        if (string.IsNullOrEmpty(signature) || string.IsNullOrEmpty(payload)
            || !WebhookVerifier.VerifySignature(webhookSecret, payload, signature))
        {
            await RespondAsync(context, HttpStatusCode.Unauthorized, "Invalid signature");
            return;
        }

        try
        {
            var webhookData = WebhookVerifier.ParseWebhookPayload(payload);
            foreach (var evt in webhookData.Events)
            {
                Console.WriteLine($"[{evt.GetValueOrDefault("event")}] id={evt.GetValueOrDefault("id")} " +
                    $"message_id={evt.GetValueOrDefault("message_id")} event={JsonSerializer.Serialize(evt)}");
            }
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Invalid webhook payload: {ex.Message}");
            await RespondAsync(context, HttpStatusCode.BadRequest, "Invalid payload");
            return;
        }

        await RespondAsync(context, HttpStatusCode.OK, "OK");
    }

    private static async Task RespondAsync(HttpListenerContext context, HttpStatusCode status, string message)
    {
        var body = Encoding.UTF8.GetBytes(message);
        context.Response.StatusCode = (int)status;
        context.Response.ContentLength64 = body.Length;
        await context.Response.OutputStream.WriteAsync(body);
        context.Response.Close();
    }
}
