<template>
  <div>
    <el-row>
      <el-space wrap>
        <el-upload accept=".dll" :action="ApiPrefix + '/uploaddll'" multiple :show-file-list="false"
          :on-success="onSuccess">
          <el-button type="success" :icon="Upload">上传插件</el-button>
        </el-upload>
        <el-button type="primary" :icon="Refresh" @click="getData">刷新</el-button>
      </el-space>
    </el-row>
    <el-row style="margin-top: 10px">
      <el-table :data="tableData" stripe style="width: 100%;height:calc(100vh - 100px);">
        <el-table-column type="index" width="50" />
        <el-table-column label="插件名" width="120" prop="name">
        </el-table-column>
        <el-table-column label="版本" width="80" prop="version">
        </el-table-column>
        <el-table-column label="状态" width="80">
          <template #default="scope">
            <el-tag type="success" v-if="scope.row.enable">启用</el-tag>
            <el-tag type="danger" v-else>禁用</el-tag>
          </template>
        </el-table-column>
        <el-table-column label="描述" prop="desc">
        </el-table-column>
        <el-table-column label="使用" prop="usage">
        </el-table-column>
        <el-table-column label="操作" width="250">
          <template #default="scope">
            <el-button size="small" type="primary" v-if="!scope.row.enable"
              @click="start(scope.row.id)">启用</el-button>
            <el-button size="small" type="warning" v-else @click="stop(scope.row.id)">禁用</el-button>
            <el-button size="small" type="danger" @click="del(scope.row.id)">删除</el-button>
            <el-button size="small" type="info" v-if="scope.row.confPath"
              @click="openConf(scope.row.confPath)">配置</el-button>
            <el-button size="small" type="info" v-if="scope.row.logPath"
              @click="openConf(scope.row.logPath)">日志</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-row>
  </div>
</template>

<script setup lang="ts" name="pic">
import { onMounted, ref } from "vue";
import { getFun, startPlugin, stopPlugin, delPlugin, open } from "@/api";
import { ApiPrefix } from '@/api/index'
import type { UploadProps } from "element-plus";
import { Refresh, Upload } from '@element-plus/icons-vue'

const tableData = ref();

const getData = async () => {
  var res = await getFun();
  tableData.value = res.data;
};

const start = async (id: number) => {
  let res = await startPlugin(id);
  if (res.success) ElMessage.success(res.msg);
  else ElMessage.error(res.msg);
  await getData();
};

const stop = async (id: number) => {
  let res = await stopPlugin(id);
  if (res.success) ElMessage.success(res.msg);
  else ElMessage.error(res.msg);
  await getData();
};

const del = async (id: number) => {
  let res = await delPlugin(id);
  if (res.success) ElMessage.success(res.msg);
  else ElMessage.error(res.msg);
  await getData();
};

const onSuccess: UploadProps['onSuccess'] = async () => {
  await getData()
}

const openConf = async (path: string) => {
  var res = await open(path)
  if (res.success) ElMessage.success(res.msg)
  else ElMessage.error(res.msg)
}

onMounted(async () => {
  await getData();
});
</script>
