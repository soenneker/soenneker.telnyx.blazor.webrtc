using System.Text.Json.Serialization;

namespace Soenneker.Telnyx.Blazor.WebRtc.Configuration;

/// <summary>
/// Represents a SIP custom header to include in call setup.
/// </summary>
public sealed class TelnyxCustomHeader
{
    /// <summary>
    /// The name of the SIP header.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// The value of the SIP header.
    /// </summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }
}