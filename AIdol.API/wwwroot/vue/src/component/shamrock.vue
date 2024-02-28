<template>
    <el-form ref="form" :model="model" label-width="100px">
        <el-switch v-model="model.use" active-text="启用" inactive-text="禁用"></el-switch>
        <template v-if="model.use">
            <el-form-item label="IP地址" :rules="rules.common">
                <el-input v-model="model.host" />
            </el-form-item>
            <el-form-item label="WebScoket端口" :rules="rules.common">
                <el-input v-model="model.websocktPort" />
            </el-form-item>
            <el-form-item label="Http端口" :rules="rules.common">
                <el-input v-model="model.httpPort" />
            </el-form-item>
            <el-form-item label="token">
                <el-input v-model="model.token" />
            </el-form-item>
        </template>
    </el-form>
</template>

<script setup lang="ts" name="bot">
import { ref, type PropType } from 'vue'
import type { Shamrock } from '@/class/model'
import type { FormRules } from 'element-plus';
defineProps({
    model: {
        type: Object as PropType<Shamrock>,
        default: null
    }
})
const form = ref(null);
const rules = ref<FormRules>(
    { common: [{ required: true, message: '请输入该值', trigger: 'blur' }] }
)
</script>