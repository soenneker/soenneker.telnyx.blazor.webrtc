using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Soenneker.Telnyx.Blazor.WebRtc.Configuration;

/// <summary>Output sent back to a Telnyx AI conversation function call.</summary>
public sealed class TelnyxFunctionCallOutput
{
    [JsonPropertyName("type")] public string Type { get; set; } = "function_call_output";
    [JsonPropertyName("call_id")] public string CallId { get; set; } = null!;
    [JsonPropertyName("output")] public string Output { get; set; } = null!;
}
