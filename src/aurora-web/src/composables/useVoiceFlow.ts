import { computed, onUnmounted, ref } from 'vue'
import { chatApi } from '../api/chatApi'
import { voiceApi } from '../api/voiceApi'
import { useAuroraStore } from '../stores/aurora.store'
import { useChatStore } from '../stores/chat.store'

export function useVoiceFlow() {
  const auroraStore = useAuroraStore()
  const chatStore = useChatStore()

  const recording = ref(false)
  const processing = ref(false)
  const speaking = ref(false)
  const lastError = ref<string | null>(null)
  const recorder = ref<MediaRecorder | null>(null)
  const activeStream = ref<MediaStream | null>(null)
  const pendingChunks = ref<BlobPart[]>([])
  const playbackUrl = ref<string | null>(null)

  const isBusy = computed(() => recording.value || processing.value || speaking.value)

  function setPhase(phase: 'idle' | 'listening' | 'processing' | 'speaking' | 'error') {
    auroraStore.setState(phase)
  }

  function cleanupStream() {
    activeStream.value?.getTracks().forEach((track) => track.stop())
    activeStream.value = null
  }

  function cleanupPlayback() {
    if (playbackUrl.value) {
      URL.revokeObjectURL(playbackUrl.value)
      playbackUrl.value = null
    }
  }

  async function startRecording() {
    if (recording.value || processing.value || speaking.value) return
    lastError.value = null

    try {
      const stream = await navigator.mediaDevices.getUserMedia({ audio: true })
      activeStream.value = stream
      pendingChunks.value = []

      const mediaRecorder = new MediaRecorder(stream)
      recorder.value = mediaRecorder
      mediaRecorder.ondataavailable = (event) => {
        if (event.data.size > 0) pendingChunks.value.push(event.data)
      }
      mediaRecorder.onstop = () => {
        void processRecordedAudio()
      }

      recording.value = true
      setPhase('listening')
      mediaRecorder.start()
    } catch (error) {
      lastError.value = 'Não foi possível acessar o microfone.'
      setPhase('error')
      cleanupStream()
      recording.value = false
      recorder.value = null
      throw error
    }
  }

  async function stopRecording() {
    if (!recording.value || !recorder.value) return
    recording.value = false
    recorder.value.stop()
    recorder.value = null
    cleanupStream()
  }

  async function processRecordedAudio() {
    const audioBlob = new Blob(pendingChunks.value, {
      type: recorder.value?.mimeType || 'audio/webm',
    })

    if (!audioBlob.size) {
      lastError.value = 'Nenhum áudio foi capturado.'
      setPhase('error')
      return
    }

    processing.value = true
    setPhase('processing')

    try {
      const transcription = await voiceApi.transcribe(audioBlob)
      const transcript = transcription.text.trim()
      if (!transcript) {
        throw new Error('Transcrição vazia')
      }

      chatStore.addMessage({ role: 'user', content: transcript })

      const response = await chatApi.chat({
        message: transcript,
        conversationId: chatStore.conversationId ?? undefined,
      })
      chatStore.setConversationId(response.conversationId)
      chatStore.addMessage({ role: 'assistant', content: response.message })

      processing.value = false
      speaking.value = true
      setPhase('speaking')

      const audio = await voiceApi.speak(response.message)
      await playAudio(audio)

      speaking.value = false
      setPhase('idle')
    } catch (error) {
      lastError.value = error instanceof Error ? error.message : 'Falha ao processar o áudio.'
      setPhase('error')
    } finally {
      processing.value = false
      speaking.value = false
      cleanupPlayback()
      cleanupStream()
      pendingChunks.value = []
    }
  }

  function playAudio(audio: Blob) {
    cleanupPlayback()
    const url = URL.createObjectURL(audio)
    playbackUrl.value = url

    return new Promise<void>((resolve, reject) => {
      const player = new Audio(url)
      player.onended = () => resolve()
      player.onerror = () => reject(new Error('Falha ao reproduzir o áudio.'))
      void player.play().catch(reject)
    })
  }

  onUnmounted(() => {
    cleanupPlayback()
    cleanupStream()
  })

  return {
    startRecording,
    stopRecording,
    recording,
    processing,
    speaking,
    isBusy,
    lastError,
  }
}
