<template>
    <div>
        <div v-for="item in logs">
            <span><el-avatar :src="item.avarar" />{{ item.name }}:</span>
            <span v-if="item.type == 'text'" :style="{ color: item.color }">{{ item.content }}</span>
            <span v-if="item.type == 'link'" :style="{ color: item.color, 'text-decoration': 'underline' }"
                @click="openUrl(item.url)">
                {{ item.url }}
            </span>
            <span v-if="item.type == 'pic'" :style="{ color: item.color }">{{ item.content }}</span>
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