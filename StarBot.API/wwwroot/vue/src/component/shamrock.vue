<template>
    <el-card class="card-shamrock">
        <template #header>
            <span id="shamrock">Shamrock机器人</span>
        </template>
        <el-form ref="botform" :rules="rules" :model="shamrock" label-width="150px" label-position="left">
            <el-form-item label="IP地址" prop="host">
                <el-input v-model="shamrock.host" />
            </el-form-item>
            <el-form-item label="WebScoket端口" prop="websocktPort">
                <el-input v-model="shamrock.websocktPort" />
            </el-form-item>
            <el-form-item label="Http端口" prop="httpPort">
                <el-input v-model="shamrock.httpPort" />
            </el-form-item>
            <el-form-item label="token">
                <el-input v-model="shamrock.token" />
            </el-form-item>
        </el-form>
    </el-card>
</template>

<script setup lang="ts" name="bot">
import { ref, type PropType } from 'vue'
import type { Shamrock } from '@/class/model'
import type { FormInstance, FormRules } from 'element-plus';
defineProps({
    shamrock: {
        type: Object as PropType<Shamrock>,
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
        common: [{ required: true, message: '请输入该值', trigger: 'blur' }],
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