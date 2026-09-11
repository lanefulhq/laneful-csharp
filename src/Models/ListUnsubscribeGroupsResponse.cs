using System.Text.Json.Serialization;

namespace Laneful.Models;

/// <summary>
/// Paginated list of unsubscribe groups.
/// </summary>
public sealed class ListUnsubscribeGroupsResponse
{
    [JsonPropertyName("unsubscribe_groups")]
    public List<UnsubscribeGroup> UnsubscribeGroups { get; init; } = new();

    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; init; }
}
