import { setActivePinia, createPinia } from 'pinia'
import { defineComponent } from 'vue'
import { mount, flushPromises } from '@vue/test-utils'
import { describe, beforeEach, afterEach, it, expect, vi } from 'vitest'
import { useAuroraStore } from '../../stores/aurora.store'
import { useChatStore } from '../../stores/chat.store'

const endSession = vi.fn().mockResolvedValue(undefined)
const sendUserMessage = vi.fn()

const startSession = vi.fn()

vi.mock('@elevenlabs/client', () => ({
  Conversation: {
    startSession: (...args: unknown[]) => startSession(...args),
  },
}))

describe('useElevenLabsConversation', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.stubEnv('VITE_ELEVENLABS_AGENT_ID', 'agent_test')
    startSession.mockReset()
    endSession.mockClear()
    sendUserMessage.mockClear()
    startSession.mockImplementation(async (options: {
      onConnect?: (props: { conversationId: string }) => void
      onModeChange?: (prop: { mode: 'speaking' | 'listening' }) => void
      onMessage?: (props: {
        message: string
        source: 'user' | 'ai'
        role: 'user' | 'agent'
      }) => void
    }) => {
      options.onConnect?.({ conversationId: 'conv_1' })
      return { endSession, sendUserMessage }
    })
    vi.stubGlobal('navigator', {
      mediaDevices: {
        getUserMedia: vi.fn().mockResolvedValue({ getTracks: () => [] }),
      },
    })
  })

  afterEach(() => {
    vi.unstubAllEnvs()
    vi.unstubAllGlobals()
  })

  it('does not start a session when the agent id is missing', async () => {
    vi.stubEnv('VITE_ELEVENLABS_AGENT_ID', '')
    const { useElevenLabsConversation } = await import('../useElevenLabsConversation')
    const { start } = useElevenLabsConversation()
    await start()

    expect(startSession).not.toHaveBeenCalled()
    expect(useAuroraStore().state).toBe('error')
  })

  it('starts a webrtc session with the configured agent id', async () => {
    const { useElevenLabsConversation } = await import('../useElevenLabsConversation')
    const { start, connected } = useElevenLabsConversation()
    await start()

    expect(startSession).toHaveBeenCalledWith(
      expect.objectContaining({
        agentId: 'agent_test',
        connectionType: 'webrtc',
      }),
    )
    expect(connected.value).toBe(true)
    expect(useAuroraStore().state).toBe('listening')
  })

  it('maps speaking mode to aurora speaking state', async () => {
    startSession.mockImplementation(async (options: {
      onConnect?: (props: { conversationId: string }) => void
      onModeChange?: (prop: { mode: 'speaking' | 'listening' }) => void
    }) => {
      options.onConnect?.({ conversationId: 'conv_1' })
      options.onModeChange?.({ mode: 'speaking' })
      return { endSession, sendUserMessage }
    })

    const { useElevenLabsConversation } = await import('../useElevenLabsConversation')
    const { start } = useElevenLabsConversation()
    await start()

    expect(useAuroraStore().state).toBe('speaking')
  })

  it('appends final voice transcripts to the chat store', async () => {
    startSession.mockImplementation(async (options: {
      onConnect?: (props: { conversationId: string }) => void
      onMessage?: (props: {
        message: string
        source: 'user' | 'ai'
        role: 'user' | 'agent'
      }) => void
    }) => {
      options.onConnect?.({ conversationId: 'conv_1' })
      options.onMessage?.({ message: 'Oi Aurora', source: 'user', role: 'user' })
      options.onMessage?.({ message: 'Oi Milena', source: 'ai', role: 'agent' })
      return { endSession, sendUserMessage }
    })

    const { useElevenLabsConversation } = await import('../useElevenLabsConversation')
    const { start } = useElevenLabsConversation()
    await start()

    const messages = useChatStore().messages
    expect(messages).toEqual([
      { role: 'user', content: 'Oi Aurora' },
      { role: 'assistant', content: 'Oi Milena' },
    ])
  })

  it('sets error state when microphone access is denied', async () => {
    vi.stubGlobal('navigator', {
      mediaDevices: {
        getUserMedia: vi.fn().mockRejectedValue(new Error('Permission denied')),
      },
    })

    const { useElevenLabsConversation } = await import('../useElevenLabsConversation')
    const { start } = useElevenLabsConversation()
    await start()

    expect(startSession).not.toHaveBeenCalled()
    expect(useAuroraStore().state).toBe('error')
  })

  it('starts listening automatically when autoStart is enabled', async () => {
    const { useElevenLabsConversation } = await import('../useElevenLabsConversation')
    const Host = defineComponent({
      setup() {
        useElevenLabsConversation({ autoStart: true })
        return () => null
      },
    })

    mount(Host)
    await flushPromises()

    expect(startSession).toHaveBeenCalled()
    expect(useAuroraStore().state).toBe('listening')
  })
})
