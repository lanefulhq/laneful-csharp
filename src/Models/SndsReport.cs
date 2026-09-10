using System.Text.Json.Serialization;

namespace Laneful.Models;

/// <summary>
/// A daily Microsoft SNDS report for a sending IP.
/// Filter result is one of GREEN, YELLOW, RED, or empty when unknown.
/// </summary>
public sealed class SndsReport
{
    public const string FilterUnknown = "";
    public const string FilterGreen = "GREEN";
    public const string FilterYellow = "YELLOW";
    public const string FilterRed = "RED";

    [JsonPropertyName("ip")]
    public string Ip { get; init; } = string.Empty;

    [JsonPropertyName("date")]
    public string Date { get; init; } = string.Empty;

    [JsonPropertyName("rcpt_commands")]
    public long RcptCommands { get; init; }

    [JsonPropertyName("data_commands")]
    public long DataCommands { get; init; }

    [JsonPropertyName("message_recipients")]
    public long MessageRecipients { get; init; }

    [JsonPropertyName("filter_result")]
    public string FilterResult { get; init; } = FilterUnknown;

    [JsonPropertyName("complaint_rate")]
    public double ComplaintRate { get; init; }

    [JsonPropertyName("trap_hits")]
    public long TrapHits { get; init; }
}
