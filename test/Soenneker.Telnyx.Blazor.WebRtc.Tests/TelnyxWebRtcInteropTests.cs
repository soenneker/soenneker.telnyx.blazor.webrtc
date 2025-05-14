using Soenneker.Telnyx.Blazor.WebRtc.Abstract;
using Soenneker.Tests.FixturedUnit;
using Xunit;

namespace Soenneker.Telnyx.Blazor.WebRtc.Tests;

[Collection("Collection")]
public class TelnyxWebRtcInteropTests : FixturedUnitTest
{
    private readonly ITelnyxWebRtcInterop _blazorlibrary;

    public TelnyxWebRtcInteropTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
        _blazorlibrary = Resolve<ITelnyxWebRtcInterop>(true);
    }

    [Fact]
    public void Default()
    {

    }
}
