using Microsoft.AspNetCore.Components;
using Soenneker.Telnyx.Blazor.WebRtc.Configuration;
using Soenneker.Telnyx.Blazor.WebRtc.Dtos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Soenneker.Quark.Components.Cancellable.Abstract;

namespace Soenneker.Telnyx.Blazor.WebRtc.Abstract;

/// <summary>
/// Defines a complete contract for managing Telnyx WebRTC client operations within a Blazor WebAssembly component.
/// Supports call lifecycle, media control, device handling, conferencing, and statistics.
/// </summary>
public interface ITelnyxWebRtc : ICancellableComponent
{
    bool RenderHiddenAudio { get; set; }

    bool RenderVideo { get; set; }

    /// <summary>
    /// Gets or sets the unique HTML element ID used to bind the WebRTC client instance in the DOM.
    /// This ID is used by the JavaScript interop layer to reference the correct video/audio context.
    /// </summary>
    string ElementId { get; set; }

    /// <summary>
    /// Triggered when the component begins initialization.
    /// </summary>
    EventCallback OnInitialize { get; set; }

    /// <summary>
    /// Triggered when the Telnyx WebRTC client signals readiness.
    /// </summary>
    EventCallback OnReady { get; set; }

    /// <summary>
    /// Triggered when a general message is received from Telnyx, such as a non-call notification.
    /// </summary>
    EventCallback<string> OnMessage { get; set; }

    /// <summary>
    /// Triggered when an error occurs in the Telnyx WebRTC client.
    /// </summary>
    EventCallback<string> OnError { get; set; }

    /// <summary>
    /// Triggered when a new outbound or inbound call is initiated.
    /// </summary>
    EventCallback<string> OnCallInitiated { get; set; }

    /// <summary>
    /// Triggered when a call becomes active or is answered.
    /// </summary>
    EventCallback<string> OnCallAnswered { get; set; }

    /// <summary>
    /// Triggered when a call is hung up, ended, or destroyed.
    /// </summary>
    EventCallback<string> OnCallHangup { get; set; }

    /// <summary>
    /// Triggered when a call is placed on hold.
    /// </summary>
    EventCallback<string> OnCallHeld { get; set; }

    /// <summary>
    /// Triggered when a call is resumed from a held state.
    /// </summary>
    EventCallback<string> OnCallResumed { get; set; }

    /// <summary>
    /// Triggered when the local media stream is ready.
    /// </summary>
    EventCallback OnLocalStream { get; set; }

    /// <summary>
    /// Triggered when a remote media stream is received.
    /// </summary>
    EventCallback OnRemoteStream { get; set; }

    /// <summary>
    /// Triggered when a media stream is stopped.
    /// </summary>
    EventCallback OnStreamStopped { get; set; }

    /// <summary>
    /// Triggered when the list of available audio/video devices changes.
    /// </summary>
    EventCallback<string> OnDevicesChanged { get; set; }

    /// <summary>
    /// Triggered when a conference update is received.
    /// </summary>
    EventCallback<string> OnConferenceUpdate { get; set; }

    /// <summary>
    /// Triggered when new call statistics are available.
    /// </summary>
    EventCallback<string> OnStatsUpdate { get; set; }

    EventCallback<TelnyxNotification> OnNotification { get; set; }

    /// <summary>
    /// Raised when the underlying WebSocket connection opens.
    /// </summary>
    EventCallback OnSocketOpen { get; set; }

    /// <summary>
    /// Raised when the WebSocket connection is closed.
    /// </summary>
    EventCallback OnSocketClose { get; set; }

    /// <summary>
    /// Raised when a WebSocket error occurs.
    /// </summary>
    EventCallback<string> OnSocketError { get; set; }

    /// <summary>
    /// Raised when the client is attempting to reconnect.
    /// </summary>
    EventCallback OnReconnecting { get; set; }

    /// <summary>
    /// Raised when the client successfully reconnects.
    /// </summary>
    EventCallback OnReconnected { get; set; }

    /// <summary>
    /// Raised when the client becomes disconnected.
    /// </summary>
    EventCallback OnDisconnected { get; set; }

    EventCallback<double> SpeakerVolumeChanged { get; set; }

    /// <summary>
    /// Initializes the Telnyx WebRTC client and sets up event bindings.
    /// </summary>
    /// <param name="options">Client configuration options such as credentials and media settings.</param>
    /// <param name="cancellationToken">Optional cancellation token for aborting initialization.</param>
    ValueTask Initialize(TelnyxClientOptions? options = null, CancellationToken cancellationToken = default);

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
    /// Lists all available conference video layouts (grid, speaker view, etc.).
    /// </summary>
    ValueTask ListVideoLayouts(CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the video layout for the current conference session.
    /// </summary>
    /// <param name="layout">The name of the layout (e.g., "grid", "speaker").</param>
    /// <param name="canvas">Optional canvas ID to use as the video layout target.</param>
    ValueTask SetVideoLayout(string layout, string? canvas = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Plays a media file (e.g., audio clip) into the call.
    /// </summary>
    ValueTask PlayMedia(string source, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops media playback started with <see cref="PlayMedia"/>.
    /// </summary>
    ValueTask StopMedia(CancellationToken cancellationToken = default);

    /// <summary>
    /// Starts recording the current call and saves the output to the given filename.
    /// </summary>
    ValueTask StartRecord(string filename, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops an active call recording.
    /// </summary>
    ValueTask StopRecord(CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a message to all chat participants in the current session.
    /// </summary>
    ValueTask SendChatMessage(string message, string? type = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Captures a still image from the current video stream and saves it to a file.
    /// </summary>
    ValueTask Snapshot(string filename, CancellationToken cancellationToken = default);

    /// <summary>
    /// Mutes the microphone of a specific participant in a conference.
    /// </summary>
    ValueTask MuteMic(string participantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Mutes the video stream of a specific participant in a conference.
    /// </summary>
    ValueTask MuteVideoParticipant(string participantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a participant from a Telnyx conference room.
    /// </summary>
    ValueTask Kick(string participantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Increases the playback volume of a specific participant.
    /// </summary>
    ValueTask VolumeUp(string participantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Decreases the playback volume of a specific participant.
    /// </summary>
    ValueTask VolumeDown(string participantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the current statistics (latency, jitter, bandwidth) for the ongoing call.
    /// </summary>
    ValueTask<string?> GetCallStats(CancellationToken cancellationToken = default);
}