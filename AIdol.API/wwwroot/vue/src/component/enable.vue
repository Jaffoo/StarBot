<template>
    <div style="margin-bottom:1%">
        <label style="font-size:18px;margin-right:3%">启用模块</label>
        <el-checkbox v-model="data.qq" label="QQ"></el-checkbox>
        <el-checkbox v-model="data.wb" label="微博"></el-checkbox>
        <el-checkbox v-model="data.bz" label="B站"></el-checkbox>
        <el-checkbox v-model="data.kd" label="口袋48"></el-checkbox>
        <el-checkbox v-model="data.xhs" label="小红书"></el-checkbox>
        <el-checkbox v-model="data.dy" label="抖音"></el-checkbox>
        <el-checkbox v-model="data.bd" label="百度"></el-checkbox>
    </div>
</template>
<script setup lang="ts">
import { toRef, type PropType } from "vue"
import type { EnableModule } from "@/class/model";
const props = defineProps({
    enable: {
        type: Object as PropType<EnableModule>,
        default: {
            qq: false,
            wb: false,
            bz: false,
            kd: false,
            xhs: false,
            dy: false,
            bd: false
        }
    }
})
const data = toRef(props.enable);
</script>