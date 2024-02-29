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
              <Shamrock :shamrock="config.Shamrock" v-if="component === 'shamrock'" />
              <Enable :enable="config.EnableModule" v-if="component === 'enable'" />
              <QQ :qq="config.QQ" v-if="component === 'qq'" />
              <WB :wb="config.WB" v-if="component === 'wb'" />
              <BZ :bz="config.BZ" v-if="component === 'bz'" />
              <KD :kd="config.KD" v-if="component === 'kd'" />
              <XHS :xhs="config.XHS" v-if="component === 'xhs'" />
              <BD :bd="config.BD" v-if="component === 'bd'" />
              <DY :dy="config.DY" v-if="component === 'dy'" />
            </el-scrollbar>
          </el-main>
        </el-container>
      </el-card>
    </el-container>
  </el-card>
</template>

<script setup lang="ts">
import Index from '@/component/index.vue'
import Enable from '@/component/enable.vue'
import Shamrock from '@/component/shamrock.vue'
import QQ from '@/component/qq.vue'
import WB from '@/component/wb.vue'
import BZ from '@/component/bz.vue'
import KD from '@/component/kd.vue'
import XHS from '@/component/xhs.vue'
import BD from '@/component/bd.vue'
import DY from '@/component/dy.vue'
import type { EnableModule, Shamrock as ShamrockType, QQ as QQType, WB as WBType, BD as BDType, KD as KDType, DY_XHS_BZ, Config as ConfigType } from '@/class/model'
import { Menu as IconMenu, House, Link as LinkIcon, Cpu } from '@element-plus/icons-vue'
import { ref } from 'vue';

const enable = ref<EnableModule>({
  qq: false,
  wb: false,
  bz: false,
  kd: false,
  xhs: false,
  dy: false,
  bd: false
})
const shamrock = ref<ShamrockType>({
  use: false,
  host: '',
  websocktPort: 0,
  httpPort: 0
})
const qq = ref<QQType>({
  admin: '',
  group: '',
  save: false,
  notice: false,
  debug: false,
  permission: '',
  funcAdmin: []
})
const wb = ref<WBType>({
  userAll: '',
  userPart: '',
  forwardGroup: false,
  forwardQQ: false,
  chiGuaForwardGroup: false,
  chiGuaForwardQQ: false,
  qq: '',
  group: '',
  chiGuaQQ: '',
  chiGuaGroup: '',
  chiGuaUser: '',
  timeSpan: 3,
})
const bd = ref<BDType>({
  appKey: '',
  appSeret: '',
  saveAliyunDisk: false,
  faceVerify: false,
  audit: 60,
  similarity: 80,
  albumName: '',
  imageList: [],
})
const kd = ref<KDType>({
  forwardGroup: false,
  forwardQQ: false,
  idolName: '',
  account: '',
  token: '',
  liveRoomId: '',
  msgType: [],
  msgTypeAll: [],
  saveMsg: 0
})
const bz = ref<DY_XHS_BZ>({
  user: '',
  forwardGroup: false,
  forwardQQ: false,
  timeSpan: 3,
  qq: '',
  group: ''
})
const dy = ref<DY_XHS_BZ>({
  user: '',
  forwardGroup: false,
  forwardQQ: false,
  timeSpan: 3,
  qq: '',
  group: ''
})
const xhs = ref<DY_XHS_BZ>({
  user: '',
  forwardGroup: false,
  forwardQQ: false,
  timeSpan: 3,
  qq: '',
  group: ''
})
const config = ref<ConfigType>({
  QQ: qq.value,
  WB: wb.value,
  BZ: bz.value,
  DY: dy.value,
  KD: kd.value,
  EnableModule: enable.value,
  Shamrock: shamrock.value,
  XHS: xhs.value,
  BD: bd.value
})

const component = ref("index");
const changeMenu = (com: string) => {
  component.value = com;
}
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