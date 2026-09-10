namespace Laneful.Models;

/// <summary>
/// Query parameters for listing unsubscribe groups.
/// </summary>
public sealed class ListUnsubscribeGroupsParams
{
    public string? Cursor { get; init; }
    public int? Limit { get; init; }
    public string? Search { get; init; }

    internal List<KeyValuePair<string, string>> ToQuery()
    {
        var query = new List<KeyValuePair<string, string>>();

        if (!string.IsNullOrEmpty(Cursor))
            query.Add(new("cursor", Cursor));

        if (Limit is > 0)
            query.Add(new("limit", Limit.Value.ToString()));

        if (!string.IsNullOrEmpty(Search))
            query.Add(new("search", Search));

        return query;
    }
}
