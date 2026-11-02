using System;
using System.Reflection;
using System.Threading.Tasks;
using Soenneker.Telnyx.Blazor.WebRtc.Abstract;
using Soenneker.Telnyx.Blazor.WebRtc.Configuration;
namespace Soenneker.Telnyx.Blazor.WebRtc.Tests;

public class RecordingInterop : DispatchProxy
{
    public int Creates;
    public int Unmounts;
    public bool FailCreate;
    public bool FailUnmount;
    public TelnyxClientOptions? Options;
    public TaskCompletionSource? CreateGate;
    public TaskCompletionSource CreateEntered = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public static (ITelnyxWebRtcInterop Interop, RecordingInterop Recorder) Create()
    {
        var interop = DispatchProxy.Create<ITelnyxWebRtcInterop, RecordingInterop>();
        return (interop, (RecordingInterop)interop);
    }
    protected override object? Invoke(MethodInfo? method, object?[]? args)
    {
        if (method!.Name == "Create") return new ValueTask(CreateClient((TelnyxClientOptions)args![2]!));
        if (method.Name == "Unmount")
        {
            Unmounts++;
            if (FailUnmount) return ValueTask.FromException(new InvalidOperationException("cleanup failed"));
        }
        return Activator.CreateInstance(method.ReturnType);
    }
    private async Task CreateClient(TelnyxClientOptions options)
    {
        Creates++;
        Options = options;
        CreateEntered.TrySetResult();
        if (CreateGate != null) await CreateGate.Task;
        if (FailCreate) throw new InvalidOperationException("create failed");
    }
}
