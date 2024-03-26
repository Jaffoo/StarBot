<template>
    <el-form ref="bdform" label-width="150px" label-position="left">
        <el-form-item label="启用模块：">
            <el-checkbox v-model="model.shamrock" label="QQ&Shamrock" @change="botChange"></el-checkbox>
            <el-checkbox v-model="model.wb" label="微博"></el-checkbox>
            <el-checkbox v-model="model.bz" label="B站"></el-checkbox>
            <el-checkbox v-model="model.kd" label="口袋48"></el-checkbox>
            <el-checkbox v-model="model.xhs" label="小红书"></el-checkbox>
            <el-checkbox v-model="model.bd" label="百度"></el-checkbox>
            <el-checkbox v-model="model.dy" label="抖音"></el-checkbox>
        </el-form-item>
    </el-form>
</template>

<script setup lang="ts">
import { type PropType, watch, toRef } from "vue"
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
var model = toRef(props.enable);
// 定义子组件向父组件传值/事件
const emit = defineEmits(['enable-change']);
watch(model.value, () => {
    emit("enable-change", model.value)
})

const botChange = (val: boolean) => {
    model.value.qq = val;
}
</script>