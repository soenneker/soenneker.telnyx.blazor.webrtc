using Microsoft.JSInterop;
using Soenneker.Telnyx.Blazor.WebRtc.Configuration;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Telnyx.Blazor.WebRtc.Abstract;

public partial interface ITelnyxWebRtcInterop
{
    ValueTask<string> GetActiveCalls(string id, CancellationToken cancellationToken = default);
    ValueTask<bool> GetIsRegistered(string id, CancellationToken cancellationToken = default);
    ValueTask<string?> SpeedTest(string id, int bytes, CancellationToken cancellationToken = default);
    ValueTask<string?> ValidateDeviceId(string id, string deviceId, string label, string kind, CancellationToken cancellationToken = default);
    ValueTask<string> GetDeviceResolutions(string id, string deviceId, CancellationToken cancellationToken = default);
    ValueTask<string?> GetMediaConstraints(string id, CancellationToken cancellationToken = default);
    ValueTask<string> GetIceServers(string id, CancellationToken cancellationToken = default);
    ValueTask SetIceServers(string id, List<TelnyxIceServer> servers, CancellationToken cancellationToken = default);
    ValueTask<string?> GetSpeaker(string id, CancellationToken cancellationToken = default);
    ValueTask SetSpeaker(string id, string deviceId, CancellationToken cancellationToken = default);
    ValueTask SetLocalElement(string id, string elementId, CancellationToken cancellationToken = default);
    ValueTask SetRemoteElement(string id, string elementId, CancellationToken cancellationToken = default);
    ValueTask SetLocalElement(string id, IJSObjectReference element, CancellationToken cancellationToken = default);
    ValueTask SetRemoteElement(string id, IJSObjectReference element, CancellationToken cancellationToken = default);
    ValueTask Login(string id, TelnyxLoginOptions? options, CancellationToken cancellationToken = default);
    ValueTask Logout(string id, CancellationToken cancellationToken = default);
    ValueTask<bool> HasActiveCall(string id, CancellationToken cancellationToken = default);
    ValueTask<string?> WebRtcInfo(CancellationToken cancellationToken = default);
    ValueTask<string?> RunPreCallDiagnosis(TelnyxPreCallDiagnosisOptions options, CancellationToken cancellationToken = default);
    ValueTask<string?> GetCurrentCall(string id, CancellationToken cancellationToken = default);
    ValueTask<IJSObjectReference?> GetLocalStream(string id, CancellationToken cancellationToken = default);
    ValueTask<IJSObjectReference?> GetRemoteStream(string id, CancellationToken cancellationToken = default);
    ValueTask SendConversationMessage(string id, string message, string[]? attachments, CancellationToken cancellationToken = default);
    ValueTask SendAiConversationMessage(string id, TelnyxFunctionCallOutput item, CancellationToken cancellationToken = default);
    ValueTask SetAudioInDevice(string id, string deviceId, bool? muted, CancellationToken cancellationToken = default);
    ValueTask StartScreenShare(string id, TelnyxCallOptions options, IJSObjectReference? localStream, IJSObjectReference? remoteStream,
        IJSObjectReference? localElement, IJSObjectReference? remoteElement, CancellationToken cancellationToken = default);
}
