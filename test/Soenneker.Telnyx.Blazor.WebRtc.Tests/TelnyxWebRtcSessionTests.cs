using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Soenneker.Telnyx.Blazor.WebRtc.Configuration;
using Soenneker.Telnyx.Blazor.WebRtc.Dtos;
using Soenneker.Telnyx.Blazor.WebRtc.Enums;
namespace Soenneker.Telnyx.Blazor.WebRtc.Tests;

public class TelnyxWebRtcSessionTests
{
    private static TelnyxClientOptions Options() => new() { InitOptions = new() { LoginToken = "test-token" }, UseCdn = false };

    [Test]
    public async Task ConcurrentInitializeCreatesOneClientAndSnapshotsOptions()
    {
        var (interop, recorder) = RecordingInterop.Create();
        recorder.CreateGate = new(TaskCreationOptions.RunContinuationsAsynchronously);
        await using var session = new TelnyxWebRtcSession(interop, NullLogger<TelnyxWebRtcSession>.Instance);
        var options = Options();
        var first = session.Initialize(options).AsTask();
        await recorder.CreateEntered.Task;
        var second = session.Initialize(options).AsTask();
        options.InitOptions.LoginToken = "changed";
        recorder.CreateGate.SetResult();
        await Task.WhenAll(first, second);
        await Assert.That(recorder.Creates).IsEqualTo(1);
        await Assert.That(recorder.Options!.InitOptions.LoginToken).IsEqualTo("test-token");
        await Assert.That(recorder.Options.UseCdn).IsFalse();
    }

    [Test]
    public async Task DisposeDuringInitializationWaitsForCreateAndCleansUpOnce()
    {
        var (interop, recorder) = RecordingInterop.Create();
        recorder.CreateGate = new(TaskCreationOptions.RunContinuationsAsynchronously);
        var session = new TelnyxWebRtcSession(interop, NullLogger<TelnyxWebRtcSession>.Instance);
        var initialize = session.Initialize(Options()).AsTask();
        await recorder.CreateEntered.Task;
        var dispose = session.DisposeAsync().AsTask();
        await Assert.That(dispose.IsCompleted).IsFalse();
        recorder.CreateGate.SetResult();
        await Task.WhenAll(initialize, dispose);
        await session.DisposeAsync();
        await Assert.That(recorder.Unmounts).IsEqualTo(1);
        await Assert.That(session.State).IsEqualTo(TelnyxConnectionState.Disposed);
    }

    [Test]
    public async Task FailedCreateRollsBackAndCanBeRetried()
    {
        var (interop, recorder) = RecordingInterop.Create();
        await using var session = new TelnyxWebRtcSession(interop, NullLogger<TelnyxWebRtcSession>.Instance);
        recorder.FailCreate = true;
        try { await session.Initialize(Options()); throw new Exception("Expected failure"); }
        catch (InvalidOperationException) { }
        await Assert.That(recorder.Unmounts).IsEqualTo(1);
        recorder.FailCreate = false;
        await session.Initialize(Options());
        await Assert.That(recorder.Creates).IsEqualTo(2);
    }

    [Test]
    public async Task CleanupFailureStillDisposesAndDropsLateCallbacks()
    {
        var (interop, recorder) = RecordingInterop.Create();
        var session = new TelnyxWebRtcSession(interop, NullLogger<TelnyxWebRtcSession>.Instance);
        await session.Initialize(Options());
        var events = 0;
        session.EventReceived += _ => { events++; return Task.CompletedTask; };
        recorder.FailUnmount = true;
        try { await session.DisposeAsync(); throw new Exception("Expected failure"); }
        catch (InvalidOperationException) { }
        await session.HandleTelnyxEvent("ready", "null");
        await session.DisposeAsync();
        await Assert.That(events).IsEqualTo(0);
        await Assert.That(session.State).IsEqualTo(TelnyxConnectionState.Disposed);
    }

    [Test]
    public async Task TypedErrorsPreserveContextAndIsolateSubscriberExceptions()
    {
        var (interop, _) = RecordingInterop.Create();
        await using var session = new TelnyxWebRtcSession(interop, NullLogger<TelnyxWebRtcSession>.Instance);
        await session.Initialize(Options());
        TelnyxSessionEvent? received = null;
        session.EventReceived += _ => throw new InvalidOperationException("application failure");
        session.EventReceived += value => { received = value; return Task.CompletedTask; };
        await session.HandleTelnyxEvent("connectionState", "registered");
        await session.HandleTelnyxEvent("error", """{"error":{"code":1001,"message":"denied","solutions":["allow microphone"]},"callId":"call","sessionId":"session","recoverable":true}""");
        await Assert.That(received!.ConnectionState).IsEqualTo(TelnyxConnectionState.Registered);
        await Assert.That(received.Error!.CallId).IsEqualTo("call");
        await Assert.That(received.Error.Recoverable).IsTrue();
        await Assert.That(received.RawPayload).IsNull();
    }

    [Test]
    public async Task CancellationBeforeDispatchDoesNotCreateClient()
    {
        var (interop, recorder) = RecordingInterop.Create();
        await using var session = new TelnyxWebRtcSession(interop, NullLogger<TelnyxWebRtcSession>.Instance);
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        try { await session.Initialize(Options(), cancellation.Token); throw new Exception("Expected cancellation"); }
        catch (OperationCanceledException) { }
        await Assert.That(recorder.Creates).IsEqualTo(0);
    }
}
