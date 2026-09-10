<script setup lang="ts">
import { computed } from 'vue'

export type AuroraState = 'idle' | 'listening' | 'thinking' | 'working' | 'speaking' | 'error'

const props = defineProps<{
  state: AuroraState
  compact?: boolean
}>()

const size = computed(() => (props.compact ? 240 : 500))
const energySize = computed(() => Math.round(size.value * 0.74))
const ribbonSize = computed(() => Math.round(size.value * 0.50))
const centerSize = computed(() => Math.round(size.value * 0.33))

const stateLabel = computed(() => {
  const map: Record<AuroraState, string> = {
    idle: 'A U R O R A',
    listening: 'Estou ouvindo...',
    thinking: 'Pensando...',
    working: 'Executando...',
    speaking: 'Aurora está falando',
    error: 'Offline',
  }
  return map[props.state]
})

const showSub = computed(() => props.state === 'idle' && !props.compact)
</script>

<template>
  <div
    class="core-root"
    :class="[`state-${state}`, { compact }]"
    :style="{ width: `${size}px`, height: `${size}px` }"
    role="img"
    :aria-label="`Aurora — ${state}`"
  >
    <!-- Particles -->
    <div class="particles" aria-hidden="true">
      <span class="particle p1" />
      <span class="particle p2" />
      <span class="particle p3" />
      <span class="particle p4" />
      <span class="particle p5" />
      <span class="particle p6" />
      <span class="particle p7" />
      <span class="particle p8" />
    </div>

    <!-- Layer 1: Outer orbit (thin lines, slow rotation) -->
    <div
      class="layer outer-orbit"
      :style="{ width: `${size}px`, height: `${size}px` }"
      aria-hidden="true"
    />

    <!-- Layer 2: Energy ring (SVG gradient, medium speed reverse rotation) -->
    <svg
      class="layer energy-ring-svg"
      :style="{ width: `${energySize}px`, height: `${energySize}px` }"
      :viewBox="`0 0 ${energySize} ${energySize}`"
      xmlns="http://www.w3.org/2000/svg"
      aria-hidden="true"
    >
      <defs>
        <linearGradient id="aurora-energy-grad" x1="0%" y1="0%" x2="100%" y2="100%">
          <stop offset="0%" stop-color="#38BDF8" stop-opacity="0.9" />
          <stop offset="33%" stop-color="#7C3AED" stop-opacity="0.9" />
          <stop offset="66%" stop-color="#A855F7" stop-opacity="0.85" />
          <stop offset="100%" stop-color="#EC4899" stop-opacity="0.8" />
        </linearGradient>
        <filter id="aurora-ring-glow" x="-40%" y="-40%" width="180%" height="180%">
          <feGaussianBlur stdDeviation="3.5" result="blur" />
          <feMerge>
            <feMergeNode in="blur" />
            <feMergeNode in="SourceGraphic" />
          </feMerge>
        </filter>
      </defs>
      <circle
        :cx="energySize / 2"
        :cy="energySize / 2"
        :r="energySize / 2 - 1.5"
        stroke="url(#aurora-energy-grad)"
        stroke-width="1.5"
        fill="none"
        filter="url(#aurora-ring-glow)"
      />
    </svg>

    <!-- Layer 3: Neural ribbon (morphing organic shape) -->
    <div
      class="layer neural-ribbon"
      :style="{ width: `${ribbonSize}px`, height: `${ribbonSize}px` }"
      aria-hidden="true"
    />

    <!-- Layer 4: Core center -->
    <div
      class="layer core-center"
      :style="{ width: `${centerSize}px`, height: `${centerSize}px` }"
    >
      <template v-if="!compact">
        <span class="core-label" :class="{ 'is-logo': state === 'idle' }">
          {{ stateLabel }}
        </span>
        <span v-if="showSub" class="core-sub">Pronta.</span>
      </template>
    </div>
  </div>
</template>

<style scoped>
.core-root {
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  transition:
    width 0.7s cubic-bezier(0.4, 0, 0.2, 1),
    height 0.7s cubic-bezier(0.4, 0, 0.2, 1);
}

/* All layers are absolutely centered */
.layer {
  position: absolute;
  top: 50%;
  left: 50%;
  pointer-events: none;
}

/* ── Layer 1: Outer orbit ─────────────────────── */
.outer-orbit {
  border-radius: 50%;
  border: 1px solid rgba(99, 102, 241, 0.24);
  transform: translate(-50%, -50%);
  animation: aurora-orbit 25s linear infinite;
}

/* ── Layer 2: Energy ring SVG ─────────────────── */
.energy-ring-svg {
  transform: translate(-50%, -50%);
  animation: aurora-orbit-reverse 10s linear infinite;
  overflow: visible;
}

/* ── Layer 3: Neural ribbon ───────────────────── */
.neural-ribbon {
  border-radius: 60% 40% 55% 45% / 50% 60% 40% 50%;
  border: 1px solid rgba(168, 85, 247, 0.30);
  background: radial-gradient(
    circle,
    rgba(124, 58, 237, 0.07) 0%,
    transparent 70%
  );
  animation: aurora-neural 10s ease-in-out infinite;
}

/* ── Layer 4: Core center ─────────────────────── */
.core-center {
  border-radius: 50%;
  background: radial-gradient(
    circle,
    rgba(30, 41, 59, 0.85) 0%,
    rgba(2, 6, 23, 0.96) 100%
  );
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 6px;
  animation: aurora-breathe 4s ease-in-out infinite;
  box-shadow:
    0 0 40px rgba(59, 130, 246, 0.20),
    0 0 80px rgba(168, 85, 247, 0.16),
    0 0 140px rgba(236, 72, 153, 0.08);
}

.core-label {
  font-family: 'Inter', sans-serif;
  font-size: 12px;
  color: rgba(148, 163, 184, 0.85);
  text-align: center;
  letter-spacing: 0.10em;
  font-weight: 300;
  padding: 0 6px;
  line-height: 1.3;
}

.core-label.is-logo {
  font-size: 13px;
  letter-spacing: 0.32em;
  color: rgba(186, 230, 253, 0.90);
  font-weight: 400;
}

.core-sub {
  font-family: 'Space Grotesk', sans-serif;
  font-size: 10px;
  letter-spacing: 0.24em;
  text-transform: uppercase;
  color: rgba(148, 163, 184, 0.40);
}

/* ── Particles ────────────────────────────────── */
.particles {
  position: absolute;
  inset: 0;
  pointer-events: none;
}

.particle {
  position: absolute;
  border-radius: 50%;
  animation: aurora-particle 6s ease-in-out infinite;
}

.p1 { width: 2px; height: 2px; background: #38BDF8; top: 16%; left: 48%; animation-delay: 0s; animation-duration: 5.2s; opacity: 0.65; }
.p2 { width: 3px; height: 3px; background: #A855F7; top: 28%; left: 81%; animation-delay: 1.1s; animation-duration: 7.1s; opacity: 0.45; box-shadow: 0 0 4px #A855F7; }
.p3 { width: 1px; height: 1px; background: #EC4899; top: 66%; left: 84%; animation-delay: 2.3s; animation-duration: 6.4s; opacity: 0.55; }
.p4 { width: 2px; height: 2px; background: #38BDF8; top: 80%; left: 54%; animation-delay: 0.7s; animation-duration: 8.1s; opacity: 0.40; }
.p5 { width: 3px; height: 3px; background: #7C3AED; top: 72%; left: 18%; animation-delay: 3.2s; animation-duration: 5.8s; opacity: 0.50; box-shadow: 0 0 5px #7C3AED; }
.p6 { width: 1px; height: 1px; background: #A855F7; top: 38%; left: 11%; animation-delay: 1.8s; animation-duration: 7.6s; opacity: 0.45; }
.p7 { width: 2px; height: 2px; background: #EC4899; top: 14%; left: 66%; animation-delay: 4.1s; animation-duration: 6.7s; opacity: 0.60; box-shadow: 0 0 4px #EC4899; }
.p8 { width: 2px; height: 2px; background: #38BDF8; top: 22%; left: 24%; animation-delay: 2.7s; animation-duration: 9.2s; opacity: 0.38; }

/* ── State: idle ──────────────────────────────── */
.state-idle .core-center {
  box-shadow:
    0 0 30px rgba(56, 189, 248, 0.16),
    0 0 70px rgba(168, 85, 247, 0.12);
}

/* ── State: listening ─────────────────────────── */
.state-listening .core-center {
  animation: aurora-listen 1.6s ease-in-out infinite;
  box-shadow:
    0 0 45px rgba(168, 85, 247, 0.35),
    0 0 90px rgba(236, 72, 153, 0.22);
}
.state-listening .energy-ring-svg {
  animation: aurora-orbit-reverse 6.5s linear infinite;
}
.state-listening .neural-ribbon {
  border-color: rgba(236, 72, 153, 0.45);
  animation: aurora-neural 5s ease-in-out infinite;
}
.state-listening .outer-orbit {
  border-color: rgba(168, 85, 247, 0.30);
}

/* ── State: thinking ──────────────────────────── */
.state-thinking .outer-orbit {
  animation: aurora-orbit 11s linear infinite;
  border-color: rgba(99, 102, 241, 0.40);
}
.state-thinking .energy-ring-svg {
  animation: aurora-orbit-reverse 5s linear infinite;
}
.state-thinking .core-center {
  animation: aurora-breathe 2.2s ease-in-out infinite;
  box-shadow:
    0 0 50px rgba(37, 99, 235, 0.28),
    0 0 100px rgba(124, 58, 237, 0.18);
}
.state-thinking .neural-ribbon {
  animation: aurora-neural 4s ease-in-out infinite;
  border-color: rgba(99, 102, 241, 0.40);
}

/* ── State: working ───────────────────────────── */
.state-working .outer-orbit {
  animation: aurora-orbit 7s linear infinite;
  border-color: rgba(168, 85, 247, 0.42);
}
.state-working .energy-ring-svg {
  animation: aurora-orbit-reverse 4s linear infinite;
}
.state-working .core-center {
  animation: aurora-breathe 1.6s ease-in-out infinite;
  box-shadow:
    0 0 50px rgba(56, 189, 248, 0.25),
    0 0 100px rgba(168, 85, 247, 0.20),
    0 0 160px rgba(236, 72, 153, 0.10);
}
.state-working .neural-ribbon {
  border-color: rgba(99, 102, 241, 0.55);
  animation: aurora-neural 2.5s ease-in-out infinite;
}

/* ── State: speaking ──────────────────────────── */
.state-speaking .energy-ring-svg {
  animation: aurora-orbit-reverse 6s linear infinite;
}
.state-speaking .core-center {
  animation: aurora-speak 1.9s ease-in-out infinite;
  box-shadow:
    0 0 45px rgba(236, 72, 153, 0.30),
    0 0 90px rgba(168, 85, 247, 0.20),
    0 0 140px rgba(56, 189, 248, 0.10);
}
.state-speaking .outer-orbit {
  animation: aurora-orbit 17s linear infinite;
}

/* ── State: error (offline) ───────────────────── */
.state-error .core-center {
  animation: none;
  transform: translate(-50%, -50%);
  box-shadow: 0 0 18px rgba(100, 116, 139, 0.18);
}
.state-error .energy-ring-svg { opacity: 0.25; animation-play-state: paused; }
.state-error .neural-ribbon { opacity: 0.20; animation-play-state: paused; }
.state-error .outer-orbit { opacity: 0.20; animation-play-state: paused; }
.state-error .particle { opacity: 0 !important; }

/* ── Compact mode ─────────────────────────────── */
.compact .core-label.is-logo {
  font-size: 10px;
  letter-spacing: 0.28em;
}
.compact .core-label {
  font-size: 9px;
  letter-spacing: 0.08em;
}
.compact .p2,
.compact .p5,
.compact .p7 {
  display: none;
}

/* ── Animations ───────────────────────────────── */

@keyframes aurora-breathe {
  0%, 100% { transform: translate(-50%, -50%) scale(1); }
  50% { transform: translate(-50%, -50%) scale(1.025); }
}

@keyframes aurora-orbit {
  from { transform: translate(-50%, -50%) rotate(0deg); }
  to { transform: translate(-50%, -50%) rotate(360deg); }
}

@keyframes aurora-orbit-reverse {
  from { transform: translate(-50%, -50%) rotate(0deg); }
  to { transform: translate(-50%, -50%) rotate(-360deg); }
}

@keyframes aurora-listen {
  0%, 100% { transform: translate(-50%, -50%) scale(1); }
  50% { transform: translate(-50%, -50%) scale(1.055); }
}

@keyframes aurora-speak {
  0%, 100% { transform: translate(-50%, -50%) scale(1); }
  28% { transform: translate(-50%, -50%) scale(1.048); }
  62% { transform: translate(-50%, -50%) scale(1.018); }
}

@keyframes aurora-neural {
  0%, 100% {
    border-radius: 60% 40% 55% 45% / 50% 60% 40% 50%;
    transform: translate(-50%, -50%) rotate(0deg);
  }
  25% {
    border-radius: 45% 55% 40% 60% / 60% 40% 55% 45%;
    transform: translate(-50%, -50%) rotate(90deg);
  }
  50% {
    border-radius: 55% 45% 60% 40% / 45% 55% 50% 50%;
    transform: translate(-50%, -50%) rotate(180deg);
  }
  75% {
    border-radius: 40% 60% 45% 55% / 55% 45% 60% 40%;
    transform: translate(-50%, -50%) rotate(270deg);
  }
}

@keyframes aurora-particle {
  0%, 100% { transform: translateY(0) scale(1); }
  50% { transform: translateY(-14px) scale(1.25); }
}

/* ── Reduced motion ───────────────────────────── */
@media (prefers-reduced-motion: reduce) {
  .outer-orbit,
  .energy-ring-svg,
  .neural-ribbon,
  .core-center,
  .particle {
    animation: none !important;
  }
  .outer-orbit,
  .energy-ring-svg,
  .neural-ribbon,
  .core-center {
    transform: translate(-50%, -50%) !important;
  }
}
</style>
