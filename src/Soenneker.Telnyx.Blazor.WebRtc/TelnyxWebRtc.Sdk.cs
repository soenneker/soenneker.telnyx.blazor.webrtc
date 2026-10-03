using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Soenneker.Extensions.CancellationTokens;
using Soenneker.Telnyx.Blazor.WebRtc.Configuration;
using Soenneker.Telnyx.Blazor.WebRtc.Dtos;
using Soenneker.Utils.Json;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Telnyx.Blazor.WebRtc;

public partial class TelnyxWebRtc
{
    [Parameter] public EventCallback<string> OnWarning { get; set; }
    [Parameter] public EventCallback OnMediaPermissionsRecoverySuccess { get; set; }
    [Parameter] public EventCallback<string> OnMediaPermissionsRecoveryError { get; set; }
    [Parameter] public EventCallback<string> OnAiConversationMessage { get; set; }

    private async ValueTask SdkExecute<TState>(TState state, Func<TState, string, CancellationToken, ValueTask> action, CancellationToken cancellationToken)
    {
        EnsureSdkInitialized();
        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);
        using (source)
            await action(state, Id!, linked);
    }

    private async ValueTask<T> SdkExecute<TState, T>(TState state, Func<TState, string, CancellationToken, ValueTask<T>> action, CancellationToken cancellationToken)
    {
        EnsureSdkInitialized();
        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);
        using (source)
            return await action(state, Id!, linked);
    }

    private void EnsureSdkInitialized()
    {
        if (!_initialized || Id is null)
            throw new InvalidOperationException("WebRTC has not been initialized yet.");
    }

    private static T? DeserializeNullable<T>(string? json) where T : class => json is null ? null : JsonUtil.Deserialize<T>(json, LibraryJsonContext.Get<T>());
    private static JsonElement? DeserializeElement(string? json) => json is null ? null : JsonUtil.Deserialize<JsonElement>(json, LibraryJsonContext.Get<JsonElement>());

    public async ValueTask<List<TelnyxWebRtcCall>> GetActiveCalls(CancellationToken cancellationToken = default) =>
        JsonUtil.Deserialize<List<TelnyxWebRtcCall>>(await SdkExecute(TelnyxWebRtcInterop, static (interop, id, ct) => interop.GetActiveCalls(id, ct), cancellationToken), LibraryJsonContext.Get<List<TelnyxWebRtcCall>>()) ?? [];
    public ValueTask<bool> GetIsRegistered(CancellationToken cancellationToken = default) => SdkExecute(TelnyxWebRtcInterop, static (interop, id, ct) => interop.GetIsRegistered(id, ct), cancellationToken);
    public async ValueTask<JsonElement?> SpeedTest(int bytes, CancellationToken cancellationToken = default) => DeserializeElement(await SdkExecute((Interop: TelnyxWebRtcInterop, bytes), static (state, id, ct) => state.Interop.SpeedTest(id, state.bytes, ct), cancellationToken));
    public ValueTask<string?> ValidateDeviceId(string deviceId, string label, string kind, CancellationToken cancellationToken = default) => SdkExecute((Interop: TelnyxWebRtcInterop, deviceId, label, kind), static (state, id, ct) => state.Interop.ValidateDeviceId(id, state.deviceId, state.label, state.kind, ct), cancellationToken);
    public async ValueTask<List<TelnyxDeviceResolution>> GetDeviceResolutions(string deviceId, CancellationToken cancellationToken = default) => JsonUtil.Deserialize<List<TelnyxDeviceResolution>>(await SdkExecute((Interop: TelnyxWebRtcInterop, deviceId), static (state, id, ct) => state.Interop.GetDeviceResolutions(id, state.deviceId, ct), cancellationToken), LibraryJsonContext.Get<List<TelnyxDeviceResolution>>()) ?? [];
    public async ValueTask<JsonElement?> GetMediaConstraints(CancellationToken cancellationToken = default) => DeserializeElement(await SdkExecute(TelnyxWebRtcInterop, static (interop, id, ct) => interop.GetMediaConstraints(id, ct), cancellationToken));
    public async ValueTask<List<TelnyxIceServer>> GetIceServers(CancellationToken cancellationToken = default) => JsonUtil.Deserialize<List<TelnyxIceServer>>(await SdkExecute(TelnyxWebRtcInterop, static (interop, id, ct) => interop.GetIceServers(id, ct), cancellationToken), LibraryJsonContext.Get<List<TelnyxIceServer>>()) ?? [];
    public ValueTask SetIceServers(List<TelnyxIceServer> servers, CancellationToken cancellationToken = default) => SdkExecute((Interop: TelnyxWebRtcInterop, servers), static (state, id, ct) => state.Interop.SetIceServers(id, state.servers, ct), cancellationToken);
    public ValueTask<string?> GetSpeaker(CancellationToken cancellationToken = default) => SdkExecute(TelnyxWebRtcInterop, static (interop, id, ct) => interop.GetSpeaker(id, ct), cancellationToken);
    public ValueTask SetSpeaker(string deviceId, CancellationToken cancellationToken = default) => SdkExecute((Interop: TelnyxWebRtcInterop, deviceId), static (state, id, ct) => state.Interop.SetSpeaker(id, state.deviceId, ct), cancellationToken);
    public ValueTask SetLocalElement(string elementId, CancellationToken cancellationToken = default) => SdkExecute((Interop: TelnyxWebRtcInterop, elementId), static (state, id, ct) => state.Interop.SetLocalElement(id, state.elementId, ct), cancellationToken);
    public ValueTask SetRemoteElement(string elementId, CancellationToken cancellationToken = default) => SdkExecute((Interop: TelnyxWebRtcInterop, elementId), static (state, id, ct) => state.Interop.SetRemoteElement(id, state.elementId, ct), cancellationToken);
    public ValueTask SetLocalElement(IJSObjectReference element, CancellationToken cancellationToken = default) => SdkExecute((Interop: TelnyxWebRtcInterop, element), static (state, id, ct) => state.Interop.SetLocalElement(id, state.element, ct), cancellationToken);
    public ValueTask SetRemoteElement(IJSObjectReference element, CancellationToken cancellationToken = default) => SdkExecute((Interop: TelnyxWebRtcInterop, element), static (state, id, ct) => state.Interop.SetRemoteElement(id, state.element, ct), cancellationToken);
    public ValueTask Login(TelnyxLoginOptions? options = null, CancellationToken cancellationToken = default) => SdkExecute((Interop: TelnyxWebRtcInterop, options), static (state, id, ct) => state.Interop.Login(id, state.options, ct), cancellationToken);
    public ValueTask Logout(CancellationToken cancellationToken = default) => SdkExecute(TelnyxWebRtcInterop, static (interop, id, ct) => interop.Logout(id, ct), cancellationToken);
    public ValueTask<bool> HasActiveCall(CancellationToken cancellationToken = default) => SdkExecute(TelnyxWebRtcInterop, static (interop, id, ct) => interop.HasActiveCall(id, ct), cancellationToken);
    public async ValueTask<TelnyxWebRtcInfo?> GetWebRtcInfo(CancellationToken cancellationToken = default) => DeserializeNullable<TelnyxWebRtcInfo>(await TelnyxWebRtcInterop.WebRtcInfo(cancellationToken));
    public async ValueTask<TelnyxPreCallDiagnosisReport?> RunPreCallDiagnosis(TelnyxPreCallDiagnosisOptions options, CancellationToken cancellationToken = default) => DeserializeNullable<TelnyxPreCallDiagnosisReport>(await TelnyxWebRtcInterop.RunPreCallDiagnosis(options, cancellationToken));
    public async ValueTask<TelnyxWebRtcCall?> GetCurrentCall(CancellationToken cancellationToken = default) => DeserializeNullable<TelnyxWebRtcCall>(await SdkExecute(TelnyxWebRtcInterop, static (interop, id, ct) => interop.GetCurrentCall(id, ct), cancellationToken));
    public ValueTask<IJSObjectReference?> GetLocalStream(CancellationToken cancellationToken = default) => SdkExecute(TelnyxWebRtcInterop, static (interop, id, ct) => interop.GetLocalStream(id, ct), cancellationToken);
    public ValueTask<IJSObjectReference?> GetRemoteStream(CancellationToken cancellationToken = default) => SdkExecute(TelnyxWebRtcInterop, static (interop, id, ct) => interop.GetRemoteStream(id, ct), cancellationToken);
    public ValueTask SendConversationMessage(string message, string[]? attachments = null, CancellationToken cancellationToken = default) => SdkExecute((Interop: TelnyxWebRtcInterop, message, attachments), static (state, id, ct) => state.Interop.SendConversationMessage(id, state.message, state.attachments, ct), cancellationToken);
    public ValueTask SendAiConversationMessage(TelnyxFunctionCallOutput item, CancellationToken cancellationToken = default) => SdkExecute((Interop: TelnyxWebRtcInterop, item), static (state, id, ct) => state.Interop.SendAiConversationMessage(id, state.item, ct), cancellationToken);
    public ValueTask SetAudioInDevice(string deviceId, bool? muted, CancellationToken cancellationToken = default) => SdkExecute((Interop: TelnyxWebRtcInterop, deviceId, muted), static (state, id, ct) => state.Interop.SetAudioInDevice(id, state.deviceId, state.muted, ct), cancellationToken);
    public ValueTask StartScreenShare(TelnyxCallOptions options, CancellationToken cancellationToken = default) =>
        SdkExecute((Interop: TelnyxWebRtcInterop, options), static (state, id, ct) => state.Interop.StartScreenShare(id, state.options, state.options.LocalStream, state.options.RemoteStream,
            state.options.LocalElementReference, state.options.RemoteElementReference, ct), cancellationToken);
}
