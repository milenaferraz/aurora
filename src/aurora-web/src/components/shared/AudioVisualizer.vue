<script setup lang="ts">
import { computed } from 'vue'
import { useAuroraStore } from '../../stores/aurora.store'

const auroraStore = useAuroraStore()
const visible = computed(() => auroraStore.state === 'listening')
</script>

<template>
  <div v-if="visible" class="visualizer" aria-hidden="true">
    <span v-for="i in 5" :key="i" class="bar" :style="{ animationDelay: `${(i - 1) * 0.1}s` }" />
  </div>
</template>

<style scoped>
.visualizer {
  display: flex;
  align-items: center;
  gap: 4px;
  height: 32px;
}

.bar {
  display: block;
  width: 4px;
  border-radius: 2px;
  background: #06b6d4;
  animation: pulse-bar 0.8s ease-in-out infinite alternate;
}

@keyframes pulse-bar {
  from { height: 6px; opacity: 0.5; }
  to { height: 28px; opacity: 1; }
}
</style>
