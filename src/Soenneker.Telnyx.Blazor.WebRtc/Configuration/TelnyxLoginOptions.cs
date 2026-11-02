using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Soenneker.Telnyx.Blazor.WebRtc.Configuration;

/// <summary>Credentials accepted by <c>TelnyxRTC.login</c>.</summary>
public sealed class TelnyxLoginOptions
{
    [JsonPropertyName("login")] public string? Login { get; set; }
    [JsonPropertyName("password")] public string? Password { get; set; }
    [JsonPropertyName("passwd")] public string? Passwd { get; set; }
    [JsonPropertyName("login_token")] public string? LoginToken { get; set; }
    [JsonPropertyName("userVariables")] public Dictionary<string, object>? UserVariables { get; set; }
    [JsonPropertyName("anonymous_login")] public TelnyxAnonymousLoginOptions? AnonymousLogin { get; set; }
}
