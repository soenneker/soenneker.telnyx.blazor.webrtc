using System.Collections.Generic;
using System.Text.Json;

namespace Soenneker.Telnyx.Blazor.WebRtc.Dtos;

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
