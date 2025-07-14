export class TelnyxWebRtcInterop {
    constructor() {
        this._clients = new Map();
        this._observers = new Map();

        // Encapsulated unload handler: ensures cleanup on page unload, only added once
        if (typeof window !== 'undefined' && !window._telnyxUnloadHandlerAdded) {
            window.addEventListener('unload', () => {
                for (const elementId of this._clients.keys()) {
                    this.unmount(elementId);
                }
            });
            window._telnyxUnloadHandlerAdded = true;
        }
    }

    create(elementId, optionsJson, dotNetCallback) {
        if (this._clients.has(elementId)) {
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

        this._clients.set(elementId, wrapper);
        this._createClient(wrapper);

        dotNetCallback.invokeMethodAsync("HandleTelnyxEvent", "initialized", "");
    }

    _createClient(wrapper) {
        const { config } = wrapper;

        wrapper.client = new TelnyxWebRTC.TelnyxRTC(config.initOptions);

        wrapper.client.remoteElement = config.remoteElement;
        wrapper.client.localElement = config.localElement;

        if (config.audio)
            wrapper.client.enableMicrophone();
        else
            wrapper.client.disableMicrophone();

        if (config.video)
            wrapper.client.enableWebcam();
        else
            wrapper.client.disableWebcam();

        this._bindEvents(wrapper);

        wrapper.client.connect();
    }

    _bindEvents(wrapper) {

        const { client, dotNetCallback } = wrapper;

        const safeStringify = (obj, indent = 2) => {
            const seen = new WeakSet();
            return JSON.stringify(obj, (key, value) => {
                if (typeof value === "object" && value !== null) {
                    if (seen.has(value)) {
                        return "[Circular]";
                    }
                    seen.add(value);
                }
                return value;
            }, indent);
        };

        const safeInvoke = (event, args) => {
            dotNetCallback.invokeMethodAsync("HandleTelnyxEvent", event, safeStringify(args));
        };

        const forward = (eventName) => {
            client.on(`telnyx.${eventName}`, (arg) => {
                safeInvoke(eventName, arg);
            });
        };

        [
            "ready",
            "error",
            "notification",
            "socket.open",
            "socket.close",
            "socket.error",
            "reconnecting",
            "reconnected",
            "disconnected"
        ].forEach(forward);

        client.on('telnyx.notification', (notification) => {
            safeInvoke('notification', notification);
            if (notification.type === 'callUpdate' && notification.call?.state === 'ready') {
                wrapper.reconnectCount = 0;
            } else if (notification.type === 'callUpdate' && notification.call?.state === 'disconnected') {
                if (wrapper.config.autoReconnect && wrapper.reconnectCount < wrapper.config.reconnectAttempts) {
                    wrapper.reconnectCount++;
                    setTimeout(() => this._createClient(wrapper), wrapper.config.reconnectDelay);
                }
            }
        });
    }

    call(elementId, optionsJson) {
        const wrapper = this._clients.get(elementId);
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

    answer(elementId, optionsJson) {
        const wrapper = this._clients.get(elementId);
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

    hangup(elementId, optionsJson) {
        const wrapper = this._clients.get(elementId);
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

    muteAudio(elementId) {
        this._clients.get(elementId)?.client?.currentCall?.muteAudio();
    }

    unmuteAudio(elementId) {
        this._clients.get(elementId)?.client?.currentCall?.unmuteAudio();
    }

    toggleAudioMute(elementId) {
        this._clients.get(elementId)?.client?.currentCall?.toggleAudioMute();
    }

    muteVideo(elementId) {
        this._clients.get(elementId)?.client?.currentCall?.muteVideo();
    }

    unmuteVideo(elementId) {
        this._clients.get(elementId)?.client?.currentCall?.unmuteVideo();
    }

    toggleVideoMute(elementId) {
        this._clients.get(elementId)?.client?.currentCall?.toggleVideoMute();
    }

    deaf(elementId) {
        this._clients.get(elementId)?.client?.currentCall?.deaf();
    }

    undeaf(elementId) {
        this._clients.get(elementId)?.client?.currentCall?.undeaf();
    }

    toggleDeaf(elementId) {
        this._clients.get(elementId)?.client?.currentCall?.toggleDeaf();
    }

    hold(elementId) {
        this._clients.get(elementId)?.client?.currentCall?.hold();
    }

    unhold(elementId) {
        this._clients.get(elementId)?.client?.currentCall?.unhold();
    }

    toggleHold(elementId) {
        this._clients.get(elementId)?.client?.currentCall?.toggleHold();
    }

    dtmf(elementId, digit) {
        this._clients.get(elementId)?.client?.currentCall?.dtmf(digit);
    }

    message(elementId, to, body) {
        this._clients.get(elementId)?.client?.currentCall?.message(to, body);
    }

    setAudioInDevice(elementId, deviceId) {
        this._clients.get(elementId)?.client?.currentCall?.setAudioInDevice(deviceId);
    }

    setVideoDevice(elementId, deviceId) {
        this._clients.get(elementId)?.client?.currentCall?.setVideoDevice(deviceId);
    }

    setAudioOutDevice(elementId, deviceId) {
        this._clients.get(elementId)?.client?.currentCall?.setAudioOutDevice(deviceId);
    }

    startScreenShare(elementId, optionsJson) {
        const wrapper = this._clients.get(elementId);
        if (!wrapper?.client?.currentCall) return;

        const options = optionsJson ? JSON.parse(optionsJson) : {};
        wrapper.client.currentCall.startScreenShare(options);
    }

    stopScreenShare(elementId) {
        this._clients.get(elementId)?.client?.currentCall?.stopScreenShare();
    }

    setAudioBandwidth(elementId, bps) {
        this._clients.get(elementId)?.client?.currentCall?.setAudioBandwidthEncodingsMaxBps(bps);
    }

    setVideoBandwidth(elementId, bps) {
        this._clients.get(elementId)?.client?.currentCall?.setVideoBandwidthEncodingsMaxBps(bps);
    }

    getDevices(elementId) {
        return new Promise((resolve) => {
            const client = this._clients.get(elementId)?.client;
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

    getVideoDevices(elementId) {
        return new Promise((resolve) => {
            const client = this._clients.get(elementId)?.client;
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

    getAudioInDevices(elementId) {
        return new Promise((resolve) => {
            const client = this._clients.get(elementId)?.client;
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

    getAudioOutDevices(elementId) {
        return new Promise((resolve) => {
            const client = this._clients.get(elementId)?.client;
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

    checkPermissions(elementId, audio = true, video = true) {
        return new Promise((resolve) => {
            const client = this._clients.get(elementId)?.client;
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

    setAudioSettings(elementId, settingsJson) {
        return new Promise((resolve) => {
            const client = this._clients.get(elementId)?.client;
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

    setVideoSettings(elementId, settingsJson) {
        return new Promise((resolve) => {
            const client = this._clients.get(elementId)?.client;
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

    enableMicrophone(elementId) {
        this._clients.get(elementId)?.client?.enableMicrophone();
    }

    disableMicrophone(elementId) {
        this._clients.get(elementId)?.client?.disableMicrophone();
    }

    enableWebcam(elementId) {
        this._clients.get(elementId)?.client?.enableWebcam();
    }

    disableWebcam(elementId) {
        this._clients.get(elementId)?.client?.disableWebcam();
    }

    toggleAudio(elementId, enabled) {
        this._clients.get(elementId)?.client?.localStream?.getAudioTracks()?.forEach(t => t.enabled = enabled);
    }

    toggleVideo(elementId, enabled) {
        this._clients.get(elementId)?.client?.localStream?.getVideoTracks()?.forEach(t => t.enabled = enabled);
    }

    disconnect(elementId) {
        const wrapper = this._clients.get(elementId);
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

    reconnect(elementId) {
        const wrapper = this._clients.get(elementId);
        if (!wrapper) return;

        this.disconnect(elementId);
        wrapper.reconnectCount = 0;
        // Clean up observer before re-creating client
        this._disconnectObserver(elementId);
        this._createClient(wrapper);
    }

    unmount(elementId) {
        const wrapper = this._clients.get(elementId);
        if (!wrapper) return;

        try {
            this.disconnect(elementId);
        } catch (e) {
            console.warn(`Error during disconnect of client "${elementId}":`, e);
        }

        this._clients.delete(elementId);
        this._disconnectObserver(elementId);
    }

    createObserver(elementId) {
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
                this.unmount(elementId);
            }
        });

        observer.observe(el.parentNode, { childList: true });
        this._observers.set(elementId, observer);
    }

    _disconnectObserver(elementId) {
        const observer = this._observers.get(elementId);
        if (observer) {
            observer.disconnect();
            this._observers.delete(elementId);
        }
    }

    // Conference control methods
    listVideoLayouts(elementId) {
        this._clients.get(elementId)?.client?.currentCall?.listVideoLayouts();
    }

    setVideoLayout(elementId, layout, canvas) {
        this._clients.get(elementId)?.client?.currentCall?.setVideoLayout(layout, canvas);
    }

    playMedia(elementId, source) {
        this._clients.get(elementId)?.client?.currentCall?.playMedia(source);
    }

    stopMedia(elementId) {
        this._clients.get(elementId)?.client?.currentCall?.stopMedia();
    }

    startRecord(elementId, filename) {
        this._clients.get(elementId)?.client?.currentCall?.startRecord(filename);
    }

    stopRecord(elementId) {
        this._clients.get(elementId)?.client?.currentCall?.stopRecord();
    }

    sendChatMessage(elementId, message, type) {
        this._clients.get(elementId)?.client?.currentCall?.sendChatMessage(message, type);
    }

    snapshot(elementId, filename) {
        this._clients.get(elementId)?.client?.currentCall?.snapshot(filename);
    }

    muteMic(elementId, participantId) {
        this._clients.get(elementId)?.client?.currentCall?.muteMic(participantId);
    }

    muteVideoParticipant(elementId, participantId) {
        this._clients.get(elementId)?.client?.currentCall?.muteVideo(participantId);
    }

    kick(elementId, participantId) {
        this._clients.get(elementId)?.client?.currentCall?.kick(participantId);
    }

    volumeUp(elementId, participantId) {
        this._clients.get(elementId)?.client?.currentCall?.volumeUp(participantId);
    }

    volumeDown(elementId, participantId) {
        this._clients.get(elementId)?.client?.currentCall?.volumeDown(participantId);
    }

    getCallStats(elementId) {
        return new Promise((resolve) => {
            const call = this._clients.get(elementId)?.client?.currentCall;
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
}

window.TelnyxWebRtcInterop = new TelnyxWebRtcInterop();