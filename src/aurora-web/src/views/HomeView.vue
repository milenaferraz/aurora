<script setup lang="ts">
import { onMounted } from 'vue'
import { useAuroraStore } from '../stores/aurora.store'
import { useDashboardStore } from '../stores/dashboard.store'
import AuroraCore from '../components/aurora/AuroraCore.vue'
import VoiceButton from '../components/aurora/VoiceButton.vue'

const auroraStore = useAuroraStore()
const dashboardStore = useDashboardStore()

onMounted(() => {
  dashboardStore.fetchDashboard()
})
</script>

<template>
  <div class="home">
    <div class="greeting">
      <span class="greeting-text">
        {{ dashboardStore.data?.greeting ?? 'Olá' }}, Milena.
      </span>
    </div>

    <div class="core-area">
      <AuroraCore :state="auroraStore.state" />
    </div>

    <div class="voice-area">
      <VoiceButton />
    </div>
  </div>
</template>

<style scoped>
.home {
  min-height: 100vh;
  background: #050510;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 48px;
  padding: 32px;
}

.greeting {
  text-align: center;
}

.greeting-text {
  font-size: 2rem;
  font-weight: 300;
  color: #e2e8f0;
  letter-spacing: 0.02em;
}

.core-area {
  display: flex;
  align-items: center;
  justify-content: center;
}

.voice-area {
  display: flex;
  align-items: center;
  justify-content: center;
}
</style>
