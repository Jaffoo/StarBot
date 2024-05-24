<template>
  <el-affix target=".el-scrollbar__view">
    <el-card id="enable">
      <template #header>
        <el-button type="primary" native-type="button" :icon="Edit" @click="save()">保存</el-button>
        <el-button type="primary" native-type="button" :icon="Setting" @click="reset()">重置</el-button>
      </template>
      <Enable :enable="model.enableModule" @enable-change="enableChange" />
    </el-card>
  </el-affix>
  <el-scrollbar style="height: calc(100vh - 230px)" id="pdom">
    <Bot ref="botRef" :bot="model.bot" v-if="model.enableModule.bot" class="mt10" />
    <QQ ref="qqRef" :qq="model.qq" v-if="model.enableModule.qq" class="mt10" />
    <WB ref="wbRef" :wb="model.wb" v-if="model.enableModule.wb" class="mt10" />
    <KD ref="kdRef" :kd="model.kd" v-if="model.enableModule.kd" class="mt10" />
    <BZ ref="bzRef" :bz="model.bz" v-if="model.enableModule.bz" class="mt10" />
    <XHS ref="xhsRef" :xhs="model.xhs" v-if="model.enableModule.xhs" class="mt10" />
    <BD ref="bdRef" :bd="model.bd" v-if="model.enableModule.bd" />
    <DY ref="dyRef" :dy="model.dy" v-if="model.enableModule.dy" class="mt10 mb50" />
  </el-scrollbar>
</template>

<script setup lang="ts">
import { Edit, Setting } from "@element-plus/icons-vue";
import Enable from "@/component/enable.vue";
import Bot from "@/component/bot.vue";
import QQ from "@/component/qq.vue";
import WB from "@/component/wb.vue";
import KD from "@/component/kd.vue";
import BZ from "@/component/bz.vue";
import XHS from "@/component/xhs.vue";
import BD from "@/component/bd.vue";
import DY from "@/component/dy.vue";
import { type PropType, onMounted, ref } from "vue";
import type { Config, EnableModule } from "@/class/model";
import { getConfig, saveConfig } from "@/api";

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
      bd: false,
    },
  },
});
const botRef = ref();
const wbRef = ref();
const qqRef = ref();
const bzRef = ref();
const bdRef = ref();
const kdRef = ref();
const xhsRef = ref();
const dyRef = ref();

const model = ref<Config>({
  enableModule: props.enable,
});
const startModel = ref<Config>();
// 定义子组件向父组件传值/事件
const emit = defineEmits(["top-enable-change", 'change-menu', 'config-change']);

const enableChange = (enableNew: EnableModule) => {
  model.value.enableModule = enableNew;
  emit("top-enable-change", enableNew);
};

const save = async () => {
  let valid = true;
  if (model.value.enableModule.bot) valid = await botRef.value.validForm();
  if (!valid) {
    emit('change-menu', 'bot')
    ElMessage.error("请填入机器人配置中的必填项！");
    return;
  }
  if (model.value.enableModule.wb) valid = await wbRef.value.validForm();
  if (!valid) {
    emit('change-menu', 'wb')
    ElMessage.error("请填入微博配置中的必填项！");
    return;
  }
  if (model.value.enableModule.qq) valid = await qqRef.value.validForm();
  if (!valid) {
    emit('change-menu', 'qq')
    ElMessage.error("请填入QQ配置中的必填项！");
    return;
  }
  if (model.value.enableModule.bz) valid = await bzRef.value.validForm();
  if (!valid) {
    emit('change-menu', 'bz')
    ElMessage.error("请填入B站配置中的必填项！");
    return;
  }
  if (model.value.enableModule.bd) valid = await bdRef.value.validForm();
  if (!valid) {
    emit('change-menu', 'bd')
    ElMessage.error("请填入百度配置中的必填项！");
    return;
  }
  if (model.value.enableModule.kd) valid = await kdRef.value.validForm();
  if (!valid) {
    emit('change-menu', 'kd')
    ElMessage.error("请填入口袋配置中的必填项！");
    return;
  }
  if (model.value.enableModule.xhs) valid = await xhsRef.value.validForm();
  if (!valid) {
    emit('change-menu', 'xhs')
    ElMessage.error("请填入小红书配置中的必填项！");
    return;
  }
  if (model.value.enableModule.dy) valid = await dyRef.value.validForm();
  if (!valid) {
    emit('change-menu', 'dy')
    ElMessage.error("请填入抖音配置中的必填项！");
    return;
  }

  var res = await saveConfig(model.value);
  if (res && res.success) {
    startModel.value = model.value;
    ElMessage.success(res.msg);
    emit('config-change');
  } else ElMessage.error(res.msg || "保存失败！");
};

const reset = async () => {
  var res = await getConfig();
  if (res && res.success) {
    model.value.enableModule = res.data.enableModule;
    model.value.bot = res.data.bot;
    model.value.qq = res.data.qq;
    model.value.wb = res.data.wb;
    model.value.kd = res.data.kd;
    model.value.bz = res.data.bz;
    model.value.bd = res.data.bd;
    model.value.xhs = res.data.xhs;
    model.value.dy = res.data.dy;
  }
  let deepStr = JSON.stringify(model.value);
  let deepObj = JSON.parse(deepStr);
  startModel.value = deepObj;
};

const checkData = (): Promise<boolean> => {
  let temp1 = JSON.stringify(startModel.value);
  let temp2 = JSON.stringify(model.value);
  if (temp1 !== temp2) {
    return ElMessageBox.confirm("配置已被修改，但未保存，是否保存", "警告", {
      confirmButtonText: "保存",
      cancelButtonText: "取消",
      type: "warning",
      closeOnClickModal: false
    })
      .then(async () => {
        await save();
        return true;
      })
      .catch(async () => {
        return true;
      });
  } else
    return new Promise<boolean>((resolve, reject) => {
      resolve(true);
    });
};

onMounted(async () => {
  await reset();
});

defineExpose({
  checkData,
});
</script>

<style>
.mt10 {
  margin-top: 10px;
}

.mt50 {
  margin-top: 50px;
}

.mb50 {
  margin-bottom: 50px;
}
</style>
