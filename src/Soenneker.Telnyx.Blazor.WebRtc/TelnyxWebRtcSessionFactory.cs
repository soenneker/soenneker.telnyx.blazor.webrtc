using Microsoft.Extensions.Logging;
using Soenneker.Telnyx.Blazor.WebRtc.Abstract;
namespace Soenneker.Telnyx.Blazor.WebRtc;

public sealed class TelnyxWebRtcSessionFactory(ITelnyxWebRtcInterop interop, ILogger<TelnyxWebRtcSession> logger) : ITelnyxWebRtcSessionFactory
{
    public ITelnyxWebRtcSession Create() => new TelnyxWebRtcSession(interop, logger);
}
