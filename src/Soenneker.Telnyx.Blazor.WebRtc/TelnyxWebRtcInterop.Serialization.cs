using Soenneker.Utils.Json;

namespace Soenneker.Telnyx.Blazor.WebRtc;

public sealed partial class TelnyxWebRtcInterop
{
    private static string? SerializePayload<T>(T value) => JsonUtil.Serialize(value, LibraryJsonContext.Get<T>());
}
