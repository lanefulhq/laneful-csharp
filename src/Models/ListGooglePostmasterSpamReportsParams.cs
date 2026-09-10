namespace Laneful.Models;

/// <summary>
/// Query parameters for listing Google Postmaster spam reports.
/// Dates are UTC calendar days in YYYY-MM-DD format.
/// </summary>
public sealed class ListGooglePostmasterSpamReportsParams
{
    public IReadOnlyList<long> WorkspaceIds { get; init; } = Array.Empty<long>();
    public string? Domain { get; init; }
    public string? StartDate { get; init; }
    public string? EndDate { get; init; }
    public string? Cursor { get; init; }
    public int? Limit { get; init; }

    internal List<KeyValuePair<string, string>> ToQuery()
    {
        var query = new List<KeyValuePair<string, string>>();

        foreach (var id in WorkspaceIds)
            query.Add(new("workspace_ids", id.ToString()));

        if (!string.IsNullOrEmpty(Domain))
            query.Add(new("domain", Domain));

        if (!string.IsNullOrEmpty(StartDate))
            query.Add(new("start_date", StartDate));

        if (!string.IsNullOrEmpty(EndDate))
            query.Add(new("end_date", EndDate));

        if (!string.IsNullOrEmpty(Cursor))
            query.Add(new("cursor", Cursor));

        if (Limit is > 0)
            query.Add(new("limit", Limit.Value.ToString()));

        return query;
    }
}
