using System.Collections.Generic;
using System.Text.Json;

namespace Soenneker.Telnyx.Blazor.WebRtc.Dtos;

public sealed class TelnyxPreCallSessionStats
{
    public double PacketsReceived { get; set; }
    public double PacketsLost { get; set; }
    public double PacketsSent { get; set; }
    public double BytesSent { get; set; }
    public double BytesReceived { get; set; }
}
