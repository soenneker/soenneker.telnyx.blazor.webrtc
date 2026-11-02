using System.Collections.Generic;
using System.Text.Json;

namespace Soenneker.Telnyx.Blazor.WebRtc.Dtos;

public sealed class TelnyxPreCallDiagnosisReport
{
    public List<TelnyxIceCandidateStats> IceCandidateStats { get; set; } = [];
    public JsonElement IceCandidatePairStats { get; set; }
    public TelnyxPreCallSummaryStats SummaryStats { get; set; } = new();
    public TelnyxPreCallSessionStats SessionStats { get; set; } = new();
}
