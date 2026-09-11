using System.Text.Json.Serialization;

namespace Laneful.Models;

/// <summary>
/// A sending domain whose spam complaint ratio reached a critical level
/// at a mailbox provider on a given day.
/// </summary>
public sealed class DomainSpamRatioRadar
{
    [JsonPropertyName("workspace_id")]
    public long WorkspaceId { get; init; }

    [JsonPropertyName("domain")]
    public string Domain { get; init; } = string.Empty;

    [JsonPropertyName("esp")]
    public string Esp { get; init; } = string.Empty;

    [JsonPropertyName("spam_ratio")]
    public double SpamRatio { get; init; }

    [JsonPropertyName("date")]
    public string Date { get; init; } = string.Empty;
}
