class PocketMessage {
    content;
    url;
    type; //1-文本，2-图片，3-语音，4-视频
    color;
    add(msg, color) {
        this.content = msg;
        this.type = 1;
        this.color = color;
        return this;
    }
    addImg(content, url, color) {
        this.content = content;
        this.url = url;
        this.type = 2;
        this.color = color;
        return this;
    }
    addVoice(content, url, color) {
        this.content = content;
        this.url = url;
        this.type = 3;
        this.color = color;
        return this;
    }
    addVideo(content, url, color) {
        this.content = content;
        this.url = url;
        this.type = 4;
        this.color = color;
        return this;
    }
}
export default PocketMessage;
