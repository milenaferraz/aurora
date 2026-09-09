import { setActivePinia, createPinia } from 'pinia'
import { describe, beforeEach, it, expect, vi } from 'vitest'
import { useChat } from '../useChat'
import { useChatStore } from '../../stores/chat.store'
import { useAuroraStore } from '../../stores/aurora.store'

function makeSseStream(events: Array<{ event: string; data: object }>): ReadableStream {
  const lines = events.flatMap(e => [
    `event: ${e.event}`,
    `data: ${JSON.stringify(e.data)}`,
    '',
    '',
  ]).join('\n')
  const encoder = new TextEncoder()
  return new ReadableStream({
    start(controller) {
      controller.enqueue(encoder.encode(lines))
      controller.close()
    },
  })
}

describe('useChat', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
  })

  it('appendDelta called on message.delta event', async () => {
    const fakeResponse = new Response(
      makeSseStream([
        { event: 'message.delta', data: { content: 'Boa noite!' } },
        { event: 'message.completed', data: {} },
      ]),
      { status: 200, headers: { 'Content-Type': 'text/event-stream' } }
    )
    vi.stubGlobal('fetch', vi.fn().mockResolvedValue(fakeResponse))

    const { sendMessage } = useChat()
    await sendMessage('hello')

    const chatStore = useChatStore()
    const assistantMsg = chatStore.messages.find(m => m.role === 'assistant')
    expect(assistantMsg?.content).toContain('Boa noite!')

    vi.unstubAllGlobals()
  })

  it('aurora state is idle after message.completed', async () => {
    const fakeResponse = new Response(
      makeSseStream([
        { event: 'message.completed', data: {} },
      ]),
      { status: 200, headers: { 'Content-Type': 'text/event-stream' } }
    )
    vi.stubGlobal('fetch', vi.fn().mockResolvedValue(fakeResponse))

    const { sendMessage } = useChat()
    await sendMessage('hello')

    const auroraStore = useAuroraStore()
    expect(auroraStore.state).toBe('idle')

    vi.unstubAllGlobals()
  })

  it('aurora state is error and error message shown on error event', async () => {
    const fakeResponse = new Response(
      makeSseStream([
        { event: 'error', data: { error: 'hermes down' } },
      ]),
      { status: 200, headers: { 'Content-Type': 'text/event-stream' } }
    )
    vi.stubGlobal('fetch', vi.fn().mockResolvedValue(fakeResponse))

    const { sendMessage } = useChat()
    await sendMessage('hello')

    const auroraStore = useAuroraStore()
    const chatStore = useChatStore()
    expect(auroraStore.state).toBe('error')
    const assistantMsg = chatStore.messages.find(m => m.role === 'assistant')
    expect(assistantMsg?.content).toContain('Não consegui falar')

    vi.unstubAllGlobals()
  })
})
