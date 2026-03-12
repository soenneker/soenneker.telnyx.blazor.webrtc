using System.Text.Json.Serialization;

namespace Soenneker.Telnyx.Blazor.WebRtc.Configuration;

/// <summary>
/// Represents codec preference settings for a call.
/// </summary>
public sealed class TelnyxRtpCodecCapability
{
    /// <summary>
    /// The MIME type of the codec (e.g. audio/opus).
    /// </summary>
    [JsonPropertyName("mimeType")]
    public string? MimeType { get; set; }

    /// <summary>
    /// The sampling rate for the codec (Hz).
    /// </summary>
    [JsonPropertyName("clockRate")]
    public int? ClockRate { get; set; }

    /// <summary>
    /// Number of channels (e.g. 2 for stereo).
    /// </summary>
    [JsonPropertyName("channels")]
    public int? Channels { get; set; }

    /// <summary>
    /// Additional SDP format parameters.
    /// </summary>
    [JsonPropertyName("sdpFmtpLine")]
    public string? SdpFmtpLine { get; set; }
}