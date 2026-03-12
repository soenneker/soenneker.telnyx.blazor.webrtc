using System.Collections.Generic;
using System.Text.Json.Serialization;
using Soenneker.Telnyx.Blazor.WebRtc.Enums;

namespace Soenneker.Telnyx.Blazor.WebRtc.Configuration;

/// <summary>
/// Represents configuration options for initializing a Telnyx WebRTC client.
/// </summary>
public sealed class TelnyxClientInitOptions
{
    // ----- Authentication -----

    /// <summary>
    /// The username to authenticate with your SIP Connection. 'Login' and 'Password' take precedence over 'LoginToken'.
    /// </summary>
    [JsonPropertyName("login")]
    public string? Login { get; set; }

    /// <summary>
    /// The password to authenticate with your SIP Connection.
    /// </summary>
    [JsonPropertyName("password")]
    public string? Password { get; set; }

    /// <summary>
    /// Alternative password field (interchangeable with Password).
    /// </summary>
    [JsonPropertyName("passwd")]
    public string? Passwd { get; set; }

    /// <summary>
    /// A JWT token used for authentication instead of username/password.
    /// </summary>
    [JsonPropertyName("login_token")]
    public string? LoginToken { get; set; }

    /// <summary>
    /// Anonymous login options if not using authentication credentials.
    /// </summary>
    [JsonPropertyName("anonymous_login")]
    public TelnyxAnonymousLoginOptions? AnonymousLogin { get; set; }

    // ----- Connection -----

    /// <summary>
    /// The WebSocket host to connect to. Defaults to "wss://rtc.telnyx.com".
    /// </summary>
    [JsonPropertyName("host")]
    public string? Host { get; set; }

    /// <summary>
    /// The environment to use ("production" or "development").
    /// </summary>
    [JsonPropertyName("env")]
    public TelnyxEnvironment Environment { get; set; } = TelnyxEnvironment.Production;

    /// <summary>
    /// The Telnyx region to connect to.
    /// </summary>
    [JsonPropertyName("region")]
    public string? Region { get; set; }

    /// <summary>
    /// Whether the client should attempt to reconnect after being disconnected.
    /// </summary>
    [JsonPropertyName("autoReconnect")]
    public bool? AutoReconnect { get; set; }

    // ----- WebRTC and Media -----

    /// <summary>
    /// Custom ICE servers to use for the peer connection.
    /// </summary>
    [JsonPropertyName("iceServers")]
    public List<TelnyxIceServer>? IceServers { get; set; }

    /// <summary>
    /// Enable debug logging.
    /// </summary>
    [JsonPropertyName("debug")]
    public bool? Debug { get; set; }

    /// <summary>
    /// Output mode for debug logs ("socket" or "file").
    /// </summary>
    [JsonPropertyName("debugOutput")]
    public string? DebugOutput { get; set; }

    /// <summary>
    /// Enables prefetching ICE candidates before offer creation.
    /// </summary>
    [JsonPropertyName("prefetchIceCandidates")]
    public bool? PrefetchIceCandidates { get; set; }

    /// <summary>
    /// Forces use of only relay ICE candidates.
    /// </summary>
    [JsonPropertyName("forceRelayCandidate")]
    public bool? ForceRelayCandidate { get; set; }

    // ----- Media Elements -----

    /// <summary>
    /// DOM element ID or reference for local media (video/audio).
    /// </summary>
    [JsonPropertyName("localElement")]
    public object? LocalElement { get; set; }

    /// <summary>
    /// DOM element ID or reference for remote media (video/audio).
    /// </summary>
    [JsonPropertyName("remoteElement")]
    public object? RemoteElement { get; set; }

    // ----- Audio/Video Device Settings -----

    /// <summary>
    /// Microphone device ID.
    /// </summary>
    [JsonPropertyName("micId")]
    public string? MicId { get; set; }

    /// <summary>
    /// Microphone label.
    /// </summary>
    [JsonPropertyName("micLabel")]
    public string? MicLabel { get; set; }

    /// <summary>
    /// Camera device ID.
    /// </summary>
    [JsonPropertyName("camId")]
    public string? CamId { get; set; }

    /// <summary>
    /// Camera label.
    /// </summary>
    [JsonPropertyName("camLabel")]
    public string? CamLabel { get; set; }

    /// <summary>
    /// Output device ID for audio playback (speakers).
    /// </summary>
    [JsonPropertyName("speaker")]
    public string? Speaker { get; set; }

    // ----- Sound files -----

    /// <summary>
    /// Path to ringtone sound file (for incoming calls).
    /// </summary>
    [JsonPropertyName("ringtoneFile")]
    public string? RingtoneFile { get; set; }

    /// <summary>
    /// Path to ringback sound file (played to caller while waiting).
    /// </summary>
    [JsonPropertyName("ringbackFile")]
    public string? RingbackFile { get; set; }

    // ----- Custom Session Data -----

    /// <summary>
    /// User-defined variables to attach to the client session.
    /// </summary>
    [JsonPropertyName("userVariables")]
    public Dictionary<string, object>? UserVariables { get; set; }
}