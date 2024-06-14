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
                <SearchGroup :value="bz.group" multiple :bot="bot" @change="(data) => bz.group = data"></SearchGroup>
            </el-form-item>
            <el-form-item label="转发至好友">
                <el-switch v-model="bz.forwardQQ" active-text="转发" inactive-text="不转发" />
            </el-form-item>
            <el-form-item label="好友qq" v-if="bz.forwardQQ">
                <SearchFriend :value="bz.qq" multiple :bot="bot" @change="(data) => bz.qq = data"></SearchFriend>
            </el-form-item>
            <el-form-item label="监听间隔" v-if="bz.forwardQQ || bz.forwardGroup">
                <el-input-number v-model="bz.timeSpan" />
            </el-form-item>
        </el-form>
    </el-card>
</template>

<script setup lang="ts" name="bz">
import { ref, type PropType } from 'vue'
import type { DY_XHS_BZ as BZ, Bot } from '@/class/model'
import type { FormInstance, FormRules } from 'element-plus';
import SearchFriend from './searchFriend.vue';
import SearchGroup from './searchGroup.vue';

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