import { auroraApi } from './auroraApi'

export interface VoiceTranscribeResponse {
  text: string
}

export interface VoiceSpeakRequest {
  text: string
}

export const voiceApi = {
  async transcribe(audio: Blob): Promise<VoiceTranscribeResponse> {
    const formData = new FormData()
    formData.append('audio', audio, 'recording.webm')
    const response = await auroraApi.post<VoiceTranscribeResponse>('/api/voice/transcribe', formData)
    return response.data
  },

  async speak(text: string): Promise<Blob> {
    const response = await auroraApi.post('/api/voice/speak', { text } satisfies VoiceSpeakRequest, {
      responseType: 'blob',
    })
    return response.data as Blob
  },
}
