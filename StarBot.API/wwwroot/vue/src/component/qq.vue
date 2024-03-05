<template>
    <el-card class="card-qq">
        <template #header>
            <span id="qq">QQ</span>
        </template>
        <el-form ref="qqform" :model="qq" :rules="rules" label-width="150px" label-position="left">
            <el-form-item label="超管" prop="admin">
                <el-input v-model="qq.admin" />
            </el-form-item>
            <el-form-item label="管理员" prop="permission">
                <el-input v-model="qq.permission" />
            </el-form-item>
            <el-form-item label="群" prop="group">
                <el-input v-model="qq.group" />
            </el-form-item>
            <el-form-item label="程序错误通知">
                <el-switch v-model="qq.debug" active-text="启用" inactive-text="禁用" />
            </el-form-item>
            <el-form-item label="新动态消息通知">
                <el-switch v-model="qq.notice" active-text="启用" inactive-text="禁用" />
            </el-form-item>
            <el-form-item label="保存群消息">
                <el-switch v-model="qq.save" active-text="保存" inactive-text="不保存" />
            </el-form-item>
        </el-form>
    </el-card>
</template>

<script setup lang="ts" name="qq">
import { ref, type PropType, onMounted } from 'vue'
import type { QQ } from '@/class/model'
import type { FormInstance, FormRules } from 'element-plus';
import { getFun } from '@/api'

defineProps({
    qq: {
        type: Object as PropType<QQ>,
        default: {
            funcAdmin: new Array<string>,
            group: '',
            save: false,
            admin: '',
            notice: false,
            permission: '',
            debug: false,
        }
    }
})
const getQQFuns = async () => {
    var res = await getFun();
    console.log(res)
}
const qqform = ref<FormInstance>();
const rules = ref<FormRules>(
    {
        admin: [{ required: true, message: '请输入该值', trigger: 'blur' }],
        permission: [{ required: true, message: '请输入该值', trigger: 'blur' }],
        group: [{ required: true, message: '请输入该值', trigger: 'blur' }],
    }
)
const validForm = async () => {
    return await qqform.value?.validate(valid => {
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
onMounted(async () => {
    await getQQFuns();
})
</script>