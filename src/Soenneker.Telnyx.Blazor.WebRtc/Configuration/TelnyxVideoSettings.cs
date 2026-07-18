using System.Text.Json.Serialization;

namespace Soenneker.Telnyx.Blazor.WebRtc.Configuration;

/// <summary>
/// Represents configuration options for video input used in a Telnyx WebRTC session.
/// </summary>
public sealed class TelnyxVideoSettings
{
    /// <summary>
    /// The unique identifier of the selected camera device.
    /// </summary>
    [JsonPropertyName("camId")]
    public string? CamId { get; set; }

    /// <summary>
    /// The human-readable label of the selected camera device.
    /// </summary>
    [JsonPropertyName("camLabel")]
    public string? CamLabel { get; set; }

    /// <summary>
    /// The desired width of the video stream in pixels.
    /// </summary>
    [JsonPropertyName("width")]
    public object? Width { get; set; }

    /// <summary>
    /// The desired height of the video stream in pixels.
    /// </summary>
    [JsonPropertyName("height")]
    public object? Height { get; set; }

    /// <summary>
    /// The desired frame rate of the video stream in frames per second.
    /// </summary>
    [JsonPropertyName("frameRate")]
    public object? FrameRate { get; set; }

    /// <summary>
    /// The facing mode of the camera. Common values are "user" (front-facing) and "environment" (rear-facing).
    /// </summary>
    [JsonPropertyName("facingMode")]
    public object? FacingMode { get; set; }

    [JsonPropertyName("aspectRatio")] public object? AspectRatio { get; set; }
    [JsonPropertyName("deviceId")] public object? DeviceId { get; set; }
    [JsonPropertyName("displaySurface")] public object? DisplaySurface { get; set; }
    [JsonPropertyName("groupId")] public object? GroupId { get; set; }

    [JsonExtensionData]
    public System.Collections.Generic.Dictionary<string, object>? AdditionalConstraints { get; set; }
}
