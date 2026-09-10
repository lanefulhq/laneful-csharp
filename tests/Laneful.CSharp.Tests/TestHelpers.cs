using System.Net;
using System.Text;
using Laneful.Models;

namespace Laneful.Tests;

internal sealed class ScriptedHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, string?, HttpResponseMessage> _respond;

    public HttpRequestMessage? LastRequest { get; private set; }
    public string? LastBody { get; private set; }

    public ScriptedHandler(string responseJson, HttpStatusCode statusCode = HttpStatusCode.OK)
        : this((_, _) => Json(statusCode, responseJson))
    {
    }

    public ScriptedHandler(Func<HttpRequestMessage, string?, HttpResponseMessage> respond)
    {
        _respond = respond;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        LastRequest = request;
        LastBody = request.Content == null ? null : await request.Content.ReadAsStringAsync(cancellationToken);
        return _respond(request, LastBody);
    }

    public static HttpResponseMessage Json(HttpStatusCode statusCode, string json)
    {
        return new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
    }
}

internal static class TestClient
{
    public static (LanefulClient Client, ScriptedHandler Handler) Create(string responseJson)
    {
        var handler = new ScriptedHandler(responseJson);
        var http = new HttpClient(handler);
        return (new LanefulClient("https://api.example.com", "test-token", http), handler);
    }

    public static Email SampleEmail()
    {
        return new Email.Builder()
            .From(new Address("sender@example.com"))
            .To(new Address("recipient@example.com"))
            .Subject("Test")
            .TextContent("Content")
            .Build();
    }
}
