using Soenneker.Gen.EnumValues;

namespace Soenneker.Telnyx.Blazor.WebRtc.Enums;

/// <summary>
/// Represents the lifecycle states of a Telnyx WebRTC call.
/// </summary>
[EnumValue<string>]
public sealed partial class TelnyxCallState
{
    /// <summary>New call has been created in the client.</summary>
    public static readonly TelnyxCallState New = new("new");

    /// <summary>It's attempting to call someone.</summary>
    public static readonly TelnyxCallState Trying = new("trying");

    /// <summary>The outbound call is being sent to the server.</summary>
    public static readonly TelnyxCallState Requesting = new("requesting");

    /// <summary>
    /// The previous call is recovering after the page refreshes.
    /// If the user refreshes the page during a call, it will automatically join the latest call.
    /// </summary>
    public static readonly TelnyxCallState Recovering = new("recovering");

    /// <summary>Someone is attempting to call you.</summary>
    public static readonly TelnyxCallState Ringing = new("ringing");

    /// <summary>You are attempting to answer this inbound call.</summary>
    public static readonly TelnyxCallState Answering = new("answering");

    /// <summary>It receives the media before the call has been answered.</summary>
    public static readonly TelnyxCallState Early = new("early");

    /// <summary>Call has become active.</summary>
    public static readonly TelnyxCallState Active = new("active");

    /// <summary>Call has been held.</summary>
    public static readonly TelnyxCallState Held = new("held");

    /// <summary>Call has ended.</summary>
    public static readonly TelnyxCallState Hangup = new("hangup");

    /// <summary>Call has been destroyed.</summary>
    public static readonly TelnyxCallState Destroy = new("destroy");

    /// <summary>Call has been purged.</summary>
    public static readonly TelnyxCallState Purge = new("purge");
}