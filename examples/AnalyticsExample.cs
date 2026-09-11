using Laneful;
using Laneful.Models;
using Laneful.Exceptions;

namespace Laneful.Examples;

/// <summary>
/// Deliverability analytics against the organization API host.
/// </summary>
public class AnalyticsExample
{
    public static async Task RunExample(string[] args)
    {
        Console.WriteLine("📊 Analytics Example");
        Console.WriteLine("====================\n");

        var baseUrl = Environment.GetEnvironmentVariable("LANEFUL_ORG_BASE_URL")
            ?? Environment.GetEnvironmentVariable("LANEFUL_BASE_URL")
            ?? "https://api.laneful.net";
        var authToken = Environment.GetEnvironmentVariable("LANEFUL_AUTH_TOKEN")
            ?? throw new InvalidOperationException("LANEFUL_AUTH_TOKEN environment variable is required");

        try
        {
            var client = new LanefulClient(baseUrl, authToken);

            var radar = await client.ListDomainSpamRatioRadarAsync(new ListDomainSpamRatioRadarParams
            {
                StartDate = DateTime.UtcNow.AddDays(-7).ToString("yyyy-MM-dd"),
                EndDate = DateTime.UtcNow.ToString("yyyy-MM-dd")
            });
            foreach (var entry in radar.Radar)
                Console.WriteLine($"{entry.Date} {entry.Domain} @{entry.Esp}: {entry.SpamRatio}%");

            var postmaster = await client.ListGooglePostmasterSpamReportsAsync(
                new ListGooglePostmasterSpamReportsParams { Domain = "example.com" });
            foreach (var report in postmaster.SpamReports)
                Console.WriteLine($"{report.Date} {report.Domain}: {report.SpamRatio}%");

            var snds = await client.ListSndsReportsAsync();
            foreach (var report in snds.SndsReports)
                Console.WriteLine($"{report.Date} {report.Ip}: filter={report.FilterResult} complaint={report.ComplaintRate}%");
        }
        catch (ApiException ex)
        {
            Console.WriteLine($"❌ API error: {ex.Message}");
        }
        catch (HttpException ex)
        {
            Console.WriteLine($"❌ HTTP error: {ex.Message}");
        }
    }
}
