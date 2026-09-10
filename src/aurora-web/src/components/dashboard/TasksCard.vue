<script setup lang="ts">
defineProps<{
  pending: number
}>()
</script>

<template>
  <div class="card">
    <h3 class="card-title">Tarefas</h3>
    <div v-if="pending > 0" class="count">
      <span class="number">{{ pending }}</span>
      <span class="label">pendente{{ pending !== 1 ? 's' : '' }}</span>
    </div>
    <p v-else class="empty">Sem tarefas pendentes</p>
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

.count {
  display: flex;
  align-items: baseline;
  gap: 6px;
}

.number {
  font-size: 2rem;
  font-weight: 700;
  color: #8b5cf6;
}

.label {
  font-size: 0.9rem;
  color: #94a3b8;
}

.empty {
  font-size: 0.9rem;
  color: #475569;
  margin: 0;
}
</style>
