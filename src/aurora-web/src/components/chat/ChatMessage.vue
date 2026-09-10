<script setup lang="ts">
import type { ChatMessage } from '../../stores/chat.store'

defineProps<{ message: ChatMessage }>()
</script>

<template>
  <div
    class="msg-row"
    :class="message.role === 'user' ? 'msg-user' : 'msg-aurora'"
  >
    <div class="msg-bubble">{{ message.content }}</div>
  </div>
</template>

<style scoped>
.msg-row {
  display: flex;
  margin-bottom: 10px;
}

.msg-user {
  justify-content: flex-end;
}

.msg-aurora {
  justify-content: flex-start;
}

.msg-bubble {
  max-width: 78%;
  font-size: 14px;
  line-height: 1.68;
  font-family: 'Inter', sans-serif;
  font-weight: 300;
  border-radius: 16px;
  padding: 12px 18px;
  white-space: pre-wrap;
  word-break: break-word;
}

.msg-user .msg-bubble {
  background: rgba(37, 99, 235, 0.12);
  border: 1px solid rgba(59, 130, 246, 0.20);
  color: rgba(186, 230, 253, 0.95);
  border-bottom-right-radius: 4px;
}

.msg-aurora .msg-bubble {
  background: rgba(168, 85, 247, 0.08);
  border: 1px solid rgba(168, 85, 247, 0.18);
  color: rgba(226, 232, 240, 0.92);
  border-bottom-left-radius: 4px;
}
</style>
