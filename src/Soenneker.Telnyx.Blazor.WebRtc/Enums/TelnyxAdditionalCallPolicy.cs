namespace Soenneker.Telnyx.Blazor.WebRtc.Enums;

/// <summary>Policy for another call while the selected call is alive. Notify leaves the additional call unanswered; screen-share child calls are excluded.</summary>
public enum TelnyxAdditionalCallPolicy
{
    Reject, Notify
}
