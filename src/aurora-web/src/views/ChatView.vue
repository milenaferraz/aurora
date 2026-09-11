<script setup lang="ts">
import { computed, ref, onMounted, onUnmounted } from 'vue'
import { useElevenLabsConversation } from '../composables/useElevenLabsConversation'
import { extractAuroraCommand } from '../composables/voiceWakeWord'
import AuroraCore from '../components/aurora/AuroraCore.vue'
import { useAuroraStore } from '../stores/aurora.store'
import { useChat } from '../composables/useChat'
import ChatMessages from '../components/chat/ChatMessages.vue'

const auroraStore = useAuroraStore()
const { sendMessage } = useChat()
const pendingVoiceCommand = ref(false)
const lastSpokenCommand = ref('')

const { start, connecting, lastError, needsGesture, isConfigured } = useElevenLabsConversation({
  autoStart: true,
  onTranscript: (transcript) => {
    if (transcript.role !== 'user') return

    const match = extractAuroraCommand(transcript.content)
    if (!match.activated || !match.command) return

    const normalized = match.command.toLowerCase()
    if (normalized === lastSpokenCommand.value.toLowerCase()) return

    lastSpokenCommand.value = match.command
    pendingVoiceCommand.value = true
    void sendMessage(match.command, { includeUserMessage: false }).finally(() => {
      pendingVoiceCommand.value = false
    })
  },
})

const statusLabel = computed(() => {
  if (connecting.value) return 'CONECTANDO'
  switch (auroraStore.state) {
    case 'listening':
      return 'OUVINDO'
    case 'speaking':
      return 'FALANDO'
    case 'error':
      return 'OFFLINE'
    case 'thinking':
      return 'PROCESSANDO'
    case 'working':
      return 'EXECUTANDO'
    default:
      return 'ONLINE'
  }
})

const prompt = computed(() => {
  if (!isConfigured.value) {
    return 'Configure VITE_ELEVENLABS_AGENT_ID para falar com a Aurora.'
  }
  if (needsGesture.value || lastError.value) {
    return lastError.value ?? 'Toque em qualquer lugar para começar'
  }
  if (pendingVoiceCommand.value) return 'Aurora ouviu o comando e está chamando a API...'
  if (connecting.value) return 'Conectando...'
  if (auroraStore.state === 'speaking') return 'Aurora está respondendo'
  if (auroraStore.state === 'listening') return 'Diga “Aurora” seguido do comando'
  return 'Inteligência em movimento'
})

function onPagePointer() {
  if (!isConfigured.value) return
  if (needsGesture.value || auroraStore.state === 'error') {
    void start()
  }
}

const starCanvas = ref<HTMLCanvasElement | null>(null)

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

onMounted(() => {
  drawStars()
  window.addEventListener('resize', drawStars)
})

onUnmounted(() => {
  window.removeEventListener('resize', drawStars)
})
</script>

<template>
  <div class="aurora-page" @pointerdown="onPagePointer">
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
      <span class="hud-line">HERMES ROUTE</span>
    </aside>

    <main class="aurora-main">
      <div class="core-section">
        <AuroraCore :state="auroraStore.state" />
      </div>
      <p class="prompt" :class="{ alert: !isConfigured || needsGesture || lastError }">
        {{ prompt }}
      </p>

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
  position: fixed;
  top: 50%;
  transform: translateY(-50%);
  z-index: 15;
  display: flex;
  flex-direction: column;
  gap: 14px;
}

.hud-left { left: 36px; }
.hud-right { right: 36px; align-items: flex-end; }

.hud-line {
  font-family: 'Space Grotesk', sans-serif;
  font-size: 10px;
  letter-spacing: 0.24em;
  text-transform: uppercase;
  color: rgba(99, 102, 241, 0.38);
}

.aurora-main {
  position: relative;
  z-index: 10;
  flex: 1;
  overflow: hidden;
  display: flex;
  flex-direction: column;
  align-items: center;
}

.core-section {
  display: flex;
  align-items: center;
  justify-content: center;
  flex: 1;
}

.prompt {
  font-size: 13px;
  color: rgba(100, 116, 139, 0.55);
  letter-spacing: 0.06em;
  padding-bottom: 20px;
  margin: 0;
  flex-shrink: 0;
  text-align: center;
  max-width: 560px;
}

.prompt.alert {
  color: rgba(248, 113, 113, 0.85);
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

@media (max-width: 768px) {
  .hud { display: none; }
  .aurora-header { padding: 16px 20px; }
  .logo-text { font-size: 16px; letter-spacing: 0.24em; }
  .chat-panel {
    width: calc(100vw - 24px);
    margin: 0 12px 12px;
  }
}
</style>
