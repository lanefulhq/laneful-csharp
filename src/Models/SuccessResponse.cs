using System.Text.Json.Serialization;

namespace Laneful.Models;

/// <summary>
/// Generic success message returned by mutating endpoints that do not return a resource body.
/// </summary>
public sealed class SuccessResponse
{
    [JsonPropertyName("message")]
    public string Message { get; init; } = string.Empty;
}
