<template>
    <div>
        <div>
            <el-button type="primary" @click="clear">清空</el-button>
            <el-button type="primary" @click="exportLog">导出</el-button>
        </div>
        <el-scrollbar style="height: calc(100vh - 100px);margin-top: 10px;">
            <div v-for="(item, index) in logs">
                <span>{{ index + 1 }}.</span>
                <span v-if="item.type !== 'system'" style="margin-top: 10px;"><el-avatar :src="item.avatar" />{{
                    item.name }}：</span>
                <span v-else>系统信息：</span>
                <span v-if="item.type == 'text' || 'system'" :style="{ color: item.color }">{{ item.content }}</span>
                <span v-if="item.type == 'link'"
                    :style="{ color: item.color, 'text-decoration': 'underline', cursor: 'pointer' }"
                    @click="openUrl(item.url)">
                    {{ item.url }}
                </span>
                <el-image v-if="item.type == 'pic'" :src="item.url" :initial-index="getIndex(item.url)"
                    :preview-src-list="imgList()" style="width: 120px;height: auto;" />
                <span>--from {{ (item.idol || "") + (item.channel ? "【" + item.channel + "】" : '') + item.time }}</span>
            </div>
        </el-scrollbar>
    </div>
</template>

<script setup lang="ts" name="log">
import { onMounted, ref } from 'vue';
import { openWindow } from '@/api'
import { saveAs } from "file-saver";
import { type logI, logApi } from '@/class/model';

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
    if (logs.value.length <= 0) {
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
        let tempLogs = logApi().getLogs();
        if (tempLogs) logs.value = tempLogs
    }, 3000);
}
onMounted(() => {
    let tempLogs = logApi().getLogs();
    if (tempLogs) logs.value = tempLogs
    getLogTimer();
})
</script>