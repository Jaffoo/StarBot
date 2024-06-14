<template>
    <el-card class="card-dy">
        <template #header>
            <span id="dy">抖音</span>
        </template>
        <el-form ref="dyform" :model="dy" :rules="rules" label-width="150px" label-position="left">
            <el-form-item label="抖音用户" prop="user">
                <el-input v-model="dy.user" />
            </el-form-item>
            <el-form-item label="转发至群">
                <el-switch v-model="dy.forwardGroup" active-text="转发" inactive-text="不转发" />
            </el-form-item>
            <el-form-item label="群qq号" v-if="dy.forwardGroup">
                <SearchGroup :value="dy.group" multiple :bot="bot" @change="(data) => dy.group = data"></SearchGroup>
            </el-form-item>
            <el-form-item label="转发至好友">
                <el-switch v-model="dy.forwardQQ" active-text="转发" inactive-text="不转发" />
            </el-form-item>
            <el-form-item label="好友qq" v-if="dy.forwardQQ">
                <SearchFriend :value="dy.qq" multiple :bot="bot" @change="(data) => dy.qq = data"></SearchFriend>
            </el-form-item>
            <el-form-item label="监听间隔" v-if="dy.forwardQQ || dy.forwardGroup">
                <el-input-number v-model="dy.timeSpan" />
            </el-form-item>
        </el-form>
    </el-card>
</template>

<script setup lang="ts" name="dy">
import { ref, type PropType } from 'vue'
import type { Bot, DY_XHS_BZ as DY } from '@/class/model'
import type { FormInstance, FormRules } from 'element-plus';
import SearchFriend from './searchFriend.vue';
import SearchGroup from './searchGroup.vue';

defineProps({
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
})
const dyform = ref<FormInstance>();
const rules = ref<FormRules>(
    {
        user: [{ required: true, message: '请输入该值', trigger: 'blur' }]
    },
)
const validForm = async () => {
    return new Promise((resolve) => {
        dyform.value?.validate((valid: boolean) => {
            resolve(valid);
        });
    });
}
defineExpose({
    validForm
})
</script>