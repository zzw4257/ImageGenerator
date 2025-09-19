<template>
  <ConversationLayout
    :conversation-title="conversationTitle"
    :conversation-subtitle="conversationSubtitle"
    :conversation-status="conversationStatus"
    :show-generation-status="isGenerating"
    :generation-progress="generationProgress"
    :generation-status-text="generationStatusText"
    :generation-subtext="generationSubtext"
    @header-action="handleHeaderAction"
    @quick-action="handleQuickAction"
    @cancel-generation="handleCancelGeneration"
  >
    <router-view />
  </ConversationLayout>
</template>

<script lang="ts" setup>
import { ref, computed } from 'vue'
import { useRoute } from 'vue-router'
import ConversationLayout from './ConversationLayout.vue'

const route = useRoute()

// Reactive state
const isGenerating = ref(false)
const generationProgress = ref(-1)
const generationStatusText = ref('Generating image...')
const generationSubtext = ref('')

// Computed properties
const conversationTitle = computed(() => {
  // You can get this from route params or a store
  const id = (route.params as any).id as string
  if (id === 'new') {
    return 'New Conversation'
  }
  return `Conversation ${id}`
})

const conversationSubtitle = computed(() => {
  const id = (route.params as any).id as string
  if (id === 'new') {
    return 'Start creating amazing images'
  }
  return 'Continue your creative journey'
})

const conversationStatus = computed(() => {
  if (isGenerating.value) {
    return 'Generating'
  }
  return 'Active'
})

// Event handlers
const handleHeaderAction = (action: string) => {
  switch (action) {
    case 'share':
      console.log('Share conversation')
      break
    case 'download':
      console.log('Download conversation')
      break
    default:
      console.log('Header action:', action)
  }
}

const handleQuickAction = (action: string) => {
  switch (action) {
    case 'save':
      console.log('Save conversation')
      break
    case 'copy':
      console.log('Copy conversation')
      break
    case 'delete':
      console.log('Delete conversation')
      break
    default:
      console.log('Quick action:', action)
  }
}

const handleCancelGeneration = () => {
  isGenerating.value = false
  generationProgress.value = -1
  console.log('Generation cancelled')
}

// Expose methods for child components to call
defineExpose({
  startGeneration: () => {
    isGenerating.value = true
    generationProgress.value = -1
    generationStatusText.value = 'Starting generation...'
  },
  updateGenerationProgress: (progress: number, text?: string) => {
    generationProgress.value = progress
    if (text) generationStatusText.value = text
  },
  finishGeneration: () => {
    isGenerating.value = false
    generationProgress.value = 100
  }
})
</script>