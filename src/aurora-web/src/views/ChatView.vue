<script setup lang="ts">
import { computed, onMounted, onUnmounted, ref } from 'vue'
import AuroraCore from '../components/aurora/AuroraCore.vue'
import ChatMessages from '../components/chat/ChatMessages.vue'
import { useAuroraStore } from '../stores/aurora.store'
import { useVoiceFlow } from '../composables/useVoiceFlow'

const auroraStore = useAuroraStore()
const { startRecording, stopRecording, recording, processing, speaking, isBusy, lastError } = useVoiceFlow()
const starCanvas = ref<HTMLCanvasElement | null>(null)

const statusLabel = computed(() => {
  if (recording.value) return 'OUVINDO'
  if (processing.value) return 'PROCESSANDO'
  if (speaking.value) return 'FALANDO'
  if (auroraStore.state === 'error') return 'OFFLINE'
  return 'ONLINE'
})

const prompt = computed(() => {
  if (lastError.value) return lastError.value
  if (recording.value) return 'Solte o botão para enviar.'
  if (processing.value) return 'Convertendo áudio e consultando a Aurora...'
  if (speaking.value) return 'Aurora está respondendo em áudio.'
  return 'Segure o botão e fale com a Aurora.'
})

const buttonLabel = computed(() => {
  if (recording.value) return 'Soltar para enviar'
  if (processing.value) return 'Processando...'
  if (speaking.value) return 'Falando...'
  return 'Segure para falar'
})

function drawStars() {
  const canvas = starCanvas.value
  if (!canvas) return
  canvas.width = window.innerWidth
  canvas.height = window.innerHeight
  const ctx = canvas.getContext('2d')
  if (!ctx) return

  ctx.clearRect(0, 0, canvas.width, canvas.height)
  const count = 90
  for (let i = 0; i < count; i++) {
    const x = Math.random() * canvas.width
    const y = Math.random() * canvas.height
    const r = Math.random() * 1.1 + 0.3
    const a = Math.random() * 0.15 + 0.05
    ctx.beginPath()
    ctx.arc(x, y, r, 0, Math.PI * 2)
    ctx.fillStyle = `rgba(255, 255, 255, ${a})`
    ctx.fill()
  }
}

function onVoicePointerDown() {
  void startRecording()
}

function onVoicePointerUp() {
  void stopRecording()
}

onMounted(() => {
  drawStars()
  window.addEventListener('resize', drawStars)
})

onUnmounted(() => {
  window.removeEventListener('resize', drawStars)
})
</script>

<template>
  <div class="aurora-page">
    <div class="bg-deep" aria-hidden="true" />
    <div class="bg-nebula" aria-hidden="true" />
    <canvas ref="starCanvas" class="star-canvas" aria-hidden="true" />

    <header class="aurora-header">
      <div class="logo" aria-label="Aurora">
        <svg class="logo-mark" viewBox="0 0 24 24" fill="none" aria-hidden="true">
          <circle cx="12" cy="12" r="4" fill="url(#lm-a)" />
          <circle cx="12" cy="12" r="8" stroke="url(#lm-b)" stroke-width="1" opacity="0.6" />
          <circle cx="12" cy="12" r="11" stroke="url(#lm-c)" stroke-width="0.5" opacity="0.3" />
          <defs>
            <radialGradient id="lm-a" cx="50%" cy="50%" r="50%">
              <stop offset="0%" stop-color="#38BDF8" />
              <stop offset="100%" stop-color="#7C3AED" />
            </radialGradient>
            <linearGradient id="lm-b" x1="0%" y1="0%" x2="100%" y2="100%">
              <stop offset="0%" stop-color="#38BDF8" />
              <stop offset="100%" stop-color="#EC4899" />
            </linearGradient>
            <linearGradient id="lm-c" x1="0%" y1="0%" x2="100%" y2="100%">
              <stop offset="0%" stop-color="#7C3AED" />
              <stop offset="100%" stop-color="#EC4899" />
            </linearGradient>
          </defs>
        </svg>
        <span class="logo-text">A U R O R A</span>
      </div>

      <div class="status-pill">
        <span class="status-dot" />
        <span class="status-label">{{ statusLabel }}</span>
      </div>
    </header>

    <aside class="hud hud-left" aria-hidden="true">
      <span class="hud-line">PENSAR</span>
      <span class="hud-line">PLANEJAR</span>
      <span class="hud-line">REALIZAR</span>
      <span class="hud-line">EVOLUIR</span>
    </aside>

    <aside class="hud hud-right" aria-hidden="true">
      <span class="hud-line">AURORA SYSTEM</span>
      <span class="hud-line">VOICE READY</span>
      <span class="hud-line">API ROUTE</span>
    </aside>

    <main class="aurora-main">
      <div class="core-section">
        <AuroraCore :state="auroraStore.state" />
      </div>

      <p class="prompt" :class="{ alert: lastError }">
        {{ prompt }}
      </p>

      <button
        class="voice-control"
        :class="{ active: recording, busy: isBusy && !recording }"
        type="button"
        :disabled="processing || speaking"
        @pointerdown="onVoicePointerDown"
        @pointerup="onVoicePointerUp"
        @pointercancel="onVoicePointerUp"
        @pointerleave="onVoicePointerUp"
        @touchstart.prevent="onVoicePointerDown"
        @touchend.prevent="onVoicePointerUp"
      >
        {{ buttonLabel }}
      </button>

      <section class="chat-panel" aria-label="Histórico da conversa">
        <ChatMessages />
      </section>
    </main>
  </div>
</template>

<style scoped>
.aurora-page {
  position: fixed;
  inset: 0;
  overflow: hidden;
  display: flex;
  flex-direction: column;
  font-family: 'Inter', sans-serif;
  color: rgba(226, 232, 240, 0.90);
}

.bg-deep {
  position: absolute;
  inset: 0;
  background: #01040D;
  z-index: 0;
}

.bg-nebula {
  position: absolute;
  inset: 0;
  background:
    radial-gradient(circle at 50% 35%, rgba(124, 58, 237, 0.12) 0%, transparent 34%),
    radial-gradient(circle at 72% 68%, rgba(236, 72, 153, 0.06) 0%, transparent 28%),
    radial-gradient(circle at 18% 62%, rgba(56, 189, 248, 0.06) 0%, transparent 22%);
  z-index: 1;
}

.star-canvas {
  position: absolute;
  inset: 0;
  z-index: 2;
  pointer-events: none;
}

.aurora-header {
  position: relative;
  z-index: 20;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 20px 32px;
  flex-shrink: 0;
}

.logo {
  display: flex;
  align-items: center;
  gap: 10px;
}

.logo-mark {
  width: 22px;
  height: 22px;
  flex-shrink: 0;
}

.logo-text {
  font-size: 20px;
  font-weight: 400;
  letter-spacing: 0.32em;
  color: rgba(186, 230, 253, 0.88);
}

.status-pill {
  display: flex;
  align-items: center;
  gap: 7px;
  padding: 5px 14px;
  border: 1px solid rgba(148, 163, 184, 0.08);
  border-radius: 9999px;
}

.status-dot {
  width: 6px;
  height: 6px;
  border-radius: 50%;
  background: #38BDF8;
  box-shadow: 0 0 8px rgba(56, 189, 248, 0.55);
  animation: dot-pulse 2.2s ease-in-out infinite;
}

.status-label {
  font-family: 'Space Grotesk', sans-serif;
  font-size: 10px;
  letter-spacing: 0.24em;
  text-transform: uppercase;
  color: rgba(148, 163, 184, 0.65);
}

.hud {
  position: absolute;
  top: 50%;
  transform: translateY(-50%);
  z-index: 20;
  display: flex;
  flex-direction: column;
  gap: 14px;
}

.hud-left { left: 36px; }
.hud-right { right: 36px; align-items: flex-end; }

.hud-line {
  font-family: 'Space Grotesk', sans-serif;
  font-size: 11px;
  letter-spacing: 0.22em;
  color: rgba(100, 116, 139, 0.48);
}

.aurora-main {
  position: relative;
  z-index: 10;
  flex: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 16px 24px 28px;
  min-height: 0;
}

.core-section {
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.prompt {
  margin: 24px 0 14px;
  font-size: 13px;
  color: rgba(100, 116, 139, 0.62);
  letter-spacing: 0.05em;
  text-align: center;
  max-width: 660px;
}

.prompt.alert {
  color: rgba(248, 113, 113, 0.85);
}

.voice-control {
  border: 1px solid rgba(148, 163, 184, 0.12);
  background: linear-gradient(135deg, rgba(56, 189, 248, 0.14), rgba(168, 85, 247, 0.16));
  color: rgba(226, 232, 240, 0.96);
  border-radius: 9999px;
  padding: 12px 22px;
  margin-bottom: 18px;
  letter-spacing: 0.08em;
  font-size: 12px;
  text-transform: uppercase;
  cursor: pointer;
  transition: transform 0.18s ease, box-shadow 0.18s ease, opacity 0.18s ease;
  box-shadow: 0 0 28px rgba(56, 189, 248, 0.08);
}

.voice-control:hover {
  transform: translateY(-1px);
  box-shadow: 0 0 34px rgba(168, 85, 247, 0.14);
}

.voice-control.active {
  background: linear-gradient(135deg, rgba(124, 58, 237, 0.30), rgba(236, 72, 153, 0.28));
  box-shadow: 0 0 40px rgba(236, 72, 153, 0.18);
}

.voice-control:disabled {
  opacity: 0.55;
  cursor: not-allowed;
}

.chat-panel {
  width: min(920px, calc(100vw - 48px));
  margin: 0 24px 24px;
  max-height: 180px;
  overflow: hidden;
  border-radius: 24px;
  border: 1px solid rgba(148, 163, 184, 0.10);
  background: rgba(8, 15, 30, 0.45);
  backdrop-filter: blur(12px);
}

@keyframes dot-pulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.35; }
}

@media (max-width: 900px) {
  .hud { display: none; }
  .aurora-header { padding: 16px 20px; }
  .logo-text { font-size: 16px; letter-spacing: 0.24em; }
  .chat-panel {
    width: calc(100vw - 24px);
    margin: 0 12px 12px;
  }
}
</style>
