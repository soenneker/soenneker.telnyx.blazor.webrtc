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

        _scriptInitializer = new AsyncInitializer<bool>(async (useCdn, token) =>
        {
            if (useCdn)
            {
                await _resourceLoader.LoadScriptAndWaitForVariable("https://cdn.jsdelivr.net/npm/@telnyx/webrtc@2.22.17/lib/bundle.js", "TelnyxWebRTC",
                        integrity: "sha256-uiKtParibFFpEaHhD+X8rgPhdUAWgcDhHKXwTzqARbE=", cancellationToken: token)
                    .NoSync();
            }
            else
            {
                await _resourceLoader.LoadScriptAndWaitForVariable("_content/Soenneker.Telnyx.Blazor.WebRtc/js/telnyxwebrtc.js", "TelnyxWebRTC",
                        cancellationToken: token)
                    .NoSync();
            }

            await _resourceLoader.ImportModuleAndWaitUntilAvailable(_module, _moduleName, 100, token)
                .NoSync();
        });
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

        await _jsRuntime.InvokeVoidAsync($"{_moduleName}.create", cancellationToken, id, json, dotNetObjectRef)
            .NoSync();
    }

    public ValueTask CreateObserver(string id, CancellationToken cancellationToken = default)
    {
        return _jsRuntime.InvokeVoidAsync($"{_moduleName}.createObserver", cancellationToken, id);
    }

    public ValueTask Call(string id, TelnyxCallOptions callOptions, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.call", cancellationToken, id, JsonUtil.Serialize(callOptions));

    public ValueTask Answer(string id, TelnyxAnswerOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (options != null)
            return _jsRuntime.InvokeVoidAsync($"{_moduleName}.answer", cancellationToken, id, JsonUtil.Serialize(options));

        return _jsRuntime.InvokeVoidAsync($"{_moduleName}.answer", cancellationToken, id);
    }

    public ValueTask Hangup(string id, TelnyxHangupOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (options != null)
            return _jsRuntime.InvokeVoidAsync($"{_moduleName}.hangup", cancellationToken, id, JsonUtil.Serialize(options));

        return _jsRuntime.InvokeVoidAsync($"{_moduleName}.hangup", cancellationToken, id);
    }

    public ValueTask MuteAudio(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.muteAudio", cancellationToken, id);

    public ValueTask UnmuteAudio(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.unmuteAudio", cancellationToken, id);

    public ValueTask ToggleAudioMute(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.toggleAudioMute", cancellationToken, id);

    public ValueTask MuteVideo(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.muteVideo", cancellationToken, id);

    public ValueTask UnmuteVideo(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.unmuteVideo", cancellationToken, id);

    public ValueTask ToggleVideoMute(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.toggleVideoMute", cancellationToken, id);

    public ValueTask Deaf(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.deaf", cancellationToken, id);

    public ValueTask Undeaf(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.undeaf", cancellationToken, id);

    public ValueTask ToggleDeaf(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.toggleDeaf", cancellationToken, id);

    public ValueTask Hold(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.hold", cancellationToken, id);

    public ValueTask Unhold(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.unhold", cancellationToken, id);

    public ValueTask ToggleHold(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.toggleHold", cancellationToken, id);

    public ValueTask Dtmf(string id, string digit, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.dtmf", cancellationToken, id, digit);

    public ValueTask Message(string id, string to, string body, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.message", cancellationToken, id, to, body);

    public ValueTask SetAudioInDevice(string id, string deviceId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.setAudioInDevice", cancellationToken, id, deviceId);

    public ValueTask SetVideoDevice(string id, string deviceId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.setVideoDevice", cancellationToken, id, deviceId);

    public ValueTask SetAudioOutDevice(string id, string deviceId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.setAudioOutDevice", cancellationToken, id, deviceId);

    public ValueTask StartScreenShare(string id, TelnyxScreenShareOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (options != null)
            return _jsRuntime.InvokeVoidAsync($"{_moduleName}.startScreenShare", cancellationToken, id, JsonUtil.Serialize(options));

        return _jsRuntime.InvokeVoidAsync($"{_moduleName}.startScreenShare", cancellationToken, id);
    }

    public ValueTask StopScreenShare(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.stopScreenShare", cancellationToken, id);

    public ValueTask SetAudioBandwidth(string id, int bps, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.setAudioBandwidth", cancellationToken, id, bps);

    public ValueTask SetVideoBandwidth(string id, int bps, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.setVideoBandwidth", cancellationToken, id, bps);

    public ValueTask<string> GetDevices(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeAsync<string>($"{_moduleName}.getDevices", cancellationToken, id);

    public ValueTask<string> GetVideoDevices(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeAsync<string>($"{_moduleName}.getVideoDevices", cancellationToken, id);

    public ValueTask<string> GetAudioInDevices(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeAsync<string>($"{_moduleName}.getAudioInDevices", cancellationToken, id);

    public ValueTask<string> GetAudioOutDevices(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeAsync<string>($"{_moduleName}.getAudioOutDevices", cancellationToken, id);

    public ValueTask<bool> CheckPermissions(string id, bool audio = true, bool video = true, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeAsync<bool>($"{_moduleName}.checkPermissions", cancellationToken, id, audio, video);

    public ValueTask<bool> SetAudioSettings(string id, TelnyxAudioSettings settings, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeAsync<bool>($"{_moduleName}.setAudioSettings", cancellationToken, id, JsonUtil.Serialize(settings));

    public ValueTask<bool> SetVideoSettings(string id, TelnyxVideoSettings settings, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeAsync<bool>($"{_moduleName}.setVideoSettings", cancellationToken, id, JsonUtil.Serialize(settings));

    public ValueTask EnableMicrophone(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.enableMicrophone", cancellationToken, id);

    public ValueTask DisableMicrophone(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.disableMicrophone", cancellationToken, id);

    public ValueTask EnableWebcam(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.enableWebcam", cancellationToken, id);

    public ValueTask DisableWebcam(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.disableWebcam", cancellationToken, id);

    public ValueTask ToggleAudio(string id, bool enabled, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.toggleAudio", cancellationToken, id, enabled);

    public ValueTask ToggleVideo(string id, bool enabled, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.toggleVideo", cancellationToken, id, enabled);

    public ValueTask Disconnect(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.disconnect", cancellationToken, id);

    public ValueTask Reconnect(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.reconnect", cancellationToken, id);

    public ValueTask Unmount(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.unmount", cancellationToken, id);

    public ValueTask Connect(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.connect", cancellationToken, id);

    // Conference control methods
    public ValueTask ListVideoLayouts(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.listVideoLayouts", cancellationToken, id);

    public ValueTask SetVideoLayout(string id, string layout, string? canvas = null, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.setVideoLayout", cancellationToken, id, layout, canvas);

    public ValueTask PlayMedia(string id, string source, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.playMedia", cancellationToken, id, source);

    public ValueTask StopMedia(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.stopMedia", cancellationToken, id);

    public ValueTask StartRecord(string id, string filename, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.startRecord", cancellationToken, id, filename);

    public ValueTask StopRecord(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.stopRecord", cancellationToken, id);

    public ValueTask SendChatMessage(string id, string message, string? type = null, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.sendChatMessage", cancellationToken, id, message, type);

    public ValueTask Snapshot(string id, string filename, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.snapshot", cancellationToken, id, filename);

    public ValueTask MuteMic(string id, string participantId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.muteMic", cancellationToken, id, participantId);

    public ValueTask MuteVideoParticipant(string id, string participantId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.muteVideoParticipant", cancellationToken, id, participantId);

    public ValueTask Kick(string id, string participantId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.kick", cancellationToken, id, participantId);

    public ValueTask VolumeUp(string id, string participantId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.volumeUp", cancellationToken, id, participantId);

    public ValueTask VolumeDown(string id, string participantId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.volumeDown", cancellationToken, id, participantId);

    public ValueTask<string?> GetCallStats(string id, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeAsync<string?>($"{_moduleName}.getCallStats", cancellationToken, id);

    public ValueTask SetAudioVolume(string id, double volume, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.setAudioVolume", cancellationToken, id, volume);

    public async ValueTask DisposeAsync()
    {
        await _resourceLoader.DisposeModule(_module)
            .NoSync();
        await _scriptInitializer.DisposeAsync()
            .NoSync();
    }
}