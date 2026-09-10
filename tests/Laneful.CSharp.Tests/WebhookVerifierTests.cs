using Laneful.Webhooks;
using Xunit;

namespace Laneful.Tests;

public class WebhookVerifierTests
{
    [Fact]
    public void ParseWebhookPayload_AcceptsRequestEvent()
    {
        var payload = """
            {"event":"request","email":"user@example.com","lane_id":"5805dd85-ed8c-44db-91a7-1d53a41c86a5","message_id":"H-1-019844e340027d728a7cfda632e14d0a","timestamp":1753502407}
            """;

        var result = WebhookVerifier.ParseWebhookPayload(payload);

        Assert.False(result.IsBatch);
        Assert.Equal("request", result.Events[0]["event"]?.ToString());
    }
}
