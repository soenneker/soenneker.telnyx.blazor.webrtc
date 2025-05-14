using System.Text.Json.Serialization;

namespace Soenneker.Telnyx.Blazor.WebRtc.Dtos;

/// <summary>
/// Represents error information in a notification.
/// </summary>
public sealed class TelnyxWebRtcError
{
    /// <summary>
    /// The error message describing the issue.
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    /// <summary>
    /// The name or type of the error.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// The stack trace associated with the error, if available.
    /// </summary>
    [JsonPropertyName("stack")]
    public string? Stack { get; set; }
}