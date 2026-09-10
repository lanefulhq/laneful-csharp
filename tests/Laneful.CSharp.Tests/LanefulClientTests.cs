using System.Net;
using System.Text.Json;
using Laneful.Models;
using Xunit;

namespace Laneful.Tests;

public class LanefulClientTests
{
    [Fact]
    public async Task SendEmail_IncludesMailSettings()
    {
        var (client, handler) = TestClient.Create("""{"status":"accepted","message_ids":["msg-1"]}""");

        var response = await client.SendEmailAsync(
            TestClient.SampleEmail(),
            new MailSettings(sandboxMode: true, returnMessageIds: true));

        Assert.Equal(HttpMethod.Post, handler.LastRequest!.Method);
        Assert.Equal("https://api.example.com/v1/email/send", handler.LastRequest.RequestUri!.ToString());
        Assert.Contains("\"sandbox_mode\":true", handler.LastBody);
        Assert.Contains("\"return_message_ids\":true", handler.LastBody);
        Assert.Equal("accepted", response["status"].ToString());
        Assert.Equal("laneful-csharp/1.2.0", handler.LastRequest.Headers.UserAgent.ToString());
    }

    [Fact]
    public async Task ListUnsubscribeGroups_BuildsQuery()
    {
        var (client, handler) = TestClient.Create("""
            {"unsubscribe_groups":[{"unsubscribe_group_id":9,"name":"Newsletters","created_at":1710000000}],"next_cursor":"abc"}
            """);

        var result = await client.ListUnsubscribeGroupsAsync(42, new ListUnsubscribeGroupsParams
        {
            Limit = 10,
            Search = "news"
        });

        Assert.Equal(
            "https://api.example.com/v1/workspaces/42/unsubscribe-groups?limit=10&search=news",
            handler.LastRequest!.RequestUri!.ToString());
        Assert.Equal("Newsletters", result.UnsubscribeGroups[0].Name);
        Assert.Equal("abc", result.NextCursor);
    }

    [Fact]
    public async Task CreateAndUpdateUnsubscribeGroup()
    {
        var handler = new ScriptedHandler((request, body) =>
        {
            var name = request.Method == HttpMethod.Post ? "Newsletters" : "Promos";
            return ScriptedHandler.Json(
                HttpStatusCode.OK,
                "{\"unsubscribe_group\":{\"unsubscribe_group_id\":9,\"name\":\"" + name + "\",\"created_at\":1710000000}}");
        });
        var client = new Laneful.LanefulClient("https://api.example.com", "test-token", new HttpClient(handler));

        var created = await client.CreateUnsubscribeGroupAsync(42, "Newsletters");
        Assert.Equal(9, created.UnsubscribeGroupId);

        var updated = await client.UpdateUnsubscribeGroupAsync(42, 9, "Promos");
        Assert.Equal("Promos", updated.Name);
        Assert.Equal(HttpMethod.Patch, handler.LastRequest!.Method);
    }

    [Fact]
    public async Task DomainEndpoints()
    {
        const string domainJson = """
            {"domain":"example.com","tracking":"track","return_path":"bounce","verified":true,"dmarc_verified":true,"email_track_id":"track-1"}
            """;

        var handler = new ScriptedHandler((request, _) =>
        {
            var url = request.RequestUri!.ToString();
            if (request.Method == HttpMethod.Get && url.Contains("filter%5Bdomain%5D=example.com"))
            {
                return ScriptedHandler.Json(
                    HttpStatusCode.OK,
                    "{\"domains\":[" + domainJson + "],\"pagination\":{\"next_cursor\":null}}");
            }

            if (request.Method == HttpMethod.Delete)
                return ScriptedHandler.Json(HttpStatusCode.OK, """{"message":"deleted"}""");

            return ScriptedHandler.Json(HttpStatusCode.OK, domainJson);
        });
        var client = new Laneful.LanefulClient("https://api.example.com", "test-token", new HttpClient(handler));

        var list = await client.ListDomainsAsync(42, new ListDomainsParams { FilterDomain = "example.com" });
        Assert.Equal("example.com", list.Domains[0].DomainName);

        var got = await client.GetDomainAsync(42, "example.com");
        Assert.True(got.DmarcVerified);

        var created = await client.CreateDomainAsync(42, new CreateDomainRequest("example.com", "track", "bounce"));
        Assert.Equal("example.com", created.DomainName);

        var updated = await client.UpdateDomainAsync(42, "example.com", new UpdateDomainRequest("track-1"));
        Assert.Equal("track-1", updated.EmailTrackId);

        var verified = await client.VerifyDomainAsync(42, "example.com");
        Assert.True(verified.Verified);

        var deleted = await client.DeleteDomainAsync(42, "example.com");
        Assert.Equal("deleted", deleted.Message);
    }

    [Fact]
    public async Task AnalyticsEndpoints()
    {
        var call = 0;
        var handler = new ScriptedHandler((request, _) =>
        {
            call++;
            var url = request.RequestUri!.ToString();
            Assert.Equal(HttpMethod.Get, request.Method);

            if (url.Contains("/analytics/radar/domain-spam-ratio"))
            {
                Assert.Contains("workspace_ids=1", url);
                Assert.Contains("workspace_ids=2", url);
                return ScriptedHandler.Json(HttpStatusCode.OK, """
                    {"radar":[{"workspace_id":1,"domain":"example.com","esp":"Gmail","spam_ratio":0.2,"date":"2026-09-01"}]}
                    """);
            }

            if (url.Contains("/analytics/google-postmaster/spam-reports"))
            {
                return ScriptedHandler.Json(HttpStatusCode.OK, """
                    {"spam_reports":[{"workspace_id":1,"domain":"example.com","date":"2026-09-01","spam_ratio":0.01}]}
                    """);
            }

            Assert.Contains("/analytics/microsoft-snds/reports", url);
            Assert.Contains("ip=203.0.113.5", url);
            return ScriptedHandler.Json(HttpStatusCode.OK, """
                {"snds_reports":[{"ip":"203.0.113.5","date":"2026-09-01","rcpt_commands":1,"data_commands":1,"message_recipients":1,"filter_result":"GREEN","complaint_rate":0,"trap_hits":0}]}
                """);
        });
        var client = new Laneful.LanefulClient("https://api.example.com", "test-token", new HttpClient(handler));

        var radar = await client.ListDomainSpamRatioRadarAsync(new ListDomainSpamRatioRadarParams
        {
            WorkspaceIds = new[] { 1L, 2L }
        });
        Assert.Equal("Gmail", radar.Radar[0].Esp);

        var postmaster = await client.ListGooglePostmasterSpamReportsAsync();
        Assert.Equal("example.com", postmaster.SpamReports[0].Domain);

        var snds = await client.ListSndsReportsAsync(new ListSndsReportsParams { Ip = "203.0.113.5" });
        Assert.Equal(SndsReport.FilterGreen, snds.SndsReports[0].FilterResult);
        Assert.Equal(3, call);
    }
}
