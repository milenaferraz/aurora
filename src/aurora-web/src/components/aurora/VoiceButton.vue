<script setup lang="ts">
import { computed } from 'vue'
import { useAuroraStore } from '../../stores/aurora.store'

const props = withDefaults(
  defineProps<{
    voiceAgent?: boolean
    connected?: boolean
    connecting?: boolean
    disabled?: boolean
  }>(),
  {
    voiceAgent: false,
    connected: false,
    connecting: false,
    disabled: false,
  },
)

const emit = defineEmits<{
  toggle: []
}>()

const auroraStore = useAuroraStore()

const isActive = computed(
  () => props.connected || props.connecting || auroraStore.state === 'listening',
)

function onMouseDown() {
  if (props.voiceAgent || props.disabled) return
  auroraStore.setState('listening')
}

function onMouseUp() {
  if (props.voiceAgent || props.disabled) return
  if (auroraStore.state === 'listening') {
    auroraStore.setState('idle')
  }
}

function onClick() {
  if (!props.voiceAgent || props.disabled) return
  emit('toggle')
}
</script>

<template>
  <button
    class="voice-btn"
    :class="{ listening: isActive }"
    type="button"
    :disabled="disabled"
    :aria-pressed="voiceAgent ? connected : undefined"
    :aria-label="connected ? 'Encerrar conversa com Aurora' : 'Falar com Aurora'"
    @click="onClick"
    @mousedown="onMouseDown"
    @mouseup="onMouseUp"
    @mouseleave="onMouseUp"
    @touchstart.prevent="onMouseDown"
    @touchend.prevent="onMouseUp"
  >
    <span v-if="isActive" class="outer-ring" aria-hidden="true" />
    <svg viewBox="0 0 24 24" fill="currentColor" class="mic-icon" aria-hidden="true">
      <path d="M12 1a4 4 0 0 1 4 4v6a4 4 0 0 1-8 0V5a4 4 0 0 1 4-4zm0 2a2 2 0 0 0-2 2v6a2 2 0 0 0 4 0V5a2 2 0 0 0-2-2zm-7 8a1 1 0 0 1 1 1 6 6 0 0 0 12 0 1 1 0 1 1 2 0 8 8 0 0 1-7 7.938V21h3a1 1 0 1 1 0 2H8a1 1 0 1 1 0-2h3v-2.062A8 8 0 0 1 4 12a1 1 0 0 1 1-1z"/>
    </svg>
  </button>
</template>

<style scoped>
.voice-btn {
  position: relative;
  width: 52px;
  height: 52px;
  min-width: 52px;
  border-radius: 50%;
  border: none;
  background: linear-gradient(135deg, #7C3AED 0%, #A855F7 50%, #EC4899 100%);
  color: white;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: transform 0.2s ease, box-shadow 0.2s ease;
  box-shadow:
    0 0 18px rgba(124, 58, 237, 0.28),
    0 0 36px rgba(168, 85, 247, 0.12);
  flex-shrink: 0;
}

.voice-btn:hover {
  transform: scale(1.06);
  box-shadow:
    0 0 24px rgba(124, 58, 237, 0.40),
    0 0 48px rgba(168, 85, 247, 0.18);
}

.voice-btn:focus-visible {
  outline: 2px solid rgba(168, 85, 247, 0.6);
  outline-offset: 3px;
}

.voice-btn:disabled {
  opacity: 0.45;
  cursor: not-allowed;
  transform: none;
}

.voice-btn.listening {
  background: linear-gradient(135deg, #A855F7 0%, #C084FC 50%, #EC4899 100%);
  box-shadow:
    0 0 28px rgba(168, 85, 247, 0.50),
    0 0 60px rgba(236, 72, 153, 0.22);
  animation: voice-pulse 1.8s ease-in-out infinite;
}

.outer-ring {
  position: absolute;
  inset: -9px;
  border-radius: 50%;
  border: 1.5px solid rgba(168, 85, 247, 0.38);
  animation: outer-ring-expand 1.8s ease-in-out infinite;
  pointer-events: none;
}

.mic-icon {
  width: 20px;
  height: 20px;
  flex-shrink: 0;
}

@keyframes voice-pulse {
  0%, 100% { transform: scale(1); }
  50% { transform: scale(1.07); }
}

@keyframes outer-ring-expand {
  0%, 100% { transform: scale(1); opacity: 0.70; }
  50% { transform: scale(1.18); opacity: 0.22; }
}

@media (prefers-reduced-motion: reduce) {
  .voice-btn.listening { animation: none; }
  .outer-ring { animation: none; }
}
</style>
