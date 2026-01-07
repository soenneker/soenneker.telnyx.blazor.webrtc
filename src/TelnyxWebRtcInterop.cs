using Microsoft.JSInterop;
using Soenneker.Blazor.Utils.ResourceLoader.Abstract;
using Soenneker.Extensions.ValueTask;
using Soenneker.Telnyx.Blazor.WebRtc.Abstract;
using Soenneker.Utils.Json;
using System.Threading;
using System.Threading.Tasks;
using Soenneker.Asyncs.Initializers;
using Soenneker.Telnyx.Blazor.WebRtc.Configuration;

namespace Soenneker.Telnyx.Blazor.WebRtc;

///<inheritdoc cref="ITelnyxWebRtcInterop"/>
public sealed class TelnyxWebRtcInterop : ITelnyxWebRtcInterop
{
    private readonly IJSRuntime _jsRuntime;
    private readonly IResourceLoader _resourceLoader;

    private readonly AsyncInitializer<bool> _scriptInitializer;

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

    public ValueTask Initialize(bool useCdn = true, CancellationToken cancellationToken = default)
    {
        return _scriptInitializer.Init(useCdn, cancellationToken);
    }

    public async ValueTask Create(string id, DotNetObjectReference<TelnyxWebRtc> dotNetObjectRef, TelnyxClientOptions options,
        CancellationToken cancellationToken = default)
    {
        await _scriptInitializer.Init(true, cancellationToken)
                                .NoSync();
        string? json = JsonUtil.Serialize(options);

        await _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.create", cancellationToken, id, json, dotNetObjectRef)
                        .NoSync();
    }

    public ValueTask CreateObserver(string id, CancellationToken cancellationToken = default)
    {
        return _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.createObserver", cancellationToken, id);
    }

    public ValueTask Call(string id, TelnyxCallOptions callOptions, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.call", cancellationToken, id, JsonUtil.Serialize(callOptions));

    public ValueTask Answer(string id, TelnyxAnswerOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (options != null)
            return _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.answer", cancellationToken, id, JsonUtil.Serialize(options));

        return _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.answer", cancellationToken, id);
    }

    public ValueTask Hangup(string id, TelnyxHangupOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (options != null)
            return _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.hangup", cancellationToken, id, JsonUtil.Serialize(options));

        return _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.hangup", cancellationToken, id);
    }

    public ValueTask MuteAudio(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.muteAudio", cancellationToken, id);

    public ValueTask UnmuteAudio(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.unmuteAudio", cancellationToken, id);

    public ValueTask ToggleAudioMute(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.toggleAudioMute", cancellationToken, id);

    public ValueTask MuteVideo(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.muteVideo", cancellationToken, id);

    public ValueTask UnmuteVideo(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.unmuteVideo", cancellationToken, id);

    public ValueTask ToggleVideoMute(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.toggleVideoMute", cancellationToken, id);

    public ValueTask Deaf(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.deaf", cancellationToken, id);

    public ValueTask Undeaf(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.undeaf", cancellationToken, id);

    public ValueTask ToggleDeaf(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.toggleDeaf", cancellationToken, id);

    public ValueTask Hold(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.hold", cancellationToken, id);

    public ValueTask Unhold(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.unhold", cancellationToken, id);

    public ValueTask ToggleHold(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.toggleHold", cancellationToken, id);

    public ValueTask Dtmf(string id, string digit, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.dtmf", cancellationToken, id, digit);

    public ValueTask Message(string id, string to, string body, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.message", cancellationToken, id, to, body);

    public ValueTask SetAudioInDevice(string id, string deviceId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.setAudioInDevice", cancellationToken, id, deviceId);

    public ValueTask SetVideoDevice(string id, string deviceId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.setVideoDevice", cancellationToken, id, deviceId);

    public ValueTask SetAudioOutDevice(string id, string deviceId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.setAudioOutDevice", cancellationToken, id, deviceId);

    public ValueTask StartScreenShare(string id, TelnyxScreenShareOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (options != null)
            return _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.startScreenShare", cancellationToken, id, JsonUtil.Serialize(options));

        return _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.startScreenShare", cancellationToken, id);
    }

    public ValueTask StopScreenShare(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.stopScreenShare", cancellationToken, id);

    public ValueTask SetAudioBandwidth(string id, int bps, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.setAudioBandwidth", cancellationToken, id, bps);

    public ValueTask SetVideoBandwidth(string id, int bps, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.setVideoBandwidth", cancellationToken, id, bps);

    public ValueTask<string> GetDevices(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeAsync<string>("TelnyxWebRtcInterop.getDevices", cancellationToken, id);

    public ValueTask<string> GetVideoDevices(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeAsync<string>("TelnyxWebRtcInterop.getVideoDevices", cancellationToken, id);

    public ValueTask<string> GetAudioInDevices(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeAsync<string>("TelnyxWebRtcInterop.getAudioInDevices", cancellationToken, id);

    public ValueTask<string> GetAudioOutDevices(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeAsync<string>("TelnyxWebRtcInterop.getAudioOutDevices", cancellationToken, id);

    public ValueTask<bool> CheckPermissions(string id, bool audio = true, bool video = true, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeAsync<bool>("TelnyxWebRtcInterop.checkPermissions", cancellationToken, id, audio, video);

    public ValueTask<bool> SetAudioSettings(string id, TelnyxAudioSettings settings, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeAsync<bool>("TelnyxWebRtcInterop.setAudioSettings", cancellationToken, id, JsonUtil.Serialize(settings));

    public ValueTask<bool> SetVideoSettings(string id, TelnyxVideoSettings settings, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeAsync<bool>("TelnyxWebRtcInterop.setVideoSettings", cancellationToken, id, JsonUtil.Serialize(settings));

    public ValueTask EnableMicrophone(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.enableMicrophone", cancellationToken, id);

    public ValueTask DisableMicrophone(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.disableMicrophone", cancellationToken, id);

    public ValueTask EnableWebcam(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.enableWebcam", cancellationToken, id);

    public ValueTask DisableWebcam(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.disableWebcam", cancellationToken, id);

    public ValueTask ToggleAudio(string id, bool enabled, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.toggleAudio", cancellationToken, id, enabled);

    public ValueTask ToggleVideo(string id, bool enabled, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.toggleVideo", cancellationToken, id, enabled);

    public ValueTask Disconnect(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.disconnect", cancellationToken, id);

    public ValueTask Reconnect(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.reconnect", cancellationToken, id);

    public ValueTask Unmount(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.unmount", cancellationToken, id);

    public ValueTask Connect(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.connect", cancellationToken, id);

    // Conference control methods
    public ValueTask ListVideoLayouts(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.listVideoLayouts", cancellationToken, id);

    public ValueTask SetVideoLayout(string id, string layout, string? canvas = null, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.setVideoLayout", cancellationToken, id, layout, canvas);

    public ValueTask PlayMedia(string id, string source, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.playMedia", cancellationToken, id, source);

    public ValueTask StopMedia(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.stopMedia", cancellationToken, id);

    public ValueTask StartRecord(string id, string filename, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.startRecord", cancellationToken, id, filename);

    public ValueTask StopRecord(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.stopRecord", cancellationToken, id);

    public ValueTask SendChatMessage(string id, string message, string? type = null, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.sendChatMessage", cancellationToken, id, message, type);

    public ValueTask Snapshot(string id, string filename, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.snapshot", cancellationToken, id, filename);

    public ValueTask MuteMic(string id, string participantId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.muteMic", cancellationToken, id, participantId);

    public ValueTask MuteVideoParticipant(string id, string participantId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.muteVideoParticipant", cancellationToken, id, participantId);

    public ValueTask Kick(string id, string participantId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.kick", cancellationToken, id, participantId);

    public ValueTask VolumeUp(string id, string participantId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.volumeUp", cancellationToken, id, participantId);

    public ValueTask VolumeDown(string id, string participantId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.volumeDown", cancellationToken, id, participantId);

    public ValueTask<string?> GetCallStats(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeAsync<string?>("TelnyxWebRtcInterop.getCallStats", cancellationToken, id);

    public ValueTask SetAudioVolume(string id, double volume, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync("TelnyxWebRtcInterop.setAudioVolume", cancellationToken, id, volume);

    public async ValueTask DisposeAsync()
    {
        await _resourceLoader.DisposeModule(_module);
        await _scriptInitializer.DisposeAsync();
    }
}