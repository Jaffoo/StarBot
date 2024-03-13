<template>
  <el-container class="layout-container">
    <el-card>
      <el-aside width="15vw">
        <el-scrollbar style="height: calc(100vh - 30px)">
          <el-menu :default-openeds="['99']" default-active="1">
            <el-menu-item index="1" @click="changeMenu('index')">
              <el-icon>
                <House />
              </el-icon>
              开始
            </el-menu-item>
            <el-menu-item index="2" @click="changeMenu('log')">
              <el-icon>
                <House />
              </el-icon>
              日志
            </el-menu-item>
            <el-menu-item index="3" @click="changeMenu('pic')">
              <el-icon>
                <House />
              </el-icon>
              图片
            </el-menu-item>
            <el-menu-item
              index="4"
              @click="changeMenu('plugin')"
              v-show="enable.qq"
            >
              <el-icon>
                <House />
              </el-icon>
              插件
            </el-menu-item>
            <el-sub-menu index="99">
              <template #title>
                <el-icon> <Cpu /> </el-icon>配置
              </template>
              <el-menu-item index="99-0" @click="changeMenu('config')"
                >启用模块</el-menu-item
              >
              <el-menu-item
                index="99-1"
                @click="scrollSet('shamrock')"
                v-show="enable.shamrock"
                >Shamrock</el-menu-item
              >
              <el-menu-item
                index="99-2"
                @click="scrollSet('qq')"
                v-show="enable.qq"
                >QQ</el-menu-item
              >
              <el-menu-item
                index="99-3"
                @click="scrollSet('wb')"
                v-show="enable.wb"
                >微博</el-menu-item
              >
              <el-menu-item
                index="99-4"
                @click="scrollSet('kd')"
                v-show="enable.kd"
                >口袋</el-menu-item
              >
              <el-menu-item
                index="99-5"
                @click="scrollSet('bz')"
                v-show="enable.bz"
                >B站</el-menu-item
              >
              <el-menu-item
                index="99-6"
                @click="scrollSet('xhs')"
                v-show="enable.xhs"
                >小红书</el-menu-item
              >
              <el-menu-item
                index="99-7"
                @click="scrollSet('bd')"
                v-show="enable.bd"
                >百度</el-menu-item
              >
              <el-menu-item
                index="99-8"
                @click="scrollSet('dy')"
                v-show="enable.dy"
                >抖音</el-menu-item
              >
            </el-sub-menu>
          </el-menu>
        </el-scrollbar>
      </el-aside>
    </el-card>
    <el-card style="width: 90%; margin-left: 10px">
      <el-container>
        <el-main
          style="padding-left: 20px; padding-right: 20px"
          id="parentContainer"
        >
          <Index :enable="enable" v-show="component === 'index'"/>
          <Log v-show="component === 'log'" />
          <Pic v-show="component === 'pic'" />
          <Plugin v-if="component === 'plugin' && enable.qq" />
          <Config
            ref="configRef"
            :enable="enable"
            v-if="component === 'config'"
            @top-enable-change="enableChange"
          />
        </el-main>
      </el-container>
    </el-card>
  </el-container>
</template>

<script setup lang="ts">
import Index from "@/component/index.vue";
import Config from "@/component/config.vue";
import Log from "@/component/log.vue";
import Pic from "@/component/pic.vue";
import Plugin from "@/component/plugin.vue";
import { House, Cpu } from "@element-plus/icons-vue";
import { onMounted, ref } from "vue";
import type { EnableModule } from "./class/model";
import { getConfig } from "./api";
const startEnble = ref<EnableModule>();
const enable = ref<EnableModule>({
  shamrock: false,
  qq: false,
  wb: false,
  bz: false,
  kd: false,
  xhs: false,
  dy: false,
  bd: false,
});
const menu = ["index", "log", "pic", "plugin"];
const configRef = ref();
const component = ref("index");
const changeMenu = async (com: string) => {
  if (menu.find((x) => x === com)) {
    if (configRef.value) {
      await configRef.value.checkData();
      await getEnableModule();
    }
    component.value = com;
  } else {
    scrollSet("enable");
  }
};

const scrollSet = async (com: string) => {
  if (menu.find((x) => x === component.value)) {
    component.value = "config";
    setTimeout(async () => {
      await scrollSet(com);
    }, 100);
  }
  const childElement = document.getElementById(com);
  const parentContainer = document.getElementById("pdom")?.parentElement;

  if (parentContainer && childElement) {
    const parentRect = parentContainer.getBoundingClientRect();
    const childRect = childElement.getBoundingClientRect();
    const scrollTop =
      childRect.top + parentContainer.scrollTop - parentRect.top - 28;

    parentContainer.scrollTo({
      top: scrollTop,
      behavior: "smooth",
    });
  }
};
const enableChange = (enableNew: EnableModule) => {
  enable.value = enableNew;
};
const getEnableModule = async () => {
  let res = await getConfig();
  enable.value = res.data.enableModule;
  let deepStr = JSON.stringify(res.data.enableModule);
  let deepObj = JSON.parse(deepStr);
  startEnble.value = deepObj;
};

onMounted(async () => {
  await getEnableModule();
});
</script>

<style scoped>
.layout-container {
  height: calc(100vh - 18px);
}

.layout-container .el-aside {
  color: var(--el-text-color-primary);
}

.layout-container .el-menu {
  border-right: none;
}

.layout-container .el-main {
  padding: 0;
}

.layout-container .toolbar {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  height: 100%;
  right: 20px;
}

.el-main-height {
  height: calc(100% - 400px);
}
</style>
