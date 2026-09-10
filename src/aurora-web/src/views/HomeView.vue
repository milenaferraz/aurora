<script setup lang="ts">
import { onMounted } from 'vue'
import { useAuroraStore } from '../stores/aurora.store'
import { useDashboardStore } from '../stores/dashboard.store'
import AuroraCore from '../components/aurora/AuroraCore.vue'
import VoiceButton from '../components/aurora/VoiceButton.vue'
import AudioVisualizer from '../components/shared/AudioVisualizer.vue'
import SystemStatusCard from '../components/dashboard/SystemStatusCard.vue'
import NextEventCard from '../components/dashboard/NextEventCard.vue'
import TasksCard from '../components/dashboard/TasksCard.vue'
import ProjectFocusCard from '../components/dashboard/ProjectFocusCard.vue'
import ActivityFeed from '../components/dashboard/ActivityFeed.vue'
import type { ActivityEntry } from '../components/dashboard/ActivityFeed.vue'

const auroraStore = useAuroraStore()
const dashboardStore = useDashboardStore()

const mockedActivity: ActivityEntry[] = [
  { id: 1, timestamp: '09:42', emoji: '✅', label: 'Tarefa concluída: revisão do relatório' },
  { id: 2, timestamp: '08:15', emoji: '📅', label: 'Reunião adicionada à agenda' },
  { id: 3, timestamp: '07:30', emoji: '📧', label: '3 e-mails importantes recebidos' },
]

onMounted(() => {
  dashboardStore.fetchDashboard()
})
</script>

<template>
  <div class="home">
    <div class="header">
      <span class="greeting-text">
        {{ dashboardStore.data?.greeting ?? 'Olá' }}, Milena.
      </span>
    </div>

    <div class="core-section">
      <AuroraCore :state="auroraStore.state" />
      <AudioVisualizer />
    </div>

    <div class="voice-section">
      <VoiceButton />
    </div>

    <div class="cards-grid" v-if="dashboardStore.data">
      <SystemStatusCard :system="dashboardStore.data.system" />
      <NextEventCard :next-event="dashboardStore.data.agenda.nextEvent" />
      <TasksCard :pending="dashboardStore.data.tasks.pending" />
      <ProjectFocusCard />
    </div>

    <ActivityFeed :entries="mockedActivity" />
  </div>
</template>

<style scoped>
.home {
  min-height: 100vh;
  background: #050510;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 40px;
  padding: 48px 24px;
}

.header {
  text-align: center;
}

.greeting-text {
  font-size: 2rem;
  font-weight: 300;
  color: #e2e8f0;
  letter-spacing: 0.02em;
}

.core-section {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 16px;
}

.voice-section {
  display: flex;
  align-items: center;
  justify-content: center;
}

.cards-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 16px;
  width: 100%;
  max-width: 900px;
}
</style>
