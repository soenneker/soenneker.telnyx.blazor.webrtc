namespace Soenneker.Telnyx.Blazor.WebRtc.Configuration;

/// <summary>
/// Represents configuration options for audio input used in a Telnyx WebRTC session.
/// </summary>
public sealed class TelnyxAudioSettings
{
    /// <summary>
    /// The unique identifier of the selected microphone device.
    /// </summary>
    public string? MicId { get; set; }

    /// <summary>
    /// The human-readable label of the selected microphone device.
    /// </summary>
    public string? MicLabel { get; set; }

    /// <summary>
    /// Indicates whether echo cancellation is enabled for the audio input.
    /// </summary>
    public bool? EchoCancellation { get; set; }

    /// <summary>
    /// Indicates whether noise suppression is enabled for the audio input.
    /// </summary>
    public bool? NoiseSuppression { get; set; }

    /// <summary>
    /// Indicates whether automatic gain control is enabled for the audio input.
    /// </summary>
    public bool? AutoGainControl { get; set; }
}