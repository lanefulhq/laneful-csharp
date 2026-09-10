using System.Text.Json.Serialization;

namespace Laneful.Models;

/// <summary>
/// Paginated list of Google Postmaster spam reports.
/// </summary>
public sealed class ListGooglePostmasterSpamReportsResponse
{
    [JsonPropertyName("spam_reports")]
    public List<GooglePostmasterSpamReport> SpamReports { get; init; } = new();

    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; init; }
}
