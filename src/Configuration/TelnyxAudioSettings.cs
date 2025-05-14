namespace Soenneker.Telnyx.Blazor.WebRtc.Configuration;

public class TelnyxAudioSettings
{
    public string? MicId { get; set; }
    public string? MicLabel { get; set; }
    public bool? EchoCancellation { get; set; }
    public bool? NoiseSuppression { get; set; }
    public bool? AutoGainControl { get; set; }
}