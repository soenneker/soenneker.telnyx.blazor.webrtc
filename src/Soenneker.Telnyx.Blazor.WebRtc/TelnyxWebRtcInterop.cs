using System;
using Soenneker.Asyncs.Semaphores;
using System.Diagnostics.CodeAnalysis;
using Microsoft.JSInterop;
using Soenneker.Asyncs.Initializers;
using Soenneker.Blazor.Utils.ModuleImport.Abstract;
using Soenneker.Blazor.Utils.ResourceLoader.Abstract;
using Soenneker.Extensions.CancellationTokens;
using Soenneker.Telnyx.Blazor.WebRtc.Abstract;
using Soenneker.Telnyx.Blazor.WebRtc.Configuration;
using Soenneker.Utils.CancellationScopes;
using System.Threading;
using System.Threading.Tasks;
using Soenneker.Blazor.Extensions.IJSObjectReferences;

namespace Soenneker.Telnyx.Blazor.WebRtc;

public sealed partial class TelnyxWebRtcInterop : ITelnyxWebRtcInterop
{
    private readonly IModuleImportUtil _moduleImportUtil;
    private readonly IResourceLoader _resourceLoader;
    private readonly AsyncInitializer<bool> _scriptInitializer;
    private readonly CancellationScope _cancellationScope = new();

    private const string _modulePath = "./_content/Soenneker.Telnyx.Blazor.WebRtc/js/interop/telnyxwebrtcinterop.min.js";
    private const string _localScriptPath = "_content/Soenneker.Telnyx.Blazor.WebRtc/js/bundle.js";
    private const string _cdnScriptPath = "https://cdn.jsdelivr.net/npm/@telnyx/webrtc@2.28.0/lib/bundle.js";
    private const string _cdnScriptIntegrity = "sha256-Hyboisb3nnz/NjDcNRQBpCU/weanORlXs27KQGRB164=";

    private bool _useCdn = true;
    private bool? _initializedUseCdn;
    private bool _disposed;
    private readonly AsyncSemaphore _lifecycle = new(1);

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
            await _resourceLoader.LoadScriptAndWaitForVariable(_cdnScriptPath, "TelnyxWebRTC", integrity: _cdnScriptIntegrity, timeout: 15000, cancellationToken: token);
        }
        else
        {
            await _resourceLoader.LoadScriptAndWaitForVariable(_localScriptPath, "TelnyxWebRTC", timeout: 15000, cancellationToken: token);
        }

        _ = await _moduleImportUtil.GetContentModuleReference(_modulePath, token);
    }

    private async ValueTask<IJSObjectReference> GetModule(CancellationToken cancellationToken = default)
    {
        await Initialize(_useCdn, cancellationToken);
        return await _moduleImportUtil.GetContentModuleReference(_modulePath, cancellationToken);
    }

    private async ValueTask InvokeVoidAsync(string identifier, CancellationToken cancellationToken = default, params object?[] args)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);
       
        using (source)
        {
            IJSObjectReference module = await GetModule(linked);
            await module.InvokeVoidWithContextAsync(identifier, linked, args);
        }
    }

    private async ValueTask<T> InvokeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] T>(string identifier, CancellationToken cancellationToken = default, params object?[] args)
    {
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);
        
        using (source)
        {
            IJSObjectReference module = await GetModule(linked);
            return await module.InvokeWithContextAsync<T>(identifier, linked, args);
        }
    }

    public async ValueTask Initialize(bool useCdn = true, CancellationToken cancellationToken = default)
    {
        using var lease = await _lifecycle.Acquire(cancellationToken);
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_initializedUseCdn is { } selected)
        {
            if (selected != useCdn) throw new InvalidOperationException("SDK script source is fixed for this scope after initialization.");
            return;
        }
        CancellationToken linked = _cancellationScope.CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);
        using (source) await _scriptInitializer.Init(useCdn, linked);
        _useCdn = useCdn;
        _initializedUseCdn = useCdn;
    }

    public ValueTask Create(string id, DotNetObjectReference<TelnyxWebRtc> dotNetObjectRef, TelnyxClientOptions options,
        CancellationToken cancellationToken = default)
        => InvokeVoidAsync("create", cancellationToken, id, SerializePayload(options), dotNetObjectRef);

    public ValueTask Create(string id, DotNetObjectReference<TelnyxWebRtcSession> callback, TelnyxClientOptions options,
        CancellationToken cancellationToken = default)
        => InvokeVoidAsync("create", cancellationToken, id, SerializePayload(options), callback);

    public ValueTask<bool> ResumePlayback(string id, CancellationToken cancellationToken = default)
        => InvokeAsync<bool>("resumePlayback", cancellationToken, id);

    public ValueTask CreateObserver(string id, CancellationToken cancellationToken = default)
        => InvokeVoidAsync("createObserver", cancellationToken, id);

    public ValueTask CreateObserver(string id, string elementId, CancellationToken cancellationToken = default)
        => InvokeVoidAsync("createObserver", cancellationToken, id, elementId);

    public ValueTask Call(string id, TelnyxCallOptions callOptions, IJSObjectReference? localStream = null, IJSObjectReference? remoteStream = null,
        IJSObjectReference? localElement = null, IJSObjectReference? remoteElement = null, CancellationToken cancellationToken = default)
        => InvokeVoidAsync("call", cancellationToken, id, SerializePayload(callOptions), localStream, remoteStream, localElement, remoteElement);

    public ValueTask Answer(string id, TelnyxAnswerOptions? options = null, IJSObjectReference? localElement = null,
        IJSObjectReference? remoteElement = null, CancellationToken cancellationToken = default)
        => options != null
            ? InvokeVoidAsync("answer", cancellationToken, id, SerializePayload(options), localElement, remoteElement)
            : InvokeVoidAsync("answer", cancellationToken, id, null, localElement, remoteElement);

    public ValueTask Hangup(string id, TelnyxHangupOptions? options = null, bool? execute = null, CancellationToken cancellationToken = default)
        => options != null
            ? InvokeVoidAsync("hangup", cancellationToken, id, SerializePayload(options), execute)
            : InvokeVoidAsync("hangup", cancellationToken, id, null, execute);

    public ValueTask MuteAudio(string id, CancellationToken cancellationToken = default)
        => InvokeVoidAsync("muteAudio", cancellationToken, id);

    public ValueTask UnmuteAudio(string id, CancellationToken cancellationToken = default)
        => InvokeVoidAsync("unmuteAudio", cancellationToken, id);

    public ValueTask ToggleAudioMute(string id, CancellationToken cancellationToken = default)
        => InvokeVoidAsync("toggleAudioMute", cancellationToken, id);

    public ValueTask MuteVideo(string id, CancellationToken cancellationToken = default)
        => InvokeVoidAsync("muteVideo", cancellationToken, id);

    public ValueTask UnmuteVideo(string id, CancellationToken cancellationToken = default)
        => InvokeVoidAsync("unmuteVideo", cancellationToken, id);

    public ValueTask ToggleVideoMute(string id, CancellationToken cancellationToken = default)
        => InvokeVoidAsync("toggleVideoMute", cancellationToken, id);

    public ValueTask Deaf(string id, CancellationToken cancellationToken = default)
        => InvokeVoidAsync("deaf", cancellationToken, id);

    public ValueTask Undeaf(string id, CancellationToken cancellationToken = default)
        => InvokeVoidAsync("undeaf", cancellationToken, id);

    public ValueTask ToggleDeaf(string id, CancellationToken cancellationToken = default)
        => InvokeVoidAsync("toggleDeaf", cancellationToken, id);

    public ValueTask Hold(string id, CancellationToken cancellationToken = default)
        => InvokeVoidAsync("hold", cancellationToken, id);

    public ValueTask Unhold(string id, CancellationToken cancellationToken = default)
        => InvokeVoidAsync("unhold", cancellationToken, id);

    public ValueTask ToggleHold(string id, CancellationToken cancellationToken = default)
        => InvokeVoidAsync("toggleHold", cancellationToken, id);

    public ValueTask Dtmf(string id, string digit, CancellationToken cancellationToken = default)
        => InvokeVoidAsync("dtmf", cancellationToken, id, digit);

    public ValueTask Message(string id, string to, string body, CancellationToken cancellationToken = default)
        => InvokeVoidAsync("message", cancellationToken, id, to, body);

    public ValueTask SetAudioInDevice(string id, string deviceId, CancellationToken cancellationToken = default)
        => InvokeVoidAsync("setAudioInDevice", cancellationToken, id, deviceId);

    public ValueTask SetVideoDevice(string id, string deviceId, CancellationToken cancellationToken = default)
        => InvokeVoidAsync("setVideoDevice", cancellationToken, id, deviceId);

    public ValueTask SetAudioOutDevice(string id, string deviceId, CancellationToken cancellationToken = default)
        => InvokeVoidAsync("setAudioOutDevice", cancellationToken, id, deviceId);

    public ValueTask StartScreenShare(string id, TelnyxScreenShareOptions? options = null, CancellationToken cancellationToken = default)
        => options != null
            ? InvokeVoidAsync("startScreenShare", cancellationToken, id, SerializePayload(options))
            : InvokeVoidAsync("startScreenShare", cancellationToken, id);

    public ValueTask StopScreenShare(string id, CancellationToken cancellationToken = default)
        => InvokeVoidAsync("stopScreenShare", cancellationToken, id);

    public ValueTask SetAudioBandwidth(string id, int bps, CancellationToken cancellationToken = default)
        => InvokeVoidAsync("setAudioBandwidth", cancellationToken, id, bps);

    public ValueTask SetVideoBandwidth(string id, int bps, CancellationToken cancellationToken = default)
        => InvokeVoidAsync("setVideoBandwidth", cancellationToken, id, bps);

    public ValueTask<string> GetDevices(string id, CancellationToken cancellationToken = default)
        => InvokeAsync<string>("getDevices", cancellationToken, id);

    public ValueTask<string> GetVideoDevices(string id, CancellationToken cancellationToken = default)
        => InvokeAsync<string>("getVideoDevices", cancellationToken, id);

    public ValueTask<string> GetAudioInDevices(string id, CancellationToken cancellationToken = default)
        => InvokeAsync<string>("getAudioInDevices", cancellationToken, id);

    public ValueTask<string> GetAudioOutDevices(string id, CancellationToken cancellationToken = default)
        => InvokeAsync<string>("getAudioOutDevices", cancellationToken, id);

    public ValueTask<bool> CheckPermissions(string id, bool audio = true, bool video = true, CancellationToken cancellationToken = default)
        => InvokeAsync<bool>("checkPermissions", cancellationToken, id, audio, video);

    public ValueTask<bool> SetAudioSettings(string id, TelnyxAudioSettings settings, CancellationToken cancellationToken = default)
        => InvokeAsync<bool>("setAudioSettings", cancellationToken, id, SerializePayload(settings));

    public ValueTask<bool> SetVideoSettings(string id, TelnyxVideoSettings settings, CancellationToken cancellationToken = default)
        => InvokeAsync<bool>("setVideoSettings", cancellationToken, id, SerializePayload(settings));

    public ValueTask EnableMicrophone(string id, CancellationToken cancellationToken = default)
        => InvokeVoidAsync("enableMicrophone", cancellationToken, id);

    public ValueTask DisableMicrophone(string id, CancellationToken cancellationToken = default)
        => InvokeVoidAsync("disableMicrophone", cancellationToken, id);

    public ValueTask EnableWebcam(string id, CancellationToken cancellationToken = default)
        => InvokeVoidAsync("enableWebcam", cancellationToken, id);

    public ValueTask DisableWebcam(string id, CancellationToken cancellationToken = default)
        => InvokeVoidAsync("disableWebcam", cancellationToken, id);

    public ValueTask ToggleAudio(string id, bool enabled, CancellationToken cancellationToken = default)
        => InvokeVoidAsync("toggleAudio", cancellationToken, id, enabled);

    public ValueTask ToggleVideo(string id, bool enabled, CancellationToken cancellationToken = default)
        => InvokeVoidAsync("toggleVideo", cancellationToken, id, enabled);

    public ValueTask Disconnect(string id, CancellationToken cancellationToken = default)
        => InvokeVoidAsync("disconnect", cancellationToken, id);

    public ValueTask Reconnect(string id, CancellationToken cancellationToken = default)
        => InvokeVoidAsync("reconnect", cancellationToken, id);

    public ValueTask Unmount(string id, CancellationToken cancellationToken = default)
        => InvokeVoidAsync("unmount", cancellationToken, id);

    public ValueTask<string?> GetCallStats(string id, CancellationToken cancellationToken = default)
        => InvokeAsync<string?>("getCallStats", cancellationToken, id);

    public ValueTask SetAudioVolume(string id, double volume, CancellationToken cancellationToken = default)
        => InvokeVoidAsync("setAudioVolume", cancellationToken, id, volume);

    public ValueTask Connect(string id, CancellationToken cancellationToken = default)
        => InvokeVoidAsync("connect", cancellationToken, id);

    public async ValueTask DisposeAsync()
    {
        using var lease = await _lifecycle.Acquire();
        if (_disposed) return;
        _disposed = true;
        try { await _moduleImportUtil.DisposeContentModule(_modulePath); }
        finally
        {
            try { await _scriptInitializer.DisposeAsync(); }
            finally { await _cancellationScope.DisposeAsync(); }
        }
    }
}
