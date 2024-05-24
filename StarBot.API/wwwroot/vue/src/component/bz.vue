<template>
    <el-card class="card-bz">
        <template #header>
            <span id="bz">b站</span>
        </template>
        <el-form ref="bzform" :model="bz" :rules="rules" label-width="150px" label-position="left">
            <el-form-item label="b站用户" prop="user">
                <el-input v-model="bz.user" />
            </el-form-item>
            <el-form-item label="转发至群">
                <el-switch v-model="bz.forwardGroup" active-text="转发" inactive-text="不转发" />
            </el-form-item>
            <el-form-item label="群qq号" v-if="bz.forwardGroup">
                <el-input v-model="bz.group" />
            </el-form-item>
            <el-form-item label="转发至好友">
                <el-switch v-model="bz.forwardQQ" active-text="转发" inactive-text="不转发" />
            </el-form-item>
            <el-form-item label="好友qq" v-if="bz.forwardQQ">
                <el-input v-model="bz.qq" />
            </el-form-item>
            <el-form-item label="监听间隔" v-if="bz.forwardQQ || bz.forwardGroup">
                <el-input-number v-model="bz.timeSpan" />
            </el-form-item>
        </el-form>
    </el-card>
</template>

<script setup lang="ts" name="bz">
import { ref, type PropType } from 'vue'
import type { DY_XHS_BZ as BZ } from '@/class/model'
import type { FormInstance, FormRules } from 'element-plus';
defineProps({
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
const bzform = ref<FormInstance>();
const rules = ref<FormRules>(
    {
        user: [{ required: true, message: '请输入该值', trigger: 'blur' }]
    },
)
const validForm = async () => {
    return new Promise((resolve) => {
        bzform.value?.validate((valid: boolean) => {
            resolve(valid);
        });
    });
}
defineExpose({
    validForm
})
</script>