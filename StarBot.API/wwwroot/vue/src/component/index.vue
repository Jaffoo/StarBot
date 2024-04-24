<template>
    <el-scrollbar style="height: calc(100vh - 30px)">
        <el-row>
            <el-col :span="12">
                <el-card shadow="hover" class="ch250">
                    <template #header>
                        StarBot
                    </template>
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
                                <el-icon :size="18">
                                    <Refresh class="et5" @click="async () => errLogs = (await getLogs()).data" />
                                </el-icon>
                            </el-col>
                        </el-row>
                    </template>
                    <el-scrollbar height="180px" style="margin-top:-10px">
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
                                    <label>系统日志</label>
                                    <div>
                                        <span>全部:{{ info.log.total }}</span>
                                        <el-divider direction="vertical" />
                                        <span>主要:{{ info.log.idol }}</span>
                                        <el-divider direction="vertical" />
                                        <span>其他:{{ info.log.other }}</span>
                                    </div>
                                </div>
                                <el-divider />
                                <div>
                                    <label>图片信息</label>
                                    <div>
                                        <span>全部:{{ info.pic.total }}</span>
                                        <el-divider direction="vertical" />
                                        <span>今日:{{ info.pic.today }}</span>
                                        <el-divider direction="vertical" />
                                        <span>更早:{{ info.pic.old }}</span>
                                    </div>
                                </div>
                                <el-divider />
                                <div v-if="enable.bot">
                                    <label>插件信息</label>
                                    <div>
                                        <span>全部:{{ info.plugin.total }}</span>
                                        <el-divider direction="vertical" />
                                        <span>启用:{{ info.plugin.using }}</span>
                                        <el-divider direction="vertical" />
                                        <span>启用:{{ info.plugin.unusing }}</span>
                                    </div>
                                </div>
                            </el-card>
                        </el-col>
                        <el-col :span="8" v-if="enable.kd">
                            <el-card shadow="hover" class="ch320">
                                <template #header><span title="实时更新">口袋消息</span>
                                </template>
                                <div>
                                    <ul>
                                        <li v-for="item in kdLogs">
                                            <span v-if="item.type == 'text'"
                                                @click="viewLog(item.name + ':' + item.content + '--' + item.time, '口袋消息')"
                                                title="点击查看">
                                                {{ item.name + ':' + item.content?.substring(0, 20) }}--{{ item.time }}
                                            </span>
                                            <span v-if="item.type == 'pic' || item.type == 'link'">{{ item.url }}</span>
                                        </li>
                                    </ul>
                                </div>
                            </el-card>
                        </el-col>
                        <el-col :span="8" v-show="carousel == 'help'">
                            <el-card shadow="hover" class="ch320">
                                <template #header>帮助反馈
                                    <el-icon :size="16">
                                        <Switch class="et2" circle @click="() => carousel = 'log'"></Switch>
                                    </el-icon>
                                </template>
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
                                    <ul style="margin-top:-3px">
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
const currentTime = ref();
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
        other: 0
    }
})

const nim = ref<NIMSDK>();
const qChat = ref<QChatSDK>();
const liveNim = ref<NimChatroomSocket>();

const oneMin = ref<number>();
const oneSec = ref<number>()

const carousel = ref('help')

const startBot = async () => {
    const allFalse = Object.values(props.enable).every(value => value === false);
    if (allFalse) {
        ElMessageBox.alert('没有启用任何功能')
        return;
    }
    startMsg.value = "启动中！"
    if (!oneMin.value || oneMin.value < 0)
        oneMinTimer();
    if (!oneSec.value || oneSec.value < 0)
        oneSecTimer();
    destroy()
    let startRes = await startBotAPI();
    if (startRes.success) {
        ElMessage.success("启动成功")
        botStart.value = true;
        startMsg.value = "正在运行";
        lastStart.value = currentTime.value;
        if (props.enable.bd && config.value?.bd?.saveAliyunDisk) await startAliYunPan();
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
            })

            qChat.value.on("logined", handleLogined);
            qChat.value.on("message", handleMessage);
            qChat.value.on("disconnect", handleRoomSocketDisconnect);
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
            serverIds: [config.value!.kd!.serverId!],
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
    msg.fromType = 1;
    msg.ext = JSON.parse(msg.ext as string);
    msg.channelName = await getChannel(msg.channelId);
    msg.time = dayjs(msg.time).format("YYYY-MM-DD HH:mm:ss");
    await postMsg({ content: JSON.stringify(msg) });
    let kdMsg: logI = {
        type: 'text',
        name: msg.ext.user.nickName,
        time: msg.time,
        avatar: "https://source3.48.cn" + msg.ext.user.avatar ?? '',
        color: '#409eff',
        channel: msg.channelName,
        idol: config.value?.kd?.idolName,
        roleId: msg.ext.user.roleId
    }
    if (msg.type == "text") {
        kdMsg.content = msg.body;
    }
    else if (msg.type == "image") {
        kdMsg.type = 'pic'
        kdMsg.url = msg?.attach?.url;
    }
    else if (msg.type == "video" || msg.type == "audio") {
        kdMsg.type = 'link'
        kdMsg.url = msg?.attach?.url;
    }
    else {
        kdMsg.content = '发送了一条特殊消息！'
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
const getTenLog = () => {
    let logs = logApi().getLogs();
    if (!logs || logs.length <= 0) return
    infoLogs.value = logs!.filter(x => x.type == 'system').splice(-10).reverse();
    if (!props.enable.kd || !config.value || !config.value.kd || !config.value.kd.idolName) return
    logs = logs.filter(x => x.name && x.name.includes(config.value!.kd!.idolName!) && x.roleId == 3)
    kdLogs.value = logs.slice(-10).reverse();
}
const viewLog = (log: string, title = '错误') => {
    ElMessageBox.alert(log, title + "详情")
}

const oneMinTimer = () => {
    oneMin.value = setInterval(oneMinFun, 1000 * 60)
}
const oneMinFun = async () => {
    errLogs.value = (await getLogs()).data
    let plugInfo = (await getFun()).data
    let picInfo = await (await getCache()).data
    info.value.plugin.total = plugInfo.length
    info.value.plugin.unusing = plugInfo.filter((x: any) => !x.status).length
    info.value.plugin.using = plugInfo.filter((x: any) => x.status).length

    info.value.pic.total = picInfo.length
    info.value.pic.today = picInfo.filter((x: any) => new Date(x.createDate).getDate() === new Date().getDate()).length
    info.value.pic.old = info.value.pic.total - info.value.pic.today

    let tempLogs = logApi().getLogs();
    if (tempLogs) {
        info.value.log.total = tempLogs.length ?? 0
        if (config.value?.kd?.idolName)
            info.value.log.idol = tempLogs.filter(x => x.name?.includes(config.value!.kd!.idolName!) && x.roleId == 3).length
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
    if (hour >= 0 && hour < 6) currentTimeType.value = "凌晨好";
    if (hour >= 6 && hour < 11) currentTimeType.value = "早上好";
    if (hour >= 11 && hour < 14) currentTimeType.value = "中午好";
    if (hour >= 14 && hour < 17) currentTimeType.value = "下午好";
    if (hour >= 17 && hour < 19) currentTimeType.value = "傍晚好";
    if (hour >= 19 && hour < 0) currentTimeType.value = "晚上好";
    if (lastStart.value == "无记录") runTime.value = '0小时0分钟';
    else {
        let tempLastTime = new Date(lastStart.value);
        var timeDiff = date.getTime() - tempLastTime.getTime();
        var hoursDiff = Math.floor(timeDiff / (1000 * 60 * 60)); // 计算小时差
        var minutesDiff = Math.floor((timeDiff % (1000 * 60 * 60)) / (1000 * 60)); // 计算分钟差
        runTime.value = `${hoursDiff}小时${minutesDiff}分钟`;
    }
}

onMounted(async () => {
    let configTemp = await getConfig();
    config.value = configTemp.data;
    oneMinFun();
    oneSecFun();
    oneSecTimer();
    getTenLog();
});
</script>
<style>
.ch250 {
    height: 250px;
}

.ch320 {
    height: 320px;
}

.chauto {
    height: calc(100vh - 340px);
}

.mt10 {
    margin-top: 10px;
}

.et2 {
    position: absolute;
    top: 2px;
    cursor: pointer;
}

.et5 {
    position: absolute;
    top: 5px;
    cursor: pointer;
}
</style>
<!-- <template>
    <el-container>
        <el-header height="50px">
            <el-button type="primary" native-type="button" :icon="Setting" @click="config">
                修改配置
            </el-button>
            <el-button title="不启用QQ机器人无需配置" type="primary" native-type="button" :icon="Setting" @click="miraiSetting"
                v-if="!windStatus">
                Mirai配置
            </el-button>
            <el-button v-if="useMirai && !windStatus" type="primary" native-type="button"
                @click="startMirai">启动Mirai机器人</el-button>
            <el-button v-if="!started" type="primary" native-type="button" @click="start">启动机器人</el-button>
            <el-button v-else type="danger" native-type="button" @click="close">关闭机器人</el-button>
            <el-button v-if="useAli" type="primary" native-type="button" @click="startAli">启动阿里云盘</el-button>
        </el-header>
        <el-dropdown style="right: 1vw;position: fixed;">
            <el-button type="primary">更多
                <el-icon>
                    <ArrowDownBold />
                </el-icon>
            </el-button>
            <template #dropdown>
                <el-dropdown-menu>
                    <el-dropdown-item @click="saveBlogByid">抓取微博</el-dropdown-item>
                    <el-dropdown-item @click="windControl">
                        <span v-if="!windStatus" style="color: green;">开启风控</span>
                        <span v-else style="color: orange;">关闭风控</span>
                    </el-dropdown-item>
                    <el-dropdown-item @click="closeRemote" v-if="windStatus">关闭远程</el-dropdown-item>
                    <el-dropdown-item @click="clearCache">清空缓存</el-dropdown-item>
                </el-dropdown-menu>
            </template>
        </el-dropdown>
        <el-main>
            <el-row :gutter="20">
                <el-col :span="12">
                    <h3 style="margin-left: 40%;cursor: pointer;" @click="clear" title="点击清空日志">消息及日志</h3>
                    <div style="height: 530px;overflow:auto;" id="textArae">
                        <div v-for="(item, index) in log" style="margin:5px">
                            {{ (index + 1) + ':' }}
                            <span v-if="item.type != 2" :style="{ color: item.color || '' }">{{ item.content }}</span>
                            <span v-if="item.type == 2">
                                {{ item.content }}
                                <el-image style="height: 80px;width: 80px;" :src="item.url"
                                    :preview-src-list="[item.url]"></el-image>
                            </span>
                        </div>
                    </div>
                </el-col>
                <el-col :span="12">
                    <h3 title="点此刷新" @click="refresh" style="cursor: pointer;margin-left: 40%;">图片列表</h3>
                    <div style="height: 530px;overflow:auto;">
                        <table>
                            <tr>
                                <th width="300px">图片</th>
                                <th>操作</th>
                            </tr>
                            <tr v-for="(item, index) in pic">
                                <td align="center">
                                    <el-image style="height: 80px;width: 80px;" :src="item.content"
                                        :preview-src-list="[item.content]"></el-image>
                                </td>
                                <td>
                                    <el-button size="small" @click="checkTrue(item.id, index)">保存</el-button>
                                    <el-button size="small" type="danger" @click="checkFalse(item.id, index)">删除</el-button>
                                </td>
                            </tr>
                        </table>
                    </div>
                </el-col>
            </el-row>
        </el-main>
    </el-container>
    <span title="点击复制"
        style="position: absolute;right: 10px;bottom: 0;color:rgb(194, 191, 191);font-size: 15px;">如发现问题，反馈QQ：1615842006</span>
</template>
<script setup lang="ts">
import { ref, onMounted, watch } from "vue";
import { ArrowDownBold, Setting } from '@element-plus/icons-vue';
import axios from "axios";
import QChatSDK from "nim-web-sdk-ng/dist/QCHAT_BROWSER_SDK";
import NIMSDK from "nim-web-sdk-ng/dist/NIM_BROWSER_SDK";
import type { SubscribeAllChannelResult } from "nim-web-sdk-ng/dist/QCHAT_BROWSER_SDK/QChatServerServiceInterface";
import dayjs from 'dayjs';
import NimChatroomSocket from '@/class/live'
import type { LiveRoomMessage } from "@/class/messageType";
import PocketMessage from "@/class/type";
import { ElMessage, ElMessageBox } from 'element-plus'

const started = ref<boolean>(false);
const windStatus = ref<boolean>(false);
const config = ref({} as any);
const mirai = ref({} as any);
const useMirai = ref(false);
const useAli = ref(false);
const nim = ref<NIMSDK>();
const qChat = ref<QChatSDK>();
const liveNim = ref<NimChatroomSocket>();
const log = ref<Array<PocketMessage>>(new Array<PocketMessage>());
const pic = ref<Array<any>>(new Array<any>());
const ws = ref<WebSocket>();
const wsReady = ref<boolean>(false);

watch(log.value, (newVal, OldVal) => {
    if (newVal.length >= 100) {
        log.value.splice(0, 1);
    }
    var logDiv: HTMLElement | null = document.getElementById("textArae");
    if (logDiv === null) return;
    logDiv.scrollTop = logDiv.scrollHeight;
})

const startAli = async () => {
    await axios({
        url: "http://parkerbot.api/api/StartAliYunApi"
    });
}

const startMirai = async () => {
    var res = await axios({
        url: "http://parkerbot.api/api/StartMiraiConsole"
    });
    if (!res.data) {
        ElMessage({
            showClose: true,
            message: 'Mirai目录不存在或路径错误，请检查后重试！',
            type: 'error'
        });
    }
    if (res.data) {
        ElMessage({
            showClose: true,
            message: '已为您打开Mirai控制台！',
            type: 'success'
        });
    }
};
const start = async () => {
    if (nim.value) {
        nim.value.destroy()
    }
    if (qChat.value) {
        qChat.value.destroy()
    }
    if (liveNim.value) {
        liveNim.value.disconnect();
    }
    const res = await getConfig();
    config.value = res.data.config;
    const myConfig = res.data.config.kd;

    var useKd: boolean = false;
    res.data.enable.forEach((item: any) => {
        if (item.key == "kd") {
            useKd = JSON.parse(item.value);
        }
    });
    if (useKd) {
        nim.value = new NIMSDK({
            appkey: atob(myConfig.appKey),
            account: myConfig.account,
            token: myConfig.token,
        });
        await nim.value.connect();
        qChat.value = new QChatSDK({
            appkey: atob(myConfig.appKey),
            account: myConfig.account,
            token: myConfig.token,
            linkAddresses: await nim.value.plugin.getQChatAddress({
                ipType: 2,
            }),
        })

        qChat.value.on("logined", handleLogined);
        qChat.value.on("message", handleMessage);
        qChat.value.on("disconnect", handleRoomSocketDisconnect);
        await qChat.value.login();
    } else {
        var msg = "机器人启动中！";
        log.value.push(new PocketMessage().add(msg));
        msg = "";
        var res1 = await axios({ url: "http://parkerbot.api/api/start" });
        if (useMirai.value) {
            if (res1.data.mirai) {
                msg += "QQ机器人启动成功。";
            } else {
                msg += "QQ机器人启动失败。";
            }
        }
        setTimeout(() => {
            if (msg) log.value.push(new PocketMessage().add(msg));
            log.value.push(new PocketMessage().add("机器人已启动!"));
        }, 500);
    }
    started.value = true;
};

const close = () => {
    var msg = "机器人关闭中！";
    log.value.push(new PocketMessage().add(msg));
    if (nim.value) {
        nim.value.destroy()
    }
    if (qChat.value) {
        qChat.value.destroy()
    }
    if (liveNim.value) {
        liveNim.value.disconnect();
    }
    if (ws.value) {
        ws.value.close();
    }
    msg = "机器人已关闭！";
    log.value.push(new PocketMessage().add(msg));
    started.value = false;
}

const handleLogined = async function () {
    var msg = `口袋登录成功。订阅小偶像${config.value.kd.name}的房间。`;
    log.value.push(new PocketMessage().add(msg));
    if (qChat.value == null) throw ("聊天室未成功实例化");
    const result: SubscribeAllChannelResult =
        await qChat.value.qchatServer.subscribeAllChannel({
            type: 1,
            serverIds: [config.value.kd.serverId],
        });
    if (result.failServerIds.length) {
        msg = `小偶像${config.value.kd.name}的房间订阅失败。请检查配置后重试，如仍有问题，请联系开发者。`;
        log.value.push(new PocketMessage().add(msg));
        return;
    }
    msg = `小偶像${config.value.kd.name}的房间订阅成功。`;
    log.value.push(new PocketMessage().add(msg));
    //同时订阅直播间
    liveNim.value = new NimChatroomSocket({ liveId: config.value.kd.liveRoomId, onMessage: liveMsg })
    liveNim.value.init(config.value.kd.appKey);

    var res = await axios({ url: "http://parkerbot.api/api/start" });
    ws.value = new window.WebSocket("ws://localhost:6001");
    ws.value.onopen = () => {
        wsReady.value = true;
        log.value.push(new PocketMessage().add("连接消息推送服务器成功。"));
    };
    ws.value.onclose = () => {
        wsReady.value = false;
        log.value.push(new PocketMessage().add("连接消息推送服务器失败，请尝试再次启动机器人。请检查配置后重试，如仍有问题，请联系开发者。"));
    };
    if (useMirai.value) {
        if (res.data.mirai) {
            msg = "QQ机器人启动成功。";
        } else {
            msg = "QQ机器人启动失败，请尝试再次启动机器人。请检查配置后重试，如仍有问题，请联系开发者。";
        }
        log.value.push(new PocketMessage().add(msg));
    }
};

const handleMessage = async function (msg: any) {
    msg.fromType = 1;
    msg.ext = JSON.parse(msg.ext as string);
    msg.channelName = await getChannel(msg.channelId);
    msg.time = dayjs(msg.time).format("YYYY-MM-DD HH:mm:ss");
    if (wsReady.value) {
        ws.value?.send(JSON.stringify(msg));
    }
    if (msg.type == "text") {
        var mess = `【${msg.channelName}|${msg.time}】${msg.ext.user.nickName}:${msg.body}`;
        log.value.push(new PocketMessage().add(mess));
    }
    else if (msg.type == "image") {
        log.value.push(new PocketMessage().addImg(`【${msg.channelName}|${msg.time}】${msg.ext.user.nickName}:`, msg?.attach?.url));
    }
    else if (msg.type == "video") {
        log.value.push(new PocketMessage().addVideo(`【${msg.channelName}|${msg.time}】${msg.ext.user.nickName}:`, msg?.attach?.url));
    }
    else if (msg.type == "audio") {
        log.value.push(new PocketMessage().addVoice(`【${msg.channelName}|${msg.time}】${msg.ext.user.nickName}:`, msg?.attach?.url));
    }
    else if (msg.type == "custom") {
        log.value.push(new PocketMessage().add(`【${msg.channelName}|${msg.time}】${msg.ext.user.nickName}:发送了一条特殊消息！`));
    }
};

const liveMsg = function (t: any, event: Array<LiveRoomMessage>) {
    event.forEach(item => {
        if (wsReady.value) {
            item.fromType = 2;
            ws.value?.send(JSON.stringify(item));
        }
    })
}

const handleRoomSocketDisconnect = function (...context: any): void {
    log.value.push(new PocketMessage().add("登录连接状态已断开。"));
};

const getChannel = async function (id: number) {
    if (qChat.value == null) throw ("聊天室未成功实例化。");
    const channelResult = await qChat.value.qchatChannel.getChannels({
        channelIds: [`${id}`],
    });
    if (channelResult) {
        return channelResult[0].name;
    }
    return "";
};

const config = () => {
    if (nim.value) {
        nim.value.destroy()
    }
    if (qChat.value) {
        qChat.value.destroy()
    }
    if (liveNim.value) {
        liveNim.value.disconnect();
    }
};
const miraiSetting = () => {
    if (nim.value) {
        nim.value.destroy()
    }
    if (qChat.value) {
        qChat.value.destroy()
    }
    if (liveNim.value) {
        liveNim.value.disconnect();
    }
};

const getConfig = async (): Promise<any> => {
    var res = await axios({
        url: "http://parkerbot.api/api/GetBaseConfig"
    });
    return res;
};
onMounted(async () => {
    const res = await getConfig();
    config.value = res.data.config;
    mirai.value = res.data.mirai;
    useMirai.value = res.data.mirai.useMirai;
    res.data.enable.forEach((item: any) => {
        if (item.key === "BD") {
            if (JSON.parse(item.value) && res.data.config.BD.saveAliyunDisk) {
                useAli.value = true;
            }
        }
    })
    windStatus.value = res.data.windStatus;
    await refresh();
    let msg = `请仔细阅读：QQ被风控时使用，发送消息原理是模拟鼠标键盘发送。如果你使用的是云服务器或挂机宝一类的，请不要手动关闭远程连接，请到【更多】里点击【关闭远程】。`;
    if (windStatus.value) {
        var mess = msg;
        if (!log.value.find(e => e.content === msg)) log.value.push(new PocketMessage().add(mess, 'red'));
    } else {
        log.value = log.value.filter(e => e.content !== msg)
    }
});

const checkTrue = async (id: number, index: number) => {
    //传给后端
    var res = await axios({
        url: "http://parkerbot.api/api/piccheck?id=" + id + "&type=1"
    });
    if (res.data) {
        ElMessage({
            showClose: true,
            message: '保存成功！',
            type: 'success'
        });
        pic.value.splice(index, 1);
    }
    if (!res.data) {
        ElMessage({
            showClose: true,
            message: '保存失败！',
            type: 'error'
        });
    }
};

const checkFalse = async (id: number, index: number) => {
    //传给后端
    var res = await axios({
        url: "http://parkerbot.api/api/piccheck?id=" + id + "&type=2"
    });
    if (res.data) {
        ElMessage({
            showClose: true,
            message: '删除成功！',
            type: 'success'
        });
        pic.value.splice(index, 1);
    }
    if (!res.data) {
        ElMessage({
            showClose: true,
            message: '删除失败！',
            type: 'error'
        });
    }
};

const refresh = async () => {
    var res = await axios({
        url: "http://parkerbot.api/api/refresh"
    });
    pic.value = res.data;
}

const saveBlogByid = () => {
    ElMessageBox.prompt('', '请输入微博id', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        beforeClose: (action: string, instance: any, done: Function) => {
            if (action == "confirm") {
                const value = instance.inputValue;
                if (!value || value.trim() === '') {
                    ElMessage({
                        showClose: true,
                        message: "请输入微博id",
                        type: 'warning'
                    });
                } else {
                    done();
                }
            } else {
                done();
            }
        }
    })
        .then(async ({ value }: { value: string }) => {
            if (!value || value.trim() === '') return;
            var res = await axios({
                url: "http://parkerbot.api/api/SaveByBlogId?blogId=" + value
            });
            if (res.data.success) {
                ElMessage({
                    showClose: true,
                    message: res.data.msg || '抓取成功！',
                    type: 'success'
                })
            } else {
                ElMessage({
                    showClose: true,
                    message: res.data.msg || "抓取失败！",
                    type: 'error'
                });
            }
        })
}
const windControl = () => {
    close();
    windStatus.value = !windStatus.value
    ElMessage.success(windStatus.value ? "已开启风控模式" : "已关闭风控模式")
    axios.get('http://parkerbot.api/api/SetWindStatus', { params: { windStatus: windStatus.value } })
    let msg = `请仔细阅读：QQ被风控时使用，发送消息原理是模拟鼠标键盘发送。如果你使用的是云服务器或挂机宝一类的，请不要手动关闭远程连接，请到【更多】里点击【关闭远程】。`;
    if (windStatus.value) {
        var mess = msg;
        if (!log.value.find(e => e.content === msg)) log.value.push(new PocketMessage().add(mess, 'red'));
    } else {
        log.value = log.value.filter(e => e.content !== msg)
    }
}
const closeRemote = () => {
    ElMessageBox.alert('确认后会打开一个窗口，作用是5秒后会自动关闭远程连接，你需要在5秒内，点击qq窗口输入框，并显示出光标！', '请仔细阅读', {
        confirmButtonText: '我知道了',
        beforeClose: (action: string, instance: any, done: Function) => {
            if (action === "confirm") {
                axios.get('http://parkerbot.api/api/closeRemote')
            }
            done();
        }
    })
}
const clearCache = () => {
    ElMessageBox.alert('若发现配置项没有生效，可尝试清空缓存！', '提示', {
        confirmButtonText: '确定',
        callback: () => {
            axios.get('http://parkerbot.api/api/clearCache').then(res => {
                if (res.data) {
                    ElMessage.success("缓存已经清空！")
                } else ElMessage.error("操作失败")
            })
        }
    })
}
const clear = () => {
    log.value.length = 0;
}
</script> -->
