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
        console.warn(`Telnyx WebRTC client already exists for "${elementId}".`);
        return;
    }

    const config = JSON.parse(optionsJson);
    const wrapper = {
        id: elementId,
        dotNetCallback,
        config,
        client: null,
        currentCall: null,
        eventHandlers: new Map()
    };

    clients.set(elementId, wrapper);

    try {
        await createClient(wrapper);
        await invokeDotNet(wrapper, 'initialized', '');
    } catch (error) {
        clients.delete(elementId);
        throw error;
    }
}

async function createClient(wrapper) {
    const initOptions = { ...wrapper.config.initOptions };

    if (initOptions.mediaPermissionsRecovery) {
        initOptions.mediaPermissionsRecovery = {
            ...initOptions.mediaPermissionsRecovery,
            onSuccess: () => invokeDotNet(wrapper, 'mediaPermissionsRecoverySuccess', null),
            onError: error => invokeDotNet(wrapper, 'mediaPermissionsRecoveryError', normalizeError(error))
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

    if (initOptions.speaker) {
        wrapper.client.speaker = initOptions.speaker;
    }

    if (initOptions.micId || initOptions.micLabel) {
        await wrapper.client.setAudioSettings({ micId: initOptions.micId, micLabel: initOptions.micLabel });
    }

    bindEvents(wrapper);
    await wrapper.client.connect();
}

function bindEvents(wrapper) {
    const notificationHandler = async notification => {
        if (notification?.type === 'callUpdate' && notification.call) {
            wrapper.currentCall = notification.call;
        }

        await invokeDotNet(wrapper, 'notification', normalizeNotification(notification));

        if (notification?.type !== 'callUpdate' || !notification.call) {
            return;
        }

        const call = notification.call;
        const state = call.state;

        if (terminalCallStates.has(state) && wrapper.currentCall === call) {
            wrapper.currentCall = null;
        }
    };

    wrapper.client.on('telnyx.notification', notificationHandler);
    wrapper.eventHandlers.set('telnyx.notification', notificationHandler);

    for (const eventName of sessionEvents) {
        const handler = value => {
            const dotNetEventName = eventName === 'telnyx.socket.message'
                ? 'rawSocketMessage'
                : eventName.substring('telnyx.'.length);

            return invokeDotNet(wrapper, dotNetEventName, value);
        };

        wrapper.client.on(eventName, handler);
        wrapper.eventHandlers.set(eventName, handler);
    }
}

function unbindEvents(wrapper) {
    if (!wrapper.client) {
        return;
    }

    for (const [eventName, handler] of wrapper.eventHandlers) {
        wrapper.client.off(eventName, handler);
    }

    wrapper.eventHandlers.clear();
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
        return { name: error.name, message: error.message, stack: error.stack };
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
    if (!wrapper.dotNetCallback) {
        return Promise.resolve();
    }

    return wrapper.dotNetCallback.invokeMethodAsync('HandleTelnyxEvent', eventName, safeStringify(value));
}

function getWrapper(elementId) {
    return clients.get(elementId);
}

function getCall(elementId) {
    return getWrapper(elementId)?.currentCall;
}

function parseOptionalJson(json) {
    return json ? JSON.parse(json) : undefined;
}

export function call(elementId, optionsJson, localStream, remoteStream, localElement, remoteElement) {
    const wrapper = getWrapper(elementId);
    if (!wrapper?.client) {
        return;
    }

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
}

export function answer(elementId, optionsJson, localElement, remoteElement) {
    const options = parseOptionalJson(optionsJson) ?? {};
    if (localElement) options.localElement = localElement;
    if (remoteElement) options.remoteElement = remoteElement;
    return getCall(elementId)?.answer(options);
}

export async function hangup(elementId, optionsJson, execute) {
    const wrapper = getWrapper(elementId);
    if (!wrapper?.currentCall) {
        return;
    }

    const callToHangUp = wrapper.currentCall;
    await callToHangUp.hangup(parseOptionalJson(optionsJson), execute);

    if (wrapper.currentCall === callToHangUp) {
        wrapper.currentCall = null;
    }
}

export function muteAudio(elementId) { getCall(elementId)?.muteAudio(); }
export function unmuteAudio(elementId) { getCall(elementId)?.unmuteAudio(); }
export function toggleAudioMute(elementId) { getCall(elementId)?.toggleAudioMute(); }
export function muteVideo(elementId) { getCall(elementId)?.muteVideo(); }
export function unmuteVideo(elementId) { getCall(elementId)?.unmuteVideo(); }
export function toggleVideoMute(elementId) { getCall(elementId)?.toggleVideoMute(); }
export function deaf(elementId) { getCall(elementId)?.deaf(); }
export function undeaf(elementId) { getCall(elementId)?.undeaf(); }
export function toggleDeaf(elementId) { getCall(elementId)?.toggleDeaf(); }
export function hold(elementId) { return getCall(elementId)?.hold(); }
export function unhold(elementId) { return getCall(elementId)?.unhold(); }
export function toggleHold(elementId) { return getCall(elementId)?.toggleHold(); }
export function dtmf(elementId, digit) { getCall(elementId)?.dtmf(digit); }
export function message(elementId, to, body) { getCall(elementId)?.message(to, body); }
export function setAudioInDevice(elementId, deviceId, muted) { return getCall(elementId)?.setAudioInDevice(deviceId, muted); }
export function setVideoDevice(elementId, deviceId) { return getCall(elementId)?.setVideoDevice(deviceId); }
export function setAudioOutDevice(elementId, deviceId) { return getCall(elementId)?.setAudioOutDevice(deviceId); }

export function startScreenShare(elementId, optionsJson, localStream, remoteStream, localElement, remoteElement) {
    const options = parseOptionalJson(optionsJson) ?? {};
    if (localStream) options.localStream = localStream;
    if (remoteStream) options.remoteStream = remoteStream;
    if (localElement) options.localElement = localElement;
    if (remoteElement) options.remoteElement = remoteElement;
    return getCall(elementId)?.startScreenShare(options);
}

export function stopScreenShare(elementId) { return getCall(elementId)?.stopScreenShare(); }
export function setAudioBandwidth(elementId, bps) { return getCall(elementId)?.setAudioBandwidthEncodingsMaxBps(bps); }
export function setVideoBandwidth(elementId, bps) { return getCall(elementId)?.setVideoBandwidthEncodingsMaxBps(bps); }

async function getDevicesBy(elementId, methodName) {
    const client = getWrapper(elementId)?.client;
    if (!client) {
        return '[]';
    }

    const devices = await client[methodName]();
    return JSON.stringify(devices);
}

export function getDevices(elementId) { return getDevicesBy(elementId, 'getDevices'); }
export function getVideoDevices(elementId) { return getDevicesBy(elementId, 'getVideoDevices'); }
export function getAudioInDevices(elementId) { return getDevicesBy(elementId, 'getAudioInDevices'); }
export function getAudioOutDevices(elementId) { return getDevicesBy(elementId, 'getAudioOutDevices'); }

export function getActiveCalls(elementId) {
    const calls = getWrapper(elementId)?.client?.getActiveCalls() ?? [];
    return JSON.stringify(calls.map(normalizeCall));
}

export async function getIsRegistered(elementId) {
    return await getWrapper(elementId)?.client?.getIsRegistered() ?? false;
}

export function speedTest(elementId, bytes) {
    return getWrapper(elementId)?.client?.speedTest(bytes).then(safeStringify);
}

export function validateDeviceId(elementId, deviceId, label, kind) {
    return getWrapper(elementId)?.client?.validateDeviceId(deviceId, label, kind);
}

export async function getDeviceResolutions(elementId, deviceId) {
    const resolutions = await getWrapper(elementId)?.client?.getDeviceResolutions(deviceId) ?? [];
    return JSON.stringify(resolutions);
}

export function getMediaConstraints(elementId) {
    return safeStringify(getWrapper(elementId)?.client?.mediaConstraints ?? null);
}

export function getIceServers(elementId) {
    return safeStringify(getWrapper(elementId)?.client?.iceServers ?? []);
}

export function setIceServers(elementId, serversJson) {
    const client = getWrapper(elementId)?.client;
    if (client) {
        client.iceServers = JSON.parse(serversJson);
    }
}

export function getSpeaker(elementId) {
    return getWrapper(elementId)?.client?.speaker ?? null;
}

export function setSpeaker(elementId, deviceId) {
    const client = getWrapper(elementId)?.client;
    if (client) {
        client.speaker = deviceId;
    }
}

export function setLocalElement(elementId, mediaElement) {
    const client = getWrapper(elementId)?.client;
    if (client) {
        client.localElement = mediaElement;
    }
}

export function setRemoteElement(elementId, mediaElement) {
    const client = getWrapper(elementId)?.client;
    if (client) {
        client.remoteElement = mediaElement;
    }
}

export async function checkPermissions(elementId, audio = true, video = true) {
    return await getWrapper(elementId)?.client?.checkPermissions(audio, video) ?? false;
}

export async function setAudioSettings(elementId, settingsJson) {
    const client = getWrapper(elementId)?.client;
    if (!client) {
        return false;
    }

    await client.setAudioSettings(JSON.parse(settingsJson));
    return true;
}

export async function setVideoSettings(elementId, settingsJson) {
    const call = getCall(elementId);
    if (!call) {
        return false;
    }

    const settings = JSON.parse(settingsJson);
    if (settings.camId) {
        await call.setVideoDevice(settings.camId);
    }

    const { camId, camLabel, ...constraints } = settings;
    const track = call.localStream?.getVideoTracks()?.[0];
    if (track && Object.keys(constraints).length > 0) {
        await track.applyConstraints(constraints);
    }

    return true;
}

export function enableMicrophone(elementId) { getWrapper(elementId)?.client?.enableMicrophone(); }
export function disableMicrophone(elementId) { getWrapper(elementId)?.client?.disableMicrophone(); }
export function enableWebcam(elementId) { getCall(elementId)?.unmuteVideo(); }
export function disableWebcam(elementId) { getCall(elementId)?.muteVideo(); }
export function toggleAudio(elementId, enabled) { enabled ? unmuteAudio(elementId) : muteAudio(elementId); }
export function toggleVideo(elementId, enabled) { enabled ? unmuteVideo(elementId) : muteVideo(elementId); }

export function login(elementId, loginOptionsJson) {
    const client = getWrapper(elementId)?.client;
    if (!client) {
        return;
    }

    const creds = loginOptionsJson ? JSON.parse(loginOptionsJson) : undefined;
    return client.login(creds ? { creds } : undefined);
}

export function logout(elementId) {
    return getWrapper(elementId)?.client?.logout();
}

export function hasActiveCall(elementId) {
    return getWrapper(elementId)?.client?.hasActiveCall() ?? false;
}

export function webRtcInfo() {
    return safeStringify(TelnyxWebRTC.TelnyxRTC.webRTCInfo());
}

export async function runPreCallDiagnosis(optionsJson) {
    return safeStringify(await TelnyxWebRTC.PreCallDiagnosis.run(JSON.parse(optionsJson)));
}

export function setAudioVolume(elementId, volume) {
    const element = document.getElementById(elementId);
    if (element instanceof HTMLMediaElement) {
        element.volume = Math.max(0, Math.min(1, volume));
    }
}

export async function disconnect(elementId) {
    const wrapper = getWrapper(elementId);
    if (!wrapper?.client) {
        return;
    }

    unbindEvents(wrapper);
    wrapper.currentCall = null;
    const client = wrapper.client;

    try {
        await client.disconnect();
    } finally {
        if (wrapper.client === client) {
            wrapper.client = null;
        }
    }
}

export async function reconnect(elementId) {
    const wrapper = getWrapper(elementId);
    if (!wrapper) {
        return;
    }

    await disconnect(elementId);
    await createClient(wrapper);
}

export function connect(elementId) {
    const wrapper = getWrapper(elementId);
    if (!wrapper) {
        return;
    }

    return wrapper.client?.connect() ?? createClient(wrapper);
}

export async function unmount(elementId) {
    const wrapper = getWrapper(elementId);
    if (!wrapper) {
        return;
    }

    try {
        await disconnect(elementId);
    } finally {
        wrapper.dotNetCallback = null;
        clients.delete(elementId);
        disconnectObserver(elementId);
    }
}

export function createObserver(elementId) {
    const element = document.getElementById(elementId);
    if (!element?.parentNode || observers.has(elementId)) {
        return;
    }

    const observer = new MutationObserver(() => {
        if (!element.isConnected) {
            void unmount(elementId);
        }
    });

    observer.observe(element.parentNode, { childList: true });
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

export function getLocalStream(elementId) { return getCall(elementId)?.localStream ?? null; }
export function getRemoteStream(elementId) { return getCall(elementId)?.remoteStream ?? null; }
export function sendConversationMessage(elementId, message, attachments) {
    return getCall(elementId)?.sendConversationMessage(message, attachments);
}
export function sendAiConversationMessage(elementId, itemJson) {
    getCall(elementId)?.sendAIConversationMessage(JSON.parse(itemJson));
}
