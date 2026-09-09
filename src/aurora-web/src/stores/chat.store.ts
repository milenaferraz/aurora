import { defineStore } from 'pinia'
import { ref } from 'vue'

export interface ChatMessage {
  role: 'user' | 'assistant'
  content: string
}

export const useChatStore = defineStore('chat', () => {
  const messages = ref<ChatMessage[]>([])
  const streaming = ref(false)
  const abortController = ref<AbortController | null>(null)

  function addMessage(message: ChatMessage) {
    messages.value.push(message)
  }

  function appendDelta(content: string) {
    const last = messages.value[messages.value.length - 1]
    if (last && last.role === 'assistant') {
      last.content += content
    } else {
      messages.value.push({ role: 'assistant', content })
    }
  }

  function setStreaming(value: boolean) {
    streaming.value = value
  }

  function setAbortController(controller: AbortController | null) {
    abortController.value = controller
  }

  function clearAbort() {
    abortController.value = null
  }

  return { messages, streaming, abortController, addMessage, appendDelta, setStreaming, setAbortController, clearAbort }
})
