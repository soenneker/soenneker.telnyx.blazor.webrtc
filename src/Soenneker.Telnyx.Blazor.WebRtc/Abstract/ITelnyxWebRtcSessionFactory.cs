namespace Soenneker.Telnyx.Blazor.WebRtc.Abstract;

/// <summary>Creates independently owned sessions using scoped browser interop.</summary>
public interface ITelnyxWebRtcSessionFactory
{
    /// <summary>Creates a session. The caller owns its initialization and disposal.</summary>
    ITelnyxWebRtcSession Create();
}
