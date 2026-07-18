using System.Collections.Generic;
using System.Text.Json;

namespace Soenneker.Telnyx.Blazor.WebRtc.Dtos;

public sealed class TelnyxDeviceResolution
{
    public string? Resolution { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}

public sealed class TelnyxPreCallDiagnosisReport
{
    public List<TelnyxIceCandidateStats> IceCandidateStats { get; set; } = [];
    public JsonElement IceCandidatePairStats { get; set; }
    public TelnyxPreCallSummaryStats SummaryStats { get; set; } = new();
    public TelnyxPreCallSessionStats SessionStats { get; set; } = new();
}

public sealed class TelnyxIceCandidateStats
{
    public string? Address { get; set; }
    public string? CandidateType { get; set; }
    public bool Deleted { get; set; }
    public string? Id { get; set; }
    public int? Port { get; set; }
    public double Priority { get; set; }
    public string? Protocol { get; set; }
    public string? RelayProtocol { get; set; }
    public double Timestamp { get; set; }
    public string? TransportId { get; set; }
    public string? Type { get; set; }
    public string? Url { get; set; }
}

public sealed class TelnyxMinMaxAverage
{
    public double Min { get; set; }
    public double Max { get; set; }
    public double Average { get; set; }
}

public sealed class TelnyxPreCallSummaryStats
{
    public TelnyxMinMaxAverage Jitter { get; set; } = new();
    public TelnyxMinMaxAverage Rtt { get; set; } = new();
    public double Mos { get; set; }
    public string? Quality { get; set; }
}

public sealed class TelnyxPreCallSessionStats
{
    public double PacketsReceived { get; set; }
    public double PacketsLost { get; set; }
    public double PacketsSent { get; set; }
    public double BytesSent { get; set; }
    public double BytesReceived { get; set; }
}

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
