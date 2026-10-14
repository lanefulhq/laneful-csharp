using System.Text;
using System.Text.Json.Serialization;
using Laneful.Exceptions;

namespace Laneful.Models;

/// <summary>
/// Request-level mail settings (sandbox mode, return message IDs, idempotency key).
/// </summary>
public sealed class MailSettings
{
    private readonly string? _idempotencyKey;

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

    /// <summary>
    /// Key for duplicate send protection, 1 to 36 bytes. Laneful accepts a key once per
    /// organization for one hour: a repeated request with the same key is answered with status
    /// "accepted" (and no message IDs) and sends nothing.
    /// </summary>
    /// <exception cref="ValidationException">If the key is not 1 to 36 bytes long</exception>
    [JsonPropertyName("idempotency_key")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? IdempotencyKey
    {
        get => _idempotencyKey;
        init
        {
            if (value != null)
            {
                var length = Encoding.UTF8.GetByteCount(value);
                if (length < 1 || length > 36)
                    throw new ValidationException("IdempotencyKey must contain between 1 and 36 bytes");
            }
            _idempotencyKey = value;
        }
    }

    public MailSettings(bool? sandboxMode = null, bool? returnMessageIds = null)
    {
        SandboxMode = sandboxMode;
        ReturnMessageIds = returnMessageIds;
    }
}
