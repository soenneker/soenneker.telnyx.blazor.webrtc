using System.Text.Json.Serialization;

namespace Soenneker.Telnyx.Blazor.WebRtc.Configuration;

/// <summary>
/// Represents options used when answering a Telnyx WebRTC call.
/// </summary>
public sealed class TelnyxAnswerOptions
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
    public TelnyxCustomHeader[]? CustomHeaders { get; set; }

    /// <summary>
    /// DOM element ID used to render local media for this call.
    /// </summary>
    [JsonPropertyName("localElement")]
    public string? LocalElement { get; set; }

    /// <summary>
    /// DOM element ID used to render remote media for this call.
    /// </summary>
    [JsonPropertyName("remoteElement")]
    public string? RemoteElement { get; set; }

    [JsonIgnore]
    public Microsoft.JSInterop.IJSObjectReference? LocalElementReference { get; set; }

    [JsonIgnore]
    public Microsoft.JSInterop.IJSObjectReference? RemoteElementReference { get; set; }
}
