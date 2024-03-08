<template>
    <el-scrollbar style="height: calc(100vh - 230px);">
        <el-table :data="tableData" stripe style="width: 100%">
            <el-table-column type="index" width="50" />
            <el-table-column prop="createDate" label="时间"></el-table-column>
            <el-table-column prop="content" label="图片">
                <template #default="scope">
                    <el-image style="width: auto; height: 120px" :src="scope.row.content"
                        :preview-src-list="[scope.row.content]" />
                </template>
            </el-table-column>
            <el-table-column label="操作">
                <template #default="scope">
                    <el-button type="primary" @click="save(scope.row.id)">保存</el-button>
                    <el-button type="primary" @click="del(scope.row.id)">删除</el-button>
                </template>
            </el-table-column>
        </el-table>
    </el-scrollbar>
</template>

<script setup lang="ts" name="pic">
import { onMounted, ref } from 'vue';
import { getCache, saveImg, delImg } from '@/api'

const tableData = ref();
const save = async (id: number) => {
    let res = await saveImg(id);
    if (res.success) ElMessage.success(res.msg)
    else ElMessage.error(res.msg)
    await getData();
}
const del = async (id: number) => {
    let res = await delImg(id);
    if (res.success) ElMessage.success(res.msg)
    else ElMessage.error(res.msg)
    await getData();
}
const getData = async () => {
    var res = await getCache();
    tableData.value = res.data;
}
onMounted(async () => {
    await getData();
})
</script>