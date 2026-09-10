using System.Text.Json.Serialization;

namespace Laneful.Models;

/// <summary>
/// Pagination details for a domains listing.
/// </summary>
public sealed class DomainsPagination
{
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; init; }
}

/// <summary>
/// Paginated list of sending domains.
/// </summary>
public sealed class ListDomainsResponse
{
    [JsonPropertyName("domains")]
    public List<Domain> Domains { get; init; } = new();

    [JsonPropertyName("pagination")]
    public DomainsPagination? Pagination { get; init; }

    [JsonIgnore]
    public string? NextCursor => Pagination?.NextCursor;
}
