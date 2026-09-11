using System.Text.Json.Serialization;

namespace Laneful.Models;

/// <summary>
/// Paginated list of domain spam-ratio radar entries.
/// </summary>
public sealed class ListDomainSpamRatioRadarResponse
{
    [JsonPropertyName("radar")]
    public List<DomainSpamRatioRadar> Radar { get; init; } = new();

    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; init; }
}
