<template>
    <el-form ref="form" :model="model" label-width="100px">
        <el-form-item label="超管" :rules="rules.common">
            <el-input v-model="model.admin" />
        </el-form-item>
        <el-form-item label="管理员" :rules="rules.common">
            <el-input v-model="model.permission" />
        </el-form-item>
        <el-form-item label="群" :rules="rules.common">
            <el-input v-model="model.group" />
        </el-form-item>
        <el-form-item label="程序错误通知">
            <el-switch v-model="model.debug" active-text="启用" inactive-text="禁用" />
        </el-form-item>
        <el-form-item label="新动态消息通知">
            <el-switch v-model="model.notice" active-text="启用" inactive-text="禁用" />
        </el-form-item>
        <el-form-item label="保存群消息">
            <el-switch v-model="model.save" active-text="保存" inactive-text="不保存" />
        </el-form-item>
    </el-form>
</template>

<script setup lang="ts" name="qq">
import { ref, type PropType, onMounted } from 'vue'
import type { QQ } from '@/class/model'
import type { FormRules } from 'element-plus';
import { getFun } from '@/api'
defineProps({
    model: {
        type: Object as PropType<QQ>,
        default: null
    }
})
const getQQFuns = async () => {
    var res = await getFun();
}
const form = ref(null);
const rules = ref<FormRules>(
    { common: [{ required: true, message: '请输入该值', trigger: 'blur' }] }
)
onMounted(() => {
    getQQFuns
})
</script>