function objectSome(obj, callback) {
    let result = false;
    for (const key in obj) {
        if (obj.hasOwnProperty(key)) {
            result = callback(key, obj[key]);

            if (result) break;
        }
    }

    return result;
}

function globalInject() {
    class NIMWebsocket extends WebSocket { }
    NIMWebsocket.prototype.ORIGINAL_send = NIMWebsocket.prototype.send;
    NIMWebsocket.prototype.send = function () {
        if (/3:::/.test(arguments[0])) {
            const message = arguments[0].replace(/3:::/, '');
            let data = null;

            try {
                data = JSON.parse(message);
            } catch { /* noop */ }

            if (data && data?.SER === 1 && data?.SID === 2 && data?.Q?.length) {
                for (const Q of data.Q) {
                    if (/Property/i.test(Q.t) && Q.v && objectSome(Q.v, (k, v) => /Microsoft Edge/i.test(v))) {
                        Q.v['3'] = 2;
                        Q.v['42'] = 'PocketFans201807/24020203';
                        arguments[0] = `3:::${JSON.stringify(data)}`;
                        break;
                    }
                }
            }
        }

        return this.ORIGINAL_send.apply(this, arguments);
    };
    window.NIMWebsocket = NIMWebsocket;

    class QchatWebsocket extends WebSocket { }
    QchatWebsocket.prototype.ORIGINAL_send = QchatWebsocket.prototype.send;

    QchatWebsocket.prototype.send = function () {
        if (/3:::/.test(arguments[0])) {
            const message = arguments[0].replace(/3:::/, '');
            let data = null;

            try {
                data = JSON.parse(message);
            } catch { /* noop */ }

            if (data && data?.SER === 1 && data?.SID === 24 && data?.Q?.length) {
                for (const Q of data.Q) {
                    if (/Property/i.test(Q.t) && Q.v && objectSome(Q.v, (k, v) => /Microsoft Edge/i.test(v))) {
                        Q.v['6'] = 2;
                        arguments[0] = `3:::${JSON.stringify(data)}`;
                        break;
                    }
                }
            }
        }

        return this.ORIGINAL_send.apply(this, arguments);
    };

    window.QchatWebsocket = QchatWebsocket;

    class LiveWebsocket extends WebSocket { }
    LiveWebsocket.prototype.ORIGINAL_send = LiveWebsocket.prototype.send;

    LiveWebsocket.prototype.send = function () {
        if (/3:::/.test(arguments[0])) {
            const message = arguments[0].replace(/3:::/, '');
            let data = null;

            try {
                data = JSON.parse(message);
            } catch { /* noop */ }

            if (data && data?.SER === 1 && data?.SID === 24 && data?.Q?.length) {
                for (const Q of data.Q) {
                    if (/Property/i.test(Q.t) && Q.v && objectSome(Q.v, (k, v) => /BROWSER/i.test(v))) {
                        Q.v['3'] = 2;
                        Q.v['42'] = 'PocketFans201807/24020203';
                        arguments[0] = `3:::${JSON.stringify(data)}`;
                        break;
                    }
                }
            }
        }

        return this.ORIGINAL_send.apply(this, arguments);
    };

    window.LiveWebsocket = LiveWebsocket;
}
globalInject();
//使用方法
//找到node_modules/nim-web-sdk-ng/dist/NIM_BROWSER_SDK.js和QCHAT_BROWSER_SDK.js
//替换NIM_BROWSER_SDK.js中的window.WebSocket为window.NIMWebsocket
//替换QCHAT_BROWSER_SDK.js中的window.WebSocket为window.QchatWebsocket