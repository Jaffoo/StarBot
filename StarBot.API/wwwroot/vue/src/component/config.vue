<template>
    <el-affix target=".el-scrollbar__view">
        <el-card id="enable">
            <template #header>
                <el-button type="primary" native-type="button" :icon="Edit" @click="save()">保存</el-button>
                <el-button type="primary" native-type="button" :icon="Setting" @click="reset()">重置</el-button>
            </template>
            <Enable :enable="model.EnableModule" @enable-change="enableChange" />
        </el-card>
    </el-affix>
    <el-scrollbar style="height: calc(100vh - 230px);" id="pdom">
        <Shamrock ref="botRef" :shamrock="model.Shamrock" v-if="model.EnableModule.shamrock" class="mt10" />
        <QQ ref="qqRef" :qq="model.QQ" v-if="model.EnableModule.qq" class="mt10" />
        <WB ref="wbRef" :wb="model.WB" v-if="model.EnableModule.wb" class="mt10" />
        <KD ref="kdRef" :kd="model.KD" v-if="model.EnableModule.kd" class="mt10" />
        <BZ ref="bzRef" :bz="model.BZ" v-if="model.EnableModule.bz" class="mt10" />
        <XHS ref="xhsRef" :xhs="model.XHS" v-if="model.EnableModule.xhs" class="mt10" />
        <BD ref="bdRef" :bd="model.BD" v-if="model.EnableModule.bd" class="mt10" />
        <DY ref="dyRef" :dy="model.DY" v-if="model.EnableModule.dy" class="mt10 mb50" />
    </el-scrollbar>
</template>

<script setup lang="ts">
import { Edit, Setting } from '@element-plus/icons-vue'
import Enable from '@/component/enable.vue';
import Shamrock from '@/component/shamrock.vue';
import QQ from '@/component/qq.vue';
import WB from '@/component/wb.vue';
import KD from '@/component/kd.vue';
import BZ from '@/component/bz.vue';
import XHS from '@/component/xhs.vue';
import BD from '@/component/bd.vue';
import DY from '@/component/dy.vue';
import { type PropType, onMounted, ref } from "vue"
import type { Config, EnableModule } from "@/class/model";
import { getConfig, saveConfig } from "@/api"

const props = defineProps({
    enable: {
        type: Object as PropType<EnableModule>,
        default: {
            qq: false,
            wb: false,
            bz: false,
            kd: false,
            xhs: false,
            dy: false,
            bd: false
        }
    }
})
const botRef = ref();
const wbRef = ref();
const qqRef = ref();
const bzRef = ref();
const bdRef = ref();
const kdRef = ref();
const xhsRef = ref();
const dyRef = ref();

const model = ref<Config>({
    EnableModule: props.enable
});
// 定义子组件向父组件传值/事件
const emit = defineEmits(['top-enable-change']);

const enableChange = (enableNew: EnableModule) => {
    model.value.EnableModule = enableNew
    emit("top-enable-change", enableNew)
}

const save = async () => {
    let valid = true;
    if (model.value.EnableModule.shamrock) valid = await botRef.value.validForm();
    if (model.value.EnableModule.wb) valid = await wbRef.value.validForm();
    if (model.value.EnableModule.qq) valid = await qqRef.value.validForm();
    if (model.value.EnableModule.bz) valid = await bzRef.value.validForm();
    if (model.value.EnableModule.bd) valid = await bdRef.value.validForm();
    if (model.value.EnableModule.kd) valid = await kdRef.value.validForm();
    if (model.value.EnableModule.xhs) valid = await xhsRef.value.validForm();
    if (model.value.EnableModule.dy) valid = await dyRef.value.validForm();
    if (!valid) {
        ElMessage.error("请填入配置中的必填项！");
        return;
    }
    var res = await saveConfig(model.value);
    if (res && res.success)
        ElMessage.success(res.msg);
    else
        ElMessage.error(res.msg || "保存失败！")
}

const reset = async () => {
    var res = await getConfig();
    if (res && res.success) {
        model.value.EnableModule = res.data.enableModule
        model.value.Shamrock = res.data.shamrock
        model.value.QQ = res.data.qq
        model.value.WB = res.data.wb
        model.value.KD = res.data.kd
        model.value.BZ = res.data.bz
        model.value.BD = res.data.bd
        model.value.XHS = res.data.xhs
        model.value.DY = res.data.dy
    }
}

onMounted(async () => {
    await reset();
})
</script>

<style>
.mt10 {
    margin-top: 10px;
}

.mb50 {
    margin-bottom: 50px;
}
</style>