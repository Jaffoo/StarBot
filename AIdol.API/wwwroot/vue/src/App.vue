<template>
  <el-card>
    <el-container class="layout-container">
      <el-card>
        <el-aside width="10vw">
          <el-scrollbar>
            <el-menu :default-openeds="['3']" default-active="0">
              <el-menu-item index="0" @click="changeMenu('index')">
                <el-icon>
                  <House />
                </el-icon>
                开始
              </el-menu-item>
              <el-menu-item index="1" @click="changeMenu('shamrock', 'Shamrock')">
                <el-icon>
                  <Cpu />
                </el-icon>
                Shamrock
              </el-menu-item>
              <el-menu-item index="2" @click="changeMenu('enable')">
                <el-icon>
                  <LinkIcon />
                </el-icon>
                启用模块
              </el-menu-item>
              <el-sub-menu index="3">
                <template #title>
                  <el-icon>
                    <IconMenu />
                  </el-icon>配置
                </template>
                <el-menu-item index="2-1" @click="changeMenu('qq')" v-show="enable.qq">QQ</el-menu-item>
                <el-menu-item index="2-2" @click="changeMenu('wb')" v-show="enable.wb">微博</el-menu-item>
                <el-menu-item index="2-3" @click="changeMenu('xhs')" v-show="enable.xhs">小红书</el-menu-item>
                <el-menu-item index="2-4" @click="changeMenu('dy')" v-show="enable.dy">抖音</el-menu-item>
                <el-menu-item index="2-5" @click="changeMenu('bz')" v-show="enable.bz">B站</el-menu-item>
                <el-menu-item index="2-6" @click="changeMenu('kd')" v-show="enable.kd">口袋</el-menu-item>
                <el-menu-item index="2-7" @click="changeMenu('bd')" v-show="enable.bd">百度</el-menu-item>
              </el-sub-menu>
            </el-menu>
          </el-scrollbar>
        </el-aside>
      </el-card>
      <el-card style="width: 90%;margin-left: 10px;">
        <el-container>
          <el-main style="padding-left: 20px;padding-right: 20px;">
            <el-scrollbar>
              <Index v-show="component === 'index'" />
              <Config v-if="component === 'config'" @enable-change="enableChange" />
            </el-scrollbar>
          </el-main>
        </el-container>
      </el-card>
    </el-container>
  </el-card>
</template>

<script setup lang="ts">
import Index from '@/component/index.vue'
import Config from '@/component/config.vue'
import { Menu as IconMenu, House, Link as LinkIcon, Cpu } from '@element-plus/icons-vue'
import { onMounted, ref } from 'vue';
import type { EnableModule } from './class/model';
import { getConfig } from './api';

const enable = ref<EnableModule>({
  qq: false,
  wb: false,
  bz: false,
  kd: false,
  xhs: false,
  dy: false,
  bd: false
})

const component = ref("index");
const changeMenu = (com: string) => {
  component.value = com;
}
const enableChange = (enableNew: EnableModule) => {
  enable.value = enableNew
}
onMounted(async () => {
  var res = await getConfig();
  enable.value=res.data.enable
}),
</script>

<style scoped>
.layout-container {
  height: calc(100vh - 60px);
  /* 20px 是滚动条的宽度 */
  overflow-y: hidden;
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
</style>