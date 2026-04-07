const clients = new Map();
const observers = new Map();

if (typeof window !== 'undefined' && !window._telnyxUnloadHandlerAdded) {
    window.addEventListener('unload', () => {
        for (const id of clients.keys()) {
            unmount(id);
        }
    });
    window._telnyxUnloadHandlerAdded = true;
}
export function create( elementId, optionsJson, dotNetCallback) {
    if (clients.has(elementId)) {
        console.warn(`Telnyx WebRTC client already exists for "${elementId}".`);
        return;
    }

    const config = JSON.parse(optionsJson);

    const wrapper = {
        id: elementId,
        dotNetCallback,
        reconnectCount: 0,
        config,
        client: null
    };

    clients.set( elementId, wrapper);
    _createClient(wrapper);
    dotNetCallback.invokeMethodAsync('HandleTelnyxEvent', 'initialized', '');
}

function _createClient(wrapper) {
    const { config } = wrapper;

    wrapper.client = new TelnyxWebRTC.TelnyxRTC(config.initOptions);

    wrapper.client.remoteElement = config.initOptions.remoteElement;
    wrapper.client.localElement = config.initOptions.localElement;

    config.audio ? wrapper.client.enableMicrophone() : wrapper.client.disableMicrophone();
    config.video ? wrapper.client.enableWebcam() : wrapper.client.disableWebcam();

    _bindEvents(wrapper);
    wrapper.client.connect();
    
    // --- Forward all raw WebSocket messages to .NET ---
    // Find the underlying WebSocket and hook into onmessage
    const ws = wrapper.client.connection && wrapper.client.connection._wsClient;
    if (ws) {
        ws.addEventListener('message', (evt) => {
            if (wrapper.dotNetCallback && evt && evt.data) {
                wrapper.dotNetCallback.invokeMethodAsync('HandleTelnyxEvent', 'rawSocketMessage', evt.data);
            }
        });
    }
}

/* ---------- EVENT BINDING & HANDLERS ---------- */

function _bindEvents(wrapper) {
    const { client, dotNetCallback } = wrapper;

    const safeStringify = (o, indent = 2) => {
        const seen = new WeakSet();
        return JSON.stringify(o, (k, v) => {
            if (typeof v === 'object' && v !== null) {
                if (seen.has(v)) return '[Circular]';
                seen.add(v);
            }
            return v;
        }, indent);
    };
    const safeInvoke = (evt, args) =>
        dotNetCallback.invokeMethodAsync('HandleTelnyxEvent', evt, safeStringify(args));

    // session‑level events we simply forward
    [
        'ready', 'error', 'socket.open', 'socket.close',
        'socket.error', 'reconnecting', 'reconnected', 'disconnected',
        'socket.message', 'ping', 'pong'
    ].forEach(e => client.on(`telnyx.${e}`, arg => safeInvoke(e, arg)));

    // central handler for call updates & inbound ringing
    client.on('telnyx.notification', notification => {
        safeInvoke('notification', notification);

        if (notification.type !== 'callUpdate' || !notification.call) return;

        _handleCallUpdate(wrapper, notification.call, safeInvoke);
    });
}

/**
 * Handles all call state transitions (both outbound & inbound).
 */
function _handleCallUpdate(wrapper, call, safeInvoke) {
    const { config } = wrapper;
    const state = call.state;

    // Always keep the reference fresh
    wrapper.client.currentCall = call;

    switch (state) {
        case 'ringing': {                        // <-- INCOMING
            safeInvoke('incoming', call);

            if (config.autoAnswer) {
                try { call.answer(); }
                catch (err) { console.error('Auto‑answer failed:', err); }
            }
            break;
        }

        case 'active':                           // answered / connected
            safeInvoke('incomingAnswered', call);
            wrapper.reconnectCount = 0;
            break;

        case 'done':
        case 'disconnected':                    // call ended (abnormal or connection loss)
            safeInvoke('incomingRejected', call);
            wrapper.client.currentCall = null;

            if (config.autoReconnect &&
                wrapper.reconnectCount < config.reconnectAttempts) {
                wrapper.reconnectCount++;
                setTimeout(() => _createClient(wrapper), config.reconnectDelay);
            }
            break;

        case 'hangup':                          // explicit/intentional end
        case 'destroy':                      
        case 'purge':                         
            safeInvoke('incomingRejected', call);
            wrapper.client.currentCall = null;
            // Do NOT auto-reconnect here
            break;

        default:
            // other states: new, held, hold, etc. – still forward
            safeInvoke(`state:${state}`, call);
    }
}


export function call( elementId, optionsJson) {
    const wrapper = clients.get(elementId);
    if (!wrapper?.client) return;

    // Prevent new call if one is already active
    if (wrapper.client.currentCall) {
        console.warn('TelnyxWebRtcInterop: Tried to start a new call while one is already active. Hang up first.');
        return;
    }

    const callOptions = JSON.parse(optionsJson);
    let call = null;
    try {
        call = wrapper.client.newCall(callOptions);
    } catch (err) {
        console.error('TelnyxWebRtcInterop: Exception during newCall:', err);
    }
    if (call) {
        wrapper.client.currentCall = call;
    } else {
        console.warn('TelnyxWebRtcInterop: Failed to create a new call.');
    }
}

export function answer( elementId, optionsJson) {
    const wrapper = clients.get(elementId);
    if (!wrapper?.client?.currentCall) {
        console.warn('TelnyxWebRtcInterop: Tried to answer but no currentCall is set.');
        return;
    }

    if (optionsJson) {
        const options = JSON.parse(optionsJson);
        wrapper.client.currentCall.answer(options);
    } else {
        wrapper.client.currentCall.answer();
    }
}

export function hangup( elementId, optionsJson) {
    const wrapper = clients.get(elementId);
    if (!wrapper?.client?.currentCall) {
        console.warn('TelnyxWebRtcInterop: Tried to hangup but no currentCall is set.');
        return;
    }

    if (optionsJson) {
        const options = JSON.parse(optionsJson);
        wrapper.client.currentCall.hangup(options);
    } else {
        wrapper.client.currentCall.hangup();
    }
    // Clean up currentCall after hangup
    wrapper.client.currentCall = null;
}

export function muteAudio(elementId) {
    clients.get(elementId)?.client?.currentCall?.muteAudio();
}

export function unmuteAudio(elementId) {
    clients.get(elementId)?.client?.currentCall?.unmuteAudio();
}

export function toggleAudioMute(elementId) {
    clients.get(elementId)?.client?.currentCall?.toggleAudioMute();
}

export function muteVideo(elementId) {
    clients.get(elementId)?.client?.currentCall?.muteVideo();
}

export function unmuteVideo(elementId) {
    clients.get(elementId)?.client?.currentCall?.unmuteVideo();
}

export function toggleVideoMute(elementId) {
    clients.get(elementId)?.client?.currentCall?.toggleVideoMute();
}

export function deaf(elementId) {
    clients.get(elementId)?.client?.currentCall?.deaf();
}

export function undeaf(elementId) {
    clients.get(elementId)?.client?.currentCall?.undeaf();
}

export function toggleDeaf(elementId) {
    clients.get(elementId)?.client?.currentCall?.toggleDeaf();
}

export function hold(elementId) {
    clients.get(elementId)?.client?.currentCall?.hold();
}

export function unhold(elementId) {
    clients.get(elementId)?.client?.currentCall?.unhold();
}

export function toggleHold(elementId) {
    clients.get(elementId)?.client?.currentCall?.toggleHold();
}

export function dtmf( elementId, digit) {
    clients.get(elementId)?.client?.currentCall?.dtmf(digit);
}

export function message( elementId, to, body) {
    clients.get(elementId)?.client?.currentCall?.message(to, body);
}

export function setAudioInDevice( elementId, deviceId) {
    clients.get(elementId)?.client?.currentCall?.setAudioInDevice(deviceId);
}

export function setVideoDevice( elementId, deviceId) {
    clients.get(elementId)?.client?.currentCall?.setVideoDevice(deviceId);
}

export function setAudioOutDevice( elementId, deviceId) {
    clients.get(elementId)?.client?.currentCall?.setAudioOutDevice(deviceId);
}

export function startScreenShare( elementId, optionsJson) {
    const wrapper = clients.get(elementId);
    if (!wrapper?.client?.currentCall) return;

    const options = optionsJson ? JSON.parse(optionsJson) : {};
    wrapper.client.currentCall.startScreenShare(options);
}

export function stopScreenShare(elementId) {
    clients.get(elementId)?.client?.currentCall?.stopScreenShare();
}

export function setAudioBandwidth( elementId, bps) {
    clients.get(elementId)?.client?.currentCall?.setAudioBandwidthEncodingsMaxBps(bps);
}

export function setVideoBandwidth( elementId, bps) {
    clients.get(elementId)?.client?.currentCall?.setVideoBandwidthEncodingsMaxBps(bps);
}

export function getDevices(elementId) {
    return new Promise((resolve) => {
        const client = clients.get(elementId)?.client;
        if (!client) {
            resolve(JSON.stringify([]));
            return;
        }

        client.getDevices().then(devices => {
            resolve(JSON.stringify(devices));
        }).catch(() => {
            resolve(JSON.stringify([]));
        });
    });
}

export function getVideoDevices(elementId) {
    return new Promise((resolve) => {
        const client = clients.get(elementId)?.client;
        if (!client) {
            resolve(JSON.stringify([]));
            return;
        }

        client.getVideoDevices().then(devices => {
            resolve(JSON.stringify(devices));
        }).catch(() => {
            resolve(JSON.stringify([]));
        });
    });
}

export function getAudioInDevices(elementId) {
    return new Promise((resolve) => {
        const client = clients.get(elementId)?.client;
        if (!client) {
            resolve(JSON.stringify([]));
            return;
        }

        client.getAudioInDevices().then(devices => {
            resolve(JSON.stringify(devices));
        }).catch(() => {
            resolve(JSON.stringify([]));
        });
    });
}

export function getAudioOutDevices(elementId) {
    return new Promise((resolve) => {
        const client = clients.get(elementId)?.client;
        if (!client) {
            resolve(JSON.stringify([]));
            return;
        }

        client.getAudioOutDevices().then(devices => {
            resolve(JSON.stringify(devices));
        }).catch(() => {
            resolve(JSON.stringify([]));
        });
    });
}

export function checkPermissions( elementId, audio = true, video = true) {
    return new Promise((resolve) => {
        const client = clients.get(elementId)?.client;
        if (!client) {
            resolve(false);
            return;
        }

        client.checkPermissions(audio, video).then(result => {
            resolve(result);
        }).catch(() => {
            resolve(false);
        });
    });
}

export function setAudioSettings( elementId, settingsJson) {
    return new Promise((resolve) => {
        const client = clients.get(elementId)?.client;
        if (!client) {
            resolve(false);
            return;
        }

        const settings = JSON.parse(settingsJson);
        client.setAudioSettings(settings).then(() => {
            resolve(true);
        }).catch(() => {
            resolve(false);
        });
    });
}

export function setVideoSettings( elementId, settingsJson) {
    return new Promise((resolve) => {
        const client = clients.get(elementId)?.client;
        if (!client) {
            resolve(false);
            return;
        }

        const settings = JSON.parse(settingsJson);
        client.setVideoSettings(settings).then(() => {
            resolve(true);
        }).catch(() => {
            resolve(false);
        });
    });
}

export function enableMicrophone(elementId) {
    clients.get(elementId)?.client?.enableMicrophone();
}

export function disableMicrophone(elementId) {
    clients.get(elementId)?.client?.disableMicrophone();
}

export function enableWebcam(elementId) {
    clients.get(elementId)?.client?.enableWebcam();
}

export function disableWebcam(elementId) {
    clients.get(elementId)?.client?.disableWebcam();
}

export function toggleAudio( elementId, enabled) {
    clients.get(elementId)?.client?.localStream?.getAudioTracks()?.forEach(t => t.enabled = enabled);
}

export function toggleVideo( elementId, enabled) {
    clients.get(elementId)?.client?.localStream?.getVideoTracks()?.forEach(t => t.enabled = enabled);
}

export function setAudioVolume( elementId, volume) {
    const el = document.getElementById(elementId);
    if (el && el.tagName === "AUDIO") {
        el.volume = Math.max(0, Math.min(1, volume));
    }
}

export function disconnect(elementId) {
    const wrapper = clients.get(elementId);
    if (!wrapper?.client) return;

    wrapper.client.off('telnyx.ready');
    wrapper.client.off('telnyx.notification');
    wrapper.client.off('telnyx.error');
    wrapper.client.off('telnyx.socket.close');

    // Clean up currentCall on disconnect
    if (wrapper.client.currentCall) {
        wrapper.client.currentCall = null;
    }

    wrapper.client.disconnect();
}

export function reconnect(elementId) {
    const wrapper = clients.get(elementId);
    if (!wrapper) return;

    disconnect(elementId);
    wrapper.reconnectCount = 0;
    // Clean up observer before re-creating client
    _disconnectObserver(elementId);
    _createClient(wrapper);
}

export function connect(elementId) {
    const wrapper = clients.get(elementId);
    if (wrapper?.client) {
        wrapper.client.connect();
    }
}

export function unmount(elementId) {
    const wrapper = clients.get(elementId);
    if (!wrapper) return;

    try {
        disconnect(elementId);
    } catch (e) {
        console.warn(`Error during disconnect of client "${elementId}":`, e);
    }

    clients.delete(elementId);
    _disconnectObserver(elementId);
}

export function createObserver(elementId) {
    const el = document.getElementById(elementId);
    if (!el || !el.parentNode) {
        console.warn(`Element "${elementId}" not found for mutation observer.`);
        return;
    }

    const observer = new MutationObserver((mutations) => {
        const removed = mutations.some(m =>
            Array.from(m.removedNodes).includes(el)
        );

        if (removed) {
            unmount(elementId);
        }
    });

    observer.observe(el.parentNode, { childList: true });
    observers.set( elementId, observer);
}

function _disconnectObserver(elementId) {
    const observer = observers.get(elementId);
    if (observer) {
        observer.disconnect();
        observers.delete(elementId);
    }
}

// Conference control methods
export function listVideoLayouts(elementId) {
    clients.get(elementId)?.client?.currentCall?.listVideoLayouts();
}

export function setVideoLayout( elementId, layout, canvas) {
    clients.get(elementId)?.client?.currentCall?.setVideoLayout(layout, canvas);
}

export function playMedia( elementId, source) {
    clients.get(elementId)?.client?.currentCall?.playMedia(source);
}

export function stopMedia(elementId) {
    clients.get(elementId)?.client?.currentCall?.stopMedia();
}

export function startRecord( elementId, filename) {
    clients.get(elementId)?.client?.currentCall?.startRecord(filename);
}

export function stopRecord(elementId) {
    clients.get(elementId)?.client?.currentCall?.stopRecord();
}

export function sendChatMessage( elementId, message, type) {
    clients.get(elementId)?.client?.currentCall?.sendChatMessage(message, type);
}

export function snapshot( elementId, filename) {
    clients.get(elementId)?.client?.currentCall?.snapshot(filename);
}

export function muteMic( elementId, participantId) {
    clients.get(elementId)?.client?.currentCall?.muteMic(participantId);
}

export function muteVideoParticipant( elementId, participantId) {
    clients.get(elementId)?.client?.currentCall?.muteVideo(participantId);
}

export function kick( elementId, participantId) {
    clients.get(elementId)?.client?.currentCall?.kick(participantId);
}

export function volumeUp( elementId, participantId) {
    clients.get(elementId)?.client?.currentCall?.volumeUp(participantId);
}

export function volumeDown( elementId, participantId) {
    clients.get(elementId)?.client?.currentCall?.volumeDown(participantId);
}

export function getCallStats(elementId) {
    return new Promise((resolve) => {
        const call = clients.get(elementId)?.client?.currentCall;
        if (!call) {
            resolve(null);
            return;
        }

        const stats = [];
        call.getStats((stat) => {
            stats.push(stat);
            resolve(JSON.stringify(stats));
        });
    });
}
