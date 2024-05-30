<template>
    <el-card class="card-qq">
        <template #header>
            <span id="qq">QQ</span>
        </template>
        <el-form ref="qqform" :model="qq" :rules="rules" label-width="150px" label-position="left">
            <el-form-item label="超管" prop="admin">
                <el-select v-model="superAdmin.value" filterable remote :remote-method="searchSuperAdmin"
                    :loading="superAdmin.loading" placeholder="请选择好友" @change="superChange">
                    <el-option v-for="item in superAdmin.options" :key="item.user_id" :label="item.nickname"
                        :value="item.user_id">
                    </el-option>
                </el-select>
            </el-form-item>
            <el-form-item label="群" prop="group">
                <el-select v-model="group.value" filterable remote multiple :remote-method="searchGroup"
                    :loading="group.loading" placeholder="请选择群" @change="groupChange">
                    <el-option v-for="item in group.options" :key="item.group_id" :label="item.group_name"
                        :value="item.group_id">
                    </el-option>
                </el-select>
            </el-form-item>
            <el-form-item label="管理员" prop="permission">
                <el-select v-model="admin.value" filterable remote multiple :remote-method="searchAdmin"
                    :loading="admin.loading" placeholder="请选择群成员" @change="memberChange">
                    <el-option v-for="item in admin.options" :key="item.user_id" :label="item.nickname"
                        :value="item.user_id">
                    </el-option>
                </el-select>
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
import { onMounted, ref, type PropType } from 'vue'
import type { Bot, QQ } from '@/class/model'
import type { FormInstance, FormRules } from 'element-plus';
import { getSuperAdmin, getAdmin, getGroup } from '@/api/index'

const props = defineProps({
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
const superAdmin = ref({
    value: '',
    loading: false,
    options: [] as any[],
})
const admin = ref({
    value: [] as any[],
    loading: false,
    options: [] as any[],
})
const group = ref({
    value: [] as any[],
    loading: false,
    options: [] as any[],
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

const searchSuperAdmin = async (keywords: string, first = false) => {
    if (!props.bot.host && !props.bot.websocktPort && !props.bot.httpPort) {
        ElMessage.warning("请先完填入bot信息")
        return;
    }
    if (first && !props.qq.admin) return;
    superAdmin.value.loading = true;
    var res = await getSuperAdmin(keywords, props.bot.host, props.bot.websocktPort, props.bot.httpPort, props.bot.token)
    if (res.success) {
        superAdmin.value.options = res.data
        superAdmin.value.loading = false;
        if (first)
            superAdmin.value.value = superAdmin.value.options.find(x => x.user_id == props.qq.admin)?.nickname
    } else {
        ElMessage.error(res.msg)
        superAdmin.value.loading = false;
    }
}

const searchGroup = async (keywords: string, first = false) => {
    if (!props.bot.host && !props.bot.websocktPort && !props.bot.httpPort) {
        ElMessage.warning("请先完填入bot信息")
        return;
    }
    if (first && !props.qq.group) return;
    group.value.loading = true;
    var res = await getGroup(keywords, props.bot.host, props.bot.websocktPort, props.bot.httpPort, props.bot.token)
    if (res.success) {
        group.value.options = res.data
        group.value.loading = false;
        if (first)
            group.value.value = group.value.options.filter(x => props.qq.group!.includes(x.group_id)).map(e => e.group_name)
    } else {
        ElMessage.error(res.msg)
        group.value.loading = false;
    }
}

const searchAdmin = async (keywords: string, first = false) => {
    if (!props.bot.host && !props.bot.websocktPort && !props.bot.httpPort) {
        ElMessage.warning("请先完填入bot信息")
        return;
    }
    if (!props.qq.group) return;
    if (first && !props.qq.permission) return;
    admin.value.loading = true;
    var res = await getAdmin(props.qq.group, keywords, props.bot.host, props.bot.websocktPort, props.bot.httpPort, props.bot.token)
    if (res.success) {
        admin.value.options = res.data
        admin.value.loading = false;
        if (first)
            admin.value.value = admin.value.options.filter(x => props.qq.permission!.includes(x.user_id)).map(e => e.nickname)
    } else {
        ElMessage.error(res.msg)
        admin.value.loading = false;
    }
}

const superChange = () => {
    props.qq.admin = superAdmin.value.value?.toString()
}

const groupChange = () => {
    props.qq.group = group.value.value?.toString()
}

const memberChange = () => {
    props.qq.permission = admin.value.value.toString()
}

defineExpose({
    validForm
})

onMounted(() => {
    setTimeout(async () => {
        if (!props.new) {
            await searchAdmin("", true);
            await searchGroup("", true);
            await searchSuperAdmin("", true);
        }
    }, 100);
})


</script>