using Microsoft.JSInterop;
using Soenneker.Extensions.CancellationTokens;
using Soenneker.Extensions.ValueTask;
using Soenneker.Telnyx.Blazor.WebRtc.Abstract;
using Soenneker.Telnyx.Blazor.WebRtc.Configuration;
using Soenneker.Telnyx.Blazor.WebRtc.Dtos;
using Soenneker.Utils.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Telnyx.Blazor.WebRtc;

public sealed partial class TelnyxWebRtcSession
{
    public async ValueTask Call(TelnyxCallOptions callOptions, CancellationToken cancellationToken = default)
    {
        EnsureSdkInitialized();

        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? cts);

        using (cts)
        {
            await TelnyxWebRtcInterop.Call(Id!, callOptions, callOptions.LocalStream, callOptions.RemoteStream,
                callOptions.LocalElementReference, callOptions.RemoteElementReference, linked).NoSync();
        }
    }

    public async ValueTask Answer(TelnyxAnswerOptions? options = null, CancellationToken cancellationToken = default)
    {
        EnsureSdkInitialized();

        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? cts);

        using (cts)
        {
            await TelnyxWebRtcInterop.Answer(Id!, options, options?.LocalElementReference, options?.RemoteElementReference, linked).NoSync();
        }
    }

    public async ValueTask Hangup(TelnyxHangupOptions? options = null, CancellationToken cancellationToken = default)
    {
        EnsureSdkInitialized();

        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? cts);

        using (cts)
        {
            await TelnyxWebRtcInterop.Hangup(Id!, options, options?.Execute, linked).NoSync();
        }
    }

    public async ValueTask MuteAudio(CancellationToken cancellationToken = default)
    {
        EnsureSdkInitialized();

        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? cts);

        using (cts)
        {
            await TelnyxWebRtcInterop.MuteAudio( Id!, linked).NoSync();
        }
    }

    public async ValueTask UnmuteAudio(CancellationToken cancellationToken = default)
    {
        EnsureSdkInitialized();

        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? cts);

        using (cts)
        {
            await TelnyxWebRtcInterop.UnmuteAudio( Id!, linked).NoSync();
        }
    }

    public async ValueTask ToggleAudioMute(CancellationToken cancellationToken = default)
    {
        EnsureSdkInitialized();

        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? cts);

        using (cts)
        {
            await TelnyxWebRtcInterop.ToggleAudioMute( Id!, linked).NoSync();
        }
    }

    public async ValueTask MuteVideo(CancellationToken cancellationToken = default)
    {
        EnsureSdkInitialized();

        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? cts);

        using (cts)
        {
            await TelnyxWebRtcInterop.MuteVideo( Id!, linked).NoSync();
        }
    }

    public async ValueTask UnmuteVideo(CancellationToken cancellationToken = default)
    {
        EnsureSdkInitialized();

        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? cts);

        using (cts)
        {
            await TelnyxWebRtcInterop.UnmuteVideo( Id!, linked).NoSync();
        }
    }

    public async ValueTask ToggleVideoMute(CancellationToken cancellationToken = default)
    {
        EnsureSdkInitialized();

        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? cts);

        using (cts)
        {
            await TelnyxWebRtcInterop.ToggleVideoMute( Id!, linked).NoSync();
        }
    }

    public async ValueTask Deaf(CancellationToken cancellationToken = default)
    {
        EnsureSdkInitialized();

        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? cts);

        using (cts)
        {
            await TelnyxWebRtcInterop.Deaf( Id!, linked).NoSync();
        }
    }

    public async ValueTask Undeaf(CancellationToken cancellationToken = default)
    {
        EnsureSdkInitialized();

        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? cts);

        using (cts)
        {
            await TelnyxWebRtcInterop.Undeaf( Id!, linked).NoSync();
        }
    }

    public async ValueTask ToggleDeaf(CancellationToken cancellationToken = default)
    {
        EnsureSdkInitialized();

        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? cts);

        using (cts)
        {
            await TelnyxWebRtcInterop.ToggleDeaf( Id!, linked).NoSync();
        }
    }

    public async ValueTask Hold(CancellationToken cancellationToken = default)
    {
        EnsureSdkInitialized();

        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? cts);

        using (cts)
        {
            await TelnyxWebRtcInterop.Hold( Id!, linked).NoSync();
        }
    }

    public async ValueTask Unhold(CancellationToken cancellationToken = default)
    {
        EnsureSdkInitialized();

        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? cts);

        using (cts)
        {
            await TelnyxWebRtcInterop.Unhold( Id!, linked).NoSync();
        }
    }

    public async ValueTask ToggleHold(CancellationToken cancellationToken = default)
    {
        EnsureSdkInitialized();

        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? cts);

        using (cts)
        {
            await TelnyxWebRtcInterop.ToggleHold( Id!, linked).NoSync();
        }
    }

    public async ValueTask Dtmf(string digit, CancellationToken cancellationToken = default)
    {
        EnsureSdkInitialized();

        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? cts);

        using (cts)
        {
            await TelnyxWebRtcInterop.Dtmf( Id!, digit, linked).NoSync();
        }
    }

    public async ValueTask Message(string to, string body, CancellationToken cancellationToken = default)
    {
        EnsureSdkInitialized();

        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? cts);

        using (cts)
        {
            await TelnyxWebRtcInterop.Message( Id!, to, body, linked).NoSync();
        }
    }

    public async ValueTask SetAudioInDevice(string deviceId, CancellationToken cancellationToken = default)
    {
        EnsureSdkInitialized();

        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? cts);

        using (cts)
        {
            await TelnyxWebRtcInterop.SetAudioInDevice( Id!, deviceId, linked).NoSync();
        }
    }

    public async ValueTask SetVideoDevice(string deviceId, CancellationToken cancellationToken = default)
    {
        EnsureSdkInitialized();

        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? cts);

        using (cts)
        {
            await TelnyxWebRtcInterop.SetVideoDevice( Id!, deviceId, linked).NoSync();
        }
    }

    public async ValueTask SetAudioOutDevice(string deviceId, CancellationToken cancellationToken = default)
    {
        EnsureSdkInitialized();

        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? cts);

        using (cts)
        {
            await TelnyxWebRtcInterop.SetAudioOutDevice( Id!, deviceId, linked).NoSync();
        }
    }

    public async ValueTask StartScreenShare(TelnyxScreenShareOptions? options = null, CancellationToken cancellationToken = default)
    {
        EnsureSdkInitialized();

        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? cts);

        using (cts)
        {
            await TelnyxWebRtcInterop.StartScreenShare( Id!, options, linked).NoSync();
        }
    }

    public async ValueTask StopScreenShare(CancellationToken cancellationToken = default)
    {
        EnsureSdkInitialized();

        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? cts);

        using (cts)
        {
            await TelnyxWebRtcInterop.StopScreenShare( Id!, linked).NoSync();
        }
    }

    public async ValueTask SetAudioBandwidth(int bps, CancellationToken cancellationToken = default)
    {
        EnsureSdkInitialized();

        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? cts);

        using (cts)
        {
            await TelnyxWebRtcInterop.SetAudioBandwidth( Id!, bps, linked).NoSync();
        }
    }

    public async ValueTask SetVideoBandwidth(int bps, CancellationToken cancellationToken = default)
    {
        EnsureSdkInitialized();

        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? cts);

        using (cts)
        {
            await TelnyxWebRtcInterop.SetVideoBandwidth( Id!, bps, linked).NoSync();
        }
    }

    public async ValueTask<List<MediaDeviceInfo>> GetDevices(CancellationToken cancellationToken = default)
    {
        EnsureSdkInitialized();

        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? cts);

        using (cts)
        {
            string json = await TelnyxWebRtcInterop.GetDevices( Id!, linked).NoSync();
            return JsonUtil.Deserialize<List<MediaDeviceInfo>>(json, LibraryJsonContext.Get<List<MediaDeviceInfo>>()) ?? [];
        }
    }

    public async ValueTask<List<MediaDeviceInfo>> GetVideoDevices(CancellationToken cancellationToken = default)
    {
        EnsureSdkInitialized();

        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? cts);

        using (cts)
        {
            string json = await TelnyxWebRtcInterop.GetVideoDevices( Id!, linked).NoSync();
            return JsonUtil.Deserialize<List<MediaDeviceInfo>>(json, LibraryJsonContext.Get<List<MediaDeviceInfo>>()) ?? [];
        }
    }

    public async ValueTask<List<MediaDeviceInfo>> GetAudioInDevices(CancellationToken cancellationToken = default)
    {
        EnsureSdkInitialized();

        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? cts);

        using (cts)
        {
            string json = await TelnyxWebRtcInterop.GetAudioInDevices( Id!, linked).NoSync();
            return JsonUtil.Deserialize<List<MediaDeviceInfo>>(json, LibraryJsonContext.Get<List<MediaDeviceInfo>>()) ?? [];
        }
    }

    public async ValueTask<List<MediaDeviceInfo>> GetAudioOutDevices(CancellationToken cancellationToken = default)
    {
        EnsureSdkInitialized();

        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? cts);

        using (cts)
        {
            string json = await TelnyxWebRtcInterop.GetAudioOutDevices( Id!, linked).NoSync();
            return JsonUtil.Deserialize<List<MediaDeviceInfo>>(json, LibraryJsonContext.Get<List<MediaDeviceInfo>>()) ?? [];
        }
    }

    public async ValueTask<bool> CheckPermissions(bool audio = true, bool video = true, CancellationToken cancellationToken = default)
    {
        EnsureSdkInitialized();

        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? cts);

        using (cts)
        {
            return await TelnyxWebRtcInterop.CheckPermissions( Id!, audio, video, linked).NoSync();
        }
    }

    public async ValueTask<bool> SetAudioSettings(TelnyxAudioSettings settings, CancellationToken cancellationToken = default)
    {
        EnsureSdkInitialized();

        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? cts);

        using (cts)
        {
            return await TelnyxWebRtcInterop.SetAudioSettings( Id!, settings, linked).NoSync();
        }
    }

    public async ValueTask<bool> SetVideoSettings(TelnyxVideoSettings settings, CancellationToken cancellationToken = default)
    {
        EnsureSdkInitialized();

        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? cts);

        using (cts)
        {
            return await TelnyxWebRtcInterop.SetVideoSettings( Id!, settings, linked).NoSync();
        }
    }

    public async ValueTask EnableMicrophone(CancellationToken cancellationToken = default)
    {
        EnsureSdkInitialized();

        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? cts);

        using (cts)
        {
            await TelnyxWebRtcInterop.EnableMicrophone( Id!, linked).NoSync();
        }
    }

    public async ValueTask DisableMicrophone(CancellationToken cancellationToken = default)
    {
        EnsureSdkInitialized();

        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? cts);

        using (cts)
        {
            await TelnyxWebRtcInterop.DisableMicrophone( Id!, linked).NoSync();
        }
    }

    public async ValueTask EnableWebcam(CancellationToken cancellationToken = default)
    {
        EnsureSdkInitialized();

        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? cts);

        using (cts)
        {
            await TelnyxWebRtcInterop.EnableWebcam( Id!, linked).NoSync();
        }
    }

    public async ValueTask DisableWebcam(CancellationToken cancellationToken = default)
    {
        EnsureSdkInitialized();

        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? cts);

        using (cts)
        {
            await TelnyxWebRtcInterop.DisableWebcam( Id!, linked).NoSync();
        }
    }

    public async ValueTask ToggleAudio(bool enabled, CancellationToken cancellationToken = default)
    {
        EnsureSdkInitialized();

        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? cts);

        using (cts)
        {
            await TelnyxWebRtcInterop.ToggleAudio( Id!, enabled, linked).NoSync();
        }
    }

    public async ValueTask ToggleVideo(bool enabled, CancellationToken cancellationToken = default)
    {
        EnsureSdkInitialized();

        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? cts);

        using (cts)
        {
            await TelnyxWebRtcInterop.ToggleVideo( Id!, enabled, linked).NoSync();
        }
    }

    public async ValueTask Disconnect(CancellationToken cancellationToken = default)
    {
        using var lease = await _lifecycle.Acquire(cancellationToken);
        EnsureSdkInitialized();
        cancellationToken.ThrowIfCancellationRequested();
        // Once dispatched, finish lifecycle operations before allowing another one.
        await TelnyxWebRtcInterop.Disconnect(Id, CancellationToken.None);
    }

    public async ValueTask Reconnect(CancellationToken cancellationToken = default)
    {
        using var lease = await _lifecycle.Acquire(cancellationToken);
        EnsureSdkInitialized();
        cancellationToken.ThrowIfCancellationRequested();
        // Once dispatched, finish lifecycle operations before allowing another one.
        await TelnyxWebRtcInterop.Reconnect(Id, CancellationToken.None);
    }

    public async ValueTask Connect(CancellationToken cancellationToken = default)
    {
        using var lease = await _lifecycle.Acquire(cancellationToken);
        EnsureSdkInitialized();
        cancellationToken.ThrowIfCancellationRequested();
        // Once dispatched, finish lifecycle operations before allowing another one.
        await TelnyxWebRtcInterop.Connect(Id, CancellationToken.None);
    }

    public async ValueTask<string?> GetCallStats(CancellationToken cancellationToken = default)
    {
        EnsureSdkInitialized();

        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? cts);

        using (cts)
        {
            return await TelnyxWebRtcInterop.GetCallStats( Id!, linked).NoSync();
        }
    }

}
