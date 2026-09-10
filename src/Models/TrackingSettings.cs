using System.Text.Json.Serialization;

namespace Laneful.Models;

/// <summary>
/// Configuration for email tracking settings.
/// </summary>
public record TrackingSettings
{
    [JsonPropertyName("opens")]
    public bool Opens { get; }

    [JsonPropertyName("clicks")]
    public bool Clicks { get; }

    [JsonPropertyName("unsubscribes")]
    public bool Unsubscribes { get; }

    [JsonPropertyName("unsubscribe_group_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public long? UnsubscribeGroupId { get; }

    [JsonPropertyName("unsubscribe_group_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? UnsubscribeGroupName { get; }

    /// <summary>
    /// Creates new tracking settings.
    /// </summary>
    /// <param name="opens">Whether to track email opens</param>
    /// <param name="clicks">Whether to track link clicks</param>
    /// <param name="unsubscribes">Whether to track unsubscribes</param>
    /// <param name="unsubscribeGroupId">Optional unsubscribe group ID</param>
    /// <param name="unsubscribeGroupName">Optional unsubscribe group name, ignored if unsubscribeGroupId is set</param>
    public TrackingSettings(
        bool opens = false,
        bool clicks = false,
        bool unsubscribes = false,
        long? unsubscribeGroupId = null,
        string? unsubscribeGroupName = null)
    {
        Opens = opens;
        Clicks = clicks;
        Unsubscribes = unsubscribes;
        UnsubscribeGroupId = unsubscribeGroupId;
        UnsubscribeGroupName = unsubscribeGroupName;
    }

    /// <summary>
    /// Creates tracking settings from a dictionary representation.
    /// </summary>
    /// <param name="data">Dictionary containing tracking settings</param>
    /// <returns>New TrackingSettings instance</returns>
    public static TrackingSettings FromDictionary(Dictionary<string, object> data)
    {
        var opens = data.GetValueOrDefault("opens") is bool opensValue && opensValue;
        var clicks = data.GetValueOrDefault("clicks") is bool clicksValue && clicksValue;
        var unsubscribes = data.GetValueOrDefault("unsubscribes") is bool unsubscribesValue && unsubscribesValue;
        long? groupId = data.GetValueOrDefault("unsubscribe_group_id") switch
        {
            long l => l,
            int i => i,
            _ => null
        };
        var groupName = data.GetValueOrDefault("unsubscribe_group_name")?.ToString();

        return new TrackingSettings(opens, clicks, unsubscribes, groupId, groupName);
    }
}
