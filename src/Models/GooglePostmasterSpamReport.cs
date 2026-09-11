using System.Text.Json.Serialization;

namespace Laneful.Models;

/// <summary>
/// A daily Gmail spam-rate report from Google Postmaster Tools.
/// </summary>
public sealed class GooglePostmasterSpamReport
{
    [JsonPropertyName("workspace_id")]
    public long WorkspaceId { get; init; }

    [JsonPropertyName("domain")]
    public string Domain { get; init; } = string.Empty;

    [JsonPropertyName("date")]
    public string Date { get; init; } = string.Empty;

    [JsonPropertyName("spam_ratio")]
    public double SpamRatio { get; init; }
}
