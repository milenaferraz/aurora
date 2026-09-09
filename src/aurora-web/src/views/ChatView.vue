<script setup lang="ts">
import { useChat } from '../composables/useChat'
import ChatMessages from '../components/chat/ChatMessages.vue'
import ChatInput from '../components/chat/ChatInput.vue'
import AuroraThinking from '../components/chat/AuroraThinking.vue'
import ToolActivity from '../components/chat/ToolActivity.vue'
import AuroraCore from '../components/aurora/AuroraCore.vue'
import { useAuroraStore } from '../stores/aurora.store'

const auroraStore = useAuroraStore()
const { sendMessage, activeTool } = useChat()
</script>

<template>
  <div class="min-h-screen bg-[#050510] text-white flex flex-col">
    <!-- Header -->
    <div class="flex items-center gap-3 px-6 py-4 border-b border-white/10">
      <AuroraCore :state="auroraStore.state" />
      <div>
        <h1 class="text-lg font-semibold text-blue-300">Aurora</h1>
        <p class="text-xs text-gray-400">Assistente Pessoal</p>
      </div>
    </div>

    <!-- Messages -->
    <ChatMessages />

    <!-- Tool activity + thinking -->
    <ToolActivity :tool="activeTool" />
    <AuroraThinking />

    <!-- Input -->
    <ChatInput @submit="sendMessage" />
  </div>
</template>
