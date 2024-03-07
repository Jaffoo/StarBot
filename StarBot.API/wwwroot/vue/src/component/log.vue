<template>
    <div>
        <div><el-button>清空</el-button></div>
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
        </div>
    </div>
</template>

<script setup lang="ts" name="bz">
import { ref } from 'vue';
import { openWindow } from '@/api'

interface log {
    name?: string,
    time?: Date,
    avarar?: string,
    type: 'pic' | 'text' | 'link',
    content?: string,
    url?: string,
    color?: '#409eff' | '#67c23a' | '#f56c6c'
}

const openUrl = async (url?: string) => {
    if (url) await openWindow(url)
}

const logs = ref<Array<log>>(new Array<log>)

const imgList = (): string[] => {
    var res = logs.value.filter(x => x.url != undefined && x.url.length > 0).map(e => e.url)
    if (res == undefined) return [] as string[]
    return res as string[];
}

const getIndex = (url?: string): number => {
    var index = imgList().findIndex(x => x == url)
    return index;
}

const add = (model: log) => {
    logs.value.push(model)
}
const addRange = (models: Array<log>) => {
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