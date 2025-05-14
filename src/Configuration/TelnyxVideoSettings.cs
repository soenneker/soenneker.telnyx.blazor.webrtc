namespace Soenneker.Telnyx.Blazor.WebRtc.Configuration;

public class TelnyxVideoSettings
{
    public string? CamId { get; set; }
    public string? CamLabel { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public int? FrameRate { get; set; }
    public string? FacingMode { get; set; }
}