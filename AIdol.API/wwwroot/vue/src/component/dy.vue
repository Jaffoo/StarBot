<template>
    <el-card class="card-dy">
        <template #header>
            <span id="dy">抖音</span>
        </template>
        <el-form ref="dyform" :model="model" :rules="rules" label-width="150px" label-position="left">
            <el-form-item label="抖音用户" prop="user">
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
    </el-card>
</template>

<script setup lang="ts" name="dy">
import { ref, type PropType, toRef } from 'vue'
import type { DY_XHS_BZ as DY } from '@/class/model'
import type { FormInstance, FormRules } from 'element-plus';
const props = defineProps({
    dy: {
        type: Object as PropType<DY>,
        default: {
            user: '',
            group: '',
            forwardGroup: false,
            forwardQQ: false,
            qq: '',
            timeSpan: 3
        }
    }
})
const model = toRef(props.dy);
const dyform = ref<FormInstance>();
const rules = ref<FormRules>(
    {
        user: [{ required: true, message: '请输入该值', trigger: 'blur' }]
    },
)
const validForm = () => {
    dyform.value?.validate(valid => {
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