<template>
    <el-form ref="form" :model="model" label-width="100px">
        <el-form-item label="超管" prop="admin" :rules="rules.common"></el-form-item>
        <el-form-item label="管理员" prop="permission" :rules="rules.common"></el-form-item>
        <el-form-item label="超管" prop="admin" :rules="rules.common"></el-form-item>
    </el-form>
</template>

<script setup lang="ts" name="bot">
import { ref, type PropType } from 'vue'
import type { QQ } from '@/class/model'
import type { FormRules } from 'element-plus';
const prop = defineProps({
    model: {
        type: Object as PropType<QQ>,
        default: null
    }
})
const form = ref(null);
const rules = ref<FormRules>(
    { common: [{ required: true, message: '请输入该值', trigger: 'blur' }] }
)
</script>