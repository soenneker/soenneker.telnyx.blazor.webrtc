namespace Soenneker.Telnyx.Blazor.WebRtc.Configuration;

/// <summary>
/// Represents configuration options for video input used in a Telnyx WebRTC session.
/// </summary>
public sealed class TelnyxVideoSettings
{
    /// <summary>
    /// The unique identifier of the selected camera device.
    /// </summary>
    public string? CamId { get; set; }

    /// <summary>
    /// The human-readable label of the selected camera device.
    /// </summary>
    public string? CamLabel { get; set; }

    /// <summary>
    /// The desired width of the video stream in pixels.
    /// </summary>
    public int? Width { get; set; }

    /// <summary>
    /// The desired height of the video stream in pixels.
    /// </summary>
    public int? Height { get; set; }

    /// <summary>
    /// The desired frame rate of the video stream in frames per second.
    /// </summary>
    public int? FrameRate { get; set; }

    /// <summary>
    /// The facing mode of the camera. Common values are "user" (front-facing) and "environment" (rear-facing).
    /// </summary>
    public string? FacingMode { get; set; }
}