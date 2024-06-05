<template>
    <el-scrollbar style="height: calc(100vh - 30px)">
        <el-row>
            <el-col :span="12">
                <el-card shadow="hover" header="StarBot" class="ch250">
                    <el-row>
                        <el-col :span="6">
                            <img title="点击关闭" v-if="botStart" @click="closeBot" src="/src/asset/rocket.gif"
                                style="width: 100px;height: 100px;cursor: pointer;margin-left: -17px;" />
                            <img title="点击启动" v-else @click="startBot" src="/src/asset/rocket.png"
                                style="width: 100px;height: 100px;cursor: pointer;margin-left: -17px;" />
                            <div :style="{ color: (botStart ? 'green' : startMsg.includes('启动中') ? 'red' : 'blue') }">
                                <span>{{ startMsg }}</span>
                            </div>
                        </el-col>
                        <el-col :span="18">
                            <div class="mt10">
                                <span style="color: gray;font-size: small">{{ currentTimeType
                                    }}！生活变的再糟糕，也不妨碍我变得更好！</span>
                            </div>
                            <div class="mt10">
                                <label>当前时间：</label><span style="color: gray;">{{ currentTime }}</span>
                            </div>
                            <div class="mt10">
                                <label>上次启动：</label><span style="color: gray;">{{ lastStart }}</span>
                            </div>
                            <div class="mt10">
                                <label>运行时长：</label><span style="color: gray;">
                                    {{ botStart ? '已运行' + runTime : '未启动' }}
                                </span>
                            </div>
                        </el-col>
                    </el-row>
                </el-card>
            </el-col>
            <el-col :span="1"></el-col>
            <!-- 消息通知 -->
            <el-col :span="11">
                <el-card shadow="hover" class="ch250">
                    <template #header>
                        <el-row>
                            <el-col><span title="每分钟更新一次">错误日志</span>
                                <el-icon :size="18" title="每分钟更新一次">
                                    <Refresh class="et3"
                                        @click="async () => errLogs = (await getLogs()).data.reverse()" />
                                </el-icon>
                            </el-col>
                        </el-row>
                    </template>
                    <el-scrollbar height="180px" style="margin-top:-10px;margin-left: -20px;">
                        <ul style="margin-top:-3px">
                            <li v-for="item in errLogs" style="cursor: pointer;">
                                <span @click="viewLog(item.content)" title="点击查看">
                                    {{ item.content.substring(0, 20) }}--{{ item.time }}
                                </span>
                            </li>
                        </ul>
                    </el-scrollbar>
                </el-card>
            </el-col>
        </el-row>
        <el-row style="margin-top: 20px;">
            <el-col :span="24">
                <el-card shadow="hover" header="快速预览" class="chauto">
                    <el-row :gutter="15">
                        <el-col :span="8">
                            <el-card shadow="hover" class="ch320">
                                <template #header><span title="每分钟更新一次">数据总览</span>
                                </template>
                                <div>
                                    <label>日志</label>
                                    <div>
                                        <span class="fs14">全部:{{ info.log.total }}</span>
                                        <el-divider direction="vertical" />
                                        <span class="fs14" v-if="enable.kd">偶像:{{ info.log.idol }}</span>
                                        <span class="fs14" v-else>系统:{{ info.log.system }}</span>
                                        <el-divider direction="vertical" />
                                        <span class="fs14">其他:{{ info.log.other }}</span>
                                    </div>
                                </div>
                                <el-divider />
                                <div>
                                    <label>图片信息</label>
                                    <div>
                                        <span class="fs14">全部:{{ info.pic.total }}</span>
                                        <el-divider direction="vertical" />
                                        <span class="fs14">今日:{{ info.pic.today }}</span>
                                        <el-divider direction="vertical" />
                                        <span class="fs14">更早:{{ info.pic.old }}</span>
                                    </div>
                                </div>
                                <el-divider />
                                <div v-if="enable.bot">
                                    <label>插件信息</label>
                                    <div>
                                        <span class="fs14">全部:{{ info.plugin.total }}</span>
                                        <el-divider direction="vertical" />
                                        <span class="fs14">启用:{{ info.plugin.using }}</span>
                                        <el-divider direction="vertical" />
                                        <span class="fs14">禁用:{{ info.plugin.unusing }}</span>
                                    </div>
                                </div>
                            </el-card>
                        </el-col>
                        <el-col :span="8" v-if="enable.kd">
                            <el-card shadow="hover" class="ch320">
                                <template #header><span title="实时更新">口袋消息</span>
                                </template>
                                <el-scrollbar height="250px" style="margin-top:-10px">
                                    <ul style="margin-top:-3px;margin-left: -20px;">
                                        <li v-for="item in kdLogs">
                                            <span>{{ item.name }}</span>
                                            <span style="color: gray;font-size: 14px;">{{ item.time }}</span>
                                            <div v-if="item.content" @click="viewLog(item.content!, '口袋消息')"
                                                title="点击查看" style="color:skyblue;cursor: pointer;">
                                                {{ item.content?.substring(0, 20) }}
                                                {{ (item.content?.length || 0) > 20 ? '...' : '' }}
                                            </div>
                                            <div v-if="item.type == 'pic'">
                                                <el-image :src="item.url" :preview-src-list="[item.url]"
                                                    style="width: 120px;height: auto;" />
                                            </div>
                                            <div v-if="item.type == 'link'"><a target="_blank"
                                                    :href="item.url">链接消息，点击查看！</a>
                                            </div>
                                            <div v-if="item.type == 'audio' || item.type == 'video'">
                                                <a :href="item.url" target="_blank">点击播放视频或音频</a>
                                            </div>
                                        </li>
                                    </ul>
                                </el-scrollbar>
                            </el-card>
                        </el-col>
                        <el-col :span="8" v-show="carousel == 'help'">
                            <el-card shadow="hover" class="ch320">
                                <template #header>帮助反馈
                                    <el-icon :size="16">
                                        <Switch class="et2" circle @click="() => carousel = 'log'"></Switch>
                                    </el-icon>
                                </template>
                                <div>
                                    <span>1、QQ机器人部署(2选1)：</span>
                                    <div class="mt5">
                                        <a href="https://llonebot.github.io/zh-CN/" target="_blank">LLOneBot</a>
                                        <span style="font-size: 14px;color: gray;">：配置简单，可视化配置，占用电脑资源较高(>=300m内存)</span>
                                    </div>
                                    <div class="mt5">
                                        <a href="https://napneko.github.io/zh-CN/" target="_blank">NapCatBot</a>
                                        <span style="font-size: 14px;color: gray;">：非可视化配置，配置过程相对LLOneBot复杂，占用资源低(
                                            <=100m内存) </span>
                                    </div>
                                </div>
                                <div class="mt10">
                                    <span>2、配置项说明：</span>
                                    <a href="https://gitee.com/jaffoo/ParkerBot#配置教程" target="_blank">参照此文档</a>
                                    <span style="font-size: 14px;color: gray;"></span>
                                </div>
                                <div class="mt10">
                                    <span>3、联系作者QQ：</span>
                                    <span style="color: blue;">1615842006</span>
                                </div>
                            </el-card>
                        </el-col>
                        <el-col :span="8" v-show="carousel == 'log'">
                            <el-card shadow="hover" class="ch320">
                                <template #header>本地日志
                                    <el-icon :size="16">
                                        <Switch class="et2" circle @click="() => carousel = 'help'"></Switch>
                                    </el-icon>
                                </template>
                                <el-scrollbar height="250px" style="margin-top:-10px">
                                    <ul style="margin-top:-3px;margin-left: -20px;">
                                        <li v-for="item in infoLogs" style="cursor: pointer;">
                                            <span @click="viewLog(item.content ?? '', '日志')" title="点击查看">
                                                {{ item.content?.substring(0, 20) }}--{{ item.time }}
                                            </span>
                                        </li>
                                    </ul>
                                </el-scrollbar>
                            </el-card>
                        </el-col>
                    </el-row>
                </el-card>
            </el-col>
        </el-row>
    </el-scrollbar>
</template>
<script setup lang="ts" name="index">
import { type EnableModule, type Config, logApi, type logI } from "@/class/model";
import { ref, type PropType, onMounted } from "vue";
import { ElMessageBox, dayjs } from 'element-plus'
import { getLogs, getConfig, startBot as startBotAPI, postMsg, getFun, getCache, startAliYunPan } from "@/api";
import NimChatroomSocket from "@/class/live";
import QChatSDK from "nim-web-sdk-ng/dist/QCHAT_BROWSER_SDK";
import NIMSDK from "nim-web-sdk-ng/dist/NIM_BROWSER_SDK";
import type { SubscribeAllChannelResult } from "nim-web-sdk-ng/dist/QCHAT_BROWSER_SDK/QChatServerServiceInterface";
import type { LiveRoomMessage } from "@/class/messageType";
import { Switch, Refresh } from '@element-plus/icons-vue'

const props = defineProps({
    enable: {
        type: Object as PropType<EnableModule>,
        default: {
            bot: false,
            qq: false,
            wb: false,
            bz: false,
            kd: false,
            xhs: false,
            dy: false,
            bd: false,
        },
    },
});

const config = ref<Config>()
const startMsg = ref("点击启动")
const currentTime = ref('');
const currentTimeType = ref();
const errLogs = ref();
const botStart = ref(false);
const lastStart = ref("无记录");
const runTime = ref('0小时0分钟');
const kdLogs = ref<logI[]>()
const infoLogs = ref<logI[]>()
const info = ref({
    pic: {
        total: 0,
        today: 0,
        old: 0
    },
    plugin: {
        total: 0,
        using: 0,
        unusing: 0
    },
    log: {
        total: 0,
        idol: 0,
        other: 0,
        system: 0,
    }
})

const nim = ref<NIMSDK>();
const qChat = ref<QChatSDK>();
const liveNim = ref<NimChatroomSocket>();

const oneMin = ref<NodeJS.Timeout>();
const oneSec = ref<NodeJS.Timeout>()

const carousel = ref('help')

const startBot = async () => {
    const allFalse = Object.values(props.enable).every(value => value === false);
    if (allFalse) {
        ElMessageBox.alert('没有启用任何功能')
        return;
    }
    startMsg.value = "启动中！"
    if (!oneMin.value)
        oneMinTimer();
    if (!oneSec.value)
        oneSecTimer();
    destroy()
    let startRes = await startBotAPI();
    if (startRes.success) {
        ElMessage.success("启动成功")
        botStart.value = true;
        startMsg.value = "正在运行";
        localStorage.setItem("lastStart", currentTime.value)
        runTime.value = "0小时0分钟";
        lastStart.value = currentTime.value;
        if (props.enable.bd && config.value?.bd?.saveAliyunDisk) {
            ElMessage.info("正在启动阿里云盘服务")
            await startAliYunPan();
        }
    } else {
        let emsg = startRes.msg ?? "启动失败：未知错误";
        ElMessage.error(emsg)
        logApi().addSystem(emsg);
        getTenLog();
        startMsg.value = "点击启动"
    }
    if (props.enable.kd) {
        await initPocket()
        initPocketLive()
    }
}
const closeBot = () => {
    startMsg.value = "关闭中！"
    ElMessageBox.confirm("确定要关闭机器人吗？", "温馨提示！", { type: 'warning', confirmButtonText: '关闭', cancelButtonText: '我点错了' })
        .then(async () => {
            //关闭
            await startBotAPI(false);
            ElMessage.success("关闭成功")
            botStart.value = false;
            startMsg.value = "点击启动"
        })
        .catch(() => {
            //不关闭
            botStart.value = true;
            startMsg.value = "正在运行"
        })
}

const destroy = () => {
    if (nim.value) {
        nim.value.destroy()
    }
    if (qChat.value) {
        qChat.value.destroy()
    }
    if (liveNim.value) {
        liveNim.value.disconnect();
    }
}

const initPocket = async () => {
    destroy();
    if (config.value && config.value.kd) {
        try {
            nim.value = new NIMSDK({
                appkey: atob(config.value.kd.appKey!),
                account: config.value.kd.account!,
                token: config.value.kd.token!,
            });
            await nim.value.connect();

            qChat.value = new QChatSDK({
                appkey: atob(config.value.kd.appKey!),
                account: config.value.kd.account!,
                token: config.value.kd.token!,
                linkAddresses: await nim.value.plugin.getQChatAddress({
                    ipType: 2,
                }),
            });

            (qChat.value as any).on("logined", handleLogined);
            (qChat.value as any).on("message", handleMessage);
            (qChat.value as any).on("disconnect", handleRoomSocketDisconnect);
            await qChat.value.login();

        }
        catch (e) {
            throw (e)
        }
    }
}

const initPocketLive = () => {
    liveNim.value = new NimChatroomSocket({ liveId: config.value!.kd!.liveRoomId!, onMessage: liveMsg })
    liveNim.value.init(config.value!.kd!.appKey!);
}

const handleLogined = async function () {
    var msg = `口袋已登录。正在进入小偶像${config.value!.kd!.idolName}的口袋房间。`;
    logApi().addSystem(msg);
    if (qChat.value == null) throw ("进入口袋房间失败，聊天室未成功实例化");
    const result: SubscribeAllChannelResult =
        await qChat.value.qchatServer.subscribeAllChannel({
            type: 1,
            serverIds: config.value!.kd!.serverId!.split(",").map(String),
        });
    if (result.failServerIds.length) {
        msg = `进入小偶像${config.value!.kd!.idolName}的口袋房间失败。请检查配置后重试，如仍有问题，请联系开发者。`;
        logApi().addSystem(msg);
        return;
    }
    msg = `成功进入小偶像${config.value!.kd!.idolName}的口袋房间。`;
    logApi().addSystem(msg);
};

const handleMessage = async function (msg: any) {
    let domain = config.value?.kd?.imgDomain;
    msg.fromType = 1;
    msg.ext = JSON.parse(msg.ext as string);
    msg.channelName = await getChannel(msg.channelId);
    msg.time = dayjs(msg.time).format("YYYY-MM-DD HH:mm:ss");
    await postMsg({ content: JSON.stringify(msg) });
    let kdMsg: logI = {
        type: 'text',
        name: msg.ext.user.nickName,
        time: msg.time,
        avatar: domain + msg.ext.user.avatar ?? '',
        color: '#409eff',
        channel: msg.channelName,
        idol: getIdolName(msg.serverId),
        roleId: msg.ext.user.roleId
    }
    if (msg.type == "text") {
        kdMsg.content = msg.body;
    }
    else if (msg.type == "image") {
        kdMsg.type = 'pic'
        kdMsg.url = msg?.attach?.url;
    }
    else if (msg.type == "video") {
        kdMsg.type = 'video'
        kdMsg.url = msg?.attach?.url;
    }
    else if (msg.type == "audio") {
        kdMsg.type = 'audio'
        kdMsg.url = msg?.attach?.url;
    }
    else if (msg.type == "custom") {
        let attach = msg?.attach;
        let customType = attach?.messageType;
        //回复消息
        if (customType == "REPLY") {
            kdMsg.content = attach?.replyInfo?.text;
            kdMsg.reply = attach?.replyInfo?.replyName + ":" + attach?.replyInfo?.replyText
        }
        //礼物回复
        else if (customType == "GIFTREPLY") {
            kdMsg.content = attach?.giftReplyInfo?.replyName + ":" + attach?.giftReplyInfo?.replyText + "<br/>" + attach?.giftReplyInfo?.text;
        }
        //表情
        else if (customType == "EXPRESSIMAGE") {
            kdMsg.type = 'pic'
            kdMsg.url = attach?.expressImgInfo?.emotionRemote;
        }
        //直播
        else if (customType == "LIVEPUSH") {
            kdMsg.type = 'pic'
            kdMsg.content = "直播啦！<br/>标题：" + attach?.livePushInfo?.liveTitle;
            kdMsg.url = domain + attach?.livePushInfo?.liveCover;
        }
        //语音
        else if (customType == "AUDIO") {
            kdMsg.type = "audio"
            kdMsg.url = attach?.audioInfo?.url;
        }
        //视频
        else if (customType == "VIDEO") {
            kdMsg.type = "video"
            kdMsg.url = attach?.videoInfo?.url;
        }
        //房间电台
        else if (customType == "") {
            kdMsg.content = kdMsg.idol + "开启了房间电台";
        }
        //文字翻牌
        else if (customType = "FLIPCARD") {
            kdMsg.reply = "[文字翻牌]粉丝提问：<br/>" + attach?.filpCardInfo?.question;
            kdMsg.content = attach?.filpCardInfo?.answer;
        }
        //语言翻牌
        else if (customType = "FLIPCARD_AUDIO") {
            kdMsg.type = 'audio'
            kdMsg.reply = "[语言翻牌]粉丝提问：<br/>" + attach?.filpCardInfo?.question;
            kdMsg.url = domain + attach?.filpCardInfo?.answer?.url;
        }
        //视频翻牌
        else if (customType = "FLIPCARD_VIDEO") {
            kdMsg.type = 'video'
            kdMsg.reply = "[视频翻牌]粉丝提问：<br/>" + attach?.filpCardInfo?.question;
            kdMsg.url = domain + attach?.filpCardInfo?.answer?.url;
        } else {
            kdMsg.content = '暂不支持此类型消息，请前往口袋查看。'
            console.log("不支持的消息类型:", msg)
        }
    }
    else {
        kdMsg.content = '暂不支持此类型消息，请前往口袋查看。'
        console.log("不支持的消息类型:", msg)
    }
    logApi().add(kdMsg);
    if (kdMsg.roleId == 3) getTenLog();
};

const handleRoomSocketDisconnect = function (...context: any): void {
    logApi().addSystem("口袋登录连接状态已断开。");
};

const liveMsg = function (t: any, event: Array<LiveRoomMessage>) {
    event.forEach(item => {
        postMsg({ content: JSON.stringify(item) }, 1);
    })
}

const getChannel = async function (id: number) {
    if (qChat.value == null) return;
    const channelResult = await qChat.value.qchatChannel.getChannels({
        channelIds: [`${id}`],
    });
    if (channelResult) {
        return channelResult[0].name;
    }
    return "";
};

const getIdolName = (serverId: string) => {
    var names = config.value!.kd!.idolName!.split(",");
    var serverIds = config.value!.kd!.serverId!.split(",");
    var index = serverIds.indexOf(serverId);
    if (index == -1) return '未匹配';
    return names[index];
}

const getTenLog = () => {
    let logs = logApi().getLogs();
    if (!logs || logs.length <= 0) return
    infoLogs.value = logs!.filter(x => x.type == 'system').reverse().splice(0, 10);
    if (!config.value?.enableModule.kd) return;
    logs = logs.filter(x => x.roleId == 3)
    kdLogs.value = logs.reverse().slice(0, 10);
}
const viewLog = (log: string, title = '错误') => {
    ElMessageBox.alert(log, title + "详情")
}

const oneMinTimer = () => {
    oneMin.value = setInterval(oneMinFun, 10000)
}
const oneMinFun = async () => {
    errLogs.value = (await getLogs()).data.reverse();
    let plugInfo = (await getFun()).data
    let picInfo = await getCache({ pageIndex: 1, pageSize: 9999 })
    info.value.plugin.total = plugInfo.length
    info.value.plugin.unusing = plugInfo.filter((x: any) => !x.status).length
    info.value.plugin.using = plugInfo.filter((x: any) => x.status).length

    info.value.pic.total = picInfo.count
    info.value.pic.today = picInfo.data.filter((x: any) => new Date(x.createDate).getDate() === new Date().getDate()).length
    info.value.pic.old = info.value.pic.total - info.value.pic.today

    let tempLogs = logApi().getLogs();
    if (tempLogs) {
        info.value.log.total = tempLogs.length ?? 0
        info.value.log.idol = tempLogs.filter(x => x.roleId == 3).length
        info.value.log.system = tempLogs.filter(x => x.type == 'system').length
        info.value.log.other = info.value.log.total - info.value.log.idol
    }
}

const oneSecTimer = () => {
    oneSec.value = setInterval(oneSecFun, 1000);
}
const oneSecFun = () => {
    var date = new Date();
    var hour = date.getHours();
    currentTime.value = date.toLocaleString();
    if (hour >= 0 && hour <= 6) currentTimeType.value = "凌晨好";
    if (hour >= 7 && hour <= 10) currentTimeType.value = "上午好";
    if (hour >= 11 && hour <= 13) currentTimeType.value = "中午好";
    if (hour >= 14 && hour <= 17) currentTimeType.value = "下午好";
    if (hour >= 18 && hour <= 19) currentTimeType.value = "傍晚好";
    if (hour >= 20 && hour <= 23) currentTimeType.value = "晚上好";
    let lastStartTime = localStorage.getItem("lastStart")
    if (lastStartTime)
        lastStart.value = lastStartTime
    else
        lastStart.value = "无记录"
    if (lastStart.value == "无记录") runTime.value = '0小时0分钟';
    else {
        let tempLastTime = new Date(lastStart.value);
        var timeDiff = date.getTime() - tempLastTime.getTime();
        var hoursDiff = Math.floor(timeDiff / (1000 * 60 * 60)); // 计算小时差
        var minutesDiff = Math.floor((timeDiff % (1000 * 60 * 60)) / (1000 * 60)); // 计算分钟差
        runTime.value = `${hoursDiff}小时${minutesDiff}分钟`;
    }
}

const refreshConfig = async () => {
    let configTemp = await getConfig();
    config.value = configTemp.data;
}

const openDebug = () => {
    // @ts-ignore
    window.external.devTool.openDevTool();
}

onMounted(async () => {
    await refreshConfig();
    oneMinFun();
    oneSecFun();
    oneSecTimer();
    getTenLog();
});

defineExpose({
    refreshConfig
})
</script>
<style>
.ch250 {
    height: 250px;
}

.ch320 {
    height: 43vh;
}

.chauto {
    height: calc(100vh - 320px);
}

.mt10 {
    margin-top: 10px;
}

.mt5 {
    margin-top: 5px;
}

.et2 {
    position: absolute;
    top: 2px;
    cursor: pointer;
}

.et3 {
    position: absolute;
    top: 3px;
    cursor: pointer;
}

.et4 {
    top: 4px;
    cursor: pointer;
}


.et5 {
    top: 5px;
    cursor: pointer;
}

.fs14 {
    font-size: 14px;
}
</style>