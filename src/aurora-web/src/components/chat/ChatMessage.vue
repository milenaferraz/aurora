<script setup lang="ts">
import type { ChatMessage } from '../../stores/chat.store'

defineProps<{ message: ChatMessage }>()
</script>

<template>
  <div
    class="flex mb-3"
    :class="message.role === 'user' ? 'justify-end' : 'justify-start'"
  >
    <div
      class="max-w-[80%] rounded-xl px-4 py-2 text-sm whitespace-pre-wrap"
      :class="
        message.role === 'user'
          ? 'bg-blue-600 text-white'
          : 'bg-white/10 border border-white/10 text-gray-100'
      "
    >
      {{ message.content }}
    </div>
  </div>
</template>
