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

    /// <summary>
    /// Creates new tracking settings.
    /// </summary>
    /// <param name="opens">Whether to track email opens</param>
    /// <param name="clicks">Whether to track link clicks</param>
    /// <param name="unsubscribes">Whether to track unsubscribes</param>
    public TrackingSettings(bool opens = false, bool clicks = false, bool unsubscribes = false)
    {
        Opens = opens;
        Clicks = clicks;
        Unsubscribes = unsubscribes;
    }

    /// <summary>
    /// Creates tracking settings from a dictionary representation.
    /// </summary>
    /// <param name="data">Dictionary containing tracking settings</param>
    /// <returns>New TrackingSettings instance</returns>
    public static TrackingSettings FromDictionary(Dictionary<string, object> data)
    {
        var opens = data.GetValueOrDefault("opens") is bool opensValue ? opensValue : false;
        var clicks = data.GetValueOrDefault("clicks") is bool clicksValue ? clicksValue : false;
        var unsubscribes = data.GetValueOrDefault("unsubscribes") is bool unsubscribesValue ? unsubscribesValue : false;
        
        return new TrackingSettings(opens, clicks, unsubscribes);
    }
}

