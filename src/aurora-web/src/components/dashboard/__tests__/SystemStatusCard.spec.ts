import { mount } from '@vue/test-utils'
import { describe, it, expect } from 'vitest'
import SystemStatusCard from '../SystemStatusCard.vue'
import type { SystemStatus } from '../../../types/dashboard'

describe('SystemStatusCard', () => {
  const onlineSystem: SystemStatus = { api: 'online', hermes: 'online', memory: 'unknown' }
  const offlineHermes: SystemStatus = { api: 'online', hermes: 'offline', memory: 'unknown' }

  it('renders Aurora Online and Hermes Online when both are online', () => {
    const wrapper = mount(SystemStatusCard, { props: { system: onlineSystem } })
    expect(wrapper.text()).toContain('Aurora Online')
    expect(wrapper.text()).toContain('Hermes Online')
  })

  it('does not show offline warning when hermes is online', () => {
    const wrapper = mount(SystemStatusCard, { props: { system: onlineSystem } })
    expect(wrapper.text()).not.toContain('Hermes parece estar offline')
  })

  it('shows offline warning when hermes is offline', () => {
    const wrapper = mount(SystemStatusCard, { props: { system: offlineHermes } })
    expect(wrapper.text()).toContain('Hermes parece estar offline')
  })

  it('shows red dot when hermes is offline', () => {
    const wrapper = mount(SystemStatusCard, { props: { system: offlineHermes } })
    const dots = wrapper.findAll('.dot')
    expect(dots[1].classes()).toContain('offline')
  })
})
