using System.Text.Json.Serialization;

namespace Laneful.Models;

/// <summary>
/// An unsubscribe group in a workspace.
/// </summary>
public sealed class UnsubscribeGroup
{
    [JsonPropertyName("unsubscribe_group_id")]
    public long UnsubscribeGroupId { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("created_at")]
    public long CreatedAt { get; init; }
}

/// <summary>
/// API envelope for a single unsubscribe group.
/// </summary>
public sealed class UnsubscribeGroupResponse
{
    [JsonPropertyName("unsubscribe_group")]
    public UnsubscribeGroup UnsubscribeGroup { get; init; } = null!;
}
