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

    [Fact]
    public void ParseWebhookPayload_RealRequestEventKeepsAllFields()
    {
        // Shape of a real request event: empty lane_id, id and mx_host at the top level.
        var payload = """
            {"id":"01a136a7f4f77430913f16d9d3e195a8","event":"request","email":"user@example.com","message_id":"H-0-01a136a7f4ef7bc2ac33bc34d2377fab-21183","lane_id":"","tag":"","timestamp":1791918339,"mx_host":"aspmx.l.google.com","metadata":{"run_id":"r1"}}
            """;

        var evt = WebhookVerifier.ParseWebhookPayload(payload).Events[0];

        Assert.Equal("01a136a7f4f77430913f16d9d3e195a8", evt["id"]);
        Assert.Equal("", evt["lane_id"]);
        Assert.Equal("aspmx.l.google.com", evt["mx_host"]);
        Assert.Equal(1791918339L, evt["timestamp"]);
        var metadata = Assert.IsType<Dictionary<string, object>>(evt["metadata"]);
        Assert.Equal("r1", metadata["run_id"].ToString());
    }

    [Fact]
    public void ParseWebhookPayload_KeepsEventSpecificFields()
    {
        var payload = """
            {"id":"8f14e45f-ceea-4e7a-b1c2-6d3f9a0b2c71","event":"bounce","email":"user@example.com","message_id":"H-1-abc","lane_id":"5805dd85-ed8c-44db-91a7-1d53a41c86a5","timestamp":1753502700,"code":"550","extended_code":"5.1.1","is_hard":true,"classification":"Mailbox Unavailable","deliverability_issue":false}
            """;

        var evt = WebhookVerifier.ParseWebhookPayload(payload).Events[0];

        Assert.Equal("550", evt["code"]);
        Assert.Equal("5.1.1", evt["extended_code"]);
        Assert.Equal(true, evt["is_hard"]);
        Assert.Equal("Mailbox Unavailable", evt["classification"]);
        Assert.Equal(false, evt["deliverability_issue"]);
    }

    [Fact]
    public void ParseWebhookPayload_KeepsUnknownEventTypesInBatch()
    {
        var payload = """
            [{"event":"delivery","email":"user@example.com","lane_id":"","message_id":"H-1-a","timestamp":1753502407},
             {"event":"future_event_type","email":"user@example.com","lane_id":"","message_id":"H-1-b","timestamp":1753502408}]
            """;

        var result = WebhookVerifier.ParseWebhookPayload(payload);

        Assert.True(result.IsBatch);
        Assert.Equal(2, result.Events.Count);
        Assert.Equal("future_event_type", result.Events[1]["event"]);
    }

    [Fact]
    public void ParseWebhookPayload_StillRequiresCoreFields()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            WebhookVerifier.ParseWebhookPayload("""{"event":"delivery"}"""));

        Assert.Contains("Missing required field", ex.Message);
    }
}
