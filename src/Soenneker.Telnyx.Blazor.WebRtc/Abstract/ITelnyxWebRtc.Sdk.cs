using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Soenneker.Telnyx.Blazor.WebRtc.Configuration;
using Soenneker.Telnyx.Blazor.WebRtc.Dtos;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Telnyx.Blazor.WebRtc.Abstract;

public partial interface ITelnyxWebRtc
{
    EventCallback<string> OnWarning { get; set; }
    EventCallback OnMediaPermissionsRecoverySuccess { get; set; }
    EventCallback<string> OnMediaPermissionsRecoveryError { get; set; }
    EventCallback<string> OnAiConversationMessage { get; set; }

    ValueTask<List<TelnyxWebRtcCall>> GetActiveCalls(CancellationToken cancellationToken = default);
    ValueTask<bool> GetIsRegistered(CancellationToken cancellationToken = default);
    ValueTask<JsonElement?> SpeedTest(int bytes, CancellationToken cancellationToken = default);
    ValueTask<string?> ValidateDeviceId(string deviceId, string label, string kind, CancellationToken cancellationToken = default);
    ValueTask<List<TelnyxDeviceResolution>> GetDeviceResolutions(string deviceId, CancellationToken cancellationToken = default);
    ValueTask<JsonElement?> GetMediaConstraints(CancellationToken cancellationToken = default);
    ValueTask<List<TelnyxIceServer>> GetIceServers(CancellationToken cancellationToken = default);
    ValueTask SetIceServers(List<TelnyxIceServer> servers, CancellationToken cancellationToken = default);
    ValueTask<string?> GetSpeaker(CancellationToken cancellationToken = default);
    ValueTask SetSpeaker(string deviceId, CancellationToken cancellationToken = default);
    ValueTask SetLocalElement(string elementId, CancellationToken cancellationToken = default);
    ValueTask SetRemoteElement(string elementId, CancellationToken cancellationToken = default);
    ValueTask SetLocalElement(IJSObjectReference element, CancellationToken cancellationToken = default);
    ValueTask SetRemoteElement(IJSObjectReference element, CancellationToken cancellationToken = default);
    ValueTask Login(TelnyxLoginOptions? options = null, CancellationToken cancellationToken = default);
    ValueTask Logout(CancellationToken cancellationToken = default);
    ValueTask<bool> HasActiveCall(CancellationToken cancellationToken = default);
    ValueTask<TelnyxWebRtcInfo?> GetWebRtcInfo(CancellationToken cancellationToken = default);
    ValueTask<TelnyxPreCallDiagnosisReport?> RunPreCallDiagnosis(TelnyxPreCallDiagnosisOptions options, CancellationToken cancellationToken = default);

    ValueTask<TelnyxWebRtcCall?> GetCurrentCall(CancellationToken cancellationToken = default);
    ValueTask<IJSObjectReference?> GetLocalStream(CancellationToken cancellationToken = default);
    ValueTask<IJSObjectReference?> GetRemoteStream(CancellationToken cancellationToken = default);
    ValueTask SendConversationMessage(string message, string[]? attachments = null, CancellationToken cancellationToken = default);
    ValueTask SendAiConversationMessage(TelnyxFunctionCallOutput item, CancellationToken cancellationToken = default);
    ValueTask SetAudioInDevice(string deviceId, bool? muted, CancellationToken cancellationToken = default);
    ValueTask StartScreenShare(TelnyxCallOptions options, CancellationToken cancellationToken = default);
}
