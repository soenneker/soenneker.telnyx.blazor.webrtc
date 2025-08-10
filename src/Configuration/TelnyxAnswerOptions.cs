using System.Text.Json.Serialization;

namespace Soenneker.Telnyx.Blazor.WebRtc.Configuration;

/// <summary>
/// Represents options used when answering a Telnyx WebRTC call.
/// </summary>
public class TelnyxAnswerOptions
{
    /// <summary>
    /// Indicates whether video should be enabled when answering the call.
    /// </summary>
    [JsonPropertyName("video")]
    public bool? Video { get; set; }

    /// <summary>
    /// An optional array of custom SIP headers to include in the answer request.
    /// </summary>
    [JsonPropertyName("customHeaders")]
    public string[]? CustomHeaders { get; set; }

    /// <summary>
    /// An optional array of preferred media codecs to use during the call.
    /// The format and type of each object depends on the codec negotiation logic.
    /// </summary>
    [JsonPropertyName("preferredCodecs")]
    public object[]? PreferredCodecs { get; set; }

    [JsonPropertyName("clientState")]
    public string? ClientState { get; set; }
}