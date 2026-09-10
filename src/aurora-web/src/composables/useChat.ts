import { ref } from 'vue'
import { useAuroraStore } from '../stores/aurora.store'
import { useChatStore } from '../stores/chat.store'

const AURORA_API_URL = import.meta.env.VITE_AURORA_API_URL as string ?? 'http://localhost:8080'

export function useChat() {
  const auroraStore = useAuroraStore()
  const chatStore = useChatStore()
  const activeTool = ref<string | null>(null)

  async function sendMessage(message: string) {
    if (!message.trim()) return

    auroraStore.setState('thinking')
    chatStore.setStreaming(true)
    chatStore.addMessage({ role: 'user', content: message })
    chatStore.addMessage({ role: 'assistant', content: '' })

    const controller = new AbortController()
    chatStore.setAbortController(controller)

    try {
      const response = await fetch(`${AURORA_API_URL}/api/chat/stream`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ message }),
        signal: controller.signal,
      })

      if (!response.ok || !response.body) {
        throw new Error(`HTTP error: ${response.status}`)
      }

      const reader = response.body.getReader()
      const decoder = new TextDecoder()
      let buffer = ''

      while (true) {
        const { done, value } = await reader.read()
        if (done) break

        buffer += decoder.decode(value, { stream: true })
        const lines = buffer.split('\n')
        buffer = lines.pop() ?? ''

        let currentEventType = ''
        for (const line of lines) {
          if (line.startsWith('event: ')) {
            currentEventType = line.slice(7).trim()
          } else if (line.startsWith('data: ')) {
            const dataStr = line.slice(6).trim()
            try {
              const data = JSON.parse(dataStr)
              handleEvent(currentEventType, data)
            } catch {
              // ignore parse errors
            }
            currentEventType = ''
          }
        }
      }
    } catch (err) {
      if (err instanceof Error && err.name === 'AbortError') return
      const lastMsg = chatStore.messages[chatStore.messages.length - 1]
      if (lastMsg?.role === 'assistant') {
        lastMsg.content = 'Não consegui falar com o núcleo da Aurora.'
      }
      auroraStore.setState('error')
    } finally {
      chatStore.setStreaming(false)
      chatStore.clearAbort()
    }
  }

  function handleEvent(eventType: string, data: Record<string, unknown>) {
    switch (eventType) {
      case 'message.delta':
        if (typeof data.content === 'string') {
          chatStore.appendDelta(data.content)
          if (auroraStore.state === 'thinking') {
            auroraStore.setState('speaking')
          }
        }
        break
      case 'tool.started':
        if (typeof data.tool === 'string') activeTool.value = data.tool
        auroraStore.setState('working')
        break
      case 'tool.completed':
        activeTool.value = null
        auroraStore.setState('thinking')
        break
      case 'message.completed':
        auroraStore.setState('idle')
        break
      case 'error': {
        const lastMsg = chatStore.messages[chatStore.messages.length - 1]
        if (lastMsg?.role === 'assistant') {
          lastMsg.content = 'Não consegui falar com o núcleo da Aurora.'
        }
        auroraStore.setState('error')
        break
      }
    }
  }

  return { sendMessage, activeTool }
}
