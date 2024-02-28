<template>
    <el-form ref="form" :model="model" label-width="100px">
        <el-form-item label="idol用户">
            <el-input v-model="model.userAll" /><span style="color: red;">用于转发动态和图片保存，一般是填idol</span>
        </el-form-item>
        <el-form-item label="返图用户">
            <el-input v-model="model.userAll" /><span style="color: red;">仅用于图片保存，一般是填返图用户)</span>
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
        <el-form-item label="吃瓜用户">
            <el-input v-model="model.userAll" /><span style="color: red;">监听某些用户是否发送了包含关键词的动态</span>
        </el-form-item>
        <el-form-item label="关键词">
            <el-input v-model="model.keyword" />
        </el-form-item>
        <el-form-item label="转发至群">
            <el-switch v-model="model.chiGuaForwardGroup" active-text="转发" inactive-text="不转发" />
        </el-form-item>
        <el-form-item label="群qq号" v-if="model.chiGuaForwardGroup">
            <el-input v-model="model.chiGuaGroup" />
        </el-form-item>
        <el-form-item label="转发至好友">
            <el-switch v-model="model.chiGuaForwardQQ" active-text="转发" inactive-text="不转发" />
        </el-form-item>
        <el-form-item label="好友qq" v-if="model.chiGuaForwardQQ">
            <el-input v-model="model.chiGuaQQ" />
        </el-form-item>
        <el-form-item label="监听间隔"
            v-if="model.forwardQQ || model.forwardGroup || model.chiGuaForwardGroup || model.chiGuaForwardQQ">
            <el-input-number v-model="model.timeSpan" />
        </el-form-item>
    </el-form>
</template>

<script setup lang="ts" name="wb">
import { ref, type PropType } from 'vue'
import type { WB } from '@/class/model'
defineProps({
    model: {
        type: Object as PropType<WB>,
        default: null
    }
})
const form = ref(null);
</script>