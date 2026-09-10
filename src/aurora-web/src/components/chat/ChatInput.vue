<script setup lang="ts">
import { ref } from 'vue'
import { useChatStore } from '../../stores/chat.store'
import VoiceButton from '../aurora/VoiceButton.vue'

const emit = defineEmits<{
  submit: [message: string]
}>()

const chatStore = useChatStore()
const input = ref('')
const focused = ref(false)

function handleSubmit() {
  if (!input.value.trim() || chatStore.streaming) return
  emit('submit', input.value.trim())
  input.value = ''
}
</script>

<template>
  <div class="pill-wrap" :class="{ focused }">
    <div class="pill-inner">
      <input
        v-model="input"
        type="text"
        placeholder="Fale com a Aurora..."
        :disabled="chatStore.streaming"
        class="pill-input"
        @keyup.enter="handleSubmit"
        @focus="focused = true"
        @blur="focused = false"
      />
      <VoiceButton />
    </div>
  </div>
</template>

<style scoped>
.pill-wrap {
  border-radius: 9999px;
  padding: 1px;
  background: linear-gradient(
    135deg,
    rgba(56, 189, 248, 0.18),
    rgba(168, 85, 247, 0.18),
    rgba(236, 72, 153, 0.12)
  );
  transition: background 0.3s ease, box-shadow 0.3s ease;
}

.pill-wrap.focused {
  background: linear-gradient(
    135deg,
    rgba(56, 189, 248, 0.42),
    rgba(168, 85, 247, 0.42),
    rgba(236, 72, 153, 0.30)
  );
  box-shadow: 0 0 30px rgba(168, 85, 247, 0.12);
}

.pill-inner {
  display: flex;
  align-items: center;
  height: 68px;
  border-radius: 9999px;
  background: rgba(8, 15, 30, 0.72);
  backdrop-filter: blur(16px);
  -webkit-backdrop-filter: blur(16px);
  padding: 0 10px 0 28px;
  gap: 10px;
}

.pill-input {
  flex: 1;
  min-width: 0;
  background: transparent;
  border: none;
  outline: none;
  color: rgba(226, 232, 240, 0.90);
  font-size: 15px;
  font-family: 'Inter', sans-serif;
  font-weight: 300;
  letter-spacing: 0.01em;
}

.pill-input::placeholder {
  color: rgba(100, 116, 139, 0.48);
  font-size: 14px;
  font-weight: 300;
}

.pill-input:disabled {
  opacity: 0.45;
  cursor: not-allowed;
}
</style>
