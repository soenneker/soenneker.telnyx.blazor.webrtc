using System.Collections.Generic;
using System.Text.Json;

namespace Soenneker.Telnyx.Blazor.WebRtc.Dtos;

public sealed class TelnyxWebRtcInfo
{
    public JsonElement BrowserInfo { get; set; }
    public string? BrowserName { get; set; }
    public double BrowserVersion { get; set; }
    public bool SupportWebRTC { get; set; }
    public bool SupportWebRTCAudio { get; set; }
    public bool SupportWebRTCVideo { get; set; }
    public bool SupportRTCPeerConnection { get; set; }
    public bool SupportSessionDescription { get; set; }
    public bool SupportIceCandidate { get; set; }
    public bool SupportMediaDevices { get; set; }
    public bool SupportGetUserMedia { get; set; }
}
