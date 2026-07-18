using System.Text.Json.Serialization;

namespace Soenneker.Telnyx.Blazor.WebRtc.Configuration;

/// <summary>
/// Represents anonymous login options.
/// </summary>
public sealed class TelnyxAnonymousLoginOptions
{
    /// <summary>
    /// The target ID to use for the anonymous login. This is typically the ID of the AI assistant you want to connect to.
    /// </summary>
    [JsonPropertyName("target_id")]
    public string? TargetId { get; set; }

    /// <summary>
    /// A string indicating the target type. For now, only 'ai_assistant' is supported.
    /// </summary>
    [JsonPropertyName("target_type")]
    public string? TargetType { get; set; }

    /// <summary>
    /// Optional version identifier for the anonymous target.
    /// </summary>
    [JsonPropertyName("target_version_id")]
    public string? TargetVersionId { get; set; }

    /// <summary>
    /// Optional target-specific parameters.
    /// </summary>
    [JsonPropertyName("target_params")]
    public System.Collections.Generic.Dictionary<string, object>? TargetParams { get; set; }
}
