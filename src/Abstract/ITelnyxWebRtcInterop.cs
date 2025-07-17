using Microsoft.JSInterop;
using Soenneker.Telnyx.Blazor.WebRtc.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Telnyx.Blazor.WebRtc.Abstract;

/// <summary>
/// Provides a comprehensive Blazor WebAssembly interop interface for controlling Telnyx's WebRTC JavaScript SDK.
/// This interface exposes functionality for managing calls, media devices, screen sharing, bandwidth, and advanced participant control.
/// </summary>
public interface ITelnyxWebRtcInterop : IAsyncDisposable
{
    /// <summary>
    /// Initializes and loads the Telnyx WebRTC JavaScript module.
    /// This must be called once before any other operations.
    /// </summary>
    ValueTask Initialize(bool useCdn = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates and registers a Telnyx WebRTC client instance with configuration and event callback support.
    /// </summary>
    /// <param name="elementId">The DOM element identifier hosting the WebRTC client.</param>
    /// <param name="dotNetObjectRef">A .NET reference to the component receiving event callbacks.</param>
    /// <param name="options">Configuration options for the client.</param>
    /// <param name="cancellationToken"></param>
    ValueTask Create(string elementId, DotNetObjectReference<TelnyxWebRtc> dotNetObjectRef, TelnyxClientOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a passive observer that monitors call and media events without participating.
    /// </summary>
    ValueTask CreateObserver(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Starts an outbound call with the provided call options.
    /// </summary>
    ValueTask Call(string elementId, TelnyxCallOptions callOptions, CancellationToken cancellationToken = default);

    /// <summary>
    /// Answers an incoming call with optional answer parameters (e.g., video support).
    /// </summary>
    ValueTask Answer(string elementId, TelnyxAnswerOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Hangs up the current call with optional signaling metadata.
    /// </summary>
    ValueTask Hangup(string elementId, TelnyxHangupOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Mutes the local user's microphone.
    /// </summary>
    ValueTask MuteAudio(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unmutes the local user's microphone.
    /// </summary>
    ValueTask UnmuteAudio(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Toggles the local microphone mute state.
    /// </summary>
    ValueTask ToggleAudioMute(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Mutes the local user's camera.
    /// </summary>
    ValueTask MuteVideo(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unmutes the local user's camera.
    /// </summary>
    ValueTask UnmuteVideo(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Toggles the local video stream.
    /// </summary>
    ValueTask ToggleVideoMute(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Disables both microphone and speaker output.
    /// </summary>
    ValueTask Deaf(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Re-enables audio input/output after a deaf operation.
    /// </summary>
    ValueTask Undeaf(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Toggles the deaf state (both mic and speaker).
    /// </summary>
    ValueTask ToggleDeaf(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Places the current call on hold.
    /// </summary>
    ValueTask Hold(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resumes the call from a hold state.
    /// </summary>
    ValueTask Unhold(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Toggles hold/resume state for the current call.
    /// </summary>
    ValueTask ToggleHold(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a DTMF tone to the remote party.
    /// </summary>
    ValueTask Dtmf(string elementId, string digit, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a chat or in-band message during an active call.
    /// </summary>
    ValueTask Message(string elementId, string to, string body, CancellationToken cancellationToken = default);

    /// <summary>
    /// Changes the microphone input device.
    /// </summary>
    ValueTask SetAudioInDevice(string elementId, string deviceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Changes the video input (camera) device.
    /// </summary>
    ValueTask SetVideoDevice(string elementId, string deviceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Changes the audio output device (e.g., speakers/headphones).
    /// </summary>
    ValueTask SetAudioOutDevice(string elementId, string deviceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Starts a screen sharing session.
    /// </summary>
    ValueTask StartScreenShare(string elementId, TelnyxScreenShareOptions? options = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops an active screen sharing session.
    /// </summary>
    ValueTask StopScreenShare(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the maximum audio bitrate (in bits per second).
    /// </summary>
    ValueTask SetAudioBandwidth(string elementId, int bps, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the maximum video bitrate (in bits per second).
    /// </summary>
    ValueTask SetVideoBandwidth(string elementId, int bps, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of all available media devices as a JSON string.
    /// </summary>
    ValueTask<string> GetDevices(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves available video input devices.
    /// </summary>
    ValueTask<string> GetVideoDevices(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves available audio input devices.
    /// </summary>
    ValueTask<string> GetAudioInDevices(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves available audio output devices.
    /// </summary>
    ValueTask<string> GetAudioOutDevices(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies whether permissions for microphone and camera access have been granted.
    /// </summary>
    ValueTask<bool> CheckPermissions(string elementId, bool audio = true, bool video = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Applies new audio settings (e.g., selected mic, gain, suppression).
    /// </summary>
    ValueTask<bool> SetAudioSettings(string elementId, TelnyxAudioSettings settings, CancellationToken cancellationToken = default);

    /// <summary>
    /// Applies new video settings (e.g., resolution, frame rate).
    /// </summary>
    ValueTask<bool> SetVideoSettings(string elementId, TelnyxVideoSettings settings, CancellationToken cancellationToken = default);

    /// <summary>
    /// Enables the user's microphone stream.
    /// </summary>
    ValueTask EnableMicrophone(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Disables the user's microphone stream.
    /// </summary>
    ValueTask DisableMicrophone(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Enables the user's webcam stream.
    /// </summary>
    ValueTask EnableWebcam(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Disables the user's webcam stream.
    /// </summary>
    ValueTask DisableWebcam(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Toggles the audio stream explicitly.
    /// </summary>
    ValueTask ToggleAudio(string elementId, bool enabled, CancellationToken cancellationToken = default);

    /// <summary>
    /// Toggles the video stream explicitly.
    /// </summary>
    ValueTask ToggleVideo(string elementId, bool enabled, CancellationToken cancellationToken = default);

    /// <summary>
    /// Disconnects the current Telnyx WebRTC session.
    /// </summary>
    ValueTask Disconnect(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Attempts to reconnect to the previous Telnyx session.
    /// </summary>
    ValueTask Reconnect(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Fully disposes the WebRTC client for the given element.
    /// </summary>
    ValueTask Unmount(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists available video layout configurations.
    /// </summary>
    ValueTask ListVideoLayouts(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the active video layout configuration.
    /// </summary>
    ValueTask SetVideoLayout(string elementId, string layout, string? canvas = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins playing a media file to the remote party.
    /// </summary>
    ValueTask PlayMedia(string elementId, string source, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops currently playing media.
    /// </summary>
    ValueTask StopMedia(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Starts recording the call to a specified filename.
    /// </summary>
    ValueTask StartRecord(string elementId, string filename, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops the current recording session.
    /// </summary>
    ValueTask StopRecord(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a chat message to other call participants.
    /// </summary>
    ValueTask SendChatMessage(string elementId, string message, string? type = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Captures a snapshot from the video feed and stores it as a file.
    /// </summary>
    ValueTask Snapshot(string elementId, string filename, CancellationToken cancellationToken = default);

    /// <summary>
    /// Mutes the microphone of a specified participant.
    /// </summary>
    ValueTask MuteMic(string elementId, string participantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Mutes the video feed of a specified participant.
    /// </summary>
    ValueTask MuteVideoParticipant(string elementId, string participantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Kicks a participant from the current session.
    /// </summary>
    ValueTask Kick(string elementId, string participantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Increases the playback volume of a specific participant.
    /// </summary>
    ValueTask VolumeUp(string elementId, string participantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Decreases the playback volume of a specific participant.
    /// </summary>
    ValueTask VolumeDown(string elementId, string participantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves runtime call statistics such as bitrate, jitter, packet loss, etc., as a JSON string.
    /// </summary>
    ValueTask<string?> GetCallStats(string elementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets the volume of the audio element (0.0 to 1.0).
    /// </summary>
    ValueTask SetAudioVolume(string elementId, double volume, CancellationToken cancellationToken = default);

    /// <summary>
    /// Explicitly connects the Telnyx WebRTC client for the given element.
    /// </summary>
    ValueTask Connect(string elementId, CancellationToken cancellationToken = default);
}
