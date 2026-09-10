using System.Text.Json.Serialization;

namespace Laneful.Models;

/// <summary>
/// Request body for creating a sending domain.
/// </summary>
public sealed class CreateDomainRequest
{
    [JsonPropertyName("domain")]
    public string Domain { get; }

    [JsonPropertyName("tracking")]
    public string Tracking { get; }

    [JsonPropertyName("return_path")]
    public string ReturnPath { get; }

    [JsonPropertyName("require_tls")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? RequireTls { get; }

    [JsonPropertyName("email_track_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? EmailTrackId { get; }

    public CreateDomainRequest(
        string domain,
        string tracking,
        string returnPath,
        bool? requireTls = null,
        string? emailTrackId = null)
    {
        Domain = domain;
        Tracking = tracking;
        ReturnPath = returnPath;
        RequireTls = requireTls;
        EmailTrackId = string.IsNullOrEmpty(emailTrackId) ? null : emailTrackId;
    }
}
