using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Soenneker.Asyncs.Semaphores;
using Soenneker.Telnyx.Blazor.WebRtc.Abstract;
using Soenneker.Telnyx.Blazor.WebRtc.Configuration;
using Soenneker.Telnyx.Blazor.WebRtc.Dtos;
using Soenneker.Telnyx.Blazor.WebRtc.Enums;

namespace Soenneker.Telnyx.Blazor.WebRtc;

public partial class TelnyxWebRtc
{
    [Inject] private ITelnyxWebRtcSessionFactory SessionFactory { get; set; } = null!;
    [Inject] private ITelnyxWebRtcInterop Interop { get; set; } = null!;
    [Parameter] public TelnyxClientOptions? Options { get; set; }
    [Parameter] public TelnyxMediaMode? MediaMode { get; set; }
    [Parameter] public EventCallback<TelnyxSessionEvent> OnSessionEvent { get; set; }
    [Parameter] public EventCallback<TelnyxConnectionState> OnConnectionStateChanged { get; set; }
    [Parameter] public EventCallback<TelnyxWebRtcError> OnSdkError { get; set; }
    [Parameter] public EventCallback<TelnyxDiagnostic> OnDiagnostic { get; set; }
    [Parameter] public EventCallback OnPlaybackBlocked { get; set; }


    private readonly AsyncSemaphore _lifecycle = new(1);
    private readonly string _remoteElementId = $"telnyx-remote-{Guid.NewGuid():N}";
    private readonly string _localElementId = $"telnyx-local-{Guid.NewGuid():N}";
    private ITelnyxWebRtcSession? _session;
    private bool _disposed;
    private bool _cleanupComplete;
    private bool _initialized;
    private double? _appliedVolume;
    private TelnyxMediaMode? _initializedMode;
    private TelnyxMediaMode EffectiveMediaMode => MediaMode ?? (RenderVideo ? TelnyxMediaMode.Video : RenderHiddenAudio ? TelnyxMediaMode.Audio : TelnyxMediaMode.External);
    private ITelnyxWebRtcSession Session => !_disposed && _session != null ? _session : throw new InvalidOperationException("WebRTC session is unavailable.");
    public TelnyxConnectionState ConnectionState => _session?.State ?? TelnyxConnectionState.Uninitialized;

    public TelnyxWebRtc() => Id = $"telnyx-webrtc-{Guid.NewGuid():N}";

    protected override void OnParametersSet()
    {
        if (!double.IsFinite(SpeakerVolume) || SpeakerVolume < 0 || SpeakerVolume > 1)
            throw new ArgumentOutOfRangeException(nameof(SpeakerVolume), "Volume must be between zero and one.");
        if (_initializedMode != null && _initializedMode != EffectiveMediaMode)
            throw new InvalidOperationException("MediaMode cannot change after initialization. Recreate the component to change rendering modes.");
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        // No JavaScript interop during prerendering.
        if (firstRender) await Initialize(Options, CancellationToken);
        if (EffectiveMediaMode != TelnyxMediaMode.External && _appliedVolume != SpeakerVolume)
        {
            await Interop.SetAudioVolume(_remoteElementId, SpeakerVolume, CancellationToken);
            _appliedVolume = SpeakerVolume;
        }
    }

    public async ValueTask Initialize(TelnyxClientOptions? options = null, CancellationToken cancellationToken = default)
    {
        using var lease = await _lifecycle.Acquire(cancellationToken);
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_initialized) return;
        var config = (options ?? Options ?? throw new InvalidOperationException("Options must be supplied before the first interactive render.")).Snapshot();
        ArgumentNullException.ThrowIfNull(config.InitOptions);
        if (EffectiveMediaMode == TelnyxMediaMode.Video) config.InitOptions.LocalElement ??= _localElementId;
        if (EffectiveMediaMode != TelnyxMediaMode.External) config.InitOptions.RemoteElement ??= _remoteElementId;
        _session ??= SessionFactory.Create();
        _session.EventReceived += HandleSessionEvent;
        try
        {
            await _session.Initialize(config, cancellationToken);
            await Interop.CreateObserver(_session.Id, Id!, CancellationToken.None);
            _initialized = true;
            _initializedMode = EffectiveMediaMode;
        }
        catch
        {
            _session.EventReceived -= HandleSessionEvent;
            try { await _session.DisposeAsync(); }
            catch { /* Preserve the initialization error after best-effort cleanup. */ }
            finally { _session = null; }
            throw;
        }
    }

    public ValueTask<bool> ResumePlayback(CancellationToken cancellationToken = default) => Session.ResumePlayback(cancellationToken);

    [JSInvokable("HandleTelnyxEvent")]
    public Task HandleTelnyxEvent(string eventName, string jsonArgs) => _session?.HandleTelnyxEvent(eventName, jsonArgs) ?? Task.CompletedTask;

    private Task HandleSessionEvent(TelnyxSessionEvent value) => InvokeAsync(async () =>
    {
        if (_disposed) return;
        await OnSessionEvent.InvokeAsync(value);
        if (_disposed) return;
        if (value.Error != null) await OnSdkError.InvokeAsync(value.Error);
        var json = value.LegacyPayload;
        switch (value.Name)
        {
            case "connectionState": await OnConnectionStateChanged.InvokeAsync(value.ConnectionState); break;
            case "diagnostic": if (value.Diagnostic != null) await OnDiagnostic.InvokeAsync(value.Diagnostic); break;
            case "playbackBlocked": await OnPlaybackBlocked.InvokeAsync(); break;
            case "initialized": await OnInitialize.InvokeAsync(); break;
            case "ready": await OnReady.InvokeAsync(); break;
            case "error": case "rtc.mediaError": case "rtc.peerConnectionFailureError": case "rtc.peerConnectionSignalingStateClosed":
                await OnError.InvokeAsync(json); break;
            case "warning": await OnWarning.InvokeAsync(json); break;
            case "notification":
                if (value.Notification != null) await OnNotification.InvokeAsync(value.Notification);
                if (value.Notification?.Type == "conferenceUpdate") await OnConferenceUpdate.InvokeAsync(json);
                else if (value.Notification?.Type != "callUpdate") await OnMessage.InvokeAsync(json);
                break;
            case "callInitiated": await OnCallInitiated.InvokeAsync(json); break;
            case "callAnswered": await OnCallAnswered.InvokeAsync(json); break;
            case "callHeld": await OnCallHeld.InvokeAsync(json); break;
            case "callResumed": await OnCallResumed.InvokeAsync(json); break;
            case "callHangup": await OnCallHangup.InvokeAsync(json); break;
            case "socket.open": await OnSocketOpen.InvokeAsync(); break;
            case "socket.close": await OnSocketClose.InvokeAsync(); break;
            case "socket.error": await OnSocketError.InvokeAsync(json); break;
            case "localStream": await OnLocalStream.InvokeAsync(); break;
            case "remoteStream": await OnRemoteStream.InvokeAsync(); break;
            case "streamStopped": await OnStreamStopped.InvokeAsync(); break;
            case "deviceChange": await OnDevicesChanged.InvokeAsync(json); break;
            case "stats.report": case "stats.frame": await OnStatsUpdate.InvokeAsync(json); break;
            case "rawSocketMessage": await OnRawSocketMessage.InvokeAsync(json); break;
            case "mediaPermissionsRecoverySuccess": await OnMediaPermissionsRecoverySuccess.InvokeAsync(); break;
            case "mediaPermissionsRecoveryError": await OnMediaPermissionsRecoveryError.InvokeAsync(json); break;
            case "ai.conversation": await OnAiConversationMessage.InvokeAsync(json); break;
            default: await OnMessage.InvokeAsync(json); break;
        }
    });

    public override async ValueTask DisposeAsync()
    {
        _disposed = true;
        using var lease = await _lifecycle.Acquire();
        if (_cleanupComplete) return;
        try
        {
            if (_session != null)
            {
                _session.EventReceived -= HandleSessionEvent;
                await _session.DisposeAsync();
            }
        }
        finally
        {
            _cleanupComplete = true;
            _session = null;
            _initialized = false;
            await base.DisposeAsync();
        }
    }
}

