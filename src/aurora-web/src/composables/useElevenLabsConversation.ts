import { Conversation } from '@elevenlabs/client'
import { computed, onMounted, onUnmounted, ref, shallowRef } from 'vue'
import { useAuroraStore } from '../stores/aurora.store'
import { useChatStore } from '../stores/chat.store'

type VoiceSession = {
  endSession: () => Promise<void>
  sendUserMessage: (text: string) => void
}

type TranscriptMessage = {
  role: 'user' | 'assistant'
  content: string
}

type UseElevenLabsConversationOptions = {
  autoStart?: boolean
  onTranscript?: (message: TranscriptMessage) => void
}

function readAgentId(): string {
  return (import.meta.env.VITE_ELEVENLABS_AGENT_ID as string | undefined)?.trim() ?? ''
}

function isPermissionDenied(error: unknown): boolean {
  if (error instanceof DOMException) {
    return error.name === 'NotAllowedError' || error.name === 'SecurityError'
  }
  return error instanceof Error && /permission|notallowed|denied/i.test(error.message)
}

export function useElevenLabsConversation(options: UseElevenLabsConversationOptions = {}) {
  const auroraStore = useAuroraStore()
  const chatStore = useChatStore()

  const session = shallowRef<VoiceSession | null>(null)
  const connecting = ref(false)
  const connected = ref(false)
  const lastError = ref<string | null>(null)
  const needsGesture = ref(false)
  const disposed = ref(false)
  const isConfigured = computed(() => readAgentId().length > 0)

  async function start() {
    if (disposed.value || session.value || connecting.value) return

    const agentId = readAgentId()
    if (!agentId) {
      lastError.value = 'Agente ElevenLabs não configurado. Defina VITE_ELEVENLABS_AGENT_ID.'
      auroraStore.setState('error')
      return
    }

    connecting.value = true
    lastError.value = null

    try {
      await navigator.mediaDevices.getUserMedia({ audio: true })
      needsGesture.value = false
      auroraStore.setState('listening')

      const conversation = await Conversation.startSession({
        agentId,
        connectionType: 'webrtc',
        onConnect: () => {
          connected.value = true
          connecting.value = false
        },
        onDisconnect: () => {
          connected.value = false
          connecting.value = false
          session.value = null
          if (disposed.value || auroraStore.state === 'error') return
        },
        onError: (message) => {
          lastError.value = message || 'Falha na conversa de voz'
          auroraStore.setState('error')
        },
        onModeChange: ({ mode }) => {
          auroraStore.setState(mode === 'speaking' ? 'speaking' : 'listening')
        },
        onMessage: ({ message, role }) => {
          const content = message.trim()
          if (!content) return
          const transcript: TranscriptMessage = {
            role: role === 'user' ? 'user' : 'assistant',
            content,
          }
          chatStore.addMessage(transcript)
          options.onTranscript?.(transcript)
        },
      })

      session.value = conversation
      connected.value = true
      connecting.value = false
    } catch (error) {
      connecting.value = false
      connected.value = false
      session.value = null
      if (isPermissionDenied(error)) {
        needsGesture.value = true
        lastError.value = 'Toque em qualquer lugar para permitir o microfone'
      } else {
        lastError.value = 'Não foi possível iniciar a conversa de voz'
      }
      auroraStore.setState('error')
    }
  }

  async function stop() {
    disposed.value = true
    const current = session.value
    session.value = null
    connected.value = false
    connecting.value = false
    if (current) {
      await current.endSession()
    }
    if (auroraStore.state !== 'error') {
      auroraStore.setState('idle')
    }
  }

  onMounted(() => {
    if (options.autoStart) {
      void start()
    }
  })

  onUnmounted(() => {
    void stop()
  })

  return {
    start,
    connecting,
    connected,
    lastError,
    needsGesture,
    isConfigured,
  }
}
