<template>
    <div>
        <div>
            <el-button type="primary" @click="clear">清空</el-button>
            <el-button type="primary" @click="exportLog">导出</el-button>
        </div>
        <el-scrollbar style="height: calc(100vh - 230px);">
            <div v-for="(item, index) in logs">
                <span>{{ index + 1 }}.</span>
                <span><el-avatar :src="item.avarar" />{{ item.name }}:</span>
                <span v-if="item.type == 'text'" :style="{ color: item.color }">{{ item.content }}</span>
                <span v-if="item.type == 'link'" :style="{ color: item.color, 'text-decoration': 'underline' }"
                    @click="openUrl(item.url)">
                    {{ item.url }}
                </span>
                <el-image v-if="item.type == 'pic'" :src="item.url" :initial-index="getIndex(item.url)"
                    :preview-src-list="imgList()" />
                <span>--{{ item.time }}</span>
            </div>
        </el-scrollbar>
    </div>
</template>

<script setup lang="ts" name="log">
import { ref } from 'vue';
import { openWindow } from '@/api'
import { saveAs } from "file-saver";

interface logI {
    name?: string,
    time?: Date,
    avarar?: string,
    type: 'pic' | 'text' | 'link',
    content?: string,
    url?: string,
    color?: '#409eff' | '#67c23a' | '#f56c6c'
}
const logs = ref<Array<logI>>(new Array<logI>)
const openUrl = (url?: string) => {
    if (url) openWindow(url)
}
const clear = async () => {
    logs.value = new Array<logI>;
}

const exportLog = () => {
    if (logs.value.length <= 0) return;
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
    var res = logs.value.filter(x => x.url != undefined && x.url.length > 0).map(e => e.url)
    if (res == undefined) return [] as string[]
    return res as string[];
}

const getIndex = (url?: string): number => {
    var index = imgList().findIndex(x => x == url)
    return index;
}

const add = (model: logI) => {
    logs.value.push(model)
    var localLog = localStorage.getItem("localLog");
    if (localLog) {
        localStorage.removeItem("localLog");
    }
    var logmodel = {
        count: logs.value.length,
        newLog: model
    }
    localStorage.setItem("localLog", JSON.stringify(logmodel))
}
const addRange = (models: Array<logI>) => {
    if (!logs.value) logs.value = [];
    models.forEach(item => {
        logs.value?.push(item)
    })
}

defineExpose({
    add,
    addRange
})
</script>