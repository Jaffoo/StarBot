<template>
    <el-select v-model="selected" filterable remote :multiple="multiple" :remote-method="searchMember"
        :loading="loading" placeholder="请选择群成员" @change="(data: any) => emit('change', data.toString())">
        <el-option v-for="item in members" :key="item.user_id" :label="item.nickname" :value="item.user_id">
        </el-option>
    </el-select>
</template>

<script setup lang="ts">
import { getAdmin } from '@/api/index'
import { onMounted, ref, type PropType} from 'vue';
import type { Bot } from '@/class/model'

const props = defineProps({
    value: {
        type: [Array<String>, String],
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
    },
    group: {
        type: [String, Array<String>],
        default: '',
        required: true
    }
})

const selected = ref<Number[] | Number>()
const members = ref<any[]>([])
const loading = ref(false)
const emit = defineEmits(["change"]);

const searchMember = async (keywords = "", group = "") => {
    if (!group) {
        if (props.group) {
            group = props.group.toString()
        }
    }
    if (!props.bot.host && !props.bot.websocktPort && !props.bot.httpPort) {
        ElMessage.warning("请先完填入bot信息")
        return;
    }
    loading.value = true;
    var res = await getAdmin(group, keywords, props.bot.host, props.bot.websocktPort, props.bot.httpPort, props.bot.token)
    if (res.success) {
        members.value = res.data
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
        if (props.group) {
            if (typeof props.group === "string")
                searchMember("", props.group);
            else
                searchMember("", props.group.toString());
        }
    }, 1000);
})
</script>