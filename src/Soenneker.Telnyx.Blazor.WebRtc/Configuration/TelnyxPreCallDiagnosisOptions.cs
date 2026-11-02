using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Soenneker.Telnyx.Blazor.WebRtc.Configuration;

/// <summary>Options for the Telnyx pre-call diagnosis.</summary>
public sealed class TelnyxPreCallDiagnosisOptions
{
    [JsonPropertyName("texMLApplicationNumber")] public string TexMlApplicationNumber { get; set; } = null!;
    [JsonPropertyName("credentials")] public TelnyxPreCallDiagnosisCredentials Credentials { get; set; } = new();
}
