using System.Text.Json.Serialization;

namespace Soenneker.Telnyx.Blazor.WebRtc.Configuration;

/// <summary>
/// Configures the Telnyx SDK media-permission recovery flow.
/// </summary>
public sealed class TelnyxMediaPermissionsRecoveryOptions
{
    /// <summary>
    /// Enables media-permission recovery.
    /// </summary>
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    /// <summary>
    /// Maximum recovery time in milliseconds.
    /// </summary>
    [JsonPropertyName("timeout")]
    public int Timeout { get; set; }
}
