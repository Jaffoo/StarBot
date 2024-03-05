<template>
    <el-card class="card-wb">
        <template #header>
            <span id="wb">微博</span>
        </template>
        <el-form ref="wbform" :model="wb" :rules="rules" label-width="150px" label-position="left">
            <el-form-item label="微博用户" prop="userAll">
                <el-input v-model="wb.userAll" /><span style="color: red;">用于转发动态和图片保存，一般是填idol</span>
            </el-form-item>
            <el-form-item label="返图用户">
                <el-input v-model="wb.userPart" /><span style="color: red;">仅用于图片保存，一般是填返图用户)</span>
            </el-form-item>
            <el-form-item label="转发至群">
                <el-switch v-model="wb.forwardGroup" active-text="转发" inactive-text="不转发" />
            </el-form-item>
            <el-form-item label="群qq号" v-if="wb.forwardGroup">
                <el-input v-model="wb.group" />
            </el-form-item>
            <el-form-item label="转发至好友">
                <el-switch v-model="wb.forwardQQ" active-text="转发" inactive-text="不转发" />
            </el-form-item>
            <el-form-item label="好友qq" v-if="wb.forwardQQ">
                <el-input v-model="wb.qq" />
            </el-form-item>
            <el-form-item label="吃瓜用户">
                <el-input v-model="wb.chiGuaUser" /><span style="color: red;">监听某些用户是否发送了包含关键词的动态</span>
            </el-form-item>
            <el-form-item label="关键词">
                <el-input v-model="wb.keyword" />
            </el-form-item>
            <el-form-item label="转发至群">
                <el-switch v-model="wb.chiGuaForwardGroup" active-text="转发" inactive-text="不转发" />
            </el-form-item>
            <el-form-item label="群qq号" v-if="wb.chiGuaForwardGroup">
                <el-input v-model="wb.chiGuaGroup" />
            </el-form-item>
            <el-form-item label="转发至好友">
                <el-switch v-model="wb.chiGuaForwardQQ" active-text="转发" inactive-text="不转发" />
            </el-form-item>
            <el-form-item label="好友qq" v-if="wb.chiGuaForwardQQ">
                <el-input v-model="wb.chiGuaQQ" />
            </el-form-item>
            <el-form-item label="监听间隔"
                v-if="wb.forwardQQ || wb.forwardGroup || wb.chiGuaForwardGroup || wb.chiGuaForwardQQ">
                <el-input-number v-model="wb.timeSpan" />
            </el-form-item>
        </el-form>
    </el-card>
</template>

<script setup lang="ts" name="wb">
import { ref, type PropType } from 'vue'
import type { WB } from '@/class/model'
import type { FormInstance, FormRules } from 'element-plus';
const props = defineProps({
    wb: {
        type: Object as PropType<WB>,
        default: {
            userAll: '',
            userPart: '',
            chiGuaUser: '',
            keyword: '',
            chiGuaQQ: '',
            chiGuaForwardQQ: false,
            chiGuaGroup: '',
            chiGuaForwardGroup: false,
            timeSpan: 3,
            group: '',
            forwardGroup: false,
            forwardQQ: false,
            qq: '',
        }
    }
})
const wbform = ref<FormInstance>();
const rules = ref<FormRules>(
    {
        userAll: [{ required: true, message: '请输入该值', trigger: 'blur' }]
    },
)
const validForm = async () => {
    return await wbform.value?.validate(valid => {
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