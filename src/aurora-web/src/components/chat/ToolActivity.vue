<script setup lang="ts">
defineProps<{
  tool: string | null
}>()
</script>

<template>
  <div v-if="tool" class="px-4 py-1 text-xs text-purple-300 flex items-center gap-1">
    <span>⚙️</span>
    <span>{{ tool }}</span>
  </div>
</template>
