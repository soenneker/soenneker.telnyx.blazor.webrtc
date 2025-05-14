using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Soenneker.Telnyx.Blazor.WebRtc.Configuration;

/// <summary>
/// Represents options for initiating a WebRTC call using the Telnyx SDK.
/// </summary>
public sealed class TelnyxCallOptions
{
    /// <summary>
    /// The number to call. Required for outbound calls.
    /// </summary>
    [JsonPropertyName("destinationNumber")]
    public string? DestinationNumber { get; set; }

    /// <summary>
    /// Display name for the caller (outgoing).
    /// </summary>
    [JsonPropertyName("callerName")]
    public string? CallerName { get; set; }

    /// <summary>
    /// Caller number for the outbound call.
    /// </summary>
    [JsonPropertyName("callerNumber")]
    public string? CallerNumber { get; set; }

    /// <summary>
    /// Remote party's display name.
    /// </summary>
    [JsonPropertyName("remoteCallerName")]
    public string? RemoteCallerName { get; set; }

    /// <summary>
    /// Remote party's phone number.
    /// </summary>
    [JsonPropertyName("remoteCallerNumber")]
    public string? RemoteCallerNumber { get; set; }

    /// <summary>
    /// Enable audio in the call (defaults to true).
    /// </summary>
    [JsonPropertyName("audio")]
    public bool? Audio { get; set; } = true;

    /// <summary>
    /// Enable video in the call (defaults to false).
    /// </summary>
    [JsonPropertyName("video")]
    public bool? Video { get; set; } = false;

    /// <summary>
    /// Enable stereo audio transmission.
    /// </summary>
    [JsonPropertyName("useStereo")]
    public bool? UseStereo { get; set; }

    /// <summary>
    /// Attach to an existing call.
    /// </summary>
    [JsonPropertyName("attach")]
    public bool? Attach { get; set; }

    /// <summary>
    /// Indicates this is a screen sharing call.
    /// </summary>
    [JsonPropertyName("screenShare")]
    public bool? ScreenShare { get; set; }

    /// <summary>
    /// Local video/audio element or element ID to render local media.
    /// </summary>
    [JsonPropertyName("localElement")]
    public object? LocalElement { get; set; }

    /// <summary>
    /// Remote video/audio element or element ID to render remote media.
    /// </summary>
    [JsonPropertyName("remoteElement")]
    public object? RemoteElement { get; set; }

    /// <summary>
    /// Selected microphone device ID.
    /// </summary>
    [JsonPropertyName("micId")]
    public string? MicId { get; set; }

    /// <summary>
    /// Selected microphone label.
    /// </summary>
    [JsonPropertyName("micLabel")]
    public string? MicLabel { get; set; }

    /// <summary>
    /// Selected camera device ID.
    /// </summary>
    [JsonPropertyName("camId")]
    public string? CamId { get; set; }

    /// <summary>
    /// Selected camera label.
    /// </summary>
    [JsonPropertyName("camLabel")]
    public string? CamLabel { get; set; }

    /// <summary>
    /// Selected speaker output device ID.
    /// </summary>
    [JsonPropertyName("speakerId")]
    public string? SpeakerId { get; set; }

    /// <summary>
    /// Enable or disable negotiation for audio streams.
    /// </summary>
    [JsonPropertyName("negotiateAudio")]
    public bool? NegotiateAudio { get; set; }

    /// <summary>
    /// Enable or disable negotiation for video streams.
    /// </summary>
    [JsonPropertyName("negotiateVideo")]
    public bool? NegotiateVideo { get; set; }

    /// <summary>
    /// Maximum bitrate for audio/video streams in bits per second.
    /// </summary>
    [JsonPropertyName("googleMaxBitrate")]
    public int? GoogleMaxBitrate { get; set; }

    /// <summary>
    /// Minimum bitrate for audio/video streams in bits per second.
    /// </summary>
    [JsonPropertyName("googleMinBitrate")]
    public int? GoogleMinBitrate { get; set; }

    /// <summary>
    /// Initial bitrate for audio/video streams in bits per second.
    /// </summary>
    [JsonPropertyName("googleStartBitrate")]
    public int? GoogleStartBitrate { get; set; }

    /// <summary>
    /// Media constraint and SDP bandwidth settings.
    /// </summary>
    [JsonPropertyName("mediaSettings")]
    public TelnyxMediaSettings? MediaSettings { get; set; }

    /// <summary>
    /// Preferred audio codecs in priority order.
    /// </summary>
    [JsonPropertyName("preferred_codecs")]
    public List<TelnyxRtpCodecCapability>? PreferredCodecs { get; set; }

    /// <summary>
    /// Enable detailed debugging logs.
    /// </summary>
    [JsonPropertyName("debug")]
    public bool? Debug { get; set; }

    /// <summary>
    /// Destination for debug logs (e.g., "file" or "socket").
    /// </summary>
    [JsonPropertyName("debugOutput")]
    public string? DebugOutput { get; set; }

    /// <summary>
    /// Optional key-value pairs to pass into the call session.
    /// </summary>
    [JsonPropertyName("userVariables")]
    public Dictionary<string, object>? UserVariables { get; set; }

    /// <summary>
    /// Custom SIP headers to include in signaling.
    /// </summary>
    [JsonPropertyName("customHeaders")]
    public List<TelnyxCustomHeader>? CustomHeaders { get; set; }

    /// <summary>
    /// Telnyx Call Control ID, used to continue or manage existing calls.
    /// </summary>
    [JsonPropertyName("telnyxCallControlId")]
    public string? TelnyxCallControlId { get; set; }

    /// <summary>
    /// Telnyx session ID for correlating related events.
    /// </summary>
    [JsonPropertyName("telnyxSessionId")]
    public string? TelnyxSessionId { get; set; }

    /// <summary>
    /// Telnyx leg ID for uniquely identifying the call leg.
    /// </summary>
    [JsonPropertyName("telnyxLegId")]
    public string? TelnyxLegId { get; set; }

    /// <summary>
    /// Enables prefetching of ICE candidates before negotiation.
    /// </summary>
    [JsonPropertyName("prefetchIceCandidates")]
    public bool? PrefetchIceCandidates { get; set; }

    /// <summary>
    /// Forces the use of relay-only ICE candidates.
    /// </summary>
    [JsonPropertyName("forceRelayCandidate")]
    public bool? ForceRelayCandidate { get; set; }
}