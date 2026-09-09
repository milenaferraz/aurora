import { mount } from '@vue/test-utils'
import { describe, it, expect } from 'vitest'
import AuroraCore from '../AuroraCore.vue'

const states = ['idle', 'listening', 'thinking', 'speaking', 'working', 'error'] as const

describe('AuroraCore', () => {
  states.forEach((state) => {
    it(`renders without errors in state: ${state}`, () => {
      const wrapper = mount(AuroraCore, { props: { state } })
      expect(wrapper.exists()).toBe(true)
    })
  })

  it('applies state class based on prop', () => {
    const wrapper = mount(AuroraCore, { props: { state: 'thinking' } })
    expect(wrapper.find('.aurora-core').classes()).toContain('aurora-thinking')
  })
})
