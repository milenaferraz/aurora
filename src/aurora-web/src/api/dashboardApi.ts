import { auroraApi } from './auroraApi'
import type { DashboardResponse } from '../types/dashboard'

export const dashboardApi = {
  async getDashboard(): Promise<DashboardResponse> {
    const response = await auroraApi.get<DashboardResponse>('/api/dashboard')
    return response.data
  },
}
