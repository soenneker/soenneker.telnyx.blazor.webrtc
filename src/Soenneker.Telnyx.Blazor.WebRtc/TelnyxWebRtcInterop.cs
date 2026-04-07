using Microsoft.JSInterop;
using Soenneker.Asyncs.Initializers;
using Soenneker.Blazor.Utils.ModuleImport.Abstract;
using Soenneker.Blazor.Utils.ResourceLoader.Abstract;
using Soenneker.Extensions.CancellationTokens;
using Soenneker.Telnyx.Blazor.WebRtc.Abstract;
using Soenneker.Telnyx.Blazor.WebRtc.Configuration;
using Soenneker.Utils.CancellationScopes;
using Soenneker.Utils.Json;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Telnyx.Blazor.WebRtc;

///<inheritdoc cref="ITelnyxWebRtcInterop"/>
public sealed class TelnyxWebRtcInterop : ITelnyxWebRtcInterop
{
    private readonly IModuleImportUtil _moduleImportUtil;
    private readonly IResourceLoader _resourceLoader;
    private readonly AsyncInitializer<bool> _scriptInitializer;
    private readonly CancellationScope _cancellationScope = new();

    private const string _modulePath = "/_content/Soenneker.Telnyx.Blazor.WebRtc/js/telnyxwebrtcinterop.js";
    private const string _localScriptPath = "/_content/Soenneker.Telnyx.Blazor.WebRtc/js/telnyxwebrtc.js";
    private const string _cdnScriptPath = "https://cdn.jsdelivr.net/npm/@telnyx/webrtc@2.25.25/lib/bundle.js";
    private const string _cdnScriptIntegrity = "sha256-AUocXf/8wgVxmt1FxnklCHMSI9rDzmhRdBJyrVBHzr8=";

    private bool _useCdn = true;

    public TelnyxWebRtcInterop(IResourceLoader resourceLoader, IModuleImportUtil moduleImportUtil)
    {
        _resourceLoader = resourceLoader;
        _moduleImportUtil = moduleImportUtil;
        _scriptInitializer = new AsyncInitializer<bool>(InitializeScripts);
    }

    private async ValueTask InitializeScripts(bool useCdn, CancellationToken token)
    {
        if (useCdn)
        {
            await _resourceLoader.LoadScriptAndWaitForVariable(_cdnScriptPath, "TelnyxWebRTC", integrity: _cdnScriptIntegrity, cancellationToken: token);
        }
        else
        {
            await _resourceLoader.LoadScriptAndWaitForVariable(_localScriptPath, "TelnyxWebRTC", cancellationToken: token);
        }

        _ = await _moduleImportUtil.GetContentModuleReference(_modulePath, token);
    }

    private async ValueTask Execute(Func<CancellationToken, ValueTask> action, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
        {
            await action(linked);
        }
    }

    private async ValueTask<T> Execute<T>(Func<CancellationToken, ValueTask<T>> action, CancellationToken cancellationToken = default)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);

        using (source)
        {
            return await action(linked);
        }
    }

    private async ValueTask<IJSObjectReference> GetModule(CancellationToken cancellationToken = default)
    {
        await _scriptInitializer.Init(_useCdn, cancellationToken);
        return await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
    }

    private async ValueTask InvokeVoidAsync(string identifier, CancellationToken cancellationToken = default, params object?[] args)
    {
        IJSObjectReference module = await GetModule(cancellationToken);
        await module.InvokeVoidAsync(identifier, cancellationToken, args);
    }

    private async ValueTask<T> InvokeAsync<T>(string identifier, CancellationToken cancellationToken = default, params object?[] args)
    {
        IJSObjectReference module = await GetModule(cancellationToken);
        return await module.InvokeAsync<T>(identifier, cancellationToken, args);
    }

    public ValueTask Initialize(bool useCdn = true, CancellationToken cancellationToken = default)
    {
        _useCdn = useCdn;
        return Execute(linked => _scriptInitializer.Init(_useCdn, linked), cancellationToken);
    }

    public ValueTask Create(string id, DotNetObjectReference<TelnyxWebRtc> dotNetObjectRef, TelnyxClientOptions options,
        CancellationToken cancellationToken = default)
        => Execute(linked => InvokeVoidAsync("create", linked, id, JsonUtil.Serialize(options), dotNetObjectRef), cancellationToken);

    public ValueTask CreateObserver(string id, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeVoidAsync("createObserver", linked, id), cancellationToken);

    public ValueTask Call(string id, TelnyxCallOptions callOptions, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeVoidAsync("call", linked, id, JsonUtil.Serialize(callOptions)), cancellationToken);

    public ValueTask Answer(string id, TelnyxAnswerOptions? options = null, CancellationToken cancellationToken = default)
        => Execute(linked => options != null
            ? InvokeVoidAsync("answer", linked, id, JsonUtil.Serialize(options))
            : InvokeVoidAsync("answer", linked, id), cancellationToken);

    public ValueTask Hangup(string id, TelnyxHangupOptions? options = null, CancellationToken cancellationToken = default)
        => Execute(linked => options != null
            ? InvokeVoidAsync("hangup", linked, id, JsonUtil.Serialize(options))
            : InvokeVoidAsync("hangup", linked, id), cancellationToken);

    public ValueTask MuteAudio(string id, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeVoidAsync("muteAudio", linked, id), cancellationToken);

    public ValueTask UnmuteAudio(string id, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeVoidAsync("unmuteAudio", linked, id), cancellationToken);

    public ValueTask ToggleAudioMute(string id, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeVoidAsync("toggleAudioMute", linked, id), cancellationToken);

    public ValueTask MuteVideo(string id, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeVoidAsync("muteVideo", linked, id), cancellationToken);

    public ValueTask UnmuteVideo(string id, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeVoidAsync("unmuteVideo", linked, id), cancellationToken);

    public ValueTask ToggleVideoMute(string id, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeVoidAsync("toggleVideoMute", linked, id), cancellationToken);

    public ValueTask Deaf(string id, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeVoidAsync("deaf", linked, id), cancellationToken);

    public ValueTask Undeaf(string id, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeVoidAsync("undeaf", linked, id), cancellationToken);

    public ValueTask ToggleDeaf(string id, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeVoidAsync("toggleDeaf", linked, id), cancellationToken);

    public ValueTask Hold(string id, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeVoidAsync("hold", linked, id), cancellationToken);

    public ValueTask Unhold(string id, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeVoidAsync("unhold", linked, id), cancellationToken);

    public ValueTask ToggleHold(string id, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeVoidAsync("toggleHold", linked, id), cancellationToken);

    public ValueTask Dtmf(string id, string digit, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeVoidAsync("dtmf", linked, id, digit), cancellationToken);

    public ValueTask Message(string id, string to, string body, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeVoidAsync("message", linked, id, to, body), cancellationToken);

    public ValueTask SetAudioInDevice(string id, string deviceId, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeVoidAsync("setAudioInDevice", linked, id, deviceId), cancellationToken);

    public ValueTask SetVideoDevice(string id, string deviceId, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeVoidAsync("setVideoDevice", linked, id, deviceId), cancellationToken);

    public ValueTask SetAudioOutDevice(string id, string deviceId, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeVoidAsync("setAudioOutDevice", linked, id, deviceId), cancellationToken);

    public ValueTask StartScreenShare(string id, TelnyxScreenShareOptions? options = null, CancellationToken cancellationToken = default)
        => Execute(linked => options != null
            ? InvokeVoidAsync("startScreenShare", linked, id, JsonUtil.Serialize(options))
            : InvokeVoidAsync("startScreenShare", linked, id), cancellationToken);

    public ValueTask StopScreenShare(string id, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeVoidAsync("stopScreenShare", linked, id), cancellationToken);

    public ValueTask SetAudioBandwidth(string id, int bps, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeVoidAsync("setAudioBandwidth", linked, id, bps), cancellationToken);

    public ValueTask SetVideoBandwidth(string id, int bps, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeVoidAsync("setVideoBandwidth", linked, id, bps), cancellationToken);

    public ValueTask<string> GetDevices(string id, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeAsync<string>("getDevices", linked, id), cancellationToken);

    public ValueTask<string> GetVideoDevices(string id, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeAsync<string>("getVideoDevices", linked, id), cancellationToken);

    public ValueTask<string> GetAudioInDevices(string id, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeAsync<string>("getAudioInDevices", linked, id), cancellationToken);

    public ValueTask<string> GetAudioOutDevices(string id, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeAsync<string>("getAudioOutDevices", linked, id), cancellationToken);

    public ValueTask<bool> CheckPermissions(string id, bool audio = true, bool video = true, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeAsync<bool>("checkPermissions", linked, id, audio, video), cancellationToken);

    public ValueTask<bool> SetAudioSettings(string id, TelnyxAudioSettings settings, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeAsync<bool>("setAudioSettings", linked, id, JsonUtil.Serialize(settings)), cancellationToken);

    public ValueTask<bool> SetVideoSettings(string id, TelnyxVideoSettings settings, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeAsync<bool>("setVideoSettings", linked, id, JsonUtil.Serialize(settings)), cancellationToken);

    public ValueTask EnableMicrophone(string id, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeVoidAsync("enableMicrophone", linked, id), cancellationToken);

    public ValueTask DisableMicrophone(string id, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeVoidAsync("disableMicrophone", linked, id), cancellationToken);

    public ValueTask EnableWebcam(string id, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeVoidAsync("enableWebcam", linked, id), cancellationToken);

    public ValueTask DisableWebcam(string id, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeVoidAsync("disableWebcam", linked, id), cancellationToken);

    public ValueTask ToggleAudio(string id, bool enabled, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeVoidAsync("toggleAudio", linked, id, enabled), cancellationToken);

    public ValueTask ToggleVideo(string id, bool enabled, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeVoidAsync("toggleVideo", linked, id, enabled), cancellationToken);

    public ValueTask Disconnect(string id, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeVoidAsync("disconnect", linked, id), cancellationToken);

    public ValueTask Reconnect(string id, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeVoidAsync("reconnect", linked, id), cancellationToken);

    public ValueTask Unmount(string id, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeVoidAsync("unmount", linked, id), cancellationToken);

    public ValueTask ListVideoLayouts(string id, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeVoidAsync("listVideoLayouts", linked, id), cancellationToken);

    public ValueTask SetVideoLayout(string id, string layout, string? canvas = null, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeVoidAsync("setVideoLayout", linked, id, layout, canvas), cancellationToken);

    public ValueTask PlayMedia(string id, string source, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeVoidAsync("playMedia", linked, id, source), cancellationToken);

    public ValueTask StopMedia(string id, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeVoidAsync("stopMedia", linked, id), cancellationToken);

    public ValueTask StartRecord(string id, string filename, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeVoidAsync("startRecord", linked, id, filename), cancellationToken);

    public ValueTask StopRecord(string id, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeVoidAsync("stopRecord", linked, id), cancellationToken);

    public ValueTask SendChatMessage(string id, string message, string? type = null, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeVoidAsync("sendChatMessage", linked, id, message, type), cancellationToken);

    public ValueTask Snapshot(string id, string filename, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeVoidAsync("snapshot", linked, id, filename), cancellationToken);

    public ValueTask MuteMic(string id, string participantId, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeVoidAsync("muteMic", linked, id, participantId), cancellationToken);

    public ValueTask MuteVideoParticipant(string id, string participantId, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeVoidAsync("muteVideoParticipant", linked, id, participantId), cancellationToken);

    public ValueTask Kick(string id, string participantId, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeVoidAsync("kick", linked, id, participantId), cancellationToken);

    public ValueTask VolumeUp(string id, string participantId, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeVoidAsync("volumeUp", linked, id, participantId), cancellationToken);

    public ValueTask VolumeDown(string id, string participantId, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeVoidAsync("volumeDown", linked, id, participantId), cancellationToken);

    public ValueTask<string?> GetCallStats(string id, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeAsync<string?>("getCallStats", linked, id), cancellationToken);

    public ValueTask SetAudioVolume(string id, double volume, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeVoidAsync("setAudioVolume", linked, id, volume), cancellationToken);

    public ValueTask Connect(string id, CancellationToken cancellationToken = default)
        => Execute(linked => InvokeVoidAsync("connect", linked, id), cancellationToken);

    public async ValueTask DisposeAsync()
    {
        await _moduleImportUtil.DisposeContentModule(_modulePath);
        await _scriptInitializer.DisposeAsync();
        await _cancellationScope.DisposeAsync();
    }
}
