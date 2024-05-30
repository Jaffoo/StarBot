<template>
    <el-card class="card-bd" style="margin-top: 10px;">
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
            <el-form-item label="基础人脸" v-show="bd.faceVerify" prop="imageList">
                <el-upload v-model:file-list="tempImgList" :action="ApiPrefix + '/upload'" :on-success="onSuccess"
                    :on-preview="handlePictureCardPreview" :on-remove="onRemove" list-type="picture-card" :limit="3"
                    accept=".jpg,.png,.jpeg">
                    <el-icon>
                        <Plus />
                    </el-icon>
                </el-upload>
                <span style="color:red">*上传人脸轮廓清晰的图片</span>
                <el-input v-show="false" v-model="bd.imageList"></el-input>
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
            <el-form-item label="相册名称" v-show="bd.saveAliyunDisk && bd.faceVerify" prop="albumName">
                <el-col :span="8">
                    <el-input v-model="bd.albumName"></el-input>
                </el-col>
            </el-form-item>
        </el-form>
    </el-card>
    <el-dialog v-model="dialogVisible">
        <img w-full style="height: auto;width: 100%;" :src="dialogImageUrl" alt="Preview Image" />
    </el-dialog>
</template>

<script setup lang="ts" name="bd">
import { ref, watchEffect, type PropType } from 'vue'
import type { ApiResult, BD, BDImg } from '@/class/model'
import type { FormInstance, FormRules, UploadFile, UploadProps } from 'element-plus';
import { ApiPrefix } from '@/api/index'
import { Plus } from "@element-plus/icons-vue";

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
            imageList: new Array<BDImg>
        }
    }
})
const tempImgList = ref<BDImg[]>([])
const bdform = ref<FormInstance>();
const onSuccess: UploadProps['onSuccess'] = (response: ApiResult) => {
    tempImgList.value.push({ name: response.data.name, url: response.data.url })
    if (props.bd.imageList)
        props.bd.imageList.push({ name: response.data.name, url: response.data.url })
}
watchEffect(() => {
    if (props.bd.imageList) {
        tempImgList.value = JSON.parse(JSON.stringify(props.bd.imageList));
    }
})
const onRemove: UploadProps['onRemove'] = (file: UploadFile) => {
    if (!props.bd.imageList) return;
    props.bd.imageList = props.bd.imageList.filter(item => item.name !== file.name);
}

const dialogImageUrl = ref('')
const dialogVisible = ref(false)

const handlePictureCardPreview: UploadProps['onPreview'] = (uploadFile) => {
    dialogImageUrl.value = uploadFile.url!
    dialogVisible.value = true
}

const rules = ref<FormRules>(
    {
        appKey: [{ required: true, message: '请输入该值', trigger: 'blur' }],
        appSeret: [{ required: true, message: '请输入该值', trigger: 'blur' }],
        albumName: [{ required: true, message: '请输入该值', trigger: 'blur' }],
        imageList: [{ required: true, message: '请上传人脸', trigger: 'change' }]
    },
)
const validForm = async () => {
    return new Promise((resolve) => {
        bdform.value?.validate((valid: boolean) => {
            resolve(valid);
        });
    });
}
defineExpose({
    validForm
})
</script>