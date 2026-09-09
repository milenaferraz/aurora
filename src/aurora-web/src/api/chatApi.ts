import { auroraApi } from './auroraApi'

export interface ChatRequest {
  message: string
  conversationId?: string
}

export interface ChatResponse {
  conversationId: string
  message: string
}

export const chatApi = {
  async chat(request: ChatRequest): Promise<ChatResponse> {
    const response = await auroraApi.post<ChatResponse>('/api/chat', request)
    return response.data
  },
}
