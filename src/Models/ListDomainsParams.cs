namespace Laneful.Models;

/// <summary>
/// Query parameters for listing domains.
/// </summary>
public sealed class ListDomainsParams
{
    public string? Cursor { get; init; }
    public int? Limit { get; init; }
    public string? FilterDomain { get; init; }

    internal List<KeyValuePair<string, string>> ToQuery()
    {
        var query = new List<KeyValuePair<string, string>>();

        if (!string.IsNullOrEmpty(Cursor))
            query.Add(new("cursor", Cursor));

        if (Limit is > 0)
            query.Add(new("limit", Limit.Value.ToString()));

        if (!string.IsNullOrEmpty(FilterDomain))
            query.Add(new("filter[domain]", FilterDomain));

        return query;
    }
}
