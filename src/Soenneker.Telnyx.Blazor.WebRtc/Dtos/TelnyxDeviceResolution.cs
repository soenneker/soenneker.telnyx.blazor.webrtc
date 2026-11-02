using System.Collections.Generic;
using System.Text.Json;

namespace Soenneker.Telnyx.Blazor.WebRtc.Dtos;

public sealed class TelnyxDeviceResolution
{
    public string? Resolution { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}
