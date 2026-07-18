using System.Text.Json.Serialization;

namespace Soenneker.Telnyx.Blazor.WebRtc.Configuration;

/// <summary>
/// Represents a STUN/TURN server configuration used for ICE.
/// </summary>
public sealed class TelnyxIceServer
{
    /// <summary>
    /// The STUN/TURN server URLs.
    /// </summary>
    [JsonPropertyName("urls")]
    public string[]? Urls { get; set; }

    /// <summary>
    /// Username for TURN authentication.
    /// </summary>
    [JsonPropertyName("username")]
    public string? Username { get; set; }

    /// <summary>
    /// Credential for TURN authentication.
    /// </summary>
    [JsonPropertyName("credential")]
    public string? Credential { get; set; }

    /// <summary>Credential mechanism, when supported by the browser (for example <c>password</c>).</summary>
    [JsonPropertyName("credentialType")]
    public string? CredentialType { get; set; }
}
