<template>
    <el-card class="card-bot">
        <template #header>
            <span id="bot">机器人</span>
        </template>
        <el-form ref="botform" :rules="rules" :model="bot" label-width="150px" label-position="left">
            <el-form-item label="IP地址" prop="host">
                <el-input v-model="bot.host" />
            </el-form-item>
            <el-form-item label="WebScoket端口" prop="websocktPort">
                <el-input v-model="bot.websocktPort" />
            </el-form-item>
            <el-form-item label="Http端口" prop="httpPort">
                <el-input v-model="bot.httpPort" />
            </el-form-item>
            <el-form-item label="token">
                <el-input v-model="bot.token" />
            </el-form-item>
        </el-form>
    </el-card>
</template>

<script setup lang="ts" name="bot">
import { ref, type PropType } from 'vue'
import type { Bot } from '@/class/model'
import type { FormInstance, FormRules } from 'element-plus';
defineProps({
    bot: {
        type: Object as PropType<Bot>,
        default: {
            host: '',
            websocktPort: '',
            httpPort: '',
            token: ''
        }
    }
})
const botform = ref<FormInstance>();
const rules = ref<FormRules>(
    {
        host: [{ required: true, message: '请输入该值', trigger: 'blur' }],
        websocktPort: [{ required: true, message: '请输入该值', trigger: 'blur' }],
        httpPort: [{ required: true, message: '请输入该值', trigger: 'blur' }]
    },
)
const validForm = async () => {
    return await botform.value?.validate(valid => {
        if (valid) {
            return true
        } else {
            return false
        }
    })
}
defineExpose({
    validForm
})
</script>