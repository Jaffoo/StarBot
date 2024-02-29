<template>
    <el-form ref="form" :model="model" label-width="100px">
        <el-form-item label="apiKey">
            <el-input v-model="model.appKey" placeholder="百度appKey"></el-input>
        </el-form-item>
        <el-form-item label="appSeret">
            <el-input v-model="model.appSeret" placeholder="百度appSeret"></el-input>
        </el-form-item>
        <el-form-item label="开启人脸验证">
            <el-switch v-model="model.faceVerify" :active-value="true" :inactive-value="false"></el-switch>
        </el-form-item>
        <el-form-item label="基础人脸" v-show="model.faceVerify">
            <el-upload :file-list="model.imageList" action="http://127.0.0.1:6051/api/v1/upload" :on-success="onSuccess"
                :on-remove="onRemove" list-type="picture-card" :limit="3" accept=".jpg,.png,.jpeg">
                <el-icon>
                    <Plus />
                </el-icon>
            </el-upload>
            <span style="color:red">*上传人脸轮廓清晰的图片</span>
        </el-form-item>
        <el-form-item label="人脸相似度" v-show="model.faceVerify">
            <el-col :span="8">
                <el-input type="number" v-model="model.similarity"></el-input>
            </el-col>
            <span style="color:red">*直接保存(非双胞胎建议80，双胞胎建议70)</span>
        </el-form-item>
        <el-form-item label="审核相似度" v-show="model.faceVerify">
            <el-col :span="8">
                <el-input type="number" v-model="model.audit"></el-input>
            </el-col>
            <span style="color:red">*超过该值，但未超过上面的值，将加入审核列表，审核通过才会保存</span>
        </el-form-item>
        <el-form-item label="上传云盘" v-show="model.faceVerify">
            <el-switch v-model="model.saveAliyunDisk" :active-value="true" :inactive-value="false"></el-switch>
            <span style="color:red">*启用会将图片自动上传到阿里云盘相册</span>
        </el-form-item>
        <el-form-item label="相册名称" v-show="model.saveAliyunDisk">
            <el-col :span="8">
                <el-input v-model="model.albumName"></el-input>
            </el-col>
        </el-form-item>
    </el-form>
</template>

<script setup lang="ts" name="bd">
import { ref, type PropType } from 'vue'
import type { BD } from '@/class/model'
import type { UploadFile, UploadProps } from 'element-plus';
const props = defineProps({
    model: {
        type: Object as PropType<BD>,
        default: null
    }
})
const form = ref(null);
const onSuccess: UploadProps['onSuccess'] = (response: any, uploadFile: UploadFile) => {
    if (props.model.imageList) props.model.imageList.push(uploadFile.name)
    else props.model.imageList = []
}

const onRemove: UploadProps['onRemove'] = (file: UploadFile) => {
    if (!props.model.imageList) return
    var i = -1;
    props.model.imageList.forEach((item, index) => {
        if (item === file.name) {
            i = index;
            return;
        }
    })
    if (i >= 0) {
        props.model.imageList.splice(i, 1);
    }
}
</script>