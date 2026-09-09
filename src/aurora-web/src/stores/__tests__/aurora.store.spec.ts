import { setActivePinia, createPinia } from 'pinia'
import { describe, beforeEach, it, expect } from 'vitest'
import { useAuroraStore } from '../aurora.store'

describe('auroraStore', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
  })

  it('initializes to idle state', () => {
    const store = useAuroraStore()
    expect(store.state).toBe('idle')
  })

  it('setState updates state', () => {
    const store = useAuroraStore()
    store.setState('thinking')
    expect(store.state).toBe('thinking')
  })
})
