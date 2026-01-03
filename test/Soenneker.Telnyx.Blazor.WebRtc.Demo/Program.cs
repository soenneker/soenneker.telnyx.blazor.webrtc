using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Serilog;
using Serilog.Debugging;
using Soenneker.Quark;
using Soenneker.Serilog.Sinks.Browser.Blazor.Registrars;
using Soenneker.Telnyx.Blazor.WebRtc.Registrars;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Soenneker.Telnyx.Blazor.WebRtc.Demo;

public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            ConfigureLogging(builder.Services);

            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped(sp => new HttpClient
            {
                BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
            });

            var theme = new Theme
            {
            };

            var provider = new ThemeProvider();
            provider.AddTheme(theme);

            builder.Services.AddThemeProviderAsScoped(provider);

            var quarkOptions = new QuarkOptions
            {
                Debug = true,
                AutomaticBootstrapLoading = true,
                AutomaticFontAwesomeLoading = true
            };

            builder.Services.AddQuarkOptionsAsScoped(quarkOptions);

            builder.Services.AddQuarkSuiteAsScoped();

            builder.Services.AddTelnyxWebRtcInteropAsScoped();

            WebAssemblyHost host = builder.Build();

            var jsRuntime = (IJSRuntime) host.Services.GetService(typeof(IJSRuntime))!;

            SetGlobalLogger(jsRuntime);

            await host.RunAsync();
        }
        catch (Exception e)
        {
            Log.Error(e, "Stopped program because of exception");
            throw;
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }

    private static void ConfigureLogging(IServiceCollection services)
    {
        SelfLog.Enable(m => Console.Error.WriteLine(m));

        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddFilter("Microsoft.AspNetCore.Components.RenderTree.*", LogLevel.None);

            builder.AddSerilog(dispose: false);
        });
    }

    private static void SetGlobalLogger(IJSRuntime jsRuntime)
    {
        var loggerConfig = new LoggerConfiguration();

        loggerConfig.WriteTo.BlazorConsole(jsRuntime: jsRuntime);

        Log.Logger = loggerConfig.CreateLogger();
    }
}
