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

var orgSend = WebSocket.prototype.send;
WebSocket.prototype.send = function () {
    if (/3:::/.test(arguments[0])) {
        const message = arguments[0].replace(/3:::/, '');
        let data = null;

        try {
            data = JSON.parse(message);
        } catch { /* noop */ }

        //NIM_BROWSER_SDK.js拦截处理
        if (data?.SID === 2 && data?.Q?.length) {
            for (const Q of data.Q) {
                if (/Property/i.test(Q.t) && Q.v && objectSome(Q.v, (k, v) => /0.17.2/i.test(v))) {
                    Q.v['3'] = 2;
                    Q.v['42'] = 'PocketFans201807/24020203';
                    arguments[0] = `3:::${JSON.stringify(data)}`;
                    break;
                }
            }
        }

        //NIM_BROWSER_SDK.js拦截处理
        if (data?.SID === 24 && data?.Q?.length) {
            for (const Q of data.Q) {
                if (/Property/i.test(Q.t) && Q.v && objectSome(Q.v, (k, v) => /0.17.2/i.test(v))) {
                    Q.v['6'] = 2;
                    arguments[0] = `3:::${JSON.stringify(data)}`;
                    break;
                }
            }
        }

        //直播websocket拦截处理
        if (data?.SID === 13 && data?.Q?.length) {
            for (const Q of data.Q) {
                if (/Property/i.test(Q.t) && Q.v) {
                    Q.v['3'] = 2;
                    Q.v['6'] = 2;
                    Q.v['42'] = 'PocketFans201807/24020203';
                    arguments[0] = `3:::${JSON.stringify(data)}`;
                }
            }
        }
    }
    return orgSend.apply(this, arguments);
};