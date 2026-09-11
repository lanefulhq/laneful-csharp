using System.Text.Json.Serialization;

namespace Laneful.Models;

/// <summary>
/// Request body for updating a domain's mutable settings.
/// Pass a track ID to set the email track, an empty string to clear it
/// (the domain falls back to the default track), or null to leave it unchanged.
/// </summary>
public sealed class UpdateDomainRequest
{
    [JsonPropertyName("email_track_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? EmailTrackId { get; }

    public UpdateDomainRequest(string? emailTrackId = null)
    {
        EmailTrackId = emailTrackId;
    }
}
