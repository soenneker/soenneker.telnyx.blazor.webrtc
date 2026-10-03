using Microsoft.JSInterop;
using Soenneker.Telnyx.Blazor.WebRtc.Configuration;
using Soenneker.Utils.Json;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Telnyx.Blazor.WebRtc;

public sealed partial class TelnyxWebRtcInterop
{
    public ValueTask<string> GetActiveCalls(string id, CancellationToken cancellationToken = default) => InvokeAsync<string>("getActiveCalls", cancellationToken, id);
    public ValueTask<bool> GetIsRegistered(string id, CancellationToken cancellationToken = default) => InvokeAsync<bool>("getIsRegistered", cancellationToken, id);
    public ValueTask<string?> SpeedTest(string id, int bytes, CancellationToken cancellationToken = default) => InvokeAsync<string?>("speedTest", cancellationToken, id, bytes);
    public ValueTask<string?> ValidateDeviceId(string id, string deviceId, string label, string kind, CancellationToken cancellationToken = default) => InvokeAsync<string?>("validateDeviceId", cancellationToken, id, deviceId, label, kind);
    public ValueTask<string> GetDeviceResolutions(string id, string deviceId, CancellationToken cancellationToken = default) => InvokeAsync<string>("getDeviceResolutions", cancellationToken, id, deviceId);
    public ValueTask<string?> GetMediaConstraints(string id, CancellationToken cancellationToken = default) => InvokeAsync<string?>("getMediaConstraints", cancellationToken, id);
    public ValueTask<string> GetIceServers(string id, CancellationToken cancellationToken = default) => InvokeAsync<string>("getIceServers", cancellationToken, id);
    public ValueTask SetIceServers(string id, List<TelnyxIceServer> servers, CancellationToken cancellationToken = default) => InvokeVoidAsync("setIceServers", cancellationToken, id, JsonUtil.Serialize(servers));
    public ValueTask<string?> GetSpeaker(string id, CancellationToken cancellationToken = default) => InvokeAsync<string?>("getSpeaker", cancellationToken, id);
    public ValueTask SetSpeaker(string id, string deviceId, CancellationToken cancellationToken = default) => InvokeVoidAsync("setSpeaker", cancellationToken, id, deviceId);
    public ValueTask SetLocalElement(string id, string elementId, CancellationToken cancellationToken = default) => InvokeVoidAsync("setLocalElement", cancellationToken, id, elementId);
    public ValueTask SetRemoteElement(string id, string elementId, CancellationToken cancellationToken = default) => InvokeVoidAsync("setRemoteElement", cancellationToken, id, elementId);
    public ValueTask SetLocalElement(string id, IJSObjectReference element, CancellationToken cancellationToken = default) => InvokeVoidAsync("setLocalElement", cancellationToken, id, element);
    public ValueTask SetRemoteElement(string id, IJSObjectReference element, CancellationToken cancellationToken = default) => InvokeVoidAsync("setRemoteElement", cancellationToken, id, element);
    public ValueTask Login(string id, TelnyxLoginOptions? options, CancellationToken cancellationToken = default) => InvokeVoidAsync("login", cancellationToken, id, options is null ? null : JsonUtil.Serialize(options));
    public ValueTask Logout(string id, CancellationToken cancellationToken = default) => InvokeVoidAsync("logout", cancellationToken, id);
    public ValueTask<bool> HasActiveCall(string id, CancellationToken cancellationToken = default) => InvokeAsync<bool>("hasActiveCall", cancellationToken, id);
    public ValueTask<string?> WebRtcInfo(CancellationToken cancellationToken = default) => InvokeAsync<string?>("webRtcInfo", cancellationToken);
    public ValueTask<string?> RunPreCallDiagnosis(TelnyxPreCallDiagnosisOptions options, CancellationToken cancellationToken = default) => InvokeAsync<string?>("runPreCallDiagnosis", cancellationToken, JsonUtil.Serialize(options));
    public ValueTask<string?> GetCurrentCall(string id, CancellationToken cancellationToken = default) => InvokeAsync<string?>("getCurrentCall", cancellationToken, id);
    public ValueTask<IJSObjectReference?> GetLocalStream(string id, CancellationToken cancellationToken = default) => InvokeAsync<IJSObjectReference?>("getLocalStream", cancellationToken, id);
    public ValueTask<IJSObjectReference?> GetRemoteStream(string id, CancellationToken cancellationToken = default) => InvokeAsync<IJSObjectReference?>("getRemoteStream", cancellationToken, id);
    public ValueTask SendConversationMessage(string id, string message, string[]? attachments, CancellationToken cancellationToken = default) => InvokeVoidAsync("sendConversationMessage", cancellationToken, id, message, attachments);
    public ValueTask SendAiConversationMessage(string id, TelnyxFunctionCallOutput item, CancellationToken cancellationToken = default) => InvokeVoidAsync("sendAiConversationMessage", cancellationToken, id, JsonUtil.Serialize(item));
    public ValueTask SetAudioInDevice(string id, string deviceId, bool? muted, CancellationToken cancellationToken = default) => InvokeVoidAsync("setAudioInDevice", cancellationToken, id, deviceId, muted);
    public ValueTask StartScreenShare(string id, TelnyxCallOptions options, IJSObjectReference? localStream, IJSObjectReference? remoteStream,
        IJSObjectReference? localElement, IJSObjectReference? remoteElement, CancellationToken cancellationToken = default) =>
        InvokeVoidAsync("startScreenShare", cancellationToken, id, JsonUtil.Serialize(options), localStream, remoteStream, localElement, remoteElement);
}
