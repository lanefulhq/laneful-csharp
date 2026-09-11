using System.Text.Json.Serialization;

namespace Laneful.Models;

/// <summary>
/// A sending domain and its verification state.
/// </summary>
public sealed class Domain
{
    [JsonPropertyName("domain")]
    public string DomainName { get; init; } = string.Empty;

    [JsonPropertyName("tracking")]
    public string Tracking { get; init; } = string.Empty;

    [JsonPropertyName("return_path")]
    public string ReturnPath { get; init; } = string.Empty;

    [JsonPropertyName("verified")]
    public bool Verified { get; init; }

    [JsonPropertyName("tracking_verified")]
    public bool TrackingVerified { get; init; }

    [JsonPropertyName("return_path_verified")]
    public bool ReturnPathVerified { get; init; }

    [JsonPropertyName("dkim1_verified")]
    public bool Dkim1Verified { get; init; }

    [JsonPropertyName("dkim2_verified")]
    public bool Dkim2Verified { get; init; }

    [JsonPropertyName("dmarc_verified")]
    public bool DmarcVerified { get; init; }

    [JsonPropertyName("require_tls")]
    public bool RequireTls { get; init; }

    [JsonPropertyName("email_track_id")]
    public string EmailTrackId { get; init; } = string.Empty;
}
