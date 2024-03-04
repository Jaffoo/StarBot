<template>
    <el-card class="card-bz">
        <template #header>
            <span id="bz">b站</span>
        </template>
        <el-form ref="bzform" :model="model" :rules="rules" label-width="150px" label-position="left">
            <el-form-item label="b站用户" prop="user">
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

<script setup lang="ts" name="bz">
import { ref, type PropType, toRef } from 'vue'
import type { DY_XHS_BZ as BZ } from '@/class/model'
import type { FormInstance, FormRules } from 'element-plus';
const props = defineProps({
    bz: {
        type: Object as PropType<BZ>,
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
const model = toRef(props.bz);
const bzform = ref<FormInstance>();
const rules = ref<FormRules>(
    {
        user: [{ required: true, message: '请输入该值', trigger: 'blur' }]
    },
)
const validForm = () => {
    bzform.value?.validate(valid => {
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