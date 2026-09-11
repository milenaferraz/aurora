import { ref } from 'vue'
import { useAuroraStore } from '../stores/aurora.store'
import { useChatStore } from '../stores/chat.store'

const AURORA_API_URL = ((import.meta.env.VITE_AURORA_API_URL as string | undefined) ?? 'http://localhost:8080').replace(/\/$/, '')

type SendMessageOptions = {
  includeUserMessage?: boolean
}

export function useChat() {
  const auroraStore = useAuroraStore()
  const chatStore = useChatStore()
  const activeTool = ref<string | null>(null)

  async function sendMessage(message: string, options: SendMessageOptions = {}) {
    const trimmed = message.trim()
    if (!trimmed) return

    const includeUserMessage = options.includeUserMessage ?? true

    auroraStore.setState('thinking')
    chatStore.setStreaming(true)

    if (includeUserMessage) {
      chatStore.addMessage({ role: 'user', content: trimmed })
    }

    chatStore.addMessage({ role: 'assistant', content: '' })

    const controller = new AbortController()
    chatStore.setAbortController(controller)

    try {
      const response = await fetch(`${AURORA_API_URL}/api/chat/stream`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          message: trimmed,
          conversationId: chatStore.conversationId ?? undefined,
        }),
        signal: controller.signal,
      })

      if (!response.ok || !response.body) {
        throw new Error(`HTTP error: ${response.status}`)
      }

      const reader = response.body.getReader()
      const decoder = new TextDecoder()
      let buffer = ''
      let currentEventType = ''

      while (true) {
        const { done, value } = await reader.read()
        if (done) break

        buffer += decoder.decode(value, { stream: true })
        const lines = buffer.split('\n')
        buffer = lines.pop() ?? ''

        for (const line of lines) {
          if (line.startsWith('event: ')) {
            currentEventType = line.slice(7).trim()
          } else if (line.startsWith('data: ')) {
            const dataStr = line.slice(6).trim()
            try {
              const data = JSON.parse(dataStr) as Record<string, unknown>
              if (typeof data.conversationId === 'string' && data.conversationId.trim()) {
                chatStore.setConversationId(data.conversationId)
              }
              handleEvent(currentEventType, data)
            } catch {
              // ignore parse errors
            }
            currentEventType = ''
          }
        }
      }

      if (buffer.trim()) {
        const maybeData = buffer.split('\n').find((line) => line.startsWith('data: '))
        if (maybeData) {
          try {
            const data = JSON.parse(maybeData.slice(6).trim()) as Record<string, unknown>
            if (typeof data.conversationId === 'string' && data.conversationId.trim()) {
              chatStore.setConversationId(data.conversationId)
            }
          } catch {
            // ignore trailing parse errors
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
