<template>
    <el-card class="card-kd">
        <template #header>
            <span id="kd">口袋48</span>
        </template>
        <el-form ref="kdform" :rules="rules" :model="model" label-width="150px" label-position="left">
            <el-form-item label="姓名" prop="idolName">
                <el-input v-model="model.idolName"></el-input>
            </el-form-item>
            <el-form-item label="IMServerId" prop="serverId">
                <el-input v-model="model.serverId"></el-input>
            </el-form-item>
            <el-form-item label="直播房间Id" prop="liveRoomId">
                <el-input v-model="model.liveRoomId"></el-input>
            </el-form-item>
            <el-form-item>
                <el-button @click="searchModel.show = true">查询小偶像信息</el-button>
            </el-form-item>
            <el-form-item label="IM账号" prop="account">
                <el-input v-model="model.account" />
            </el-form-item>
            <el-form-item label="IMtoken" prop="token">
                <el-input v-model="model.token" />
            </el-form-item>
            <el-form-item>
                <el-button @click="loginKD = true">登录口袋48</el-button>
                <span style="color:red">*IM账号和IMtoken可点此登录口袋后自动获取</span>
            </el-form-item>
            <el-form-item label="监听消息类型" v-show="model.forwardGroup === true || model.forwardQQ === true">
                <el-checkbox-group v-model="model.msgType">
                    <el-checkbox v-for="(item, index) in model.msgTypeAll" :label="item.value" :key="index">{{
                        item.name
                    }}</el-checkbox>
                </el-checkbox-group>
            </el-form-item>
            <el-form-item label="转发至群">
                <el-switch v-model="model.forwardGroup" active-text="转发" inactive-text="不转发" />
            </el-form-item>
            <el-form-item label="群qq号" v-if="model.forwardGroup">
                <el-input v-model="model.group" />
            </el-form-item>
            <el-form-item label="转发至好友">
                <el-switch v-model="model.forwardQQ" active-text="转发" inactive-text="不转发" />
            </el-form-item>
            <el-form-item label="好友qq" v-if="model.forwardQQ">
                <el-input v-model="model.qq" />
            </el-form-item>
        </el-form>
        <el-dialog title="登录口袋48" v-model="loginKD" :before-close="close" :close-on-click-modal="false">
            <el-form label-width="100px" :rules="rules" :model="loginfo">
                <el-form-item label="手机号" class="mt-4" prop="phone">
                    <el-input v-model="loginfo.phone" style="width:95%">
                        <template #prepend>+{{ loginfo.area }}</template>
                    </el-input>
                </el-form-item>
                <el-form-item label="验证码" prop="code">
                    <el-input type="primary" v-model="loginfo.code" style="width:65%"></el-input><el-button
                        v-show="!loginfo.hasSend" style="width:25%;margin-left:5%" @click="send">发送验证码</el-button><el-button
                        v-show="loginfo.hasSend" style="width:25%;margin-left:5%">{{ loginfo.sec }}秒</el-button>
                </el-form-item>
                <el-form-item>
                    <el-button type="primary" @click="login">登录</el-button>
                </el-form-item>
            </el-form>
        </el-dialog>
        <el-dialog title="查询小偶像 " v-model="searchModel.show" :before-close="close" :close-on-click-modal="false">
            <el-form label-width="100px" :rules="rules" :model="searchModel">
                <el-form-item label="队伍" class="mt-4">
                    <el-cascader :props="{ expandTrigger: 'hover', checkStrictly: 'true' }" placeholder="请选择"
                        v-model="searchModel.group" :options="groups" style="width:95%"></el-cascader>
                </el-form-item>
                <el-form-item label="姓名" prop="name">
                    <el-input v-model="searchModel.name" style="width:95%">
                    </el-input>
                </el-form-item>
                <el-form-item>
                    <el-button type="primary" :loading="searchModel.loading" @click="searchXox">查询</el-button>
                </el-form-item>
                <div v-if="searchModel.url" style="width:95%;margin-top:5px">
                    <span>未查询到小偶像，检查名称等后重新查询或者自行通过下方地址获取小偶像信息填入：</span>
                    <div style="word-break:break-all">{{ searchModel.url }}</div>
                </div>
            </el-form>
        </el-dialog>
    </el-card>
</template>

<script setup lang="ts" name="qq">
import { ref, type PropType, toRef } from 'vue'
import type { KD, MsgType } from '@/class/model'
import { ElMessage, type FormInstance, type FormRules } from 'element-plus';
import { kdUserInfo, pocketLogin, sendSmsCode, searchIdol } from "@/api"

const props = defineProps({
    kd: {
        type: Object as PropType<KD>,
        default: {
            group: '',
            forwardGroup: false,
            forwardQQ: false,
            qq: '',
            idolName: '',
            account: '',
            token: '',
            serverId: '',
            liveRoomId: '',
            msgTypeAll: new Array<MsgType>,
            msgType: [],
            saveMsg: false
        }
    }
})
const model = toRef(props.kd);
const kdform = ref<FormInstance>();
const rules = ref<FormRules>(
    {
        idolName: [{ required: true, message: '请输入该值', trigger: 'blur' }],
        serverId: [{ required: true, message: '请输入该值', trigger: 'blur' }],
        liveRoomId: [{ required: true, message: '请输入该值', trigger: 'blur' }],
        account: [{ required: true, message: '请输入该值', trigger: 'blur' }],
        token: [{ required: true, message: '请输入该值', trigger: 'blur' }],
        phone: [{ required: true, message: '请输入该值', trigger: 'blur' }],
        code: [{ required: true, message: '请输入该值', trigger: 'blur' }],
        name: [{ required: true, message: '请输入该值', trigger: 'blur' }],
    }
)
const loginKD = ref(false)
const searchModel = ref({
    show: false,
    group: [],
    name: '',
    loading: false,
    url: ''
})
const loginfo = ref({
    phone: '',
    area: '86',
    hasSend: false,
    code: '',
    sec: 60
})

const groups = ref(
    [{
        label: 'SNH48',
        value: 'SNH48',
        children: [
            { value: "TEAM SII", label: "TEAM SII" },
            { value: "TEAM HII", label: "TEAM HII" },
            { value: "TEAM X", label: "TEAM X" },
        ]
    }, {
        label: 'GNZ48',
        value: 'GNZ48',
        children: [
            { value: "TEAM G", label: "TEAM G" },
            { value: "TEAM NIII", label: "TEAM NIII" },
            { value: "TEAM Z", label: "TEAM Z" },
            { value: "TEAM CII", label: "TEAM CII" },
        ]
    }, {
        label: 'CGT48',
        value: 'CGT48',
        children: [
            { value: "TEAM GII", label: "TEAM GII" },
        ]
    }, {
        label: 'BEJ48',
        value: 'BEJ48',
        children: [
            { value: "TEAM B", label: "TEAM B" },
            { value: "TEAM E", label: "TEAM E" }
        ]
    }, {
        label: 'CKG48',
        value: 'CKG48',
    }]
)
const searchXox = async () => {
    if (!searchModel.value.name) {
        ElMessage({ message: "请输入小偶像姓名", type: 'error' });
        return;
    }
    searchModel.value.loading = true;
    var res = await searchIdol(searchModel.value.group.toString(), searchModel.value.name);
    if (res.success) {
        var data = res.data;
        model.value.idolName = data.name;
        model.value.liveRoomId = data.liveId;
        model.value.serverId = data.serverId;
        close();
    } else {
        searchModel.value.url = res.data.data;
    }
    searchModel.value.loading = false;
}
const close = () => {
    searchModel.value.loading = false;
    searchModel.value.show = false;
    searchModel.value.name = '';
    searchModel.value.url = "";
    searchModel.value.group = [];
    loginKD.value = false;
    loginfo.value.hasSend = false;
    loginfo.value.sec = 60;
}
const send = async () => {
    if (!loginfo.value.phone) {
        ElMessage({ message: "请输入手机号码！", type: 'warning' });
        return;
    }
    var patrn = /^1[3456789]\d{9}$/;
    if (patrn.test(loginfo.value.phone) == false) {
        ElMessage({ message: "手机号码格式有误，请重新输入！", type: 'warning' });
        return;
    }
    loginfo.value.hasSend = true;
    subtraction();
    var res = await sendSmsCode(loginfo.value.phone, loginfo.value.area);
    if (res.success) {
        ElMessage({ message: "发送成功，请注意查收！", type: 'success' });
    } else {
        ElMessage({ message: res.msg ?? "发送失败！", type: 'error' });
    }
}
const login = async () => {
    if (!loginfo.value.phone) {
        ElMessage({ message: "请输入手机号码！", type: 'warning' });
        return;
    }
    if (!loginfo.value.code) {
        ElMessage({ message: "请输入验证码！", type: 'warning' });
        return;
    }
    var patrn = /^1[3456789]\d{9}$/;
    if (patrn.test(loginfo.value.phone) == false) {
        ElMessage({ message: "手机号码格式有误，请重新输入！", type: 'warning' });
        return;
    }
    var res = await pocketLogin(loginfo.value.phone, loginfo.value.code);
    if (res.success) {
        var tokenRes = await kdUserInfo(res.data.content.token);
        if (tokenRes.success) {
            model.value.token = tokenRes.data.content.pwd;
            model.value.account = tokenRes.data.content.accid;
            setTimeout(() => {
                close();
            }, 1000);
            ElMessage({ message: tokenRes.msg ?? '登录成功', type: 'success' });
        } else {
            ElMessage({ message: tokenRes.msg ?? '登录失败！', type: 'error' });
        }
    } else {
        ElMessage({ message: res.msg ?? '登录失败！', type: 'error' });
    }
}
const subtraction = () => {
    loginfo.value.sec = 60;
    let timer: number;
    timer = setInterval(() => {
        if (loginfo.value.sec > 0) {
            loginfo.value.sec--;
        } else {
            clearInterval(timer);
            loginfo.value.sec = 60;
            loginfo.value.hasSend = false;
        }
    }, 1000);
}
const validForm = () => {
    kdform.value?.validate(valid => {
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