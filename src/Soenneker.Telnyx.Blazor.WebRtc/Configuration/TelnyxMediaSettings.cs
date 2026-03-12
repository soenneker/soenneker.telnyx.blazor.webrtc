using System.Text.Json.Serialization;

namespace Soenneker.Telnyx.Blazor.WebRtc.Configuration;

/// <summary>
/// Media constraints and optional bandwidth limitation for SDP negotiation.
/// </summary>
public sealed class TelnyxMediaSettings
{
    /// <summary>
    /// Bandwidth limit for audio/video in kilobits per second.
    /// </summary>
    [JsonPropertyName("sdpASBandwidthKbps")]
    public int? SdpASBandwidthKbps { get; set; }

    /// <summary>
    /// Enables enforcement of the above bandwidth limit in SDP.
    /// </summary>
    [JsonPropertyName("useSdpASBandwidthKbps")]
    public bool? UseSdpASBandwidthKbps { get; set; }
}