using Laneful;
using Laneful.Models;
using Laneful.Exceptions;

namespace Laneful.Examples;

/// <summary>
/// Unsubscribe group management against the organization API host.
/// </summary>
public class UnsubscribeGroupsExample
{
    public static async Task RunExample(string[] args)
    {
        Console.WriteLine("🚫 Unsubscribe Groups Example");
        Console.WriteLine("=============================\n");

        var baseUrl = Environment.GetEnvironmentVariable("LANEFUL_ORG_BASE_URL")
            ?? Environment.GetEnvironmentVariable("LANEFUL_BASE_URL")
            ?? "https://api.laneful.net";
        var authToken = Environment.GetEnvironmentVariable("LANEFUL_AUTH_TOKEN")
            ?? throw new InvalidOperationException("LANEFUL_AUTH_TOKEN environment variable is required");
        var workspaceId = long.Parse(Environment.GetEnvironmentVariable("LANEFUL_WORKSPACE_ID") ?? "1");

        try
        {
            var client = new LanefulClient(baseUrl, authToken);

            var created = await client.CreateUnsubscribeGroupAsync(workspaceId, "Newsletters");
            Console.WriteLine($"Created group {created.UnsubscribeGroupId}: {created.Name}");

            var updated = await client.UpdateUnsubscribeGroupAsync(
                workspaceId,
                created.UnsubscribeGroupId,
                "Weekly Newsletters");
            Console.WriteLine($"Updated name: {updated.Name}");

            var list = await client.ListUnsubscribeGroupsAsync(
                workspaceId,
                new ListUnsubscribeGroupsParams { Limit = 50 });
            foreach (var group in list.UnsubscribeGroups)
                Console.WriteLine($"- {group.UnsubscribeGroupId} {group.Name}");
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
