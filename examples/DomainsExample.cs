using Laneful;
using Laneful.Models;
using Laneful.Exceptions;

namespace Laneful.Examples;

/// <summary>
/// Domain management against the organization API host.
/// </summary>
public class DomainsExample
{
    public static async Task RunExample(string[] args)
    {
        Console.WriteLine("🌐 Domains Example");
        Console.WriteLine("==================\n");

        var baseUrl = Environment.GetEnvironmentVariable("LANEFUL_ORG_BASE_URL")
            ?? Environment.GetEnvironmentVariable("LANEFUL_BASE_URL")
            ?? "https://api.laneful.net";
        var authToken = Environment.GetEnvironmentVariable("LANEFUL_AUTH_TOKEN")
            ?? throw new InvalidOperationException("LANEFUL_AUTH_TOKEN environment variable is required");
        var workspaceId = long.Parse(Environment.GetEnvironmentVariable("LANEFUL_WORKSPACE_ID") ?? "1");

        try
        {
            var client = new LanefulClient(baseUrl, authToken);

            var list = await client.ListDomainsAsync(workspaceId, new ListDomainsParams { Limit = 50 });
            Console.WriteLine($"Domains: {list.Domains.Count}");

            var domain = await client.CreateDomainAsync(workspaceId, new CreateDomainRequest(
                "mydomain.com",
                "tracking",
                "return-path"));
            Console.WriteLine($"Created {domain.DomainName}, verified={domain.Verified}");

            domain = await client.VerifyDomainAsync(workspaceId, "mydomain.com");
            Console.WriteLine($"Verification: dmarc={domain.DmarcVerified}");

            domain = await client.UpdateDomainAsync(
                workspaceId,
                "mydomain.com",
                new UpdateDomainRequest("e59f0a35-05bc-4516-b585-c06f69c3e67e"));
            Console.WriteLine($"Email track: {domain.EmailTrackId}");
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
