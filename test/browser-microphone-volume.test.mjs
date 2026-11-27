import assert from 'node:assert/strict';
import { readFile } from 'node:fs/promises';
import { setImmediate } from 'node:timers/promises';
import test from 'node:test';

const source = await readFile(new URL('../src/Soenneker.Telnyx.Blazor.WebRtc/wwwroot/js/interop/telnyxwebrtcinterop.min.js', import.meta.url), 'utf8');
const interop = await import('data:text/javascript;base64,' + Buffer.from(source).toString('base64'));

for (const direction of ['inbound', 'outbound']) {
    test(direction + ' calls apply microphone gain before and during a call', async t => {
        const contexts = [];
        const handlers = new Map();
        const mic = () => ({ kind: 'audio', enabled: true, stop() { this.stopped = true; } });
        const sender = { track: mic(), async replaceTrack(track) { this.track = track; } };
        const call = {
            id: 'call', direction, state: 'ringing',
            answer() { this.state = 'active'; this.peer = { instance: { getSenders: () => [sender] } }; notify(); },
            setAudioInDevice() { sender.track = mic(); },
            hangup() { this.state = 'hangup'; notify(); }
        };
        const notify = () => handlers.get('telnyx.notification')({ type: 'callUpdate', call });
        t.after(() => { delete globalThis.AudioContext; delete globalThis.MediaStream; delete globalThis.TelnyxWebRTC; });
        globalThis.MediaStream = class { constructor(tracks) { this.tracks = tracks; } };
        globalThis.AudioContext = class {
            constructor() { contexts.push(this); }
            createMediaStreamSource(stream) {
                this.input = stream.tracks[0];
                return { connect: node => node };
            }
            createGain() { this.gain = {}; return { gain: this.gain, connect() {} }; }
            createMediaStreamDestination() {
                this.output = mic();
                return { stream: { getAudioTracks: () => [this.output] } };
            }
            close() { this.state = 'closed'; }
        };
        globalThis.TelnyxWebRTC = { TelnyxRTC: class {
            on(name, handler) { handlers.set(name, handler); }
            off(name) { handlers.delete(name); }
            connect() { handlers.get('telnyx.ready')(); }
            newCall() { call.answer(); return call; }
            disconnect() {}
        } };
        await interop.create(direction, '{"initOptions":{}}', { invokeMethodAsync() {} });
        t.after(() => interop.unmount(direction));
        for (const invalid of [-1, 1.1, NaN]) assert.throws(() => interop.setMicrophoneVolume(direction, invalid), RangeError);
        await interop.setMicrophoneVolume(direction, 0.5);
        const original = sender.track;
        if (direction === 'inbound') {
            notify();
            await setImmediate();
            assert.equal(contexts.length, 0);
            await interop.answer(direction);
        } else {
            interop.call(direction, '{}');
        }
        await setImmediate();
        assert.equal(contexts.length, 1);
        assert.equal(contexts[0].input, original);
        assert.equal(contexts[0].output, sender.track);
        assert.equal(contexts[0].gain.value, 0.5);
        await interop.setMicrophoneVolume(direction, 0);
        assert.equal(contexts[0].gain.value, 0);
        await interop.setAudioInDevice(direction, 'another-microphone');
        await setImmediate();
        assert.equal(contexts[0].state, 'closed');
        assert.equal(contexts[0].output.stopped, true);
        assert.equal(contexts[1].gain.value, 0);
        assert.equal(contexts[1].output, sender.track);
        await interop.setMicrophoneVolume(direction, 1);
        assert.equal(sender.track, contexts[1].input);
        assert.equal(sender.track.stopped, undefined);
        await interop.setMicrophoneVolume(direction, 0.75);
        await interop.hangup(direction);
        await setImmediate();
        assert.equal(contexts[2].state, 'closed');
        assert.equal(contexts[2].output.stopped, true);
    });
}
