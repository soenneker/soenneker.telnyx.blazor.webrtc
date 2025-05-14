using Microsoft.JSInterop;
using Soenneker.Telnyx.Blazor.WebRtc.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Telnyx.Blazor.WebRtc.Abstract;

/// <summary>
/// Blazor interop library for the Telnyx WebRTC client, providing full access to Telnyx's browser-based voice and video calling features. Includes typed wrappers, event bridging, and support for advanced call control in Blazor WebAssembly apps.
/// </summary>
public interface ITelnyxWebRtcInterop : IAsyncDisposable
{
    ValueTask Initialize(CancellationToken cancellationToken = default);

    ValueTask Create(string elementId, DotNetObjectReference<TelnyxWebRtc> dotNetObjectRef,
        TelnyxClientOptions options, CancellationToken cancellationToken = default);

    ValueTask CreateObserver(string elementId, CancellationToken cancellationToken = default);

    ValueTask Call(string elementId, TelnyxCallOptions callOptions, CancellationToken cancellationToken = default);

    ValueTask Answer(string elementId, TelnyxAnswerOptions? options = null, CancellationToken cancellationToken = default);

    ValueTask Hangup(string elementId, TelnyxHangupOptions? options = null, CancellationToken cancellationToken = default);

    ValueTask MuteAudio(string elementId, CancellationToken cancellationToken = default);

    ValueTask UnmuteAudio(string elementId, CancellationToken cancellationToken = default);

    ValueTask ToggleAudioMute(string elementId, CancellationToken cancellationToken = default);

    ValueTask MuteVideo(string elementId, CancellationToken cancellationToken = default);

    ValueTask UnmuteVideo(string elementId, CancellationToken cancellationToken = default);

    ValueTask ToggleVideoMute(string elementId, CancellationToken cancellationToken = default);

    ValueTask Deaf(string elementId, CancellationToken cancellationToken = default);

    ValueTask Undeaf(string elementId, CancellationToken cancellationToken = default);

    ValueTask ToggleDeaf(string elementId, CancellationToken cancellationToken = default);

    ValueTask Hold(string elementId, CancellationToken cancellationToken = default);

    ValueTask Unhold(string elementId, CancellationToken cancellationToken = default);

    ValueTask ToggleHold(string elementId, CancellationToken cancellationToken = default);

    ValueTask Dtmf(string elementId, string digit, CancellationToken cancellationToken = default);

    ValueTask Message(string elementId, string to, string body, CancellationToken cancellationToken = default);

    ValueTask SetAudioInDevice(string elementId, string deviceId, CancellationToken cancellationToken = default);

    ValueTask SetVideoDevice(string elementId, string deviceId, CancellationToken cancellationToken = default);

    ValueTask SetAudioOutDevice(string elementId, string deviceId, CancellationToken cancellationToken = default);

    ValueTask StartScreenShare(string elementId, TelnyxScreenShareOptions? options = null, CancellationToken cancellationToken = default);

    ValueTask StopScreenShare(string elementId, CancellationToken cancellationToken = default);

    ValueTask SetAudioBandwidth(string elementId, int bps, CancellationToken cancellationToken = default);

    ValueTask SetVideoBandwidth(string elementId, int bps, CancellationToken cancellationToken = default);

    ValueTask<string> GetDevices(string elementId, CancellationToken cancellationToken = default);

    ValueTask<string> GetVideoDevices(string elementId, CancellationToken cancellationToken = default);

    ValueTask<string> GetAudioInDevices(string elementId, CancellationToken cancellationToken = default);

    ValueTask<string> GetAudioOutDevices(string elementId, CancellationToken cancellationToken = default);

    ValueTask<bool> CheckPermissions(string elementId, bool audio = true, bool video = true, CancellationToken cancellationToken = default);

    ValueTask<bool> SetAudioSettings(string elementId, TelnyxAudioSettings settings, CancellationToken cancellationToken = default);

    ValueTask<bool> SetVideoSettings(string elementId, TelnyxVideoSettings settings, CancellationToken cancellationToken = default);

    ValueTask EnableMicrophone(string elementId, CancellationToken cancellationToken = default);

    ValueTask DisableMicrophone(string elementId, CancellationToken cancellationToken = default);

    ValueTask EnableWebcam(string elementId, CancellationToken cancellationToken = default);

    ValueTask DisableWebcam(string elementId, CancellationToken cancellationToken = default);

    ValueTask ToggleAudio(string elementId, bool enabled, CancellationToken cancellationToken = default);

    ValueTask ToggleVideo(string elementId, bool enabled, CancellationToken cancellationToken = default);

    ValueTask Disconnect(string elementId, CancellationToken cancellationToken = default);

    ValueTask Reconnect(string elementId, CancellationToken cancellationToken = default);

    ValueTask Unmount(string elementId, CancellationToken cancellationToken = default);

    ValueTask ListVideoLayouts(string elementId, CancellationToken cancellationToken = default);

    ValueTask SetVideoLayout(string elementId, string layout, string? canvas = null, CancellationToken cancellationToken = default);

    ValueTask PlayMedia(string elementId, string source, CancellationToken cancellationToken = default);

    ValueTask StopMedia(string elementId, CancellationToken cancellationToken = default);

    ValueTask StartRecord(string elementId, string filename, CancellationToken cancellationToken = default);

    ValueTask StopRecord(string elementId, CancellationToken cancellationToken = default);

    ValueTask SendChatMessage(string elementId, string message, string? type = null, CancellationToken cancellationToken = default);

    ValueTask Snapshot(string elementId, string filename, CancellationToken cancellationToken = default);

    ValueTask MuteMic(string elementId, string participantId, CancellationToken cancellationToken = default);

    ValueTask MuteVideoParticipant(string elementId, string participantId, CancellationToken cancellationToken = default);

    ValueTask Kick(string elementId, string participantId, CancellationToken cancellationToken = default);

    ValueTask VolumeUp(string elementId, string participantId, CancellationToken cancellationToken = default);

    ValueTask VolumeDown(string elementId, string participantId, CancellationToken cancellationToken = default);

    ValueTask<string?> GetCallStats(string elementId, CancellationToken cancellationToken = default);
}