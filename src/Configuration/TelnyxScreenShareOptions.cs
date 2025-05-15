namespace Soenneker.Telnyx.Blazor.WebRtc.Configuration
{
    /// <summary>
    /// Represents caller information used during a screen sharing session in a Telnyx WebRTC call.
    /// </summary>
    public class TelnyxScreenShareOptions
    {
        /// <summary>
        /// The display name of the remote party initiating the screen share.
        /// </summary>
        public string? RemoteCallerName { get; set; }

        /// <summary>
        /// The phone number of the remote party initiating the screen share.
        /// </summary>
        public string? RemoteCallerNumber { get; set; }

        /// <summary>
        /// The display name of the local caller in the screen share session.
        /// </summary>
        public string? CallerName { get; set; }

        /// <summary>
        /// The phone number of the local caller in the screen share session.
        /// </summary>
        public string? CallerNumber { get; set; }
    }
}