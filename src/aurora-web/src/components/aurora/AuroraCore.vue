<script setup lang="ts">
import { computed } from 'vue'

type AuroraState = 'idle' | 'listening' | 'thinking' | 'speaking' | 'working' | 'error'

const props = defineProps<{
  state: AuroraState
}>()

const stateClass = computed(() => `aurora-${props.state}`)
</script>

<template>
  <div class="aurora-core" :class="stateClass" role="img" :aria-label="`Aurora ${state}`">
    <div class="ring ring-outer" />
    <div class="ring ring-middle" />
    <div class="ring ring-inner" />
    <div class="core-dot" />
  </div>
</template>

<style scoped>
.aurora-core {
  position: relative;
  width: 120px;
  height: 120px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.ring {
  position: absolute;
  border-radius: 50%;
  border: 2px solid transparent;
}

.ring-outer { width: 120px; height: 120px; }
.ring-middle { width: 80px; height: 80px; }
.ring-inner { width: 48px; height: 48px; }

.core-dot {
  position: absolute;
  width: 16px;
  height: 16px;
  border-radius: 50%;
}

/* idle — electric blue, slow pulse */
.aurora-idle .ring {
  border-color: #0ea5e9;
  box-shadow: 0 0 12px #0ea5e960;
  animation: pulse 3s ease-in-out infinite;
}
.aurora-idle .core-dot { background: #0ea5e9; }

/* listening — cyan, faster pulse */
.aurora-listening .ring {
  border-color: #06b6d4;
  box-shadow: 0 0 16px #06b6d480;
  animation: pulse 1s ease-in-out infinite;
}
.aurora-listening .core-dot { background: #06b6d4; }

/* thinking — purple, rotation */
.aurora-thinking .ring {
  border-color: #8b5cf6;
  box-shadow: 0 0 16px #8b5cf660;
  animation: spin 2s linear infinite;
}
.aurora-thinking .ring-middle { animation-direction: reverse; animation-duration: 1.5s; }
.aurora-thinking .core-dot { background: #8b5cf6; }

/* speaking — wave rings */
.aurora-speaking .ring {
  border-color: #22d3ee;
  box-shadow: 0 0 16px #22d3ee60;
  animation: wave 1.2s ease-in-out infinite;
}
.aurora-speaking .ring-middle { animation-delay: 0.2s; }
.aurora-speaking .ring-inner { animation-delay: 0.4s; }
.aurora-speaking .core-dot { background: #22d3ee; }

/* working — orbit dot */
.aurora-working .ring {
  border-color: #f59e0b;
  box-shadow: 0 0 12px #f59e0b40;
  animation: pulse 2s ease-in-out infinite;
}
.aurora-working .core-dot {
  background: #f59e0b;
  animation: orbit 1.5s linear infinite;
  transform-origin: 0 30px;
}

/* error — red, static */
.aurora-error .ring {
  border-color: #ef4444;
  box-shadow: 0 0 12px #ef444440;
}
.aurora-error .core-dot { background: #ef4444; }

@keyframes pulse {
  0%, 100% { opacity: 1; transform: scale(1); }
  50% { opacity: 0.6; transform: scale(0.95); }
}

@keyframes spin {
  from { transform: rotate(0deg); }
  to { transform: rotate(360deg); }
}

@keyframes wave {
  0%, 100% { transform: scale(1); opacity: 1; }
  50% { transform: scale(1.08); opacity: 0.7; }
}

@keyframes orbit {
  from { transform: rotate(0deg) translateX(30px); }
  to { transform: rotate(360deg) translateX(30px); }
}
</style>
