<template>
    <div>
        <el-row>
            <el-space wrap>
                <el-button type="primary" :icon="Refresh" @click="getData">刷新</el-button>
                <el-button type="danger" :icon="Delete" @click="delmore">删除</el-button>
            </el-space>
        </el-row>
        <el-row style="margin-top: 10px">
            <el-table ref="tableRef" :data="tableData" stripe style="width: 100%;height:calc(100vh - 150px);">
                <el-table-column type="selection" />
                <el-table-column prop="createDate" label="时间"></el-table-column>
                <el-table-column prop="content" label="图片">
                    <template #default="scope">
                        <el-image style="width: auto; height: 120px" :src="scope.row.content" preview-teleported
                            :preview-src-list="[scope.row.content]" />
                    </template>
                </el-table-column>
                <el-table-column label="操作" width="200">
                    <template #default="scope">
                        <el-button type="primary" :icon="EditPen" @click="save(scope.row.id)">保存</el-button>
                        <el-button type="danger" :icon="Delete" @click="del(scope.row.id)">删除</el-button>
                    </template>
                </el-table-column>
            </el-table>
            <el-config-provider :locale="zhCn">
                <el-pagination @size-change="getData()" @current-change="getData()" background class="mt15"
                    layout="total, sizes, prev, pager, next, jumper" :total="pageInfo.total"
                    :page-sizes="[5, 10, 20, 30]" v-model:current-page="pageInfo.pageIndex"
                    v-model:page-size="pageInfo.pageSize" />
            </el-config-provider>
        </el-row>
    </div>
</template>

<script setup lang="ts" name="pic">
import { onMounted, ref } from 'vue';
import { getCache, saveImg, delImg, delImgs } from '@/api'
import { Refresh, Delete, EditPen } from '@element-plus/icons-vue'
import zhCn from 'element-plus/es/locale/lang/zh-CN'

const pageInfo = ref({
    pageIndex: 1,
    pageSize: 5,
    total: 0
});
const tableRef = ref();
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
    var res = await getCache(pageInfo.value);
    tableData.value = res.data;
    pageInfo.value.total = res.count;
}
const delmore = async () => {
    var selects = tableRef.value.getSelectionRows();
    await delImgs(selects.map((item: any) => item.id));
    await getData();
}
onMounted(async () => {
    await getData();
})
</script>
<style>
.mt15 {
    margin-top: 15px;
}
</style>