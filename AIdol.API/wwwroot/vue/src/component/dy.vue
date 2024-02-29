<template>
    <el-form ref="dyform" :model="model" label-width="100px">
        <el-form-item label="抖音用户">
            <el-input v-model="model.user" />
        </el-form-item>
        <el-form-item label="转发至群">
            <el-switch v-model="model.forwardGroup" active-text="转发" inactive-text="不转发" />
        </el-form-item>
        <el-form-item label="群qq号" v-if="model.forwardGroup">
            <el-input v-model="model.group" />
        </el-form-item>
        <el-form-item label="转发至好友">
            <el-switch v-model="model.forwardQQ" active-text="转发" inactive-text="不转发" />
        </el-form-item>
        <el-form-item label="好友qq" v-if="model.forwardQQ">
            <el-input v-model="model.qq" />
        </el-form-item>
        <el-form-item label="监听间隔" v-if="model.forwardQQ || model.forwardGroup">
            <el-input-number v-model="model.timeSpan" />
        </el-form-item>
    </el-form>
</template>

<script setup lang="ts" name="dy">
import { ref, type PropType, toRef } from 'vue'
import type { DY_XHS_BZ as DY } from '@/class/model'
const props = defineProps({
    dy: {
        type: Object as PropType<DY>,
        default: null
    }
})
const model = toRef(props.dy);
const dyform = ref(null);
</script>