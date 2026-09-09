<script setup lang="ts">
import { watch, ref, nextTick } from 'vue'
import { useChatStore } from '../../stores/chat.store'
import ChatMessageComponent from './ChatMessage.vue'

const chatStore = useChatStore()
const container = ref<HTMLElement | null>(null)

watch(
  () => chatStore.messages.length,
  async () => {
    await nextTick()
    if (container.value) {
      container.value.scrollTop = container.value.scrollHeight
    }
  }
)
</script>

<template>
  <div ref="container" class="flex-1 overflow-y-auto px-4 py-4 space-y-1">
    <ChatMessageComponent
      v-for="(msg, i) in chatStore.messages"
      :key="i"
      :message="msg"
    />
  </div>
</template>
