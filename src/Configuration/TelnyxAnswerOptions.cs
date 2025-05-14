namespace Soenneker.Telnyx.Blazor.WebRtc.Configuration;

public class TelnyxAnswerOptions
{
    public bool? Video { get; set; }
    public string[]? CustomHeaders { get; set; }
    public object[]? PreferredCodecs { get; set; }
}