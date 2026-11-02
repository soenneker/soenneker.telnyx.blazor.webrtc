using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Soenneker.Telnyx.Blazor.WebRtc.Configuration;

/// <summary>Credentials for the Telnyx pre-call diagnosis.</summary>
public sealed class TelnyxPreCallDiagnosisCredentials
{
    [JsonPropertyName("login")] public string? Login { get; set; }
    [JsonPropertyName("password")] public string? Password { get; set; }
    [JsonPropertyName("login_token")] public string? LoginToken { get; set; }
}

