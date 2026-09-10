using System.Text.Json.Serialization;

namespace Laneful.Models;

/// <summary>
/// Paginated list of Microsoft SNDS reports.
/// </summary>
public sealed class ListSndsReportsResponse
{
    [JsonPropertyName("snds_reports")]
    public List<SndsReport> SndsReports { get; init; } = new();

    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; init; }
}
