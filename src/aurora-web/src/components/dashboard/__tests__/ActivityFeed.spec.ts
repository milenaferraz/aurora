import { mount } from '@vue/test-utils'
import { describe, it, expect } from 'vitest'
import ActivityFeed from '../ActivityFeed.vue'
import type { ActivityEntry } from '../ActivityFeed.vue'

const mockEntries: ActivityEntry[] = [
  { id: 1, timestamp: '09:00', emoji: '✅', label: 'Tarefa A' },
  { id: 2, timestamp: '08:30', emoji: '📅', label: 'Evento B' },
  { id: 3, timestamp: '08:00', emoji: '📧', label: 'Email C' },
]

describe('ActivityFeed', () => {
  it('renders all 3 entries with mocked data', () => {
    const wrapper = mount(ActivityFeed, { props: { entries: mockEntries } })
    expect(wrapper.findAll('.entry')).toHaveLength(3)
    expect(wrapper.text()).toContain('Tarefa A')
    expect(wrapper.text()).toContain('Evento B')
    expect(wrapper.text()).toContain('Email C')
  })

  it('renders empty state when entries array is empty', () => {
    const wrapper = mount(ActivityFeed, { props: { entries: [] } })
    expect(wrapper.text()).toContain('Nenhuma atividade recente')
    expect(wrapper.findAll('.entry')).toHaveLength(0)
  })
})
