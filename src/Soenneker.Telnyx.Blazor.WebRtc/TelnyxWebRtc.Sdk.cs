using Microsoft.AspNetCore.Components;
namespace Soenneker.Telnyx.Blazor.WebRtc;
public partial class TelnyxWebRtc
{
    [Parameter] public EventCallback<string> OnWarning { get; set; }
    [Parameter] public EventCallback OnMediaPermissionsRecoverySuccess { get; set; }
    [Parameter] public EventCallback<string> OnMediaPermissionsRecoveryError { get; set; }
    [Parameter] public EventCallback<string> OnAiConversationMessage { get; set; }
}
