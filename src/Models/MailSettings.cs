using System.Text.Json.Serialization;

namespace Laneful.Models;

/// <summary>
/// Request-level mail settings (sandbox mode, return message IDs).
/// </summary>
public sealed class MailSettings
{
    /// <summary>
    /// When enabled, messages are not persisted or sent (sandbox only).
    /// </summary>
    [JsonPropertyName("sandbox_mode")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? SandboxMode { get; }

    /// <summary>
    /// When enabled, the API response includes message IDs for each email sent.
    /// </summary>
    [JsonPropertyName("return_message_ids")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? ReturnMessageIds { get; }

    public MailSettings(bool? sandboxMode = null, bool? returnMessageIds = null)
    {
        SandboxMode = sandboxMode;
        ReturnMessageIds = returnMessageIds;
    }
}
