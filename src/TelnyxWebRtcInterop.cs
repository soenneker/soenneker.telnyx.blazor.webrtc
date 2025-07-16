using Microsoft.JSInterop;
using Soenneker.Blazor.Utils.ResourceLoader.Abstract;
using Soenneker.Extensions.ValueTask;
using Soenneker.Telnyx.Blazor.WebRtc.Abstract;
using Soenneker.Utils.AsyncSingleton;
using Soenneker.Utils.Json;
using System.Threading;
using System.Threading.Tasks;
using Soenneker.Telnyx.Blazor.WebRtc.Configuration;

namespace Soenneker.Telnyx.Blazor.WebRtc;

///<inheritdoc cref="ITelnyxWebRtcInterop"/>
public sealed class TelnyxWebRtcInterop : ITelnyxWebRtcInterop
{
    private readonly IJSRuntime _jsRuntime;
    private readonly IResourceLoader _resourceLoader;

    private readonly AsyncSingleton _scriptInitializer;

    private const string _module = "Soenneker.Telnyx.Blazor.WebRtc/js/telnyxwebrtcinterop.js";
    private const string _moduleName = "TelnyxWebRtcInterop";

    public TelnyxWebRtcInterop(IJSRuntime jsRuntime, IResourceLoader resourceLoader)
    {
        _jsRuntime = jsRuntime;
        _resourceLoader = resourceLoader;

        _scriptInitializer = new AsyncSingleton(async (token, arr) =>
        {
            var useCdn = true;

            if (arr.Length > 0)
                useCdn = (bool) arr[0];

            if (useCdn)
            {
                await _resourceLoader.LoadScriptAndWaitForVariable("https://cdn.jsdelivr.net/npm/@telnyx/webrtc@2.22.14/lib/bundle.js", "TelnyxWebRTC",
                                         integrity: "sha256-JgeyoGNTCgSh8q4Vkh2Obb69FTHCA6l/Y56FkwRrGG8=", cancellationToken: token)
                                     .NoSync();
            }
            else
            {
                await _resourceLoader.LoadScriptAndWaitForVariable("_content/Soenneker.Telnyx.Blazor.WebRtc/js/telnyxwebrtc.js", "TelnyxWebRTC",
                                         cancellationToken: token)
                                     .NoSync();
            }

            await _resourceLoader.ImportModuleAndWaitUntilAvailable(_module, _moduleName, 100, token).NoSync();

            return new object();
        });
    }

    public ValueTask Initialize(bool useCdn = true, CancellationToken cancellationToken = default)
    {
        return _scriptInitializer.Init(cancellationToken, useCdn);
    }

    public async ValueTask Create(string elementId, DotNetObjectReference<TelnyxWebRtc> dotNetObjectRef, TelnyxClientOptions options,
        CancellationToken cancellationToken = default)
    {
        await _scriptInitializer.Init(cancellationToken).NoSync();
        string? json = JsonUtil.Serialize(options);

        await _jsRuntime.InvokeVoidAsync($"{_moduleName}.create", cancellationToken, elementId, json, dotNetObjectRef).NoSync();
    }

    public ValueTask CreateObserver(string elementId, CancellationToken cancellationToken = default)
    {
        return _jsRuntime.InvokeVoidAsync($"{_moduleName}.createObserver", cancellationToken, elementId);
    }

    public ValueTask Call(string elementId, TelnyxCallOptions callOptions, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.call", cancellationToken, elementId, JsonUtil.Serialize(callOptions));

    public ValueTask Answer(string elementId, TelnyxAnswerOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (options != null)
            return _jsRuntime.InvokeVoidAsync($"{_moduleName}.answer", cancellationToken, elementId, JsonUtil.Serialize(options));

        return _jsRuntime.InvokeVoidAsync($"{_moduleName}.answer", cancellationToken, elementId);
    }

    public ValueTask Hangup(string elementId, TelnyxHangupOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (options != null)
            return _jsRuntime.InvokeVoidAsync($"{_moduleName}.hangup", cancellationToken, elementId, JsonUtil.Serialize(options));

        return _jsRuntime.InvokeVoidAsync($"{_moduleName}.hangup", cancellationToken, elementId);
    }

    public ValueTask MuteAudio(string elementId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.muteAudio", cancellationToken, elementId);

    public ValueTask UnmuteAudio(string elementId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.unmuteAudio", cancellationToken, elementId);

    public ValueTask ToggleAudioMute(string elementId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.toggleAudioMute", cancellationToken, elementId);

    public ValueTask MuteVideo(string elementId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.muteVideo", cancellationToken, elementId);

    public ValueTask UnmuteVideo(string elementId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.unmuteVideo", cancellationToken, elementId);

    public ValueTask ToggleVideoMute(string elementId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.toggleVideoMute", cancellationToken, elementId);

    public ValueTask Deaf(string elementId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.deaf", cancellationToken, elementId);

    public ValueTask Undeaf(string elementId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.undeaf", cancellationToken, elementId);

    public ValueTask ToggleDeaf(string elementId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.toggleDeaf", cancellationToken, elementId);

    public ValueTask Hold(string elementId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.hold", cancellationToken, elementId);

    public ValueTask Unhold(string elementId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.unhold", cancellationToken, elementId);

    public ValueTask ToggleHold(string elementId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.toggleHold", cancellationToken, elementId);

    public ValueTask Dtmf(string elementId, string digit, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.dtmf", cancellationToken, elementId, digit);

    public ValueTask Message(string elementId, string to, string body, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.message", cancellationToken, elementId, to, body);

    public ValueTask SetAudioInDevice(string elementId, string deviceId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.setAudioInDevice", cancellationToken, elementId, deviceId);

    public ValueTask SetVideoDevice(string elementId, string deviceId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.setVideoDevice", cancellationToken, elementId, deviceId);

    public ValueTask SetAudioOutDevice(string elementId, string deviceId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.setAudioOutDevice", cancellationToken, elementId, deviceId);

    public ValueTask StartScreenShare(string elementId, TelnyxScreenShareOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (options != null)
            return _jsRuntime.InvokeVoidAsync($"{_moduleName}.startScreenShare", cancellationToken, elementId, JsonUtil.Serialize(options));

        return _jsRuntime.InvokeVoidAsync($"{_moduleName}.startScreenShare", cancellationToken, elementId);
    }

    public ValueTask StopScreenShare(string elementId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.stopScreenShare", cancellationToken, elementId);

    public ValueTask SetAudioBandwidth(string elementId, int bps, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.setAudioBandwidth", cancellationToken, elementId, bps);

    public ValueTask SetVideoBandwidth(string elementId, int bps, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.setVideoBandwidth", cancellationToken, elementId, bps);

    public ValueTask<string> GetDevices(string elementId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeAsync<string>($"{_moduleName}.getDevices", cancellationToken, elementId);

    public ValueTask<string> GetVideoDevices(string elementId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeAsync<string>($"{_moduleName}.getVideoDevices", cancellationToken, elementId);

    public ValueTask<string> GetAudioInDevices(string elementId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeAsync<string>($"{_moduleName}.getAudioInDevices", cancellationToken, elementId);

    public ValueTask<string> GetAudioOutDevices(string elementId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeAsync<string>($"{_moduleName}.getAudioOutDevices", cancellationToken, elementId);

    public ValueTask<bool> CheckPermissions(string elementId, bool audio = true, bool video = true, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeAsync<bool>($"{_moduleName}.checkPermissions", cancellationToken, elementId, audio, video);

    public ValueTask<bool> SetAudioSettings(string elementId, TelnyxAudioSettings settings, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeAsync<bool>($"{_moduleName}.setAudioSettings", cancellationToken, elementId, JsonUtil.Serialize(settings));

    public ValueTask<bool> SetVideoSettings(string elementId, TelnyxVideoSettings settings, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeAsync<bool>($"{_moduleName}.setVideoSettings", cancellationToken, elementId, JsonUtil.Serialize(settings));

    public ValueTask EnableMicrophone(string elementId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.enableMicrophone", cancellationToken, elementId);

    public ValueTask DisableMicrophone(string elementId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.disableMicrophone", cancellationToken, elementId);

    public ValueTask EnableWebcam(string elementId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.enableWebcam", cancellationToken, elementId);

    public ValueTask DisableWebcam(string elementId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.disableWebcam", cancellationToken, elementId);

    public ValueTask ToggleAudio(string elementId, bool enabled, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.toggleAudio", cancellationToken, elementId, enabled);

    public ValueTask ToggleVideo(string elementId, bool enabled, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.toggleVideo", cancellationToken, elementId, enabled);

    public ValueTask Disconnect(string elementId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.disconnect", cancellationToken, elementId);

    public ValueTask Reconnect(string elementId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.reconnect", cancellationToken, elementId);

    public ValueTask Unmount(string elementId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.unmount", cancellationToken, elementId);

    // Conference control methods
    public ValueTask ListVideoLayouts(string elementId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.listVideoLayouts", cancellationToken, elementId);

    public ValueTask SetVideoLayout(string elementId, string layout, string? canvas = null, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.setVideoLayout", cancellationToken, elementId, layout, canvas);

    public ValueTask PlayMedia(string elementId, string source, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.playMedia", cancellationToken, elementId, source);

    public ValueTask StopMedia(string elementId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.stopMedia", cancellationToken, elementId);

    public ValueTask StartRecord(string elementId, string filename, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.startRecord", cancellationToken, elementId, filename);

    public ValueTask StopRecord(string elementId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.stopRecord", cancellationToken, elementId);

    public ValueTask SendChatMessage(string elementId, string message, string? type = null, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.sendChatMessage", cancellationToken, elementId, message, type);

    public ValueTask Snapshot(string elementId, string filename, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.snapshot", cancellationToken, elementId, filename);

    public ValueTask MuteMic(string elementId, string participantId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.muteMic", cancellationToken, elementId, participantId);

    public ValueTask MuteVideoParticipant(string elementId, string participantId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.muteVideoParticipant", cancellationToken, elementId, participantId);

    public ValueTask Kick(string elementId, string participantId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.kick", cancellationToken, elementId, participantId);

    public ValueTask VolumeUp(string elementId, string participantId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.volumeUp", cancellationToken, elementId, participantId);

    public ValueTask VolumeDown(string elementId, string participantId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.volumeDown", cancellationToken, elementId, participantId);

    public ValueTask<string?> GetCallStats(string elementId, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeAsync<string?>($"{_moduleName}.getCallStats", cancellationToken, elementId);

    public ValueTask SetAudioVolume(string elementId, double volume, CancellationToken cancellationToken = default) =>
        _jsRuntime.InvokeVoidAsync($"{_moduleName}.setAudioVolume", cancellationToken, elementId, volume);

    public async ValueTask DisposeAsync()
    {
        await _resourceLoader.DisposeModule(_module).NoSync();
        await _scriptInitializer.DisposeAsync().NoSync();
    }
}