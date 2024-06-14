<template>
    <el-select v-model="selected" filterable remote :multiple="multiple" :remote-method="searchFriend"
        :loading="loading" placeholder="请选择好友" @change="(data: any) => emit('change', data.toString())">
        <el-option v-for="item in friends" :key="item.user_id" :label="item.nickname" :value="item.user_id">
        </el-option>
    </el-select>
</template>

<script setup lang="ts">
import { getSuperAdmin } from '@/api/index'
import { onMounted, ref, type PropType } from 'vue';
import type { Bot } from '@/class/model'
const props = defineProps({
    value: {
        type: [Array<Number>, String],
        default: [],
        required: true
    },
    bot: {
        type: Object as PropType<Bot>,
        default: () => {
            return {
                host: '',
                websocktPort: 0,
                httpPort: 0,
                token: ''
            }
        }
    },
    multiple: {
        type: Boolean,
        default: false
    }
})
const selected = ref<Number[] | Number>();

const friends = ref<any[]>([])
const loading = ref(false)
const emit = defineEmits(["change"]);

const searchFriend = async (keywords = "") => {
    if (!props.bot.host && !props.bot.websocktPort && !props.bot.httpPort) {
        ElMessage.warning("请先完填入bot信息")
        return;
    }
    loading.value = true;
    var res = await getSuperAdmin(keywords, props.bot.host, props.bot.websocktPort, props.bot.httpPort, props.bot.token)
    if (res.success) {
        friends.value = res.data
        loading.value = false;
        if (props.value) {
            if (props.multiple) {
                selected.value = (props.value as String).split(",").map(Number)
            } else {
                selected.value = Number(props.value)
            }
        }
    } else {
        ElMessage.error(res.msg)
        loading.value = false;
    }
}

onMounted(() => {
    setTimeout(() => {
        searchFriend();
    }, 1000);
})
</script>