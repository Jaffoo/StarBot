<template>
    <div>
        {{ count }}
    </div>
</template>

<script setup lang="ts" name="qq">
import { ref } from 'vue'
const count = ref(0);
</script>