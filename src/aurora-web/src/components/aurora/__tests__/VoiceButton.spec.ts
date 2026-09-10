import { mount } from '@vue/test-utils'
import { describe, it, expect, beforeEach } from 'vitest'
import { setActivePinia, createPinia } from 'pinia'
import VoiceButton from '../VoiceButton.vue'
import { useAuroraStore } from '../../../stores/aurora.store'

describe('VoiceButton', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
  })

  it('sets aurora state to listening on mousedown', async () => {
    const wrapper = mount(VoiceButton)
    const store = useAuroraStore()

    await wrapper.find('button').trigger('mousedown')

    expect(store.state).toBe('listening')
  })

  it('sets aurora state to idle on mouseup', async () => {
    const wrapper = mount(VoiceButton)
    const store = useAuroraStore()

    await wrapper.find('button').trigger('mousedown')
    await wrapper.find('button').trigger('mouseup')

    expect(store.state).toBe('idle')
  })

  it('sets aurora state to idle on mouseleave', async () => {
    const wrapper = mount(VoiceButton)
    const store = useAuroraStore()

    await wrapper.find('button').trigger('mousedown')
    await wrapper.find('button').trigger('mouseleave')

    expect(store.state).toBe('idle')
  })
})
