<template>
    <div>
        <div>
            <el-space wrap>
                <label>日志显示数量：</label>
                <el-input-number v-model="num" :min="1" :max="1000" @change="getLogs" />
                <label>日志类型：</label>
                <el-select v-model="logType" style="width: 120px;" @change="getLogs">
                    <el-option :key="0" label="全部" value="all" />
                    <el-option :key="1" label="系统" value="system" />
                    <el-option :key="2" label="图片" value="pic" />
                    <el-option :key="3" label="文本" value="text" />
                    <el-option :key="4" label="链接" value="link" />
                    <el-option :key="5" label="小偶像" value="idol" v-if="enable.kd" />
                </el-select>
                <el-button type="primary" @click="clear">清空</el-button>
                <el-button type="primary" @click="exportLog">导出</el-button>
            </el-space>
        </div>
        <el-scrollbar style="height: calc(100vh - 100px);margin-top: 10px;">
            <el-card v-for="item in logs" style="background-color:rgba(220,220,220,0.1);margin-bottom: 10px;"
                :shadow="'never'">
                <el-row>
                    <el-col :span="1.5">
                        <el-avatar v-if="item.type !== 'system'" :src="item.avatar" />
                    </el-col>
                    <el-col :span="0.5"><span>&nbsp;</span></el-col>
                    <el-col :span="22">
                        <el-row>
                            <span v-if="item.type !== 'system'">{{ item.name }}
                                <span style="font-size: 12px;color: gray">{{
                                    (item.idol ||
                                        "") + (item.channel ?
                                            "【" + item.channel + "】" : '') +
                                    item.time
                                }}</span>
                            </span>
                            <span v-else>系统信息：<span style="font-size: 12px;color: gray">{{ item.time }}</span></span>
                        </el-row>
                        <el-row>
                            <el-card body-class="card-body-diy">
                                <span v-if="item.type == 'text' || 'system'" :style="{ color: item.color }">{{
                                    item.content
                                }}</span>
                                <span v-if="item.type == 'link'"
                                    :style="{ color: item.color, 'text-decoration': 'underline', cursor: 'pointer' }"
                                    @click="openUrl(item.url)">
                                    {{ item.url }}
                                </span>
                                <el-image v-if="item.type == 'pic'" :src="item.url" :initial-index="getIndex(item.url)"
                                    :preview-src-list="imgList()" style="width: 120px;height: auto;" />
                            </el-card>
                        </el-row>
                    </el-col>
                </el-row>
            </el-card>
        </el-scrollbar>
    </div>
</template>

<script setup lang="ts" name="log">
import { onMounted, ref, type PropType } from 'vue';
import { openWindow } from '@/api'
import { saveAs } from "file-saver";
import { type logI, logApi, type EnableModule } from '@/class/model';

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
const logType = ref<'system' | 'pic' | 'text' | 'link' | 'all' | 'idol'>('all')
const num = ref(30)
const logs = ref<Array<logI>>(new Array<logI>)
const openUrl = (url?: string) => {
    if (url) openWindow(url)
}
const clear = async () => {
    if (logs.value.length <= 0) {
        ElMessage.info("无日志")
        return;
    }
    ElMessageBox.confirm("此操作将永久清空日志，是否删除", "警告", { cancelButtonText: '取消', confirmButtonText: '确定' })
        .then(() => {
            logs.value = new Array<logI>;
            logApi().clearLog();
        })
}

const exportLog = () => {
    if (!logs.value || logs.value.length <= 0) {
        ElMessage.info("无日志")
        return;
    }
    let text = [''];
    logs.value.forEach(item => {
        if (item.type == "link" || item.type == "pic")
            text.push(item.name + ":" + item.url);
        else
            text.push(item.name + ":" + item.content);
    })
    const blob = new Blob(text, { type: "text/plain;charset=utf-8" }); // 创建一个包含文本内容的 Blob 对象
    saveAs(blob, "logs.txt"); // 使用 FileSaver.js 的 saveAs 方法导出文件
}

const imgList = (): string[] => {
    var res = logs.value.filter(x => x.type == 'pic').map(e => e.url)
    if (res == undefined) return [] as string[]
    return res as string[];
}

const getIndex = (url?: string): number => {
    var index = imgList().findIndex(x => x == url)
    return index;
}

const getLogTimer = () => {
    setInterval(() => {
        getLogs()
    }, 3000);
}

const getLogs = () => {
    let tempLogs = logApi().getLogs();
    if (!tempLogs) return;
    tempLogs = tempLogs.reverse();
    if (logType.value == 'idol') tempLogs = tempLogs.filter(x => x.idol && x.roleId == 3);
    else if (logType.value && logType.value != 'all') tempLogs = tempLogs.filter(x => x.type == logType.value);
    if (num) tempLogs = tempLogs.slice(0, num.value);
    logs.value = tempLogs;
}


onMounted(() => {
    getLogs();
    getLogTimer();
})
</script>
<style>
.card-body-diy {
    padding: 6px 16px 6px 16px;
}
</style>