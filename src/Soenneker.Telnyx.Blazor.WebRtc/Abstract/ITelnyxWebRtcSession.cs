using Microsoft.JSInterop;
using Soenneker.Extensions.CancellationTokens;
using Soenneker.Extensions.ValueTask;
using Soenneker.Telnyx.Blazor.WebRtc.Abstract;
using Soenneker.Telnyx.Blazor.WebRtc.Configuration;
using Soenneker.Telnyx.Blazor.WebRtc.Dtos;
using Soenneker.Utils.Json;
using System;
using System.Text.Json;
using Soenneker.Telnyx.Blazor.WebRtc.Enums;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Telnyx.Blazor.WebRtc.Abstract;

/// <summary>Owns one browser calling session independently of component rendering.</summary>
/// <remarks>Initialize after interactive rendering. Options are snapshotted on first initialization;
/// subsequent initialization is idempotent. Use explicit device and login methods for runtime changes.
/// Cancellation stops waiting before dispatch; it cannot undo an SDK operation already dispatched.
/// Dispose the session when its owner ends. Dispose returned JS stream references after use;
/// disposing a reference does not stop its media tracks.</remarks>
public interface ITelnyxWebRtcSession : IAsyncDisposable
{
    /// <summary>The unique browser session key.</summary>
    string Id { get; }
    /// <summary>The current connection state. Connected transport alone does not imply registration.</summary>
    TelnyxConnectionState State { get; }
    /// <summary>Receives typed event snapshots. Subscriber failures are isolated from other subscribers.</summary>
    event Func<TelnyxSessionEvent, Task>? EventReceived;
    /// <summary>Retries remote media playback. Call from a user interaction after playback is blocked.</summary>
    ValueTask<bool> ResumePlayback(CancellationToken cancellationToken = default);
    /// <summary>Receives bridge callbacks; applications should subscribe to EventReceived instead.</summary>
    Task HandleTelnyxEvent(string eventName, string jsonArgs);
    /// <summary>
    /// Initializes the Telnyx WebRTC client and sets up event bindings.
    /// </summary>
    /// <param name="options">Client configuration options such as credentials and media settings.</param>
    /// <param name="cancellationToken">Optional cancellation token for aborting initialization.</param>
    ValueTask Initialize(TelnyxClientOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Places an outbound call to a number, SIP address, or extension using the provided call options.
    /// </summary>
    /// <param name="callOptions">Options including destination, caller ID, audio/video settings, etc.</param>
    /// <param name="cancellationToken"></param>
    ValueTask Call(TelnyxCallOptions callOptions, CancellationToken cancellationToken = default);

    /// <summary>
    /// Answers an inbound call, optionally providing constraints or initial media behavior.
    /// </summary>
    /// <param name="options">Optional configuration for how to answer (e.g., with muted audio).</param>
    /// <param name="cancellationToken"></param>
    ValueTask Answer(TelnyxAnswerOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Ends the active call or conference session. Also 'rejects' the call if you haven't picked up yet.
    /// </summary>
    /// <param name="options">Optional flags such as whether to send a hangup reason.</param>
    /// <param name="cancellationToken"></param>
    ValueTask Hangup(TelnyxHangupOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Disables the local microphone without affecting incoming audio.
    /// </summary>
    ValueTask MuteAudio(CancellationToken cancellationToken = default);

    /// <summary>
    /// Re-enables the local microphone after it was muted.
    /// </summary>
    ValueTask UnmuteAudio(CancellationToken cancellationToken = default);

    /// <summary>
    /// Toggles the mute state of the microphone based on its current status.
    /// </summary>
    ValueTask ToggleAudioMute(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops sending video from the user's camera to the other party.
    /// </summary>
    ValueTask MuteVideo(CancellationToken cancellationToken = default);

    /// <summary>
    /// Starts sending video again after it was muted.
    /// </summary>
    ValueTask UnmuteVideo(CancellationToken cancellationToken = default);

    /// <summary>
    /// Toggles the current mute state of the video stream.
    /// </summary>
    ValueTask ToggleVideoMute(CancellationToken cancellationToken = default);

    /// <summary>
    /// Disables both incoming and outgoing audio streams for the local user (self-deaf).
    /// </summary>
    ValueTask Deaf(CancellationToken cancellationToken = default);

    /// <summary>
    /// Re-enables incoming and outgoing audio streams.
    /// </summary>
    ValueTask Undeaf(CancellationToken cancellationToken = default);

    /// <summary>
    /// Toggles the self-deaf state, muting or unmuting incoming audio from all participants.
    /// </summary>
    ValueTask ToggleDeaf(CancellationToken cancellationToken = default);

    /// <summary>
    /// Places the current call on hold, stopping media flow but keeping the session alive.
    /// </summary>
    ValueTask Hold(CancellationToken cancellationToken = default);

    /// <summary>
    /// Resumes media flow after a call was placed on hold.
    /// </summary>
    ValueTask Unhold(CancellationToken cancellationToken = default);

    /// <summary>
    /// Toggles the hold state of the current call.
    /// </summary>
    ValueTask ToggleHold(CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a DTMF digit (touch tone) to the remote party.
    /// </summary>
    /// <param name="digit">A valid DTMF character: 0–9, *, or #.</param>
    /// <param name="cancellationToken"></param>
    ValueTask Dtmf(string digit, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a text message over the Telnyx WebRTC signaling channel.
    /// </summary>
    ValueTask Message(string to, string body, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the input audio device (microphone) by device ID.
    /// </summary>
    ValueTask SetAudioInDevice(string deviceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the output audio device (speakers/headphones) by device ID.
    /// </summary>
    ValueTask SetAudioOutDevice(string deviceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the video input device (camera) by device ID.
    /// </summary>
    ValueTask SetVideoDevice(string deviceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Initiates a screen sharing session using the browser's screen picker.
    /// </summary>
    /// <param name="options">Optional screen share constraints such as resolution or cursor behavior.</param>
    /// <param name="cancellationToken"></param>
    ValueTask StartScreenShare(TelnyxScreenShareOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Ends an active screen share session and restores the camera stream.
    /// </summary>
    ValueTask StopScreenShare(CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets a limit on the outgoing audio bitrate for bandwidth-constrained networks.
    /// </summary>
    ValueTask SetAudioBandwidth(int bps, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets a limit on the outgoing video bitrate to manage bandwidth usage.
    /// </summary>
    ValueTask SetVideoBandwidth(int bps, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the full list of available media devices (audio/video in/out).
    /// </summary>
    ValueTask<List<MediaDeviceInfo>> GetDevices(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all available video input devices (cameras).
    /// </summary>
    ValueTask<List<MediaDeviceInfo>> GetVideoDevices(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all available audio input devices (microphones).
    /// </summary>
    ValueTask<List<MediaDeviceInfo>> GetAudioInDevices(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all available audio output devices (speakers/headsets).
    /// </summary>
    ValueTask<List<MediaDeviceInfo>> GetAudioOutDevices(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks whether the browser has granted permissions for the microphone and/or camera.
    /// </summary>
    ValueTask<bool> CheckPermissions(bool audio = true, bool video = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Applies custom audio constraints such as echo cancellation or noise suppression.
    /// </summary>
    ValueTask<bool> SetAudioSettings(TelnyxAudioSettings settings, CancellationToken cancellationToken = default);

    /// <summary>
    /// Applies custom video constraints such as resolution or frame rate.
    /// </summary>
    ValueTask<bool> SetVideoSettings(TelnyxVideoSettings settings, CancellationToken cancellationToken = default);

    /// <summary>
    /// Enables the user's microphone if previously disabled.
    /// </summary>
    ValueTask EnableMicrophone(CancellationToken cancellationToken = default);

    /// <summary>
    /// Disables the user's microphone and stops audio transmission.
    /// </summary>
    ValueTask DisableMicrophone(CancellationToken cancellationToken = default);

    /// <summary>
    /// Enables the user's camera if previously disabled.
    /// </summary>
    ValueTask EnableWebcam(CancellationToken cancellationToken = default);

    /// <summary>
    /// Disables the user's camera and stops video transmission.
    /// </summary>
    ValueTask DisableWebcam(CancellationToken cancellationToken = default);

    /// <summary>
    /// Enables or disables audio transmission based on the specified state.
    /// </summary>
    ValueTask ToggleAudio(bool enabled, CancellationToken cancellationToken = default);

    /// <summary>
    /// Enables or disables video transmission based on the specified state.
    /// </summary>
    ValueTask ToggleVideo(bool enabled, CancellationToken cancellationToken = default);

    /// <summary>
    /// Disconnects from the Telnyx signaling and media servers.
    /// </summary>
    ValueTask Disconnect(CancellationToken cancellationToken = default);

    /// <summary>
    /// Attempts to reconnect to the Telnyx servers using existing credentials.
    /// </summary>
    ValueTask Reconnect(CancellationToken cancellationToken = default);

    /// <summary>
    /// Explicitly connects the Telnyx WebRTC client to the signaling server.
    /// </summary>
    ValueTask Connect(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the current statistics (latency, jitter, bandwidth) for the ongoing call.
    /// </summary>
    ValueTask<string?> GetCallStats(CancellationToken cancellationToken = default);
    /// <summary>Performs the GetActiveCalls operation for this session.</summary>
    ValueTask<List<TelnyxWebRtcCall>> GetActiveCalls(CancellationToken cancellationToken = default);
    /// <summary>Performs the GetIsRegistered operation for this session.</summary>
    ValueTask<bool> GetIsRegistered(CancellationToken cancellationToken = default);
    /// <summary>Performs the SpeedTest operation for this session.</summary>
    ValueTask<JsonElement?> SpeedTest(int bytes, CancellationToken cancellationToken = default);
    /// <summary>Performs the ValidateDeviceId operation for this session.</summary>
    ValueTask<string?> ValidateDeviceId(string deviceId, string label, string kind, CancellationToken cancellationToken = default);
    /// <summary>Performs the GetDeviceResolutions operation for this session.</summary>
    ValueTask<List<TelnyxDeviceResolution>> GetDeviceResolutions(string deviceId, CancellationToken cancellationToken = default);
    /// <summary>Performs the GetMediaConstraints operation for this session.</summary>
    ValueTask<JsonElement?> GetMediaConstraints(CancellationToken cancellationToken = default);
    /// <summary>Performs the GetIceServers operation for this session.</summary>
    ValueTask<List<TelnyxIceServer>> GetIceServers(CancellationToken cancellationToken = default);
    /// <summary>Performs the SetIceServers operation for this session.</summary>
    ValueTask SetIceServers(List<TelnyxIceServer> servers, CancellationToken cancellationToken = default);
    /// <summary>Performs the GetSpeaker operation for this session.</summary>
    ValueTask<string?> GetSpeaker(CancellationToken cancellationToken = default);
    /// <summary>Performs the SetSpeaker operation for this session.</summary>
    ValueTask SetSpeaker(string deviceId, CancellationToken cancellationToken = default);
    /// <summary>Performs the SetLocalElement operation for this session.</summary>
    ValueTask SetLocalElement(string elementId, CancellationToken cancellationToken = default);
    /// <summary>Performs the SetRemoteElement operation for this session.</summary>
    ValueTask SetRemoteElement(string elementId, CancellationToken cancellationToken = default);
    /// <summary>Performs the SetLocalElement operation for this session.</summary>
    ValueTask SetLocalElement(IJSObjectReference element, CancellationToken cancellationToken = default);
    /// <summary>Performs the SetRemoteElement operation for this session.</summary>
    ValueTask SetRemoteElement(IJSObjectReference element, CancellationToken cancellationToken = default);
    /// <summary>Performs the Login operation for this session.</summary>
    ValueTask Login(TelnyxLoginOptions? options = null, CancellationToken cancellationToken = default);
    /// <summary>Performs the Logout operation for this session.</summary>
    ValueTask Logout(CancellationToken cancellationToken = default);
    /// <summary>Performs the HasActiveCall operation for this session.</summary>
    ValueTask<bool> HasActiveCall(CancellationToken cancellationToken = default);
    /// <summary>Performs the GetWebRtcInfo operation for this session.</summary>
    ValueTask<TelnyxWebRtcInfo?> GetWebRtcInfo(CancellationToken cancellationToken = default);
    /// <summary>Performs the RunPreCallDiagnosis operation for this session.</summary>
    ValueTask<TelnyxPreCallDiagnosisReport?> RunPreCallDiagnosis(TelnyxPreCallDiagnosisOptions options, CancellationToken cancellationToken = default);
    /// <summary>Performs the GetCurrentCall operation for this session.</summary>
    ValueTask<TelnyxWebRtcCall?> GetCurrentCall(CancellationToken cancellationToken = default);
    /// <summary>Performs the GetLocalStream operation for this session.</summary>
    ValueTask<IJSObjectReference?> GetLocalStream(CancellationToken cancellationToken = default);
    /// <summary>Performs the GetRemoteStream operation for this session.</summary>
    ValueTask<IJSObjectReference?> GetRemoteStream(CancellationToken cancellationToken = default);
    /// <summary>Performs the SendConversationMessage operation for this session.</summary>
    ValueTask SendConversationMessage(string message, string[]? attachments = null, CancellationToken cancellationToken = default);
    /// <summary>Performs the SendAiConversationMessage operation for this session.</summary>
    ValueTask SendAiConversationMessage(TelnyxFunctionCallOutput item, CancellationToken cancellationToken = default);
    /// <summary>Performs the SetAudioInDevice operation for this session.</summary>
    ValueTask SetAudioInDevice(string deviceId, bool? muted, CancellationToken cancellationToken = default);
    /// <summary>Performs the StartScreenShare operation for this session.</summary>
    ValueTask StartScreenShare(TelnyxCallOptions options, CancellationToken cancellationToken = default);
}
