using Microsoft.JSInterop;
using Soenneker.Asyncs.Initializers;
using Soenneker.Blazor.Utils.ResourceLoader.Abstract;
using Soenneker.Extensions.CancellationTokens;
using Soenneker.Extensions.ValueTask;
using Soenneker.Telnyx.Blazor.WebRtc.Abstract;
using Soenneker.Telnyx.Blazor.WebRtc.Configuration;
using Soenneker.Utils.CancellationScopes;
using Soenneker.Utils.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Telnyx.Blazor.WebRtc;

///<inheritdoc cref="ITelnyxWebRtcInterop"/>
public sealed class TelnyxWebRtcInterop : ITelnyxWebRtcInterop
{
    private readonly IJSRuntime _jsRuntime;
    private readonly IResourceLoader _resourceLoader;

    private readonly AsyncInitializer<bool> _scriptInitializer;
    private readonly CancellationScope _cancellationScope = new();

    private const string _module = "Soenneker.Telnyx.Blazor.WebRtc/js/telnyxwebrtcinterop.js";
    private const string _moduleName = "TelnyxWebRtcInterop";

    public TelnyxWebRtcInterop(IJSRuntime jsRuntime, IResourceLoader resourceLoader)
    {
        _jsRuntime = jsRuntime;
        _resourceLoader = resourceLoader;

        _scriptInitializer = new AsyncInitializer<bool>(InitializeScripts);
    }

    private async ValueTask InitializeScripts(bool useCdn, CancellationToken token)
    {
        if (useCdn)
        {
            await _resourceLoader.LoadScriptAndWaitForVariable("https://cdn.jsdelivr.net/npm/@telnyx/webrtc@2.22.17/lib/bundle.js", "TelnyxWebRTC",
                integrity: "sha256-uiKtParibFFpEaHhD+X8rgPhdUAWgcDhHKXwTzqARbE=", cancellationToken: token);
        }
        else
        {
            await _resourceLoader.LoadScriptAndWaitForVariable("_content/Soenneker.Telnyx.Blazor.WebRtc/js/telnyxwebrtc.js", "TelnyxWebRTC",
                cancellationToken: token);
        }

        await _resourceLoader.ImportModuleAndWaitUntilAvailable(_module, _moduleName, 100, token);
    }

    public async ValueTask Initialize(bool useCdn = true, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _scriptInitializer.Init(useCdn, linked);
    }

    public async ValueTask Create(string id, DotNetObjectReference<TelnyxWebRtc> dotNetObjectRef, TelnyxClientOptions options,
        CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
        {
            await _scriptInitializer.Init(true, linked)
                                    .NoSync();
            string? json = JsonUtil.Serialize(options);

            await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.create", linked, id, json, dotNetObjectRef)
                            .NoSync();
        }
    }

    public async ValueTask CreateObserver(string id, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.createObserver", linked, id);
    }

    public async ValueTask Call(string id, TelnyxCallOptions callOptions, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.call", linked, id, JsonUtil.Serialize(callOptions));
    }

    public async ValueTask Answer(string id, TelnyxAnswerOptions? options = null, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
        {
            if (options != null)
                await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.answer", linked, id, JsonUtil.Serialize(options));

            else
                await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.answer", linked, id);
        }
    }

    public async ValueTask Hangup(string id, TelnyxHangupOptions? options = null, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
        {
            if (options != null)
                await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.hangup", linked, id, JsonUtil.Serialize(options));

            else
                await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.hangup", linked, id);
        }
    }

    public async ValueTask MuteAudio(string id, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.muteAudio", linked, id);
    }

    public async ValueTask UnmuteAudio(string id, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.unmuteAudio", linked, id);
    }

    public async ValueTask ToggleAudioMute(string id, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.toggleAudioMute", linked, id);
    }

    public async ValueTask MuteVideo(string id, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.muteVideo", linked, id);
    }

    public async ValueTask UnmuteVideo(string id, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.unmuteVideo", linked, id);
    }

    public async ValueTask ToggleVideoMute(string id, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.toggleVideoMute", linked, id);
    }

    public async ValueTask Deaf(string id, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.deaf", linked, id);
    }

    public async ValueTask Undeaf(string id, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.undeaf", linked, id);
    }

    public async ValueTask ToggleDeaf(string id, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.toggleDeaf", linked, id);
    }

    public async ValueTask Hold(string id, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.hold", linked, id);
    }

    public async ValueTask Unhold(string id, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.unhold", linked, id);
    }

    public async ValueTask ToggleHold(string id, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.toggleHold", linked, id);
    }

    public async ValueTask Dtmf(string id, string digit, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.dtmf", linked, id, digit);
    }

    public async ValueTask Message(string id, string to, string body, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.message", linked, id, to, body);
    }

    public async ValueTask SetAudioInDevice(string id, string deviceId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.setAudioInDevice", linked, id, deviceId);
    }

    public async ValueTask SetVideoDevice(string id, string deviceId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.setVideoDevice", linked, id, deviceId);
    }

    public async ValueTask SetAudioOutDevice(string id, string deviceId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.setAudioOutDevice", linked, id, deviceId);
    }

    public async ValueTask StartScreenShare(string id, TelnyxScreenShareOptions? options = null, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
        {
            if (options != null)
                await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.startScreenShare", linked, id, JsonUtil.Serialize(options));
            else
                await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.startScreenShare", linked, id);
        }
    }

    public async ValueTask StopScreenShare(string id, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.stopScreenShare", linked, id);
    }

    public async ValueTask SetAudioBandwidth(string id, int bps, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.setAudioBandwidth", linked, id, bps);
    }

    public async ValueTask SetVideoBandwidth(string id, int bps, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.setVideoBandwidth", linked, id, bps);
    }

    public async ValueTask<string> GetDevices(string id, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            return await _jsRuntime.InvokeAsync<string>("TelnyxWebRtcInterop.getDevices", linked, id);
    }

    public async ValueTask<string> GetVideoDevices(string id, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            return await _jsRuntime.InvokeAsync<string>("TelnyxWebRtcInterop.getVideoDevices", linked, id);
    }

    public async ValueTask<string> GetAudioInDevices(string id, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            return await _jsRuntime.InvokeAsync<string>("TelnyxWebRtcInterop.getAudioInDevices", linked, id);
    }

    public async ValueTask<string> GetAudioOutDevices(string id, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            return await _jsRuntime.InvokeAsync<string>("TelnyxWebRtcInterop.getAudioOutDevices", linked, id);
    }

    public async ValueTask<bool> CheckPermissions(string id, bool audio = true, bool video = true, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            return await _jsRuntime.InvokeAsync<bool>("TelnyxWebRtcInterop.checkPermissions", linked, id, audio, video);
    }

    public async ValueTask<bool> SetAudioSettings(string id, TelnyxAudioSettings settings, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            return await _jsRuntime.InvokeAsync<bool>("TelnyxWebRtcInterop.setAudioSettings", linked, id, JsonUtil.Serialize(settings));
    }

    public async ValueTask<bool> SetVideoSettings(string id, TelnyxVideoSettings settings, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            return await _jsRuntime.InvokeAsync<bool>("TelnyxWebRtcInterop.setVideoSettings", linked, id, JsonUtil.Serialize(settings));
    }

    public async ValueTask EnableMicrophone(string id, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.enableMicrophone", linked, id);
    }

    public async ValueTask DisableMicrophone(string id, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.disableMicrophone", linked, id);
    }

    public async ValueTask EnableWebcam(string id, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.enableWebcam", linked, id);
    }

    public async ValueTask DisableWebcam(string id, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.disableWebcam", linked, id);
    }

    public async ValueTask ToggleAudio(string id, bool enabled, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.toggleAudio", linked, id, enabled);
    }

    public async ValueTask ToggleVideo(string id, bool enabled, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.toggleVideo", linked, id, enabled);
    }

    public async ValueTask Disconnect(string id, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.disconnect", linked, id);
    }

    public async ValueTask Reconnect(string id, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.reconnect", linked, id);
    }

    public async ValueTask Unmount(string id, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.unmount", linked, id);
    }

    public async ValueTask Connect(string id, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.connect", linked, id);
    }

    // Conference control methods
    public async ValueTask ListVideoLayouts(string id, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.listVideoLayouts", linked, id);
    }

    public async ValueTask SetVideoLayout(string id, string layout, string? canvas = null, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.setVideoLayout", linked, id, layout, canvas);
    }

    public async ValueTask PlayMedia(string id, string source, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var sourceToken);

        using (sourceToken)
            await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.playMedia", linked, id, source);
    }

    public async ValueTask StopMedia(string id, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.stopMedia", linked, id);
    }

    public async ValueTask StartRecord(string id, string filename, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.startRecord", linked, id, filename);
    }

    public async ValueTask StopRecord(string id, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.stopRecord", linked, id);
    }

    public async ValueTask SendChatMessage(string id, string message, string? type = null, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.sendChatMessage", linked, id, message, type);
    }

    public async ValueTask Snapshot(string id, string filename, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.snapshot", linked, id, filename);
    }

    public async ValueTask MuteMic(string id, string participantId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.muteMic", linked, id, participantId);
    }

    public async ValueTask MuteVideoParticipant(string id, string participantId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.muteVideoParticipant", linked, id, participantId);
    }

    public async ValueTask Kick(string id, string participantId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.kick", linked, id, participantId);
    }

    public async ValueTask VolumeUp(string id, string participantId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.volumeUp", linked, id, participantId);
    }

    public async ValueTask VolumeDown(string id, string participantId, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.volumeDown", linked, id, participantId);
    }

    public async ValueTask<string?> GetCallStats(string id, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            return await _jsRuntime.InvokeAsync<string?>("TelnyxWebRtcInterop.getCallStats", linked, id);
    }

    public async ValueTask SetAudioVolume(string id, double volume, CancellationToken cancellationToken = default)
    {
        var linked = _cancellationScope.CancellationToken.Link(cancellationToken, out var source);

        using (source)
            await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.setAudioVolume", linked, id, volume);
    }

    public async ValueTask DisposeAsync()
    {
        await _resourceLoader.DisposeModule(_module);
        await _scriptInitializer.DisposeAsync();
        await _cancellationScope.DisposeAsync();
    }
}