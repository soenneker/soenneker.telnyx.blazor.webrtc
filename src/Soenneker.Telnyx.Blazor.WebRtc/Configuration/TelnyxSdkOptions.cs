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

/// <summary>Output sent back to a Telnyx AI conversation function call.</summary>
public sealed class TelnyxFunctionCallOutput
{
    [JsonPropertyName("type")] public string Type { get; set; } = "function_call_output";
    [JsonPropertyName("call_id")] public string CallId { get; set; } = null!;
    [JsonPropertyName("output")] public string Output { get; set; } = null!;
}

/// <summary>Options for the Telnyx pre-call diagnosis.</summary>
public sealed class TelnyxPreCallDiagnosisOptions
{
    [JsonPropertyName("texMLApplicationNumber")] public string TexMlApplicationNumber { get; set; } = null!;
    [JsonPropertyName("credentials")] public TelnyxPreCallDiagnosisCredentials Credentials { get; set; } = new();
}

/// <summary>Credentials for the Telnyx pre-call diagnosis.</summary>
public sealed class TelnyxPreCallDiagnosisCredentials
{
    [JsonPropertyName("login")] public string? Login { get; set; }
    [JsonPropertyName("password")] public string? Password { get; set; }
    [JsonPropertyName("loginToken")] public string? LoginToken { get; set; }
}
