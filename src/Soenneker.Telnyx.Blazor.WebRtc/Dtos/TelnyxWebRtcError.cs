using System.Text.Json.Serialization;
using System.Text.Json;

namespace Soenneker.Telnyx.Blazor.WebRtc.Dtos;

/// <summary>
/// Represents error information in a notification.
/// </summary>
public sealed class TelnyxWebRtcError
{
    [JsonPropertyName("code")] public JsonElement? Code { get; set; }
    [JsonPropertyName("description")] public string? Description { get; set; }
    [JsonPropertyName("causes")] public string[]? Causes { get; set; }
    [JsonPropertyName("solutions")] public string[]? Solutions { get; set; }
    [JsonPropertyName("fatal")] public bool? Fatal { get; set; }
    [JsonPropertyName("recoverable")] public bool? Recoverable { get; set; }
    [JsonPropertyName("callId")] public string? CallId { get; set; }
    [JsonPropertyName("sessionId")] public string? SessionId { get; set; }

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