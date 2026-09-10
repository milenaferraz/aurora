<script setup lang="ts">
import { computed, ref, onMounted, onUnmounted } from 'vue'
import { useChat } from '../composables/useChat'
import ChatMessages from '../components/chat/ChatMessages.vue'
import ChatInput from '../components/chat/ChatInput.vue'
import AuroraCore from '../components/aurora/AuroraCore.vue'
import { useAuroraStore } from '../stores/aurora.store'
import { useChatStore } from '../stores/chat.store'

const auroraStore = useAuroraStore()
const chatStore = useChatStore()
const { sendMessage } = useChat()

const hasMessages = computed(() => chatStore.messages.length > 0)

/* Star field canvas */
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
  <div class="chat-page">
    <!-- Background layers -->
    <div class="bg-deep" aria-hidden="true" />
    <div class="bg-nebula" aria-hidden="true" />
    <canvas ref="starCanvas" class="star-canvas" aria-hidden="true" />

    <!-- Header -->
    <header class="chat-header">
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
        <span class="status-label">ONLINE</span>
      </div>
    </header>

    <!-- HUD left (idle only) -->
    <aside class="hud hud-left" :class="{ 'hud-gone': hasMessages }" aria-hidden="true">
      <span class="hud-line">PENSAR</span>
      <span class="hud-line">PLANEJAR</span>
      <span class="hud-line">REALIZAR</span>
      <span class="hud-line">EVOLUIR</span>
    </aside>

    <!-- HUD right (idle only) -->
    <aside class="hud hud-right" :class="{ 'hud-gone': hasMessages }" aria-hidden="true">
      <span class="hud-line">AURORA SYSTEM</span>
      <span class="hud-line">CORE ONLINE</span>
      <span class="hud-line">VOICE READY</span>
    </aside>

    <!-- Main content -->
    <main class="chat-main" :class="{ 'in-chat': hasMessages }">
      <!-- Aurora Core (protagonist) -->
      <div class="core-section" :class="{ 'core-compact': hasMessages }">
        <AuroraCore :state="auroraStore.state" :compact="hasMessages" />
      </div>

      <!-- Conversation (only when chatting) -->
      <div v-if="hasMessages" class="messages-section">
        <ChatMessages />
      </div>

      <!-- Input area (always present) -->
      <div class="input-area">
        <ChatInput @submit="sendMessage" />
      </div>

      <!-- Tagline (idle only) -->
      <p class="tagline" :class="{ 'tagline-gone': hasMessages }" aria-hidden="true">
        Inteligência em movimento
      </p>
    </main>
  </div>
</template>

<style scoped>
/* ── Page shell ───────────────────────────────── */
.chat-page {
  position: fixed;
  inset: 0;
  overflow: hidden;
  display: flex;
  flex-direction: column;
  font-family: 'Inter', sans-serif;
  color: rgba(226, 232, 240, 0.90);
}

/* ── Backgrounds ──────────────────────────────── */
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

/* ── Header ───────────────────────────────────── */
.chat-header {
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

/* ── HUD overlays ─────────────────────────────── */
.hud {
  position: fixed;
  top: 50%;
  transform: translateY(-50%);
  z-index: 15;
  display: flex;
  flex-direction: column;
  gap: 14px;
  transition: opacity 0.5s ease, transform 0.5s ease;
}

.hud-left  { left: 36px; }
.hud-right { right: 36px; align-items: flex-end; }

.hud-gone {
  opacity: 0;
  pointer-events: none;
}
.hud-left.hud-gone  { transform: translateY(-50%) translateX(-16px); }
.hud-right.hud-gone { transform: translateY(-50%) translateX(16px); }

.hud-line {
  font-family: 'Space Grotesk', sans-serif;
  font-size: 10px;
  letter-spacing: 0.24em;
  text-transform: uppercase;
  color: rgba(99, 102, 241, 0.38);
}

/* ── Main content area ────────────────────────── */
.chat-main {
  position: relative;
  z-index: 10;
  flex: 1;
  overflow: hidden;
  display: flex;
  flex-direction: column;
  align-items: center;
}

/* Core section: fills main in idle, compact at top in chat */
.core-section {
  display: flex;
  align-items: center;
  justify-content: center;
  flex: 1;
  transition: flex 0.7s cubic-bezier(0.4, 0, 0.2, 1),
              padding 0.6s ease;
}

.core-compact {
  flex: 0 0 auto;
  padding: 20px 0 10px;
}

/* Messages */
.messages-section {
  flex: 1;
  width: 100%;
  max-width: 780px;
  overflow: hidden;
  display: flex;
  flex-direction: column;
}

/* Input */
.input-area {
  width: 100%;
  max-width: 780px;
  padding: 14px 24px 28px;
  flex-shrink: 0;
}

/* Tagline */
.tagline {
  font-size: 13px;
  color: rgba(100, 116, 139, 0.45);
  letter-spacing: 0.06em;
  padding-bottom: 28px;
  margin: 0;
  flex-shrink: 0;
  transition: opacity 0.4s ease;
}

.tagline-gone {
  opacity: 0;
  pointer-events: none;
  position: absolute;
}

/* ── Keyframes ────────────────────────────────── */
@keyframes dot-pulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.35; }
}

/* ── Mobile ───────────────────────────────────── */
@media (max-width: 768px) {
  .hud { display: none; }
  .chat-header { padding: 16px 20px; }
  .logo-text { font-size: 16px; letter-spacing: 0.24em; }
  .input-area { padding: 10px 16px 20px; }
}
</style>
