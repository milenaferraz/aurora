<script setup lang="ts">
import { useAuroraStore } from '../../stores/aurora.store'

const auroraStore = useAuroraStore()

function onMouseDown() {
  auroraStore.setState('listening')
}

function onMouseUp() {
  auroraStore.setState('idle')
}
</script>

<template>
  <button
    class="voice-button"
    :class="{ listening: auroraStore.state === 'listening' }"
    @mousedown="onMouseDown"
    @mouseup="onMouseUp"
    @mouseleave="onMouseUp"
    aria-label="Falar com Aurora"
  >
    <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor" class="mic-icon">
      <path d="M12 1a4 4 0 0 1 4 4v6a4 4 0 0 1-8 0V5a4 4 0 0 1 4-4zm0 2a2 2 0 0 0-2 2v6a2 2 0 0 0 4 0V5a2 2 0 0 0-2-2zm-7 8a1 1 0 0 1 1 1 6 6 0 0 0 12 0 1 1 0 1 1 2 0 8 8 0 0 1-7 7.938V21h3a1 1 0 1 1 0 2H8a1 1 0 1 1 0-2h3v-2.062A8 8 0 0 1 4 12a1 1 0 0 1 1-1z"/>
    </svg>
  </button>
</template>

<style scoped>
.voice-button {
  width: 72px;
  height: 72px;
  border-radius: 50%;
  border: 2px solid rgba(14, 165, 233, 0.4);
  background: rgba(14, 165, 233, 0.1);
  color: #0ea5e9;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s ease;
}

.voice-button:hover {
  border-color: rgba(14, 165, 233, 0.7);
  background: rgba(14, 165, 233, 0.2);
  box-shadow: 0 0 20px rgba(14, 165, 233, 0.3);
}

.voice-button.listening {
  border-color: #06b6d4;
  background: rgba(6, 182, 212, 0.2);
  box-shadow: 0 0 30px rgba(6, 182, 212, 0.5);
  color: #06b6d4;
}

.mic-icon {
  width: 28px;
  height: 28px;
}
</style>
