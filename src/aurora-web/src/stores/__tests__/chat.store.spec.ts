import { setActivePinia, createPinia } from 'pinia'
import { describe, beforeEach, it, expect } from 'vitest'
import { useChatStore } from '../chat.store'

describe('chatStore', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
  })

  it('addMessage appends to messages', () => {
    const store = useChatStore()
    store.addMessage({ role: 'user', content: 'hello' })
    expect(store.messages).toHaveLength(1)
    expect(store.messages[0].content).toBe('hello')
  })

  it('appendDelta appends to last assistant message', () => {
    const store = useChatStore()
    store.addMessage({ role: 'assistant', content: 'Hello' })
    store.appendDelta(' world')
    expect(store.messages[0].content).toBe('Hello world')
  })

  it('setStreaming toggles flag', () => {
    const store = useChatStore()
    store.setStreaming(true)
    expect(store.streaming).toBe(true)
    store.setStreaming(false)
    expect(store.streaming).toBe(false)
  })
})
