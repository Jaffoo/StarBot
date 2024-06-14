<template>
    <el-card class="card-qq">
        <template #header>
            <span id="qq">QQ</span>
        </template>
        <el-form ref="qqform" :model="qq" :rules="rules" label-width="150px" label-position="left">
            <el-form-item label="超管" prop="admin">
                <SearchFriend :value="qq.admin" @change="(data)=>qq.admin=data" :bot="bot"></SearchFriend>
            </el-form-item>
            <el-form-item label="群" prop="group">
                <SearchGroup :value="qq.group" multiple @change="(data)=>qq.group=data" :bot="bot"></SearchGroup>
            </el-form-item>
            <el-form-item label="管理员" prop="permission">
                <SearchMember :value="qq.permission" multiple @change="(data)=>qq.permission=data" :group='qq.group' :bot="bot"></SearchMember>
            </el-form-item>
            <el-form-item label="程序错误通知">
                <el-switch v-model="qq.debug" active-text="启用" inactive-text="禁用" />
            </el-form-item>
            <el-form-item label="普通消息通知">
                <el-switch v-model="qq.notice" active-text="启用" inactive-text="禁用" />
            </el-form-item>
            <el-form-item label="保存群消息">
                <el-switch v-model="qq.save" active-text="保存" inactive-text="不保存" />
            </el-form-item>
        </el-form>
    </el-card>
</template>

<script setup lang="ts" name="qq">
import { ref, type PropType } from 'vue'
import type { Bot, QQ } from '@/class/model'
import type { FormInstance, FormRules } from 'element-plus';
import SearchFriend from './searchFriend.vue';
import SearchGroup from './searchGroup.vue';
import SearchMember from './searchMember.vue';

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
    },
    bot: {
        type: Object as PropType<Bot>,
        default: {
            host: '',
            websocktPort: '',
            httpPort: '',
            token: ''
        }
    },
    new: {
        type: Boolean,
        default: false
    }
})

const qqform = ref<FormInstance>();
const rules = ref<FormRules>(
    {
        admin: [{ required: true, message: '请输入该值', trigger: 'blur' }],
    }
)
const validForm = async () => {
    return new Promise((resolve) => {
        qqform.value?.validate((valid: boolean) => {
            resolve(valid);
        });
    });
}

defineExpose({
    validForm
})
</script>