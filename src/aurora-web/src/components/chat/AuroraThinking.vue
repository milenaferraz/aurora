<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { useChatStore } from '../../stores/chat.store'

const chatStore = useChatStore()
const labels = ['Consultando agenda', 'Analisando informações', 'Preparando resposta']
const currentLabel = ref(labels[0])
let interval: ReturnType<typeof setInterval> | undefined

onMounted(() => {
  let i = 0
  interval = setInterval(() => {
    i = (i + 1) % labels.length
    currentLabel.value = labels[i]
  }, 1500)
})

onUnmounted(() => {
  if (interval) clearInterval(interval)
})
</script>

<template>
  <div v-if="chatStore.streaming" class="px-4 py-2 text-blue-300 text-sm flex items-center gap-2">
    <span class="inline-block w-2 h-2 rounded-full bg-blue-400 animate-pulse" />
    {{ currentLabel }}...
  </div>
</template>
