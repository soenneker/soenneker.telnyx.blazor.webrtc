using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.Blazor.Utils.ResourceLoader.Registrars;
using Soenneker.Telnyx.Blazor.WebRtc.Abstract;

namespace Soenneker.Telnyx.Blazor.WebRtc.Registrars;

/// <summary>
/// Registers Telnyx browser calling, media control, and WebRTC event interop for Blazor applications.
/// </summary>
public static class TelnyxWebRtcInteropRegistrar
{
    /// <summary>
    /// Adds <see cref="ITelnyxWebRtcInterop"/> as a scoped service. <para/>
    /// </summary>
    public static IServiceCollection AddTelnyxWebRtcInteropAsScoped(this IServiceCollection services)
    {
        services.AddResourceLoaderAsScoped()
                .TryAddScoped<ITelnyxWebRtcInterop, TelnyxWebRtcInterop>();

        return services;
    }
}
