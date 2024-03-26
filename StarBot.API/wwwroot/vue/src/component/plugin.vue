<template>
  <div>
    <el-row><el-button type="primary">上传插件</el-button> </el-row>
    <el-row style="margin-top: 10px">
      <el-table :data="tableData" stripe style="width: 100%">
        <el-table-column type="index" width="50" />
        <el-table-column label="插件名">
          <template #default="scope">
            {{ scope.row.pluginInfo.name }}
          </template>
        </el-table-column>
        <el-table-column label="版本">
          <template #default="scope">
            {{ scope.row.pluginInfo.version }}
          </template>
        </el-table-column>
        <el-table-column label="描述">
          <template #default="scope">
            {{ scope.row.pluginInfo.desc }}
          </template>
        </el-table-column>
        <el-table-column label="状态">
          <template #default="scope">
            <el-tag type="success" v-if="scope.row.statue">启用</el-tag>
            <el-tag type="danger" v-else>禁用</el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作">
          <template #default="scope">
            <el-button type="primary" v-if="!scope.row.statue" @click="start(scope.row.pluginInfo.name)">启用</el-button>
            <el-button type="primary" v-else @click="stop(scope.row.pluginInfo.name)">禁用</el-button>
            <el-button type="primary" @click="del(scope.row.pluginInfo.name)">删除</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-row>
  </div>
</template>

<script setup lang="ts" name="pic">
import { onMounted, ref } from "vue";
import { getFun, startPlugin, stopPlugin, delPlugin } from "@/api";

const tableData = ref();

const getData = async () => {
  var res = await getFun();
  tableData.value = res.data;
};

const start = async (name: string) => {
  let res = await startPlugin(name);
  if (res.success) ElMessage.success(res.msg);
  else ElMessage.error(res.msg);
};

const stop = async (name: string) => {
  let res = await stopPlugin(name);
  if (res.success) ElMessage.success(res.msg);
  else ElMessage.error(res.msg);
};

const del = async (name: string) => {
  let res = await delPlugin(name);
  if (res.success) ElMessage.success(res.msg);
  else ElMessage.error(res.msg);
};

onMounted(async () => {
  await getData();
});
</script>
