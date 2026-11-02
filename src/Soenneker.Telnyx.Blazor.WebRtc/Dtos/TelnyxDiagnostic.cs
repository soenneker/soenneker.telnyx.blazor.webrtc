using System;
using System.Text.Json;
namespace Soenneker.Telnyx.Blazor.WebRtc.Dtos;

/// <summary>Allowlisted operational metadata. Contains no credentials, phone numbers, message bodies or raw signaling.</summary>
public sealed class TelnyxDiagnostic
{
    public string? Name { get; init; }
    public string? State { get; init; }
    public DateTimeOffset Timestamp { get; init; }
    public long ElapsedMilliseconds { get; init; }
    public int ReconnectAttempts { get; init; }
    public string? CallId { get; init; }
    public string? SessionId { get; init; }
    public JsonElement? Code { get; init; }
}
