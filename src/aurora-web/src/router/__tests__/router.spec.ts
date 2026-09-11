import { describe, it, expect } from 'vitest'
import { createMemoryHistory } from 'vue-router'
import { createAuroraRouter } from '../index'

describe('aurora router', () => {
  it('resolves / as the Aurora experience', async () => {
    const router = createAuroraRouter(createMemoryHistory())
    await router.push('/')
    expect(router.currentRoute.value.path).toBe('/')
    expect(router.currentRoute.value.name).toBe('aurora')
  })

  it('redirects /aurora to /', async () => {
    const router = createAuroraRouter(createMemoryHistory())
    await router.push('/aurora')
    expect(router.currentRoute.value.path).toBe('/')
  })

  it('redirects /chat to /', async () => {
    const router = createAuroraRouter(createMemoryHistory())
    await router.push('/chat')
    expect(router.currentRoute.value.path).toBe('/')
  })
})
