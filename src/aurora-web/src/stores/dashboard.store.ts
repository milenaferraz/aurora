import { defineStore } from 'pinia'
import { ref } from 'vue'
import { dashboardApi } from '../api/dashboardApi'
import type { DashboardResponse } from '../types/dashboard'

export const useDashboardStore = defineStore('dashboard', () => {
  const data = ref<DashboardResponse | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)

  async function fetchDashboard() {
    loading.value = true
    error.value = null
    try {
      data.value = await dashboardApi.getDashboard()
    } catch (e) {
      error.value = e instanceof Error ? e.message : 'Failed to load dashboard'
    } finally {
      loading.value = false
    }
  }

  return { data, loading, error, fetchDashboard }
})
