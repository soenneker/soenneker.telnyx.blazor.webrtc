using System.Text.Json.Serialization;
using Soenneker.Telnyx.Blazor.WebRtc.Enums;

namespace Soenneker.Telnyx.Blazor.WebRtc.Configuration;

/// <summary>
/// Represents configuration options for initializing a Telnyx WebRTC client.
/// </summary>
public sealed class TelnyxClientOptions
{
    /// <summary>Enables allowlisted diagnostic events and session logging. Disabled by default.</summary>
    [JsonPropertyName("enableDiagnostics")]
    public bool EnableDiagnostics { get; set; }

    /// <summary>Enables raw socket events and raw event payloads. May contain sensitive SDK data.</summary>
    [JsonPropertyName("enableRawEvents")]
    public bool EnableRawEvents { get; set; }

    /// <summary>Policy for additional non-screen-share calls while the selected call is alive.</summary>
    [JsonPropertyName("additionalCallPolicy")]
    public TelnyxAdditionalCallPolicy AdditionalCallPolicy { get; set; }

    internal TelnyxClientOptions Snapshot()
    {
        var copy = System.Text.Json.JsonSerializer.Deserialize(
            System.Text.Json.JsonSerializer.Serialize(this, LibraryJsonContext.Get<TelnyxClientOptions>()),
            LibraryJsonContext.Get<TelnyxClientOptions>())!;
        copy.UseCdn = UseCdn;
        return copy;
    }

    /// <summary>
    /// Gets or sets init options.
    /// </summary>
    [JsonPropertyName("initOptions")]
    public TelnyxClientInitOptions InitOptions { get; set; } = null!;

    /// <summary>
    /// Automatically attempt reconnection if the client is disconnected.
    /// </summary>
    [JsonPropertyName("autoReconnect")]
    public bool? AutoReconnect { get; set; }

    /// <summary>
    /// Maximum number of reconnection attempts.
    /// </summary>
    [JsonPropertyName("reconnectAttempts")]
    public int ReconnectAttempts { get; set; } = 5;

    /// <summary>
    /// Gets or sets a value indicating whether use cdn.
    /// </summary>
    [JsonIgnore]
    public bool UseCdn { get; set; } = true;
}
