import NIM_SDK from '@yxim/nim-web-sdk/dist/SDK/NIM_Web_SDK.js';
/* 创建网易云信sdk的socket连接 */
class NimChatroomSocket {
    liveId;
    nimChatroomSocket;
    onMessage;
    constructor(arg) {
        this.liveId = arg.liveId; // 房间id
        this.onMessage = arg.onMessage;
    }
    // 初始化
    init(appkey) {
        this.nimChatroomSocket = NIM_SDK.Chatroom.getInstance({
            appKey: atob(appkey),
            chatroomId: this.liveId,
            chatroomAddresses: ['chatweblink01.netease.im:443'],
            onconnect: this.onConnet,
            onmsgs: this.handleRoomSocketMessage,
            onerror: this.handleRoomSocketError,
            ondisconnect: this.handleRoomSocketDisconnect,
            isAnonymous: true,
            chatroomNick: this.uuid(),
            chatroomAvatar: '',
            db: false,
            dbLog: false
        });
    }
    // 事件监听
    handleRoomSocketMessage = (event) => {
        this.onMessage(this, event);
    };
    // 事件监听
    onConnet = (event) => {
        let msg = `进入小偶像的直播间成功。`;
        console.log(msg);
    };
    // 进入房间失败
    handleRoomSocketError = (err, event) => {
        console.log('发生错误', err, event);
    };
    // 断开连接
    handleRoomSocketDisconnect = (err) => {
        console.log('连接断开', err);
    };
    // 断开连接
    disconnect() {
        this.nimChatroomSocket?.disconnect?.({ done() { } });
        this.nimChatroomSocket = undefined;
    }
    uuid() {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            var r = Math.random() * 16 | 0;
            var v = c === 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
    }
}
export default NimChatroomSocket;
