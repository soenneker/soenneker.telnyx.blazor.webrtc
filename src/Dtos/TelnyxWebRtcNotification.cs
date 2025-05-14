using System.Text.Json.Serialization;

namespace Soenneker.Telnyx.Blazor.WebRtc.Dtos;

/// <summary>
/// Represents a notification event dispatched by Telnyx to notify the client of changes to the session or call.
/// </summary>
public sealed class TelnyxNotification
{
    /// <summary>
    /// Identifies the event type. Possible values include 'callUpdate' and 'userMediaError'.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; }

    /// <summary>
    /// The current call information. Present when the notification type is 'callUpdate'.
    /// </summary>
    [JsonPropertyName("call")]
    public TelnyxWebRtcCall? Call { get; set; }

    /// <summary>
    /// Error information. Present when the notification type is 'userMediaError'.
    /// </summary>
    [JsonPropertyName("error")]
    public TelnyxWebRtcError? Error { get; set; }
}