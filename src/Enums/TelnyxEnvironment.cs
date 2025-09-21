using Intellenum;

namespace Soenneker.Telnyx.Blazor.WebRtc.Enums;

/// <summary>
/// Represents the Telnyx WebRTC client environment.
/// Used to specify whether the client should connect to the development or production Telnyx environment.
/// </summary>
[Intellenum<string>]
public sealed partial class TelnyxEnvironment
{
    /// <summary>
    /// The development environment. Use this when testing with a non-production Telnyx configuration.
    /// </summary>
    public static readonly TelnyxEnvironment Development = new("development");

    /// <summary>
    /// The production environment. Use this when connecting to Telnyx's live infrastructure.
    /// </summary>
    public static readonly TelnyxEnvironment Production = new("production");
}