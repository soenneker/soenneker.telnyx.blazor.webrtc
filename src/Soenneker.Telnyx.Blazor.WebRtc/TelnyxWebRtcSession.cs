using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Soenneker.Asyncs.Semaphores;
using Soenneker.Telnyx.Blazor.WebRtc.Abstract;
using Soenneker.Telnyx.Blazor.WebRtc.Configuration;
using Soenneker.Telnyx.Blazor.WebRtc.Dtos;
using Soenneker.Telnyx.Blazor.WebRtc.Enums;

namespace Soenneker.Telnyx.Blazor.WebRtc;

public sealed partial class TelnyxWebRtcSession : ITelnyxWebRtcSession
{
    private readonly ITelnyxWebRtcInterop TelnyxWebRtcInterop;
    private readonly ILogger<TelnyxWebRtcSession> _logger;
    private readonly AsyncSemaphore _lifecycle = new(1);
    private readonly CancellationTokenSource _lifetime = new();
    private CancellationToken CancellationToken => _lifetime.Token;
    private DotNetObjectReference<TelnyxWebRtcSession>? _reference;
    private bool _initialized;
    private volatile bool _disposed;
    private TelnyxClientOptions? _options;

    public string Id { get; } = $"telnyx-session-{Guid.NewGuid():N}";
    public TelnyxConnectionState State { get; private set; }
    public event Func<TelnyxSessionEvent, Task>? EventReceived;

    public TelnyxWebRtcSession(ITelnyxWebRtcInterop interop, ILogger<TelnyxWebRtcSession> logger)
    {
        TelnyxWebRtcInterop = interop;
        _logger = logger;
    }

    public async ValueTask Initialize(TelnyxClientOptions options, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(options.InitOptions);
        using var lease = await _lifecycle.Acquire(cancellationToken);
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_initialized) return;
        _options = options.Snapshot();
        State = TelnyxConnectionState.Initializing;
        try
        {
            await TelnyxWebRtcInterop.Initialize(_options.UseCdn, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            _reference = DotNetObjectReference.Create(this);
            // Readiness callbacks may issue commands before Create's result reaches .NET.
            _initialized = true;
            // Do not release the lifecycle gate while a dispatched create is still running.
            await TelnyxWebRtcInterop.Create(Id, _reference, _options, CancellationToken.None);
        }
        catch
        {
            try
            {
                if (_reference != null) await TelnyxWebRtcInterop.Unmount(Id, CancellationToken.None);
            }
            catch { /* Preserve the initialization failure; the bridge also rolls back creation. */ }
            finally
            {
                _initialized = false;
                _reference?.Dispose();
                _reference = null;
                _options = null;
                State = TelnyxConnectionState.Disconnected;
            }
            throw;
        }
    }

    public ValueTask<bool> ResumePlayback(CancellationToken cancellationToken = default)
    {
        EnsureSdkInitialized();
        return TelnyxWebRtcInterop.ResumePlayback(Id, cancellationToken);
    }

    [JSInvokable("HandleTelnyxEvent")]
    public async Task HandleTelnyxEvent(string eventName, string jsonArgs)
    {
        if (_disposed) return;
        TelnyxSessionEvent snapshot;
        try
        {
            if (eventName == "connectionState" && Enum.TryParse(jsonArgs.Trim('"'), true, out TelnyxConnectionState state)) State = state;
            JsonElement? data = null;
            if (!string.IsNullOrEmpty(jsonArgs))
            {
                try { data = JsonSerializer.Deserialize(jsonArgs, LibraryJsonContext.Get<JsonElement>()); }
                catch (JsonException) { /* SDK string events are not necessarily JSON. */ }
            }
            TelnyxNotification? notification = null;
            if ((eventName == "notification" || eventName.StartsWith("call", StringComparison.Ordinal)) && data?.ValueKind == JsonValueKind.Object)
                notification = JsonSerializer.Deserialize(jsonArgs, LibraryJsonContext.Get<TelnyxNotification>());
            TelnyxWebRtcError? error = notification?.Error;
            if (eventName.Contains("error", StringComparison.OrdinalIgnoreCase) && data is { ValueKind: JsonValueKind.Object } element)
            {
                var value = element.TryGetProperty("error", out JsonElement nested) ? nested : element;
                error = value.ValueKind == JsonValueKind.Object
                    ? JsonSerializer.Deserialize(value, LibraryJsonContext.Get<TelnyxWebRtcError>()) : null;
                if (error != null)
                {
                    if (element.TryGetProperty("callId", out var callId)) error.CallId = callId.GetString();
                    if (element.TryGetProperty("sessionId", out var sessionId)) error.SessionId = sessionId.GetString();
                    if (element.TryGetProperty("recoverable", out var recoverable) && recoverable.ValueKind is JsonValueKind.True or JsonValueKind.False)
                        error.Recoverable = recoverable.GetBoolean();
                }
            }
            var diagnostic = eventName == "diagnostic" ? JsonSerializer.Deserialize(jsonArgs, LibraryJsonContext.Get<TelnyxDiagnostic>()) : null;
            snapshot = new TelnyxSessionEvent {
                Name = eventName, ConnectionState = State, Notification = notification,
                Call = notification?.Call ?? (eventName == "additionalCall" ? JsonSerializer.Deserialize(jsonArgs, LibraryJsonContext.Get<TelnyxWebRtcCall>()) : null),
                Error = error, Diagnostic = diagnostic, Data = data,
                RawPayload = _options?.EnableRawEvents == true ? jsonArgs : null, LegacyPayload = jsonArgs
            };
            if (_options?.EnableDiagnostics == true && diagnostic != null)
                _logger.LogInformation("Telnyx {EventName}: {ConnectionState}; elapsed {ElapsedMilliseconds}ms; reconnects {ReconnectAttempts}",
                    diagnostic.Name, diagnostic.State, diagnostic.ElapsedMilliseconds, diagnostic.ReconnectAttempts);
        }
        catch (JsonException)
        {
            snapshot = new TelnyxSessionEvent { Name = "error", ConnectionState = State,
                Error = new TelnyxWebRtcError { Name = "InvalidSdkPayload", Message = "Unable to decode SDK event." } };
        }
        var handlers = EventReceived?.GetInvocationList();
        if (handlers is null) return;
        foreach (Func<TelnyxSessionEvent, Task> handler in handlers)
        {
            if (_disposed) return;
            try { await handler(snapshot); }
            catch
            {
                // Application exceptions can contain sensitive data: never log their messages.
                if (_options?.EnableDiagnostics == true) _logger.LogWarning("Telnyx event subscriber failed.");
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        _disposed = true;
        using var lease = await _lifecycle.Acquire();
        if (State == TelnyxConnectionState.Disposed) return;
        try
        {
            if (_reference != null) await TelnyxWebRtcInterop.Unmount(Id, CancellationToken.None);
        }
        catch (JSDisconnectedException) { }
        finally
        {
            _initialized = false;
            _reference?.Dispose();
            _reference = null;
            _options = null;
            EventReceived = null;
            State = TelnyxConnectionState.Disposed;
            await _lifetime.CancelAsync();
            _lifetime.Dispose();
        }
    }
}
