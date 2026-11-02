using System.Collections.Generic;
using System.Text.Json;

namespace Soenneker.Telnyx.Blazor.WebRtc.Dtos;

public sealed class TelnyxMinMaxAverage
{
    public double Min { get; set; }
    public double Max { get; set; }
    public double Average { get; set; }
}
