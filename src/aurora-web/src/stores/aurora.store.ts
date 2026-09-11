import { defineStore } from 'pinia'
import { ref } from 'vue'

export type AuroraState = 'idle' | 'listening' | 'processing' | 'thinking' | 'speaking' | 'working' | 'error'

export const useAuroraStore = defineStore('aurora', () => {
  const state = ref<AuroraState>('idle')

  function setState(newState: AuroraState) {
    state.value = newState
  }

  return { state, setState }
})
