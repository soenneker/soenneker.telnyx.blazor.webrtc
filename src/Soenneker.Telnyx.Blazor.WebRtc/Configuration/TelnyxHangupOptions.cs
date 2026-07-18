using System.Text.Json.Serialization;

namespace Soenneker.Telnyx.Blazor.WebRtc.Configuration;

/// <summary>
/// Represents additional options and metadata for hanging up a Telnyx WebRTC call.
/// </summary>
public sealed class TelnyxHangupOptions
{
    /// <summary>
    /// A string description of the reason for the hangup.
    /// </summary>
    [JsonPropertyName("cause")]
    public string? Cause { get; set; }

    /// <summary>
    /// An optional numeric code representing the cause of the hangup.
    /// </summary>
    [JsonPropertyName("causeCode")]
    public int? CauseCode { get; set; }

    /// <summary>
    /// The SIP status code associated with the hangup, if applicable.
    /// </summary>
    [JsonPropertyName("sipCode")]
    public int? SipCode { get; set; }

    /// <summary>
    /// A textual reason provided in the SIP signaling for the hangup.
    /// </summary>
    [JsonPropertyName("sipReason")]
    public string? SipReason { get; set; }

    /// <summary>
    /// The SIP Call-ID header value associated with the session being terminated.
    /// </summary>
    [JsonPropertyName("sip_call_id")]
    public string? SipCallId { get; set; }

    /// <summary>
    /// Optional object containing dialog parameters or custom signaling values relevant to the hangup.
    /// </summary>
    [JsonPropertyName("dialogParams")]
    public object? DialogParams { get; set; }

    [JsonPropertyName("isRecovering")]
    public bool? IsRecovering { get; set; }

    [JsonPropertyName("initiator")]
    public string? Initiator { get; set; }

    [JsonIgnore]
    public bool? Execute { get; set; }
}
