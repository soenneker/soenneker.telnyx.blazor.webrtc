const clients = new Map();
const observers = new Map();

const sessionEvents = [
    'telnyx.ready',
    'telnyx.error',
    'telnyx.warning',
    'telnyx.socket.open',
    'telnyx.socket.close',
    'telnyx.socket.error',
    'telnyx.socket.message',
    'telnyx.stats.frame',
    'telnyx.stats.report',
    'telnyx.messages',
    'telnyx.calls',
    'telnyx.rtc.mediaError',
    'telnyx.rtc.peerConnectionFailureError',
    'telnyx.rtc.peerConnectionSignalingStateClosed',
    'telnyx.ai.conversation'
];

const terminalCallStates = new Set(['hangup', 'destroy', 'purge']);

export async function create(elementId, optionsJson, dotNetCallback) {
    if (clients.has(elementId)) {
        throw new Error('TELNYX_INVALID_STATE: Client already exists.');
    }

    const config = JSON.parse(optionsJson);
    const wrapper = {
        id: elementId,
        dotNetCallback,
        config,
        client: null,
        currentCall: null,
        eventHandlers: new Map(),
        generation: 0,
        disposed: false,
        eventQueue: Promise.resolve(),
        pendingTelemetry: new Set(),
        lifecycle: Promise.resolve(),
        callStates: new Map(),
        mediaCleanup: [],
        state: 'initializing',
        startedAt: Date.now(),
        reconnects: 0
    };

    clients.set(elementId, wrapper);

    try {
        await queueLifecycle(wrapper, () => createClient(wrapper));
        await invokeDotNet(wrapper, 'initialized', '');
    } catch (error) {
        try { await disconnectClient(wrapper); } finally {
            wrapper.disposed = true;
            wrapper.dotNetCallback = null;
            clients.delete(elementId);
        }
        throw error;
    }
}

async function createClient(wrapper) {
    const generation = ++wrapper.generation;
    wrapper.eventQueue = Promise.resolve();
    wrapper.pendingTelemetry = new Set();
    wrapper.callStates.clear();
    setConnectionState(wrapper, 'connecting');
    const initOptions = { ...wrapper.config.initOptions };

    if (initOptions.mediaPermissionsRecovery) {
        initOptions.mediaPermissionsRecovery = {
            ...initOptions.mediaPermissionsRecovery,
            onSuccess: () => generation === wrapper.generation && invokeDotNet(wrapper, 'mediaPermissionsRecoverySuccess', null),
            onError: error => generation === wrapper.generation && invokeDotNet(wrapper, 'mediaPermissionsRecoveryError', normalizeError(error))
        };
    }

    if (wrapper.config.autoReconnect != null && initOptions.autoReconnect == null) {
        initOptions.autoReconnect = wrapper.config.autoReconnect;
    }

    if (wrapper.config.reconnectAttempts != null && initOptions.maxReconnectAttempts == null) {
        initOptions.maxReconnectAttempts = wrapper.config.reconnectAttempts;
    }

    wrapper.client = new TelnyxWebRTC.TelnyxRTC(initOptions);
    wrapper.client.localElement = initOptions.localElement;
    wrapper.client.remoteElement = initOptions.remoteElement;
    const localElement = typeof initOptions.localElement === 'string' ? globalThis.document?.getElementById(initOptions.localElement) : initOptions.localElement;
    if (localElement) localElement.muted = true;

    if (initOptions.speaker) {
        wrapper.client.speaker = initOptions.speaker;
    }

    if (wrapper.audioSettings || initOptions.micId || initOptions.micLabel) {
        await wrapper.client.setAudioSettings(wrapper.audioSettings ?? { micId: initOptions.micId, micLabel: initOptions.micLabel });
    }

    bindEvents(wrapper);
    await wrapper.client.connect();
}

function bindEvents(wrapper) {
    const generation = wrapper.generation;
    const isCurrent = () => !wrapper.disposed && generation === wrapper.generation;
    const notificationHandler = notification => {
        if (!isCurrent()) return;
        const incoming = notification?.type === 'callUpdate' ? notification.call : null;
        if (incoming) {
            // Screen sharing is a child call, never a replacement for the selected call.
            if (incoming.options?.screenShare) return;
            const current = wrapper.currentCall;
            if (current && current.id !== incoming.id && !terminalCallStates.has(current.state)) {
                if (wrapper.config.additionalCallPolicy?.toLowerCase() === 'notify') {
                    invokeDotNet(wrapper, 'additionalCall', normalizeCall(incoming));
                } else if (!terminalCallStates.has(incoming.state)) {
                    Promise.resolve(incoming.hangup()).catch(() => diagnostic(wrapper, 'additionalCallRejectionFailed'));
                }
                return;
            }
            if (terminalCallStates.has(incoming.state)) {
                if (current?.id === incoming.id) wrapper.currentCall = null;
            } else {
                wrapper.currentCall = incoming;
            }
            syncMedia(wrapper);
        }
        // Serialize now: the SDK mutates Call objects again before consumers finish.
        const snapshot = normalizeNotification(notification);
        invokeDotNet(wrapper, 'notification', snapshot);
        if (!incoming) return;
        let previous = wrapper.callStates.get(incoming.id);
        if (!previous) {
            previous = { state: null, answered: false, ended: false };
            wrapper.callStates.set(incoming.id, previous);
            if (!terminalCallStates.has(incoming.state)) invokeDotNet(wrapper, 'callInitiated', snapshot);
        }
        if (previous.state === incoming.state || previous.ended) return;
        if (incoming.state === 'active') {
            if (!previous.answered) {
                previous.answered = true;
                invokeDotNet(wrapper, 'callAnswered', snapshot);
            } else if (previous.state === 'held') {
                invokeDotNet(wrapper, 'callResumed', snapshot);
            }
        } else if (incoming.state === 'held') {
            invokeDotNet(wrapper, 'callHeld', snapshot);
        } else if (terminalCallStates.has(incoming.state)) {
            previous.ended = true;
            invokeDotNet(wrapper, 'callHangup', snapshot);
            diagnostic(wrapper, 'callEnded', incoming);
        }
        previous.state = incoming.state;
        // Bound history without removing the most recent terminal calls.
        if (wrapper.callStates.size > 256) wrapper.callStates.delete(wrapper.callStates.keys().next().value);
    };
    wrapper.client.on('telnyx.notification', notificationHandler);
    wrapper.eventHandlers.set('telnyx.notification', notificationHandler);
    for (const eventName of sessionEvents) {
        if (eventName === 'telnyx.socket.message' && !wrapper.config.enableRawEvents) continue;
        const name = eventName === 'telnyx.socket.message' ? 'rawSocketMessage' : eventName.substring(7);
        const handler = value => {
            if (!isCurrent()) return;
            if (name === 'ready') {
                setConnectionState(wrapper, 'registered');
                diagnostic(wrapper, 'registered');
            } else if (name === 'socket.close') {
                const reconnect = wrapper.config.initOptions.autoReconnect ?? wrapper.config.autoReconnect ?? true;
                setConnectionState(wrapper, reconnect ? 'reconnecting' : 'disconnected');
                if (reconnect) wrapper.reconnects++;
            } else if (name === 'error') diagnostic(wrapper, 'sdkError', value);
            invokeDotNet(wrapper, name, value);
        };
        wrapper.client.on(eventName, handler);
        wrapper.eventHandlers.set(eventName, handler);
    }
    const devices = globalThis.navigator?.mediaDevices;
    const deviceHandler = () => {
        if (isCurrent()) invokeDotNet(wrapper, 'deviceChange', { timestamp: new Date().toISOString() });
    };
    devices?.addEventListener?.('devicechange', deviceHandler);
    wrapper.deviceCleanup = () => devices?.removeEventListener?.('devicechange', deviceHandler);
    bindPlayback(wrapper);
}

function bindPlayback(wrapper) {
    const generation = wrapper.generation;
    const element = getRemoteElement(wrapper);
    const playbackHandler = () => {
        if (!wrapper.disposed && generation === wrapper.generation) void tryPlayback(wrapper);
    };
    element?.addEventListener?.('loadedmetadata', playbackHandler);
    wrapper.playbackCleanup = () => element?.removeEventListener?.('loadedmetadata', playbackHandler);
}

function syncMedia(wrapper) {
    const call = wrapper.currentCall;
    const streams = [call?.localStream, call?.remoteStream];
    if (streams[0] === wrapper.localStream && streams[1] === wrapper.remoteStream) return;
    for (const cleanup of wrapper.mediaCleanup) cleanup();
    wrapper.mediaCleanup = [];
    const generation = wrapper.generation;
    for (const [index, stream] of streams.entries()) {
        const key = index === 0 ? 'localStream' : 'remoteStream';
        const previous = wrapper[key];
        wrapper[key] = stream;
        if (previous && previous !== stream) invokeDotNet(wrapper, 'streamStopped', { kind: key });
        if (!stream) continue;
        if (previous !== stream) invokeDotNet(wrapper, key, { streamId: stream.id });
        for (const track of stream.getTracks()) {
            const ended = () => {
                if (generation === wrapper.generation) invokeDotNet(wrapper, 'streamStopped', { kind: key, trackId: track.id });
            };
            track.addEventListener('ended', ended);
            wrapper.mediaCleanup.push(() => track.removeEventListener('ended', ended));
        }
    }
    if (streams[1]) void tryPlayback(wrapper);
}

function getRemoteElement(wrapper) {
    const element = wrapper.client?.remoteElement ?? wrapper.config.initOptions.remoteElement;
    return typeof element === 'string' ? globalThis.document?.getElementById(element) : element;
}

async function tryPlayback(wrapper) {
    const generation = wrapper.generation;
    const element = getRemoteElement(wrapper);
    if (!element?.play) return false;
    try { await element.play(); return true; }
    catch {
        if (generation === wrapper.generation) invokeDotNet(wrapper, 'playbackBlocked', { recoverable: true });
        return false;
    }
}

export function resumePlayback(elementId) { return tryPlayback(requireWrapper(elementId)); }

function setConnectionState(wrapper, state) {
    if (wrapper.state === state) return;
    wrapper.state = state;
    invokeDotNet(wrapper, 'connectionState', state);
    diagnostic(wrapper, 'connectionState');
}

function diagnostic(wrapper, name, value) {
    if (!wrapper.config?.enableDiagnostics) return;
    // Explicit allowlist: never log credentials, numbers, message bodies or raw signaling.
    invokeDotNet(wrapper, 'diagnostic', {
        name, state: wrapper.state, timestamp: new Date().toISOString(),
        elapsedMilliseconds: Date.now() - wrapper.startedAt, reconnectAttempts: wrapper.reconnects,
        callId: value?.callId ?? value?.id, sessionId: wrapper.client?.sessionid,
        code: value?.error?.code
    });
}

function unbindEvents(wrapper) {
    if (!wrapper.client) {
        return;
    }

    for (const [eventName, handler] of wrapper.eventHandlers) {
        wrapper.client.off(eventName, handler);
    }

    wrapper.eventHandlers.clear();
    wrapper.deviceCleanup?.();
    wrapper.playbackCleanup?.();
    for (const cleanup of wrapper.mediaCleanup) cleanup();
    wrapper.mediaCleanup = [];
    wrapper.localStream = null;
    wrapper.remoteStream = null;
}

function normalizeNotification(notification) {
    if (!notification || typeof notification !== 'object') {
        return notification;
    }

    return {
        ...notification,
        call: notification.call ? normalizeCall(notification.call) : undefined,
        error: normalizeError(notification.error)
    };
}

function normalizeCall(call) {
    const options = call.options ?? {};
    const ids = call.telnyxIDs ?? {};
    const inbound = call.direction === 'inbound';

    return {
        callId: call.id,
        recoveredCallId: call.recoveredCallId,
        state: call.state,
        previousState: call.prevState,
        direction: call.direction,
        cause: call.cause,
        causeCode: call.causeCode,
        sipReason: call.sipReason,
        sipCode: call.sipCode,
        sipCallId: call.sipCallId,
        role: call.role,
        extension: call.extension,
        isMuted: call.isAudioMuted,
        isOnHold: call.state === 'held',
        callerNumber: inbound ? options.remoteCallerNumber : options.callerNumber,
        callerName: inbound ? options.remoteCallerName : options.callerName,
        calleeNumber: inbound ? options.callerNumber : (options.destinationNumber ?? options.remoteCallerNumber),
        calleeName: inbound ? options.callerName : options.remoteCallerName,
        callControlId: ids.telnyxCallControlId ?? options.telnyxCallControlId,
        sessionId: ids.telnyxSessionId ?? options.telnyxSessionId,
        legId: ids.telnyxLegId ?? options.telnyxLegId,
        clientState: options.clientState,
        audio: options.audio,
        video: options.video,
        useStereo: options.useStereo,
        micId: options.micId,
        camId: options.camId,
        speakerId: options.speakerId,
        customHeaders: options.customHeaders
    };
}

function normalizeError(error) {
    if (!error) {
        return undefined;
    }

    if (error instanceof Error) {
        return { ...error, name: error.name, message: error.message, stack: error.stack };
    }

    return error;
}

function safeStringify(value) {
    if (typeof value === 'string') {
        return value;
    }

    const seen = new WeakSet();
    return JSON.stringify(value ?? null, (key, item) => {
        if (item instanceof Error) {
            return normalizeError(item);
        }

        if (typeof item === 'object' && item !== null) {
            if (seen.has(item)) {
                return '[Circular]';
            }

            seen.add(item);
        }

        return item;
    });
}

function invokeDotNet(wrapper, eventName, value) {
    const generation = wrapper.generation;
    const telemetry = wrapper.pendingTelemetry;
    const coalesce = eventName === 'stats.frame' || eventName === 'stats.report';
    if (coalesce && telemetry.has(eventName)) return Promise.resolve();
    if (coalesce) telemetry.add(eventName);
    const json = safeStringify(value);
    // SDK operations never wait on application callbacks: callbacks may call back into this bridge.
    wrapper.eventQueue = wrapper.eventQueue.then(async () => {
        if (wrapper.disposed || generation !== wrapper.generation || !wrapper.dotNetCallback) {
            if (coalesce) telemetry.delete(eventName);
            return;
        }
        try { await wrapper.dotNetCallback.invokeMethodAsync('HandleTelnyxEvent', eventName, json); }
        catch { console.warn('Telnyx WebRTC application callback failed.'); }
        finally { if (coalesce) telemetry.delete(eventName); }
    });
    return Promise.resolve();
}

function getWrapper(elementId) { return clients.get(elementId); }
function requireWrapper(elementId) {
    const wrapper = getWrapper(elementId);
    if (!wrapper || wrapper.disposed) throw new Error('TELNYX_DISPOSED: Client is unavailable.');
    return wrapper;
}
function requireClient(elementId) {
    const client = requireWrapper(elementId).client;
    if (!client) throw new Error('TELNYX_INVALID_STATE: Client is disconnected.');
    return client;
}
function getCall(elementId) { return getWrapper(elementId)?.currentCall; }
function requireCall(elementId) {
    const call = requireWrapper(elementId).currentCall;
    if (!call || terminalCallStates.has(call.state)) throw new Error('TELNYX_INVALID_STATE: No active call.');
    return call;
}

function queueLifecycle(wrapper, action) {
    const result = wrapper.lifecycle.then(() => {
        if (wrapper.disposed) throw new Error('TELNYX_DISPOSED: Client is disposed.');
        return action();
    });
    wrapper.lifecycle = result.catch(() => {});
    return result;
}

function parseOptionalJson(json) {
    return json ? JSON.parse(json) : undefined;
}

export function call(elementId, optionsJson, localStream, remoteStream, localElement, remoteElement) {
    const wrapper = requireWrapper(elementId);
    requireClient(elementId);
    if (wrapper.state !== 'registered') throw new Error('TELNYX_INVALID_STATE: Client is not registered.');

    if (wrapper.currentCall && !terminalCallStates.has(wrapper.currentCall.state)) {
        throw new Error('A call is already active. Hang it up before starting another call.');
    }

    const options = JSON.parse(optionsJson);

    if (localStream) {
        options.localStream = localStream;
    }

    if (remoteStream) {
        options.remoteStream = remoteStream;
    }

    if (localElement) {
        options.localElement = localElement;
    }

    if (remoteElement) {
        options.remoteElement = remoteElement;
    }

    wrapper.currentCall = wrapper.client.newCall(options);
    syncMedia(wrapper);
}

export function answer(elementId, optionsJson, localElement, remoteElement) {
    const options = parseOptionalJson(optionsJson) ?? {};
    if (localElement) options.localElement = localElement;
    if (remoteElement) options.remoteElement = remoteElement;
    return requireCall(elementId).answer(options);
}

export async function hangup(elementId, optionsJson, execute) {
    const wrapper = requireWrapper(elementId);
    requireCall(elementId);

    const callToHangUp = wrapper.currentCall;
    await callToHangUp.hangup(parseOptionalJson(optionsJson), execute);

    if (wrapper.currentCall === callToHangUp && terminalCallStates.has(callToHangUp.state)) {
        wrapper.currentCall = null;
        syncMedia(wrapper);
    }
}

export function muteAudio(elementId) { return requireCall(elementId).muteAudio(); }
export function unmuteAudio(elementId) { return requireCall(elementId).unmuteAudio(); }
export function toggleAudioMute(elementId) { return requireCall(elementId).toggleAudioMute(); }
export function muteVideo(elementId) { return requireCall(elementId).muteVideo(); }
export function unmuteVideo(elementId) { return requireCall(elementId).unmuteVideo(); }
export function toggleVideoMute(elementId) { return requireCall(elementId).toggleVideoMute(); }
export function deaf(elementId) { return requireCall(elementId).deaf(); }
export function undeaf(elementId) { return requireCall(elementId).undeaf(); }
export function toggleDeaf(elementId) { return requireCall(elementId).toggleDeaf(); }
export function hold(elementId) { return requireCall(elementId).hold(); }
export function unhold(elementId) { return requireCall(elementId).unhold(); }
export function toggleHold(elementId) { return requireCall(elementId).toggleHold(); }
export function dtmf(elementId, digit) { return requireCall(elementId).dtmf(digit); }
export function message(elementId, to, body) { return requireCall(elementId).message(to, body); }
export async function setAudioInDevice(elementId, deviceId, muted) { await requireCall(elementId).setAudioInDevice(deviceId, muted ?? undefined); syncMedia(requireWrapper(elementId)); }
export async function setVideoDevice(elementId, deviceId) { await requireCall(elementId).setVideoDevice(deviceId); syncMedia(requireWrapper(elementId)); }
export function setAudioOutDevice(elementId, deviceId) { return requireCall(elementId).setAudioOutDevice(deviceId); }

export function startScreenShare(elementId, optionsJson, localStream, remoteStream, localElement, remoteElement) {
    const options = parseOptionalJson(optionsJson) ?? {};
    if (localStream) options.localStream = localStream;
    if (remoteStream) options.remoteStream = remoteStream;
    if (localElement) options.localElement = localElement;
    if (remoteElement) options.remoteElement = remoteElement;
    return requireCall(elementId).startScreenShare(options);
}

export function stopScreenShare(elementId) { return requireCall(elementId).stopScreenShare(); }
export function setAudioBandwidth(elementId, bps) { return requireCall(elementId).setAudioBandwidthEncodingsMaxBps(bps); }
export function setVideoBandwidth(elementId, bps) { return requireCall(elementId).setVideoBandwidthEncodingsMaxBps(bps); }

async function getDevicesBy(elementId, methodName) {
    const client = requireClient(elementId);

    const devices = await client[methodName]();
    return JSON.stringify(devices);
}

export function getDevices(elementId) { return getDevicesBy(elementId, 'getDevices'); }
export function getVideoDevices(elementId) { return getDevicesBy(elementId, 'getVideoDevices'); }
export function getAudioInDevices(elementId) { return getDevicesBy(elementId, 'getAudioInDevices'); }
export function getAudioOutDevices(elementId) { return getDevicesBy(elementId, 'getAudioOutDevices'); }

export function getActiveCalls(elementId) {
    const calls = requireClient(elementId).getActiveCalls() ?? [];
    return JSON.stringify(calls.map(normalizeCall));
}

export async function getIsRegistered(elementId) {
    return await requireClient(elementId).getIsRegistered() ?? false;
}

export function speedTest(elementId, bytes) {
    return requireClient(elementId).speedTest(bytes).then(safeStringify);
}

export function validateDeviceId(elementId, deviceId, label, kind) {
    return requireClient(elementId).validateDeviceId(deviceId, label, kind);
}

export async function getDeviceResolutions(elementId, deviceId) {
    const resolutions = await requireClient(elementId).getDeviceResolutions(deviceId) ?? [];
    return JSON.stringify(resolutions);
}

export function getMediaConstraints(elementId) {
    return safeStringify(requireClient(elementId).mediaConstraints ?? null);
}

export function getIceServers(elementId) {
    return safeStringify(requireClient(elementId).iceServers ?? []);
}

export function setIceServers(elementId, serversJson) {
    const client = requireClient(elementId);
    if (client) {
        const servers = JSON.parse(serversJson);
        client.iceServers = servers;
        requireWrapper(elementId).config.initOptions.iceServers = servers;
    }
}

export function getSpeaker(elementId) {
    return requireClient(elementId).speaker ?? null;
}

export function setSpeaker(elementId, deviceId) {
    const client = requireClient(elementId);
    if (client) {
        client.speaker = deviceId;
        requireWrapper(elementId).config.initOptions.speaker = deviceId;
    }
}

export function setLocalElement(elementId, mediaElement) {
    const client = requireClient(elementId);
    if (client) {
        client.localElement = mediaElement;
        requireWrapper(elementId).config.initOptions.localElement = mediaElement;
        const element = typeof mediaElement === 'string' ? globalThis.document?.getElementById(mediaElement) : mediaElement;
        if (element) element.muted = true;
    }
}

export function setRemoteElement(elementId, mediaElement) {
    const client = requireClient(elementId);
    if (client) {
        const wrapper = requireWrapper(elementId);
        wrapper.playbackCleanup?.();
        client.remoteElement = mediaElement;
        wrapper.config.initOptions.remoteElement = mediaElement;
        bindPlayback(wrapper);
    }
}

export async function checkPermissions(elementId, audio = true, video = true) {
    return await requireClient(elementId).checkPermissions(audio, video) ?? false;
}

export async function setAudioSettings(elementId, settingsJson) {
    const client = requireClient(elementId);

    const settings = JSON.parse(settingsJson);
    await client.setAudioSettings(settings);
    requireWrapper(elementId).audioSettings = settings;
    return true;
}

export async function setVideoSettings(elementId, settingsJson) {
    const call = requireCall(elementId);

    const settings = JSON.parse(settingsJson);
    if (settings.camId) {
        await call.setVideoDevice(settings.camId);
    }

    const { camId, camLabel, ...constraints } = settings;
    if (!camId && camLabel) {
        const deviceId = await requireClient(elementId).validateDeviceId(undefined, camLabel, 'videoinput');
        if (!deviceId) throw new Error('TELNYX_INVALID_STATE: Camera label could not be resolved.');
        await call.setVideoDevice(deviceId);
    }
    const track = call.localStream?.getVideoTracks()?.[0];
    if (Object.keys(constraints).length > 0) {
        if (!track) throw new Error('TELNYX_INVALID_STATE: No local video track.');
        await track.applyConstraints(constraints);
    }

    syncMedia(requireWrapper(elementId));
    return true;
}

export function enableMicrophone(elementId) { return requireClient(elementId).enableMicrophone(); }
export function disableMicrophone(elementId) { return requireClient(elementId).disableMicrophone(); }
export function enableWebcam(elementId) { return requireCall(elementId).unmuteVideo(); }
export function disableWebcam(elementId) { return requireCall(elementId).muteVideo(); }
export function toggleAudio(elementId, enabled) { return enabled ? unmuteAudio(elementId) : muteAudio(elementId); }
export function toggleVideo(elementId, enabled) { return enabled ? unmuteVideo(elementId) : muteVideo(elementId); }

export function login(elementId, loginOptionsJson) {
    const client = requireClient(elementId);

    const creds = loginOptionsJson ? JSON.parse(loginOptionsJson) : undefined;
    if (creds) Object.assign(requireWrapper(elementId).config.initOptions, creds);
    return client.login(creds ? { creds } : undefined);
}

export function logout(elementId) {
    return requireClient(elementId).logout();
}

export function hasActiveCall(elementId) {
    return requireClient(elementId).hasActiveCall() ?? false;
}

export function webRtcInfo() {
    return safeStringify(TelnyxWebRTC.TelnyxRTC.webRTCInfo());
}

export async function runPreCallDiagnosis(optionsJson) {
    return safeStringify(await TelnyxWebRTC.PreCallDiagnosis.run(JSON.parse(optionsJson)));
}

export function setAudioVolume(elementId, volume) {
    const element = document.getElementById(elementId);
    if (!(element instanceof HTMLMediaElement)) throw new Error('TELNYX_INVALID_STATE: Media element was not found.');
    if (!Number.isFinite(volume) || volume < 0 || volume > 1) throw new RangeError('Volume must be between zero and one.');
    element.volume = volume;
}

async function disconnectClient(wrapper) {
    ++wrapper.generation;
    wrapper.eventQueue = Promise.resolve();
    wrapper.currentCall = null;
    syncMedia(wrapper);
    unbindEvents(wrapper);
    const client = wrapper.client;
    wrapper.client = null;
    try { await client?.disconnect(); }
    finally { setConnectionState(wrapper, 'disconnected'); }
}

export function disconnect(elementId) {
    const wrapper = requireWrapper(elementId);
    return queueLifecycle(wrapper, () => disconnectClient(wrapper));
}

export function reconnect(elementId) {
    const wrapper = requireWrapper(elementId);
    return queueLifecycle(wrapper, async () => {
        await disconnectClient(wrapper);
        wrapper.reconnects++;
        await createClient(wrapper);
    });
}

export function connect(elementId) {
    const wrapper = requireWrapper(elementId);
    return queueLifecycle(wrapper, async () => {
        if (wrapper.client) {
            if (wrapper.state === 'registered' || wrapper.state === 'connecting') return;
            setConnectionState(wrapper, 'connecting');
            await wrapper.client.connect();
        } else await createClient(wrapper);
    });
}

export async function unmount(elementId) {
    const wrapper = getWrapper(elementId);
    if (!wrapper) return;
    if (wrapper.disposePromise) return wrapper.disposePromise;
    // Immediately invalidate callbacks, including those queued behind an in-flight callback.
    wrapper.disposed = true;
    ++wrapper.generation;
    wrapper.dotNetCallback = null;
    wrapper.disposePromise = wrapper.lifecycle.catch(() => {}).then(async () => {
        try { await disconnectClient(wrapper); }
        finally {
            clients.delete(elementId);
            disconnectObserver(elementId);
            wrapper.config = null;
            wrapper.callStates.clear();
        }
    });
    return wrapper.disposePromise;
}

export function createObserver(elementId, ownerElementId = elementId) {
    const element = document.getElementById(ownerElementId);
    if (!element?.parentNode || observers.has(elementId)) {
        return;
    }

    const observer = new MutationObserver(() => {
        if (!element.isConnected) {
            void unmount(elementId).catch(() => console.warn('Telnyx WebRTC cleanup failed.'));
        }
    });

    observer.observe(document.documentElement, { childList: true, subtree: true });
    observers.set(elementId, observer);
}

function disconnectObserver(elementId) {
    observers.get(elementId)?.disconnect();
    observers.delete(elementId);
}

export async function getCallStats(elementId) {
    const peerConnection = getCall(elementId)?.peer?.instance;
    if (!peerConnection) {
        return null;
    }

    const report = await peerConnection.getStats();
    return JSON.stringify(Array.from(report.values()));
}

export function getCurrentCall(elementId) {
    const call = getCall(elementId);
    return call ? safeStringify(normalizeCall(call)) : null;
}

export function getLocalStream(elementId) {
    const stream = getCall(elementId)?.localStream;
    return stream ?? null;
}
export function getRemoteStream(elementId) {
    const stream = getCall(elementId)?.remoteStream;
    return stream ?? null;
}
export function sendConversationMessage(elementId, message, attachments) {
    return requireCall(elementId).sendConversationMessage(message, attachments ?? []);
}
export function sendAiConversationMessage(elementId, itemJson) {
    requireCall(elementId).sendAIConversationMessage(JSON.parse(itemJson));
}
