<template>
    <el-card class="card-xhs">
        <template #header>
            <span id="xhs">小红书</span>
        </template>
        <el-form ref="xhsform" :model="xhs" :rules="rules" label-width="150px" label-position="left">
            <el-form-item label="小红书用户" prop="user">
                <el-input v-model="xhs.user" />
            </el-form-item>
            <el-form-item label="转发至群" v-if="usebot">
                <el-switch v-model="xhs.forwardGroup" active-text="转发" inactive-text="不转发" />
            </el-form-item>
            <el-form-item label="群qq号" v-if="xhs.forwardGroup && usebot">
                <SearchGroup :value="xhs.group" :bot="bot" multiple @change="(val) => xhs.group = val" />
            </el-form-item>
            <el-form-item label="转发至好友" v-if="usebot">
                <el-switch v-model="xhs.forwardQQ" active-text="转发" inactive-text="不转发" />
            </el-form-item>
            <el-form-item label="好友qq" v-if="xhs.forwardQQ && usebot">
                <SearchFriend :value="xhs.qq" :bot="bot" multiple @change="(val) => xhs.qq = val" />
            </el-form-item>
            <el-form-item label="监听间隔" v-if="(xhs.forwardQQ || xhs.forwardGroup) && usebot">
                <el-input-number v-model="xhs.timeSpan" />
            </el-form-item>
        </el-form>
    </el-card>
</template>

<script setup lang="ts" name="xhs">
import { ref, type PropType } from 'vue'
import type { Bot, DY_XHS_BZ as XHS } from '@/class/model'
import type { FormInstance, FormRules } from 'element-plus';
import SearchFriend from './searchFriend.vue';
import SearchGroup from './searchGroup.vue';

const props = defineProps({
    xhs: {
        type: Object as PropType<XHS>,
        default: {
            user: '',
            group: '',
            forwardGroup: false,
            forwardQQ: false,
            qq: '',
            timeSpan: 3
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
const xhsform = ref<FormInstance>();
const rules = ref<FormRules>(
    {
        user: [{ required: true, message: '请输入该值', trigger: 'blur' }]
    },
)
const validForm = async () => {
    return new Promise((resolve) => {
        xhsform.value?.validate((valid: boolean) => {
            resolve(valid);
        });
    });
}
defineExpose({
    validForm
})
</script>