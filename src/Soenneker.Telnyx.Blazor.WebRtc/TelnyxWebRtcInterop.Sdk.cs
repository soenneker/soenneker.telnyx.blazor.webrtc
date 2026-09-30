using Microsoft.JSInterop;
using Soenneker.Telnyx.Blazor.WebRtc.Configuration;
using Soenneker.Utils.Json;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Telnyx.Blazor.WebRtc;

public sealed partial class TelnyxWebRtcInterop
{
    public ValueTask<string> GetActiveCalls(string id, CancellationToken cancellationToken = default) => Execute(t => InvokeAsync<string>("getActiveCalls", t, id), cancellationToken);
    public ValueTask<bool> GetIsRegistered(string id, CancellationToken cancellationToken = default) => Execute(t => InvokeAsync<bool>("getIsRegistered", t, id), cancellationToken);
    public ValueTask<string?> SpeedTest(string id, int bytes, CancellationToken cancellationToken = default) => Execute(t => InvokeAsync<string?>("speedTest", t, id, bytes), cancellationToken);
    public ValueTask<string?> ValidateDeviceId(string id, string deviceId, string label, string kind, CancellationToken cancellationToken = default) => Execute(t => InvokeAsync<string?>("validateDeviceId", t, id, deviceId, label, kind), cancellationToken);
    public ValueTask<string> GetDeviceResolutions(string id, string deviceId, CancellationToken cancellationToken = default) => Execute(t => InvokeAsync<string>("getDeviceResolutions", t, id, deviceId), cancellationToken);
    public ValueTask<string?> GetMediaConstraints(string id, CancellationToken cancellationToken = default) => Execute(t => InvokeAsync<string?>("getMediaConstraints", t, id), cancellationToken);
    public ValueTask<string> GetIceServers(string id, CancellationToken cancellationToken = default) => Execute(t => InvokeAsync<string>("getIceServers", t, id), cancellationToken);
    public ValueTask SetIceServers(string id, List<TelnyxIceServer> servers, CancellationToken cancellationToken = default) => Execute(t => InvokeVoidAsync("setIceServers", t, id, JsonUtil.Serialize(servers)), cancellationToken);
    public ValueTask<string?> GetSpeaker(string id, CancellationToken cancellationToken = default) => Execute(t => InvokeAsync<string?>("getSpeaker", t, id), cancellationToken);
    public ValueTask SetSpeaker(string id, string deviceId, CancellationToken cancellationToken = default) => Execute(t => InvokeVoidAsync("setSpeaker", t, id, deviceId), cancellationToken);
    public ValueTask SetLocalElement(string id, string elementId, CancellationToken cancellationToken = default) => Execute(t => InvokeVoidAsync("setLocalElement", t, id, elementId), cancellationToken);
    public ValueTask SetRemoteElement(string id, string elementId, CancellationToken cancellationToken = default) => Execute(t => InvokeVoidAsync("setRemoteElement", t, id, elementId), cancellationToken);
    public ValueTask SetLocalElement(string id, IJSObjectReference element, CancellationToken cancellationToken = default) => Execute(t => InvokeVoidAsync("setLocalElement", t, id, element), cancellationToken);
    public ValueTask SetRemoteElement(string id, IJSObjectReference element, CancellationToken cancellationToken = default) => Execute(t => InvokeVoidAsync("setRemoteElement", t, id, element), cancellationToken);
    public ValueTask Login(string id, TelnyxLoginOptions? options, CancellationToken cancellationToken = default) => Execute(t => InvokeVoidAsync("login", t, id, options is null ? null : JsonUtil.Serialize(options)), cancellationToken);
    public ValueTask Logout(string id, CancellationToken cancellationToken = default) => Execute(t => InvokeVoidAsync("logout", t, id), cancellationToken);
    public ValueTask<bool> HasActiveCall(string id, CancellationToken cancellationToken = default) => Execute(t => InvokeAsync<bool>("hasActiveCall", t, id), cancellationToken);
    public ValueTask<string?> WebRtcInfo(CancellationToken cancellationToken = default) => Execute(t => InvokeAsync<string?>("webRtcInfo", t), cancellationToken);
    public ValueTask<string?> RunPreCallDiagnosis(TelnyxPreCallDiagnosisOptions options, CancellationToken cancellationToken = default) => Execute(t => InvokeAsync<string?>("runPreCallDiagnosis", t, JsonUtil.Serialize(options)), cancellationToken);
    public ValueTask<string?> GetCurrentCall(string id, CancellationToken cancellationToken = default) => Execute(t => InvokeAsync<string?>("getCurrentCall", t, id), cancellationToken);
    public ValueTask<IJSObjectReference?> GetLocalStream(string id, CancellationToken cancellationToken = default) => Execute(t => InvokeAsync<IJSObjectReference?>("getLocalStream", t, id), cancellationToken);
    public ValueTask<IJSObjectReference?> GetRemoteStream(string id, CancellationToken cancellationToken = default) => Execute(t => InvokeAsync<IJSObjectReference?>("getRemoteStream", t, id), cancellationToken);
    public ValueTask SendConversationMessage(string id, string message, string[]? attachments, CancellationToken cancellationToken = default) => Execute(t => InvokeVoidAsync("sendConversationMessage", t, id, message, attachments), cancellationToken);
    public ValueTask SendAiConversationMessage(string id, TelnyxFunctionCallOutput item, CancellationToken cancellationToken = default) => Execute(t => InvokeVoidAsync("sendAiConversationMessage", t, id, JsonUtil.Serialize(item)), cancellationToken);
    public ValueTask SetAudioInDevice(string id, string deviceId, bool? muted, CancellationToken cancellationToken = default) => Execute(t => InvokeVoidAsync("setAudioInDevice", t, id, deviceId, muted), cancellationToken);
    public ValueTask StartScreenShare(string id, TelnyxCallOptions options, IJSObjectReference? localStream, IJSObjectReference? remoteStream,
        IJSObjectReference? localElement, IJSObjectReference? remoteElement, CancellationToken cancellationToken = default) =>
        Execute(t => InvokeVoidAsync("startScreenShare", t, id, JsonUtil.Serialize(options), localStream, remoteStream, localElement, remoteElement), cancellationToken);
}
