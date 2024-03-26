<template>
    <el-card class="card-bd">
        <template #header>
            <span id="bd">百度</span>
        </template>
        <el-form ref="bdform" :model="bd" :rules="rules" label-width="150px" label-position="left">
            <el-form-item label="apiKey" prop="appKey">
                <el-input v-model="bd.appKey" placeholder="百度appKey"></el-input>
            </el-form-item>
            <el-form-item label="appSeret" prop="appSeret">
                <el-input v-model="bd.appSeret" placeholder="百度appSeret"></el-input>
            </el-form-item>
            <el-form-item label="开启人脸验证">
                <el-switch v-model="bd.faceVerify" :active-value="true" :inactive-value="false"></el-switch>
            </el-form-item>
            <el-form-item label="基础人脸" v-show="bd.faceVerify">
                <el-upload :file-list="bd.imageList" :action="ApiPrefix + '/upload'" :on-success="onSuccess"
                    :on-remove="onRemove" list-type="picture-card" :limit="3" accept=".jpg,.png,.jpeg">
                    <el-icon>
                        <Plus />
                    </el-icon>
                </el-upload>
                <span style="color:red">*上传人脸轮廓清晰的图片</span>
            </el-form-item>
            <el-form-item label="人脸相似度" v-show="bd.faceVerify">
                <el-col :span="8">
                    <el-input type="number" v-model="bd.similarity"></el-input>
                </el-col>
                <span style="color:red">*直接保存(非双胞胎建议80，双胞胎建议70)</span>
            </el-form-item>
            <el-form-item label="审核相似度" v-show="bd.faceVerify">
                <el-col :span="8">
                    <el-input type="number" v-model="bd.audit"></el-input>
                </el-col>
                <span style="color:red">*超过该值，但未超过上面的值，将加入审核列表，审核通过才会保存</span>
            </el-form-item>
            <el-form-item label="上传云盘" v-show="bd.faceVerify">
                <el-switch v-model="bd.saveAliyunDisk" :active-value="true" :inactive-value="false"></el-switch>
                <span style="color:red">*启用会将图片自动上传到阿里云盘相册</span>
            </el-form-item>
            <el-form-item label="相册名称" v-show="bd.saveAliyunDisk" prop="albumName">
                <el-col :span="8">
                    <el-input v-model="bd.albumName"></el-input>
                </el-col>
            </el-form-item>
        </el-form>
    </el-card>
</template>

<script setup lang="ts" name="bd">
import { ref, type PropType } from 'vue'
import type { BD } from '@/class/model'
import type { FormInstance, FormRules, UploadFile, UploadProps } from 'element-plus';
import { ApiPrefix } from '@/api/index'

const props = defineProps({
    bd: {
        type: Object as PropType<BD>,
        default: {
            appKey: "",
            appSeret: "",
            similarity: 0,
            saveAliyunDisk: false,
            audit: 0,
            albumName: "",
            faceVerify: false,
            imageList: []
        }
    }
})

const bdform = ref<FormInstance>();
const onSuccess: UploadProps['onSuccess'] = (response: any, uploadFile: UploadFile) => {
    if (props.bd.imageList) props.bd.imageList.push(uploadFile.name)
    else props.bd.imageList = []
}

const onRemove: UploadProps['onRemove'] = (file: UploadFile) => {
    if (!props.bd.imageList) return
    var i = -1;
    props.bd.imageList.forEach((item, index) => {
        if (item === file.name) {
            i = index;
            return;
        }
    })
    if (i >= 0) {
        props.bd.imageList.splice(i, 1);
    }
}
const rules = ref<FormRules>(
    {
        appKey: [{ required: true, message: '请输入该值', trigger: 'blur' }],
        appSeret: [{ required: true, message: '请输入该值', trigger: 'blur' }],
        albumName: [{ required: true, message: '请输入该值', trigger: 'blur' }]
    },
)
const validForm = async () => {
    return await bdform.value?.validate(valid => {
        if (valid) {
            return true
        } else {
            return false
        }
    })
}
defineExpose({
    validForm
})
</script>