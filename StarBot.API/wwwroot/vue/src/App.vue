<template>
  <el-container class="layout-container">
    <el-card>
      <el-aside width="15vw">
        <el-scrollbar style="height: calc(100vh - 70px);padding-right: 20px;">
          <el-menu :default-openeds="['99']" default-active="1">
            <el-menu-item index="1" @click="changeMenu('index')">
              <el-icon>
                <House />
              </el-icon>
              开始
            </el-menu-item>
            <el-menu-item index="2" @click="changeMenu('log')">
              <el-icon>
                <Tickets />
              </el-icon>
              日志
            </el-menu-item>
            <el-menu-item index="3" @click="changeMenu('pic')">
              <el-icon>
                <Picture />
              </el-icon>
              图片
            </el-menu-item>
            <el-menu-item index="4" @click="changeMenu('plugin')" v-show="enable.qq">
              <el-icon>
                <Cpu />
              </el-icon>
              插件
            </el-menu-item>
            <el-sub-menu index="99">
              <template #title>
                <el-icon>
                  <SetUp />
                </el-icon>配置
              </template>
              <el-menu-item index="99-0" @click="changeMenu('config')">启用模块</el-menu-item>
              <el-menu-item index="99-1" @click="scrollSet('bot')" v-show="enable.bot">机器人</el-menu-item>
              <el-menu-item index="99-2" @click="scrollSet('qq')" v-show="enable.qq">QQ</el-menu-item>
              <el-menu-item index="99-3" @click="scrollSet('wb')" v-show="enable.wb">微博</el-menu-item>
              <el-menu-item index="99-4" @click="scrollSet('kd')" v-show="enable.kd">口袋</el-menu-item>
              <el-menu-item index="99-5" @click="scrollSet('bz')" v-show="enable.bz">B站</el-menu-item>
              <el-menu-item index="99-6" @click="scrollSet('xhs')" v-show="enable.xhs">小红书</el-menu-item>
              <el-menu-item index="99-7" @click="scrollSet('bd')" v-show="enable.bd">百度</el-menu-item>
              <el-menu-item index="99-8" @click="scrollSet('dy')" v-show="enable.dy">抖音</el-menu-item>
            </el-sub-menu>
          </el-menu>
        </el-scrollbar>

        <div>
          <el-space>
          <span @click="openDevTool" class="hoverclass">
            <el-icon title="开发者工具" size="20">
              <svg t="1716695388000" viewBox="0 0 1024 1024" version="1.1"
                xmlns="http://www.w3.org/2000/svg" p-id="2741" width="200" height="200">
                <path
                  d="M128 128h768a42.666667 42.666667 0 0 1 42.666667 42.666667v682.666666a42.666667 42.666667 0 0 1-42.666667 42.666667H128a42.666667 42.666667 0 0 1-42.666667-42.666667V170.666667a42.666667 42.666667 0 0 1 42.666667-42.666667z m42.666667 85.333333v597.333334h682.666666V213.333333z m341.333333 426.666667h256v85.333333h-256z m-142.208-128L249.088 391.338667l60.373333-60.373334L490.453333 512l-180.992 181.034667-60.373333-60.373334z"
                  p-id="2742" fill="#d81e06"></path>
              </svg>
            </el-icon>
          </span>
          <span class="hoverclass">
            <el-icon title="关于" size="20">
              <svg t="1717120695958" viewBox="0 0 1024 1024" version="1.1"
                xmlns="http://www.w3.org/2000/svg" p-id="1480" width="200" height="200">
                <path
                  d="M512 84.473c223.010 0 406.194 183.186 406.194 406.194s-183.186 406.194-406.194 406.194c-223.010 0-406.194-183.186-406.194-406.194 0-223.010 183.186-406.194 406.194-406.194zM512 40.667c-248.894 0-450 201.106-450 450s201.106 450 450 450c248.894 0 450-201.106 450-450 0-248.894-201.106-450-450-450v0zM543.858 640.004c-17.92 17.92-41.814 33.849-49.778 39.823 3.982-21.902 23.894-71.682 49.778-157.3 25.884-87.61 27.877-101.549 27.877-109.513 0-11.947-3.983-23.894-13.938-31.858-21.903-17.92-57.743-13.938-103.539 11.947-23.894 13.938-49.778 35.841-79.645 69.69l-15.929 15.929 51.771 39.823 11.947-13.938c13.938-11.947 45.796-39.823 51.771-45.796-39.823 129.425-79.645 226.99-79.645 262.832 0 15.929 5.973 29.867 13.938 39.823 9.955 9.955 23.894 15.929 39.823 15.929 13.938 0 31.859-5.973 53.761-17.92 17.92-11.947 45.796-33.849 85.62-71.682l15.929-15.929-45.796-45.796-13.938 13.938zM504.035 217.879c-9.955 11.947-15.929 27.877-15.929 45.796 0 15.929 3.982 27.877 13.938 37.832 9.955 9.955 21.903 15.929 35.841 15.929 11.947 0 27.877-3.982 43.806-21.902 11.947-11.947 17.92-27.877 17.92-45.796 0-13.938-3.983-27.877-13.938-37.832-19.912-19.912-57.743-17.92-81.637 5.973v0z"
                  p-id="1481" fill="#8a8a8a"></path>
              </svg>
            </el-icon>
          </span>
        </el-space>
        </div>

      </el-aside>
    </el-card>
    <el-card style="width: 90%; margin-left: 10px">
      <el-container>
        <el-main style="padding-left: 20px; padding-right: 20px" id="parentContainer">
          <Index ref="indexRef" :enable="enable" v-show="component === 'index'" />
          <Log v-show="component === 'log'" :enable="enable" />
          <Pic v-if="component === 'pic'" />
          <Plugin v-if="component === 'plugin' && enable.qq" />
          <Config ref="configRef" :enable="enable" v-if="component === 'config'" @top-enable-change="enableChange"
            @change-menu="scrollSet" @config-change="configChanged" />
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
import { House, Cpu, Picture, Tickets, SetUp } from "@element-plus/icons-vue";
import { onMounted, ref } from "vue";
import type { EnableModule } from "./class/model";
import { getConfig } from "./api";

const startEnble = ref<EnableModule>();
const enable = ref<EnableModule>({
  bot: false,
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
const indexRef = ref();
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

const configChanged = async () => {
  await indexRef.value.refreshConfig();
}

const openDevTool = () => {
  console.log(11)
  // @ts-ignore
  window.external.devTool.openDevTool()
}

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

.hoverclass{
  color:gray;
  cursor: pointer;
}
</style>
