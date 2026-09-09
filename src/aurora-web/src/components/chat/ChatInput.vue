<script setup lang="ts">
import { ref } from 'vue'
import { useChatStore } from '../../stores/chat.store'

const emit = defineEmits<{
  submit: [message: string]
}>()

const chatStore = useChatStore()
const input = ref('')

function handleSubmit() {
  if (!input.value.trim() || chatStore.streaming) return
  emit('submit', input.value.trim())
  input.value = ''
}
</script>

<template>
  <div class="border-t border-white/10 p-4 flex gap-2">
    <input
      v-model="input"
      type="text"
      placeholder="Digite sua mensagem..."
      :disabled="chatStore.streaming"
      class="flex-1 bg-white/10 text-white border border-white/20 rounded-lg px-4 py-2 outline-none focus:border-blue-400 disabled:opacity-50"
      @keyup.enter="handleSubmit"
    />
    <button
      :disabled="chatStore.streaming"
      class="bg-blue-600 hover:bg-blue-700 disabled:opacity-50 text-white px-4 py-2 rounded-lg transition"
      @click="handleSubmit"
    >
      Enviar
    </button>
  </div>
</template>
