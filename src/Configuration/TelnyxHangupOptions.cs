namespace Soenneker.Telnyx.Blazor.WebRtc.Configuration;

public class TelnyxHangupOptions
{
    public string? Cause { get; set; }
    public int? CauseCode { get; set; }
    public string? SipCode { get; set; }
    public string? SipReason { get; set; }
    public string? SipCallId { get; set; }
    public object? DialogParams { get; set; }
}