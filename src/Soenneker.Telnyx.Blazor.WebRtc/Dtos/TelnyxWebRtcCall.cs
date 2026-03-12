using Soenneker.Telnyx.Blazor.WebRtc.Configuration;
using Soenneker.Telnyx.Blazor.WebRtc.Enums;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Soenneker.Telnyx.Blazor.WebRtc.Dtos;

/// <summary>
/// Represents a WebRTC call instance in the Telnyx SDK.
/// </summary>
public sealed class TelnyxWebRtcCall
{
    /// <summary>
    /// Unique identifier for the call.
    /// </summary>
    [JsonPropertyName("callId")]
    public string? CallId { get; set; }

    /// <summary>
    /// The current state of the call (e.g., 'new', 'ringing', 'active', 'held', 'ended').
    /// </summary>
    [JsonPropertyName("state")]
    public TelnyxCallState? State { get; set; }

    /// <summary>
    /// The direction of the call ('inbound' or 'outbound').
    /// </summary>
    [JsonPropertyName("direction")]
    public string? Direction { get; set; }

    /// <summary>
    /// The caller's phone number or SIP URI.
    /// </summary>
    [JsonPropertyName("callerNumber")]
    public string? CallerNumber { get; set; }

    /// <summary>
    /// The caller's name.
    /// </summary>
    [JsonPropertyName("callerName")]
    public string? CallerName { get; set; }

    /// <summary>
    /// The callee's phone number or SIP URI.
    /// </summary>
    [JsonPropertyName("calleeNumber")]
    public string? CalleeNumber { get; set; }

    /// <summary>
    /// The callee's name.
    /// </summary>
    [JsonPropertyName("calleeName")]
    public string? CalleeName { get; set; }

    /// <summary>
    /// Indicates whether the call is muted.
    /// </summary>
    [JsonPropertyName("isMuted")]
    public bool? IsMuted { get; set; }

    /// <summary>
    /// Indicates whether the call is on hold.
    /// </summary>
    [JsonPropertyName("isOnHold")]
    public bool? IsOnHold { get; set; }

    /// <summary>
    /// The start time of the call.
    /// </summary>
    [JsonPropertyName("startTime")]
    public DateTimeOffset? StartTime { get; set; }

    /// <summary>
    /// The end time of the call.
    /// </summary>
    [JsonPropertyName("endTime")]
    public DateTimeOffset? EndTime { get; set; }

    /// <summary>
    /// The duration of the call in seconds.
    /// </summary>
    [JsonPropertyName("duration")]
    public int? Duration { get; set; }

    /// <summary>
    /// Custom headers associated with the call.
    /// </summary>
    [JsonPropertyName("customHeaders")]
    public Dictionary<string, string>? CustomHeaders { get; set; }

    /// <summary>
    /// The local media stream associated with the call.
    /// </summary>
    [JsonPropertyName("localStream")]
    public object? LocalStream { get; set; } // Placeholder for MediaStream

    /// <summary>
    /// The remote media stream associated with the call.
    /// </summary>
    [JsonPropertyName("remoteStream")]
    public object? RemoteStream { get; set; } // Placeholder for MediaStream

    /// <summary>
    /// The Telnyx session ID associated with the call.
    /// </summary>
    [JsonPropertyName("sessionId")]
    public string? SessionId { get; set; }

    /// <summary>
    /// The Telnyx leg ID associated with the call.
    /// </summary>
    [JsonPropertyName("legId")]
    public string? LegId { get; set; }

    /// <summary>
    /// The Telnyx call control ID associated with the call.
    /// </summary>
    [JsonPropertyName("callControlId")]
    public string? CallControlId { get; set; }

    /// <summary>
    /// Indicates whether the call is using stereo audio.
    /// </summary>
    [JsonPropertyName("useStereo")]
    public bool? UseStereo { get; set; }

    /// <summary>
    /// The codec used for the call.
    /// </summary>
    [JsonPropertyName("codec")]
    public string? Codec { get; set; }

    /// <summary>
    /// The region associated with the call.
    /// </summary>
    [JsonPropertyName("region")]
    public string? Region { get; set; }

    /// <summary>
    /// The ringback tone file URL used for the call.
    /// </summary>
    [JsonPropertyName("ringbackFile")]
    public string? RingbackFile { get; set; }

    /// <summary>
    /// The ringtone file URL used for the call.
    /// </summary>
    [JsonPropertyName("ringtoneFile")]
    public string? RingtoneFile { get; set; }

    /// <summary>
    /// The media settings associated with the call.
    /// </summary>
    [JsonPropertyName("mediaSettings")]
    public TelnyxMediaSettings? MediaSettings { get; set; }

    /// <summary>
    /// The ICE servers used for the call.
    /// </summary>
    [JsonPropertyName("iceServers")]
    public List<TelnyxIceServer>? IceServers { get; set; }

    /// <summary>
    /// Preferred codecs for the call.
    /// </summary>
    [JsonPropertyName("preferredCodecs")]
    public List<TelnyxRtpCodecCapability>? PreferredCodecs { get; set; }

    /// <summary>
    /// Indicates whether ICE candidates are prefetched.
    /// </summary>
    [JsonPropertyName("prefetchIceCandidates")]
    public bool? PrefetchIceCandidates { get; set; }

    /// <summary>
    /// The microphone device ID used for the call.
    /// </summary>
    [JsonPropertyName("micId")]
    public string? MicId { get; set; }

    /// <summary>
    /// The camera device ID used for the call.
    /// </summary>
    [JsonPropertyName("camId")]
    public string? CamId { get; set; }

    /// <summary>
    /// The speaker device ID used for the call.
    /// </summary>
    [JsonPropertyName("speakerId")]
    public string? SpeakerId { get; set; }

    /// <summary>
    /// The local element selector for rendering local media.
    /// </summary>
    [JsonPropertyName("localElement")]
    public string? LocalElement { get; set; }

    /// <summary>
    /// The remote element selector for rendering remote media.
    /// </summary>
    [JsonPropertyName("remoteElement")]
    public string? RemoteElement { get; set; }

    /// <summary>
    /// Indicates whether the call is using video.
    /// </summary>
    [JsonPropertyName("video")]
    public bool? Video { get; set; }

    /// <summary>
    /// Indicates whether the call is using audio.
    /// </summary>
    [JsonPropertyName("audio")]
    public bool? Audio { get; set; }

    /// <summary>
    /// The client state associated with the call.
    /// </summary>
    [JsonPropertyName("clientState")]
    public string? ClientState { get; set; }

    /// <summary>
    /// Indicates whether the call is using a relay ICE candidate.
    /// </summary>
    [JsonPropertyName("forceRelayCandidate")]
    public bool? ForceRelayCandidate { get; set; }

    /// <summary>
    /// Indicates whether debug mode is enabled for the call.
    /// </summary>
    [JsonPropertyName("debug")]
    public bool? Debug { get; set; }

    /// <summary>
    /// The debug output destination for the call.
    /// </summary>
    [JsonPropertyName("debugOutput")]
    public string? DebugOutput { get; set; }

    /// <summary>
    /// The anonymous login options associated with the call.
    /// </summary>
    [JsonPropertyName("anonymousLogin")]
    public TelnyxAnonymousLoginOptions? AnonymousLogin { get; set; }
}