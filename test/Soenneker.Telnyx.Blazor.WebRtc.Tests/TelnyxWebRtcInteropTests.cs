using Soenneker.Telnyx.Blazor.WebRtc.Abstract;
using Soenneker.Tests.HostedUnit;
using System.Threading.Tasks;
using Soenneker.Telnyx.Blazor.WebRtc.Enums;

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
    public async Task RegisteredFactoryCreatesAnUninitializedSession()
    {
        var factory = Resolve<ITelnyxWebRtcSessionFactory>(true);
        await using var session = factory.Create();
        await Assert.That(session.State).IsEqualTo(TelnyxConnectionState.Uninitialized);
    }
}
