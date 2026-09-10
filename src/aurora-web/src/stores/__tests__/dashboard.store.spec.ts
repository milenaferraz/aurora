import { setActivePinia, createPinia } from 'pinia'
import { describe, beforeEach, it, expect, vi } from 'vitest'
import { useDashboardStore } from '../dashboard.store'
import { dashboardApi } from '../../api/dashboardApi'
import type { DashboardResponse } from '../../types/dashboard'

vi.mock('../../api/dashboardApi')

const mockDashboard: DashboardResponse = {
  greeting: 'Boa noite',
  aurora: { status: 'online' },
  agenda: { eventsToday: 0, nextEvent: null },
  tasks: { pending: 2 },
  emails: { important: 1 },
  system: { api: 'online', hermes: 'online', memory: 'unknown' },
}

describe('dashboardStore', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  it('initializes with null data', () => {
    const store = useDashboardStore()
    expect(store.data).toBeNull()
    expect(store.loading).toBe(false)
    expect(store.error).toBeNull()
  })

  it('fetchDashboard sets data and clears loading on success', async () => {
    vi.mocked(dashboardApi.getDashboard).mockResolvedValue(mockDashboard)

    const store = useDashboardStore()
    await store.fetchDashboard()

    expect(store.data).toEqual(mockDashboard)
    expect(store.loading).toBe(false)
    expect(store.error).toBeNull()
  })

  it('fetchDashboard sets error and clears loading on failure', async () => {
    vi.mocked(dashboardApi.getDashboard).mockRejectedValue(new Error('Network error'))

    const store = useDashboardStore()
    await store.fetchDashboard()

    expect(store.data).toBeNull()
    expect(store.loading).toBe(false)
    expect(store.error).toBe('Network error')
  })
})
