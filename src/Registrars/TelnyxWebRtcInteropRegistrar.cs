using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.Blazor.Utils.ResourceLoader.Registrars;
using Soenneker.Telnyx.Blazor.WebRtc.Abstract;

namespace Soenneker.Telnyx.Blazor.WebRtc.Registrars;

/// <summary>
/// Blazor interop library for the Telnyx WebRTC client, providing full access to Telnyx's browser-based voice and video calling features. Includes typed wrappers, event bridging, and support for advanced call control in Blazor WebAssembly apps.
/// </summary>
public static class TelnyxWebRtcInteropRegistrar
{
    /// <summary>
    /// Adds <see cref="ITelnyxWebRtcInterop"/> as a scoped service. <para/>
    /// </summary>
    public static IServiceCollection AddTelnyxWebRtcInteropAsScoped(this IServiceCollection services)
    {
        services.AddResourceLoaderAsScoped().TryAddScoped<ITelnyxWebRtcInterop, TelnyxWebRtcInterop>();

        return services;
    }
}
