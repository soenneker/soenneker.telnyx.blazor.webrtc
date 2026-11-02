using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Soenneker.Telnyx.Blazor.WebRtc.Enums;
namespace Soenneker.Telnyx.Blazor.WebRtc.Dtos;

/// <summary>A detached event snapshot received from a browser session.</summary>
public sealed class TelnyxSessionEvent
{
    public required string Name { get; init; }
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
    public TelnyxConnectionState ConnectionState { get; init; }
    public TelnyxNotification? Notification { get; init; }
    public TelnyxWebRtcCall? Call { get; init; }
    public TelnyxWebRtcError? Error { get; init; }
    public TelnyxDiagnostic? Diagnostic { get; init; }
    /// <summary>Structured SDK event data, including event-specific fields not yet modeled.</summary>
    public JsonElement? Data { get; init; }
    /// <summary>Raw payload, populated only when EnableRawEvents is enabled.</summary>
    public string? RawPayload { get; init; }
    [JsonIgnore] internal string LegacyPayload { get; init; } = "";
}
