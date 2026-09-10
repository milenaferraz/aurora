<script setup lang="ts">
import type { NextEvent } from '../../types/dashboard'

defineProps<{
  nextEvent: NextEvent | null
}>()
</script>

<template>
  <div class="card">
    <h3 class="card-title">Próximo Evento</h3>
    <div v-if="nextEvent" class="event">
      <span class="event-time">{{ nextEvent.time }}</span>
      <span class="event-title">{{ nextEvent.title }}</span>
    </div>
    <p v-else class="empty">Nenhum evento hoje</p>
  </div>
</template>

<style scoped>
.card {
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

.event {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.event-time {
  font-size: 1.2rem;
  font-weight: 600;
  color: #0ea5e9;
}

.event-title {
  font-size: 0.9rem;
  color: #cbd5e1;
}

.empty {
  font-size: 0.9rem;
  color: #475569;
  margin: 0;
}
</style>
