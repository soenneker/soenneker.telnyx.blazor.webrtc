using System.Text.Json.Serialization;

namespace Soenneker.Telnyx.Blazor.WebRtc.Configuration;

/// <summary>
/// Represents configuration options for initializing a Telnyx WebRTC client.
/// </summary>
public sealed class TelnyxClientOptions
{
    [JsonPropertyName("initOptions")]
    public TelnyxClientInitOptions InitOptions { get; set; } = null!;

    /// <summary>
    /// Automatically attempt reconnection if the client is disconnected.
    /// </summary>
    [JsonPropertyName("autoReconnect")]
    public bool? AutoReconnect { get; set; }

    /// <summary>
    /// Delay in milliseconds between reconnection attempts.
    /// </summary>
    [JsonPropertyName("reconnectDelay")]
    public int ReconnectDelay { get; set; } = 2000;

    /// <summary>
    /// Maximum number of reconnection attempts.
    /// </summary>
    [JsonPropertyName("reconnectAttempts")]
    public int ReconnectAttempts { get; set; } = 5;

    [JsonIgnore]
    public bool UseCdn { get; set; } = true;
}