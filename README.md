[![](https://img.shields.io/nuget/v/soenneker.telnyx.blazor.webrtc.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.telnyx.blazor.webrtc/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.telnyx.blazor.webrtc/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.telnyx.blazor.webrtc/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.telnyx.blazor.webrtc.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.telnyx.blazor.webrtc/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.telnyx.blazor.webrtc/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.telnyx.blazor.webrtc/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Telnyx.Blazor.WebRtc

A Blazor component and JavaScript interop layer for Telnyx browser calling, including call control, media devices, screen sharing, DTMF, connection events, and call statistics.

## Installation

```bash
dotnet add package Soenneker.Telnyx.Blazor.WebRtc
```

Register the interop service in `Program.cs`:

```csharp
using Soenneker.Telnyx.Blazor.WebRtc.Registrars;

builder.Services.AddTelnyxWebRtcInteropAsScoped();
```

## Component usage

```razor
@using Soenneker.Telnyx.Blazor.WebRtc
@using Soenneker.Telnyx.Blazor.WebRtc.Configuration

<TelnyxWebRtc @ref="_rtc"
              Options="_options"
              OnReady="HandleReady"
              OnError="HandleError" />

<button @onclick="Call" disabled="@(!_ready)">Call</button>

@code {
    private TelnyxWebRtc? _rtc;
    private bool _ready;

    private readonly TelnyxClientOptions _options = new()
    {
        InitOptions = new TelnyxClientInitOptions
        {
            LoginToken = "short-lived-token-from-your-server"
        }
    };

    private void HandleReady() => _ready = true;

    private void HandleError(string error)
    {
        // Display or log the SDK error.
    }

    private async Task Call()
    {
        if (_rtc is null)
            return;

        await _rtc.Call(new TelnyxCallOptions
        {
            DestinationNumber = "+15551234567",
            CallerNumber = "+15557654321",
            Audio = true,
            Video = false
        });
    }
}
```

Generate browser login tokens on a trusted server. Do not embed Telnyx API keys or other privileged credentials in a Blazor WebAssembly application.

The component renders a hidden autoplay audio element by default. Set `RenderVideo="true"` for local and remote video elements, or `RenderHiddenAudio="false"` when the application supplies its own media elements.

The Telnyx browser SDK is loaded from its CDN by default. Set `UseCdn = false` in `TelnyxClientOptions` to load the packaged script instead. Microphone, camera, speaker selection, and screen sharing depend on browser support, user permission, and a secure browsing context.

Call operations such as `Answer`, `Hangup`, `Hold`, `Unhold`, `MuteAudio`, `Dtmf`, `StartScreenShare`, and device enumeration are available from the component reference. Use the component's `On...` parameters for SDK and call lifecycle events.
