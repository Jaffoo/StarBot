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
                <el-input v-model="wb.userPart" /><span style="color: red;">仅用于图片保存，一般是填返图用户</span>
            </el-form-item>
            <el-form-item label="转发至群" v-if="usebot">
                <el-switch v-model="wb.forwardGroup" active-text="转发" inactive-text="不转发" />
            </el-form-item>
            <el-form-item label="群qq号" v-if="wb.forwardGroup && usebot">
                <SearchGroup :value="wb.group" multiple :bot="bot" @change="(data) => wb.group = data"></SearchGroup>
            </el-form-item>
            <el-form-item label="转发至好友" v-if="usebot">
                <el-switch v-model="wb.forwardQQ" active-text="转发" inactive-text="不转发" />
            </el-form-item>
            <el-form-item label="好友qq" v-if="wb.forwardQQ && usebot">
                <SearchFriend :value="wb.qq" multiple :bot="bot" @change="(data) => wb.qq = data"></SearchFriend>
            </el-form-item>
            <el-form-item label="吃瓜用户">
                <el-input v-model="wb.chiGuaUser" /><span style="color: red;">监听某些用户是否发送了包含关键词的动态</span>
            </el-form-item>
            <el-form-item label="关键词">
                <el-input v-model="wb.keyword" />
            </el-form-item>
            <el-form-item label="转发至群" v-if="usebot">
                <el-switch v-model="wb.chiGuaForwardGroup" active-text="转发" inactive-text="不转发" />
            </el-form-item>
            <el-form-item label="群qq号" v-if="wb.chiGuaForwardGroup && usebot">
                <SearchGroup :value="wb.chiGuaGroup" multiple :bot="bot" @change="(data) => wb.chiGuaGroup = data">
                </SearchGroup>
            </el-form-item>
            <el-form-item label="转发至好友" v-if="usebot">
                <el-switch v-model="wb.chiGuaForwardQQ" active-text="转发" inactive-text="不转发" />
            </el-form-item>
            <el-form-item label="好友qq" v-if="wb.chiGuaForwardQQ && usebot">
                <SearchFriend :value="wb.chiGuaQQ" multiple :bot="bot" @change="(data) => wb.chiGuaQQ = data">
                </SearchFriend>
            </el-form-item>
            <el-form-item label="监听间隔"
                v-if="(wb.forwardQQ || wb.forwardGroup || wb.chiGuaForwardGroup || wb.chiGuaForwardQQ) && usebot">
                <el-input-number v-model="wb.timeSpan" />
            </el-form-item>
        </el-form>
    </el-card>
</template>

<script setup lang="ts" name="wb">
import { ref, type PropType } from 'vue'
import type { Bot, WB } from '@/class/model'
import type { FormInstance, FormRules } from 'element-plus';
import SearchFriend from './searchFriend.vue';
import SearchGroup from './searchGroup.vue';

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
    },
    bot: {
        type: Object as PropType<Bot>,
        default: () => {
            return {
                host: '',
                websocktPort: 0,
                httpPort: 0,
                token: ''
            }
        }
    },
    usebot: {
        type: Boolean,
        default: false
    }
})
const wbform = ref<FormInstance>();
const rules = ref<FormRules>(
    {
        userAll: [{ required: true, message: '请输入该值', trigger: 'blur' }]
    },
)
const validForm = async () => {
    return new Promise((resolve) => {
        wbform.value?.validate((valid: boolean) => {
            resolve(valid);
        });
    });
}
defineExpose({
    validForm
})
</script>