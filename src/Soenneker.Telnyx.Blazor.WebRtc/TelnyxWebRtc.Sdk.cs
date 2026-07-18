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

    private async ValueTask SdkExecute(Func<string, CancellationToken, ValueTask> action, CancellationToken cancellationToken)
    {
        EnsureSdkInitialized();
        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);
        using (source)
            await action(Id!, linked);
    }

    private async ValueTask<T> SdkExecute<T>(Func<string, CancellationToken, ValueTask<T>> action, CancellationToken cancellationToken)
    {
        EnsureSdkInitialized();
        CancellationToken linked = CancellationToken.Link(cancellationToken, out CancellationTokenSource? source);
        using (source)
            return await action(Id!, linked);
    }

    private void EnsureSdkInitialized()
    {
        if (!_initialized || Id is null)
            throw new InvalidOperationException("WebRTC has not been initialized yet.");
    }

    private static T? DeserializeNullable<T>(string? json) where T : class => json is null ? null : JsonUtil.Deserialize<T>(json);
    private static JsonElement? DeserializeElement(string? json) => json is null ? null : JsonUtil.Deserialize<JsonElement>(json);

    public async ValueTask<List<TelnyxWebRtcCall>> GetActiveCalls(CancellationToken cancellationToken = default) =>
        JsonUtil.Deserialize<List<TelnyxWebRtcCall>>(await SdkExecute(TelnyxWebRtcInterop.GetActiveCalls, cancellationToken)) ?? [];
    public ValueTask<bool> GetIsRegistered(CancellationToken cancellationToken = default) => SdkExecute(TelnyxWebRtcInterop.GetIsRegistered, cancellationToken);
    public async ValueTask<JsonElement?> SpeedTest(int bytes, CancellationToken cancellationToken = default) => DeserializeElement(await SdkExecute((id, ct) => TelnyxWebRtcInterop.SpeedTest(id, bytes, ct), cancellationToken));
    public ValueTask<string?> ValidateDeviceId(string deviceId, string label, string kind, CancellationToken cancellationToken = default) => SdkExecute((id, ct) => TelnyxWebRtcInterop.ValidateDeviceId(id, deviceId, label, kind, ct), cancellationToken);
    public async ValueTask<List<TelnyxDeviceResolution>> GetDeviceResolutions(string deviceId, CancellationToken cancellationToken = default) => JsonUtil.Deserialize<List<TelnyxDeviceResolution>>(await SdkExecute((id, ct) => TelnyxWebRtcInterop.GetDeviceResolutions(id, deviceId, ct), cancellationToken)) ?? [];
    public async ValueTask<JsonElement?> GetMediaConstraints(CancellationToken cancellationToken = default) => DeserializeElement(await SdkExecute(TelnyxWebRtcInterop.GetMediaConstraints, cancellationToken));
    public async ValueTask<List<TelnyxIceServer>> GetIceServers(CancellationToken cancellationToken = default) => JsonUtil.Deserialize<List<TelnyxIceServer>>(await SdkExecute(TelnyxWebRtcInterop.GetIceServers, cancellationToken)) ?? [];
    public ValueTask SetIceServers(List<TelnyxIceServer> servers, CancellationToken cancellationToken = default) => SdkExecute((id, ct) => TelnyxWebRtcInterop.SetIceServers(id, servers, ct), cancellationToken);
    public ValueTask<string?> GetSpeaker(CancellationToken cancellationToken = default) => SdkExecute(TelnyxWebRtcInterop.GetSpeaker, cancellationToken);
    public ValueTask SetSpeaker(string deviceId, CancellationToken cancellationToken = default) => SdkExecute((id, ct) => TelnyxWebRtcInterop.SetSpeaker(id, deviceId, ct), cancellationToken);
    public ValueTask SetLocalElement(string elementId, CancellationToken cancellationToken = default) => SdkExecute((id, ct) => TelnyxWebRtcInterop.SetLocalElement(id, elementId, ct), cancellationToken);
    public ValueTask SetRemoteElement(string elementId, CancellationToken cancellationToken = default) => SdkExecute((id, ct) => TelnyxWebRtcInterop.SetRemoteElement(id, elementId, ct), cancellationToken);
    public ValueTask SetLocalElement(IJSObjectReference element, CancellationToken cancellationToken = default) => SdkExecute((id, ct) => TelnyxWebRtcInterop.SetLocalElement(id, element, ct), cancellationToken);
    public ValueTask SetRemoteElement(IJSObjectReference element, CancellationToken cancellationToken = default) => SdkExecute((id, ct) => TelnyxWebRtcInterop.SetRemoteElement(id, element, ct), cancellationToken);
    public ValueTask Login(TelnyxLoginOptions? options = null, CancellationToken cancellationToken = default) => SdkExecute((id, ct) => TelnyxWebRtcInterop.Login(id, options, ct), cancellationToken);
    public ValueTask Logout(CancellationToken cancellationToken = default) => SdkExecute(TelnyxWebRtcInterop.Logout, cancellationToken);
    public ValueTask<bool> HasActiveCall(CancellationToken cancellationToken = default) => SdkExecute(TelnyxWebRtcInterop.HasActiveCall, cancellationToken);
    public async ValueTask<TelnyxWebRtcInfo?> GetWebRtcInfo(CancellationToken cancellationToken = default) => DeserializeNullable<TelnyxWebRtcInfo>(await TelnyxWebRtcInterop.WebRtcInfo(cancellationToken));
    public async ValueTask<TelnyxPreCallDiagnosisReport?> RunPreCallDiagnosis(TelnyxPreCallDiagnosisOptions options, CancellationToken cancellationToken = default) => DeserializeNullable<TelnyxPreCallDiagnosisReport>(await TelnyxWebRtcInterop.RunPreCallDiagnosis(options, cancellationToken));
    public async ValueTask<TelnyxWebRtcCall?> GetCurrentCall(CancellationToken cancellationToken = default) => DeserializeNullable<TelnyxWebRtcCall>(await SdkExecute(TelnyxWebRtcInterop.GetCurrentCall, cancellationToken));
    public ValueTask<IJSObjectReference?> GetLocalStream(CancellationToken cancellationToken = default) => SdkExecute(TelnyxWebRtcInterop.GetLocalStream, cancellationToken);
    public ValueTask<IJSObjectReference?> GetRemoteStream(CancellationToken cancellationToken = default) => SdkExecute(TelnyxWebRtcInterop.GetRemoteStream, cancellationToken);
    public ValueTask SendConversationMessage(string message, string[]? attachments = null, CancellationToken cancellationToken = default) => SdkExecute((id, ct) => TelnyxWebRtcInterop.SendConversationMessage(id, message, attachments, ct), cancellationToken);
    public ValueTask SendAiConversationMessage(TelnyxFunctionCallOutput item, CancellationToken cancellationToken = default) => SdkExecute((id, ct) => TelnyxWebRtcInterop.SendAiConversationMessage(id, item, ct), cancellationToken);
    public ValueTask SetAudioInDevice(string deviceId, bool? muted, CancellationToken cancellationToken = default) => SdkExecute((id, ct) => TelnyxWebRtcInterop.SetAudioInDevice(id, deviceId, muted, ct), cancellationToken);
    public ValueTask StartScreenShare(TelnyxCallOptions options, CancellationToken cancellationToken = default) =>
        SdkExecute((id, ct) => TelnyxWebRtcInterop.StartScreenShare(id, options, options.LocalStream, options.RemoteStream,
            options.LocalElementReference, options.RemoteElementReference, ct), cancellationToken);
}
