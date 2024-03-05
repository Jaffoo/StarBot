<template>
    <el-container class="layout-container">
      <el-card>
        <el-aside width="15vw">
          <el-scrollbar>
            <el-menu :default-openeds="['2']" default-active="1">
              <el-menu-item index="1" @click="changeMenu('index')">
                <el-icon>
                  <House />
                </el-icon>
                开始
              </el-menu-item>

              <el-sub-menu index="2">
                <template #title>
                  <el-icon>
                    <Cpu />
                  </el-icon>配置
                </template>
                <el-menu-item index="2-0" @click="changeMenu('config')">启用模块</el-menu-item>
                <el-menu-item index="2-1" @click="scrollSet('shamrock')" v-show="enable.shamrock">Shamrock</el-menu-item>
                <el-menu-item index="2-2" @click="scrollSet('qq')" v-show="enable.qq">QQ</el-menu-item>
                <el-menu-item index="2-3" @click="scrollSet('wb')" v-show="enable.wb">微博</el-menu-item>
                <el-menu-item index="2-4" @click="scrollSet('kd')" v-show="enable.kd">口袋</el-menu-item>
                <el-menu-item index="2-5" @click="scrollSet('bz')" v-show="enable.bz">B站</el-menu-item>
                <el-menu-item index="2-6" @click="scrollSet('xhs')" v-show="enable.xhs">小红书</el-menu-item>
                <el-menu-item index="2-7" @click="scrollSet('bd')" v-show="enable.bd">百度</el-menu-item>
                <el-menu-item index="2-8" @click="scrollSet('dy')" v-show="enable.dy">抖音</el-menu-item>
              </el-sub-menu>
            </el-menu>
          </el-scrollbar>
        </el-aside>
      </el-card>
      <el-card style="width: 90%;margin-left: 10px;">
        <el-container>
          <el-main style="padding-left: 20px;padding-right: 20px;" id="parentContainer">
            <Index v-show="component === 'index'" />
            <Config v-if="component === 'config'" @top-enable-change="enableChange" />
          </el-main>
        </el-container>
      </el-card>
    </el-container>
</template>

<script setup lang="ts">
import Index from '@/component/index.vue'
import Config from '@/component/config.vue'
import { House, Cpu } from '@element-plus/icons-vue'
import { onMounted, ref } from 'vue';
import type { EnableModule } from './class/model';
import { getConfig } from './api';
const enable = ref<EnableModule>({
  shamrock: false,
  qq: false,
  wb: false,
  bz: false,
  kd: false,
  xhs: false,
  dy: false,
  bd: false
})

const component = ref("index");
const changeMenu = async (com: string) => {
  component.value = com;
  if (component.value === "index") {
    await getEnableModule();
  } else {
    scrollSet('enable');
  }
}

const scrollSet = async (com: string) => {
  const childElement = document.getElementById(com);
  const parentContainer = document.getElementById("pdom")?.parentElement;

  if (parentContainer && childElement) {
    const parentRect = parentContainer.getBoundingClientRect();
    const childRect = childElement.getBoundingClientRect();
    const scrollTop = childRect.top + parentContainer.scrollTop - parentRect.top - 28;

    parentContainer.scrollTo({
      top: scrollTop,
      behavior: 'smooth'
    });
  }
}
const enableChange = (enableNew: EnableModule) => {
  enable.value = enableNew
}
const getEnableModule = async () => {
  var res = await getConfig();
  enable.value = res.data.enableModule
}
onMounted(async () => {
  await getEnableModule();
})
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