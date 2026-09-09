<script setup lang="ts">
import { ref } from 'vue'
import { chatApi } from '../api/chatApi'

const message = ref('')
const response = ref('')
const loading = ref(false)
const error = ref('')

async function sendMessage() {
  if (!message.value.trim()) return
  loading.value = true
  error.value = ''
  response.value = ''
  try {
    const result = await chatApi.chat({ message: message.value })
    response.value = result.message
  } catch (e) {
    error.value = 'Não consegui falar com o núcleo da Aurora.'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="min-h-screen bg-[#050510] flex flex-col items-center justify-center p-8">
    <h1 class="text-3xl font-bold text-blue-400 mb-8">Aurora</h1>

    <div class="w-full max-w-xl">
      <div class="flex gap-2 mb-4">
        <input
          v-model="message"
          type="text"
          placeholder="Digite sua mensagem..."
          class="flex-1 bg-white/10 text-white border border-white/20 rounded-lg px-4 py-2 outline-none focus:border-blue-400"
          @keyup.enter="sendMessage"
        />
        <button
          :disabled="loading"
          class="bg-blue-600 hover:bg-blue-700 disabled:opacity-50 text-white px-4 py-2 rounded-lg transition"
          @click="sendMessage"
        >
          Enviar
        </button>
      </div>

      <div v-if="loading" class="text-blue-300 text-sm">Aurora está pensando...</div>

      <div
        v-if="response"
        class="bg-white/5 border border-white/10 rounded-lg p-4 text-white whitespace-pre-wrap"
      >
        {{ response }}
      </div>

      <div v-if="error" class="text-red-400 text-sm">{{ error }}</div>
    </div>
  </div>
</template>
