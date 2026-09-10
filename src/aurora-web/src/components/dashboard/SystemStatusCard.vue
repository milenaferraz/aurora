<script setup lang="ts">
import type { SystemStatus } from '../../types/dashboard'

defineProps<{
  system: SystemStatus
}>()
</script>

<template>
  <div class="status-card">
    <h3 class="card-title">Sistema</h3>
    <div class="status-list">
      <div class="status-item">
        <span class="dot" :class="system.api === 'online' ? 'online' : 'offline'" />
        <span class="label">Aurora Online</span>
      </div>
      <div class="status-item">
        <span class="dot" :class="system.hermes === 'online' ? 'online' : 'offline'" />
        <span class="label">Hermes Online</span>
      </div>
      <div v-if="system.hermes !== 'online'" class="offline-warning">
        Hermes parece estar offline
      </div>
    </div>
  </div>
</template>

<style scoped>
.status-card {
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 12px;
  padding: 20px;
}

.card-title {
  font-size: 0.85rem;
  text-transform: uppercase;
  letter-spacing: 0.1em;
  color: #64748b;
  margin: 0 0 12px;
}

.status-list {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.status-item {
  display: flex;
  align-items: center;
  gap: 8px;
}

.dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
}

.dot.online {
  background: #22c55e;
  box-shadow: 0 0 6px rgba(34, 197, 94, 0.6);
}

.dot.offline {
  background: #ef4444;
  box-shadow: 0 0 6px rgba(239, 68, 68, 0.6);
}

.label {
  font-size: 0.9rem;
  color: #cbd5e1;
}

.offline-warning {
  font-size: 0.8rem;
  color: #ef4444;
  margin-top: 4px;
}
</style>
