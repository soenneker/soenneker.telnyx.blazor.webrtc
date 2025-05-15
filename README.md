[![](https://img.shields.io/nuget/v/soenneker.telnyx.blazor.webrtc.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.telnyx.blazor.webrtc/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.telnyx.blazor.webrtc/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.telnyx.blazor.webrtc/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.telnyx.blazor.webrtc.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.telnyx.blazor.webrtc/)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Telnyx.Blazor.WebRtc
A Blazor WebRTC component library for Telnyx, enabling real-time communication capabilities in your Blazor applications.

## Key Features

* ✅ **Real-time WebRTC Communication**
  Seamless voice and video calling powered by Telnyx’s browser-based WebRTC SDK.

* ⚙️ **Automatic JS Module Bootstrapping**
  Built-in JavaScript interop initialization with lazy loading and Blazor lifecycle integration.

* 🎥 **High-Quality Audio & Video Calls**
  Support for two-way audio and video with full control over microphone and webcam devices.

* 📞 **Advanced Call Management**
  Programmatic control over call lifecycle: initiate, answer, hangup, hold, resume, mute/unmute, deaf/undeaf, DTMF tones, and more.

* 🎛 **Device Enumeration & Dynamic Selection**
  Enumerate and switch between available microphones, speakers, and cameras at runtime.

* 📡 **Custom Headers & Signaling Options**
  Pass custom SIP headers during call setup for advanced routing or metadata requirements.

* 🌐 **ICE Server & TURN/STUN Configuration**
  Fully configurable ICE server settings for NAT traversal and improved connectivity in restricted networks.

* 📢 **Comprehensive Event Notifications**
  Capture and handle all Telnyx WebRTC events: connection, media stream, call state, device changes, stats reports, and more.

## Installation

```shell
dotnet add package Soenneker.Telnyx.Blazor.WebRtc
```

## Setup

### 1. Register Services

In your `Program.cs` or startup file:

```csharp
public static async Task Main(string[] args)
{
    var builder = WebApplication.CreateBuilder(args);

    // Register Telnyx WebRTC services
    builder.Services.AddTelnyxWebRtcInteropAsScoped();
}
```

### 2. Component Usage

Add the TelnyxWebRtc component to your Blazor page:

```razor
@using Soenneker.Telnyx.Blazor.WebRtc

<TelnyxWebRtc @ref="_telnyxWebRtc" Options="@_options" />

@code {
    private ITelnyxWebRtc? _telnyxWebRtc;
    private TelnyxClientOptions _options;

    protected override void OnInitialized()
    {
        _options = new TelnyxClientOptions
        {
           InitOptions = new TelnyxClientInitOptions
           {
              Login = "YOUR_TELNYX_LOGIN",
              Password = "YOUR_TELNYX_PASSWORD"
              // Set other properties as needed
           }
        };
    }
}
```

## Events

The component provides various events you can subscribe to:

```
_telnyxWebRtc.OnInitialize += HandleInitialize;
_telnyxWebRtc.OnReady += HandleReady;
_telnyxWebRtc.OnError += HandleError;
_telnyxWebRtc.OnMessage += HandleMessage;
_telnyxWebRtc.OnNotification += HandleNotification;
_telnyxWebRtc.OnCallInitiated += HandleCallInitiated;
_telnyxWebRtc.OnCallAnswered += HandleCallAnswered;
_telnyxWebRtc.OnCallHeld += HandleCallHeld;
_telnyxWebRtc.OnCallResumed += HandleCallResumed;
_telnyxWebRtc.OnCallHangup += HandleCallHangup;
...
```

## Example Usage

### Making a Call

```csharp
var callOptions = new TelnyxCallOptions
{
    DestinationNumber = "+1234567890",
    CallerName = "John Doe",
    CallerNumber = "+1987654321"
};

await _telnyxWebRtc.Call(callOptions);
```

### Answering a Call

```csharp
var answerOptions = new TelnyxAnswerOptions
{
    AutoPlayAudio = true
};

await _telnyxWebRtc.Answer(answerOptions);
```

### Hanging Up

```csharp
await _telnyxWebRtc.Hangup();
```