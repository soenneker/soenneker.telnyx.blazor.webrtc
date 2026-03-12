namespace Soenneker.Telnyx.Blazor.WebRtc.Dtos;

/// <summary>
/// Represents information about a media input or output device available to the WebRTC client.
/// </summary>
public sealed class MediaDeviceInfo
{
    /// <summary>
    /// A unique identifier for the represented device.
    /// </summary>
    public string? DeviceId { get; set; }

    /// <summary>
    /// A group identifier that is shared by devices that are part of the same physical group (e.g., headset).
    /// </summary>
    public string? GroupId { get; set; }

    /// <summary>
    /// The kind of media device. Possible values include "audioinput", "audiooutput", and "videoinput".
    /// </summary>
    public string? Kind { get; set; }

    /// <summary>
    /// A label describing the device (e.g., "Built-in Microphone"). This may be empty if permission to access the device has not been granted.
    /// </summary>
    public string? Label { get; set; }
}