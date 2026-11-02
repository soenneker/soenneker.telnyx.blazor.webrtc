using System.Collections.Generic;
using System.Text.Json;

namespace Soenneker.Telnyx.Blazor.WebRtc.Dtos;

public sealed class TelnyxPreCallSummaryStats
{
    public TelnyxMinMaxAverage Jitter { get; set; } = new();
    public TelnyxMinMaxAverage Rtt { get; set; } = new();
    public double Mos { get; set; }
    public string? Quality { get; set; }
}
