using Soenneker.Telnyx.Blazor.WebRtc.Configuration;
using Soenneker.Telnyx.Blazor.WebRtc.Dtos;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Telnyx.Blazor.WebRtc.Abstract;

public interface ITelnyxWebRtc : IAsyncDisposable
{
    string ElementId { get; }

    ValueTask Initialize(TelnyxClientOptions? options = null, CancellationToken cancellationToken = default);

    ValueTask Call(TelnyxCallOptions callOptions, CancellationToken cancellationToken = default);

    ValueTask Answer(TelnyxAnswerOptions? options = null, CancellationToken cancellationToken = default);

    ValueTask Hangup(TelnyxHangupOptions? options = null, CancellationToken cancellationToken = default);

    ValueTask MuteAudio(CancellationToken cancellationToken = default);

    ValueTask UnmuteAudio(CancellationToken cancellationToken = default);

    ValueTask ToggleAudioMute(CancellationToken cancellationToken = default);

    ValueTask MuteVideo(CancellationToken cancellationToken = default);

    ValueTask UnmuteVideo(CancellationToken cancellationToken = default);

    ValueTask ToggleVideoMute(CancellationToken cancellationToken = default);

    ValueTask Deaf(CancellationToken cancellationToken = default);

    ValueTask Undeaf(CancellationToken cancellationToken = default);

    ValueTask ToggleDeaf(CancellationToken cancellationToken = default);

    ValueTask Hold(CancellationToken cancellationToken = default);

    ValueTask Unhold(CancellationToken cancellationToken = default);

    ValueTask ToggleHold(CancellationToken cancellationToken = default);

    ValueTask Dtmf(string digit, CancellationToken cancellationToken = default);

    ValueTask Message(string to, string body, CancellationToken cancellationToken = default);

    ValueTask SetAudioInDevice(string deviceId, CancellationToken cancellationToken = default);

    ValueTask SetVideoDevice(string deviceId, CancellationToken cancellationToken = default);

    ValueTask SetAudioOutDevice(string deviceId, CancellationToken cancellationToken = default);

    ValueTask StartScreenShare(TelnyxScreenShareOptions? options = null, CancellationToken cancellationToken = default);

    ValueTask StopScreenShare(CancellationToken cancellationToken = default);

    ValueTask SetAudioBandwidth(int bps, CancellationToken cancellationToken = default);

    ValueTask SetVideoBandwidth(int bps, CancellationToken cancellationToken = default);

    ValueTask<List<MediaDeviceInfo>> GetDevices(CancellationToken cancellationToken = default);

    ValueTask<List<MediaDeviceInfo>> GetVideoDevices(CancellationToken cancellationToken = default);

    ValueTask<List<MediaDeviceInfo>> GetAudioInDevices(CancellationToken cancellationToken = default);

    ValueTask<List<MediaDeviceInfo>> GetAudioOutDevices(CancellationToken cancellationToken = default);

    ValueTask<bool> CheckPermissions(bool audio = true, bool video = true, CancellationToken cancellationToken = default);

    ValueTask<bool> SetAudioSettings(TelnyxAudioSettings settings, CancellationToken cancellationToken = default);

    ValueTask<bool> SetVideoSettings(TelnyxVideoSettings settings, CancellationToken cancellationToken = default);

    ValueTask EnableMicrophone(CancellationToken cancellationToken = default);

    ValueTask DisableMicrophone(CancellationToken cancellationToken = default);

    ValueTask EnableWebcam(CancellationToken cancellationToken = default);

    ValueTask DisableWebcam(CancellationToken cancellationToken = default);

    ValueTask ToggleAudio(bool enabled, CancellationToken cancellationToken = default);

    ValueTask ToggleVideo(bool enabled, CancellationToken cancellationToken = default);

    ValueTask Disconnect(CancellationToken cancellationToken = default);

    ValueTask Reconnect(CancellationToken cancellationToken = default);

    ValueTask ListVideoLayouts(CancellationToken cancellationToken = default);

    ValueTask SetVideoLayout(string layout, string? canvas = null, CancellationToken cancellationToken = default);

    ValueTask PlayMedia(string source, CancellationToken cancellationToken = default);

    ValueTask StopMedia(CancellationToken cancellationToken = default);

    ValueTask StartRecord(string filename, CancellationToken cancellationToken = default);

    ValueTask StopRecord(CancellationToken cancellationToken = default);

    ValueTask SendChatMessage(string message, string? type = null, CancellationToken cancellationToken = default);

    ValueTask Snapshot(string filename, CancellationToken cancellationToken = default);

    ValueTask MuteMic(string participantId, CancellationToken cancellationToken = default);

    ValueTask MuteVideoParticipant(string participantId, CancellationToken cancellationToken = default);

    ValueTask Kick(string participantId, CancellationToken cancellationToken = default);

    ValueTask VolumeUp(string participantId, CancellationToken cancellationToken = default);

    ValueTask VolumeDown(string participantId, CancellationToken cancellationToken = default);

    ValueTask<string?> GetCallStats(CancellationToken cancellationToken = default);
}