using System.Text.Json.Serialization;

namespace Soenneker.Telnyx.Blazor.WebRtc.Configuration;

/// <summary>
/// Represents configuration options for audio input used in a Telnyx WebRTC session.
/// </summary>
public sealed class TelnyxAudioSettings
{
    /// <summary>
    /// The unique identifier of the selected microphone device.
    /// </summary>
    [JsonPropertyName("micId")]
    public string? MicId { get; set; }

    /// <summary>
    /// The human-readable label of the selected microphone device.
    /// </summary>
    [JsonPropertyName("micLabel")]
    public string? MicLabel { get; set; }

    /// <summary>
    /// Indicates whether echo cancellation is enabled for the audio input.
    /// </summary>
    [JsonPropertyName("echoCancellation")]
    public object? EchoCancellation { get; set; }

    /// <summary>
    /// Indicates whether noise suppression is enabled for the audio input.
    /// </summary>
    [JsonPropertyName("noiseSuppression")]
    public object? NoiseSuppression { get; set; }

    /// <summary>
    /// Indicates whether automatic gain control is enabled for the audio input.
    /// </summary>
    [JsonPropertyName("autoGainControl")]
    public object? AutoGainControl { get; set; }

    [JsonPropertyName("channelCount")] public object? ChannelCount { get; set; }
    [JsonPropertyName("deviceId")] public object? DeviceId { get; set; }
    [JsonPropertyName("groupId")] public object? GroupId { get; set; }
    [JsonPropertyName("latency")] public object? Latency { get; set; }
    [JsonPropertyName("sampleRate")] public object? SampleRate { get; set; }
    [JsonPropertyName("sampleSize")] public object? SampleSize { get; set; }
    [JsonPropertyName("suppressLocalAudioPlayback")] public object? SuppressLocalAudioPlayback { get; set; }
    [JsonPropertyName("voiceIsolation")] public object? VoiceIsolation { get; set; }

    [JsonExtensionData]
    public System.Collections.Generic.Dictionary<string, object>? AdditionalConstraints { get; set; }
}
