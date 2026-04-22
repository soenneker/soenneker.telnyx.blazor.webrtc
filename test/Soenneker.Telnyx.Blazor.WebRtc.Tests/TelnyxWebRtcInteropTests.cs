using Soenneker.Telnyx.Blazor.WebRtc.Abstract;
using Soenneker.Tests.HostedUnit;

namespace Soenneker.Telnyx.Blazor.WebRtc.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public class TelnyxWebRtcInteropTests : HostedUnitTest
{
    private readonly ITelnyxWebRtcInterop _blazorlibrary;

    public TelnyxWebRtcInteropTests(Host host) : base(host)
    {
        _blazorlibrary = Resolve<ITelnyxWebRtcInterop>(true);
    }

    [Test]
    public void Default()
    {

    }
}
