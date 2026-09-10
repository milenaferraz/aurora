<script setup lang="ts">
export interface ActivityEntry {
  id: number
  timestamp: string
  emoji: string
  label: string
}

defineProps<{
  entries: ActivityEntry[]
}>()
</script>

<template>
  <div class="feed">
    <h3 class="feed-title">Atividade Recente</h3>
    <div v-if="entries.length > 0" class="entries">
      <div v-for="entry in entries" :key="entry.id" class="entry">
        <span class="emoji">{{ entry.emoji }}</span>
        <div class="entry-content">
          <span class="entry-label">{{ entry.label }}</span>
          <span class="entry-time">{{ entry.timestamp }}</span>
        </div>
      </div>
    </div>
    <p v-else class="empty">Nenhuma atividade recente</p>
  </div>
</template>

<style scoped>
.feed {
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 12px;
  padding: 20px;
}

.feed-title {
  font-size: 0.85rem;
  text-transform: uppercase;
  letter-spacing: 0.1em;
  color: #64748b;
  margin: 0 0 12px;
}

.entries {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.entry {
  display: flex;
  align-items: flex-start;
  gap: 10px;
}

.emoji {
  font-size: 1.1rem;
  line-height: 1.4;
}

.entry-content {
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.entry-label {
  font-size: 0.9rem;
  color: #cbd5e1;
}

.entry-time {
  font-size: 0.75rem;
  color: #475569;
}

.empty {
  font-size: 0.9rem;
  color: #475569;
  margin: 0;
}
</style>
