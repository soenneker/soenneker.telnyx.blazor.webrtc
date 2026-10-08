import { readFile } from 'node:fs/promises';
import assert from 'node:assert/strict';
import test from 'node:test';

test('a second incoming call cannot steal hangup or UI ownership', async () => {
    const handlers = new Map();
    globalThis.TelnyxWebRTC = { TelnyxRTC: class {
        on(name, handler) { handlers.set(name, handler); }
        async connect() {}
    } };
    const source = await readFile(new URL('../src/Soenneker.Telnyx.Blazor.WebRtc/wwwroot/js/telnyxwebrtcinterop.js', import.meta.url), 'utf8');
    const interop = await import(`data:text/javascript;base64,${Buffer.from(source).toString('base64')}`);
    const ended = [];
    const notifications = [];
    await interop.create('phone-test', JSON.stringify({initOptions:{}}), {
        invokeMethodAsync: async (...args) => notifications.push(args)
    });
    const first = { id:'first', state:'active', hangup:()=>ended.push('first') };
    const second = { id:'second', state:'ringing', hangup:()=>ended.push('second') };
    await handlers.get('telnyx.notification')({type:'callUpdate',call:first});
    const before = notifications.length;
    await handlers.get('telnyx.notification')({type:'callUpdate',call:second});
    assert.equal(notifications.length, before);
    assert.deepEqual(ended, ['second']);
    await interop.hangup('phone-test');
    assert.deepEqual(ended, ['second','first']);
});
