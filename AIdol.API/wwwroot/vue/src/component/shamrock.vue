<template>
    <el-form ref="form" :model="model" label-width="100px">
    <el-form-item label="QQ群" prop="QQ.group"></el-form-item>
    </el-form>
</template>

<script setup lang="ts" name="bot">
import { ref, type PropType } from 'vue'
import type { Shamrock } from '@/class/model'
const prop = defineProps({
    model: {
        type: Object as PropType<Shamrock>,
        default: null
    }
})
const form = ref(null);
</script>