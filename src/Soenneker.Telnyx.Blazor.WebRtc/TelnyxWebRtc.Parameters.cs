using Microsoft.AspNetCore.Components;
using Soenneker.Telnyx.Blazor.WebRtc.Dtos;
namespace Soenneker.Telnyx.Blazor.WebRtc;
public partial class TelnyxWebRtc
{
    [Parameter]
    public bool RenderHiddenAudio { get; set; } = true;

    [Parameter]
    public bool RenderVideo { get; set; }

    [Parameter]
    public EventCallback OnInitialize { get; set; }

    [Parameter]
    public EventCallback OnReady { get; set; }

    [Parameter]
    public EventCallback<string> OnMessage { get; set; }

    [Parameter]
    public EventCallback<string> OnError { get; set; }

    [Parameter]
    public EventCallback<string> OnCallInitiated { get; set; }

    [Parameter]
    public EventCallback<string> OnCallAnswered { get; set; }

    [Parameter]
    public EventCallback<string> OnCallHangup { get; set; }

    [Parameter]
    public EventCallback<string> OnCallHeld { get; set; }

    [Parameter]
    public EventCallback<string> OnCallResumed { get; set; }

    [Parameter]
    public EventCallback OnLocalStream { get; set; }

    [Parameter]
    public EventCallback OnRemoteStream { get; set; }

    [Parameter]
    public EventCallback OnStreamStopped { get; set; }

    [Parameter]
    public EventCallback<string> OnDevicesChanged { get; set; }

    [Parameter]
    public EventCallback<string> OnConferenceUpdate { get; set; }

    [Parameter]
    public EventCallback<string> OnStatsUpdate { get; set; }

    [Parameter]
    public EventCallback<TelnyxNotification> OnNotification { get; set; }

    [Parameter]
    public EventCallback OnSocketOpen { get; set; }

    [Parameter]
    public EventCallback OnSocketClose { get; set; }

    [Parameter]
    public EventCallback<string> OnSocketError { get; set; }

    [Parameter]
    public double SpeakerVolume { get; set; } = 1.0;

    [Parameter]
    public EventCallback<double> SpeakerVolumeChanged { get; set; }

    [Parameter]
    public EventCallback<string> OnRawSocketMessage { get; set; }

}
