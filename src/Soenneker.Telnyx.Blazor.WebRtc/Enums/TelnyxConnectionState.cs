namespace Soenneker.Telnyx.Blazor.WebRtc.Enums;

/// <summary>Connection lifecycle; registration is distinct from an open socket.</summary>
public enum TelnyxConnectionState
{
    Uninitialized, Initializing, Connecting, Registered, Reconnecting, Disconnected, Disposed
}
