namespace Laneful.Models;

/// <summary>
/// Query parameters for listing Microsoft SNDS reports.
/// Dates are UTC calendar days in YYYY-MM-DD format.
/// </summary>
public sealed class ListSndsReportsParams
{
    public string? Ip { get; init; }
    public string? StartDate { get; init; }
    public string? EndDate { get; init; }
    public string? Cursor { get; init; }
    public int? Limit { get; init; }

    internal List<KeyValuePair<string, string>> ToQuery()
    {
        var query = new List<KeyValuePair<string, string>>();

        if (!string.IsNullOrEmpty(Ip))
            query.Add(new("ip", Ip));

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
