<template>
  <div class="conversation-page">
    <v-container fluid class="conversation-container">
      <v-row class="fill-height">
        <!-- Main Content Area -->
        <v-col cols="12" md="7" class="main-content">
          <ImageDisplay :item="selectedItem" @download="downloadImage" @share="shareImage" @delete="deleteImage" @add-reference="addImageReference" />
        </v-col>

        <!-- Upload Sidebar -->
        <v-col cols="12" md="5" class="upload-sidebar">
          <InputPanel v-model="promptText" :images="uploadedImages" :isGenerating="isGenerating"
            @trigger-upload="triggerUpload" @generate="generateImage" @remove-image="removeImage" />
        </v-col>
      </v-row>
      <!-- Timeline Sidebar -->
      <v-row>
        <v-col cols="12" class="timeline-sidebar">
          <TimelineStrip :items="timelineItems" :selected="selectedItem" @select="selectTimelineItem" @add-reference="addTimelineReference" />
        </v-col>
      </v-row>
    </v-container>
  </div>
</template>

<script lang="ts" setup>
import { ref, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import * as convoApi from '@/services/conversation'
import type { GenerateImageDto, GenerationRecordDto, ImageDto } from '@/types/api'
import ImageDisplay from '@/components/conversation/ImageDisplay.vue'
import InputPanel from '@/components/conversation/InputPanel.vue'
import TimelineStrip from '@/components/conversation/TimelineStrip.vue'
import type { TimelineItem } from '@/types/ui'
import { useConversationTimeline } from '@/composables/useConversationTimeline'
import { useAppStore } from '@/stores/app'
import { v4 } from 'uuid'
import { GenerationType } from '@/enums'

// Define layout for this page
defineOptions({
  layout: 'conversation'
})

// types moved to src/types/ui.ts

const router = useRouter()
const route = useRoute()
const appStore = useAppStore()

const promptText = ref('')
const isGenerating = ref(false)
const selectedItem = ref<TimelineItem | null>(null)
const timelineItems = ref<TimelineItem[]>([])
const uploadedImages = ref<ImageDto[]>([])

// const promptSuggestions = ['A beautiful landscape at sunset','Abstract colorful art','Fantasy character portrait','Modern architecture','Minimalist design']

// mapping helper
const { mapRecordsToTimeline } = useConversationTimeline()

const loadConversation = async (id: string) => {
  try {
    const data = await convoApi.getConversation(id)
    // Try to find records/history array from API response
    const records = data.generationRecords;
    const items = mapRecordsToTimeline(records as GenerationRecordDto[])
    timelineItems.value = items
    selectedItem.value = items.find(i => i.type === 'image') || null
  } catch (e) {
    // fallback to empty timeline
    timelineItems.value = []
    selectedItem.value = null
  }
}

onMounted(async () => {
  // Load or create conversation based on route params
  const idParam = (route.params as any).id as string
  if (!idParam || idParam === 'new') {
    const conv: any = await convoApi.createConversation()
    const newId = conv?.id || conv?.chatId || String(Date.now())
    router.replace(`/conversation/${newId}`)
    await loadConversation(newId)
  } else {
    await loadConversation(idParam)
  }
})

const selectTimelineItem = async (item: TimelineItem) => {
  // If selecting a prompt item and current prompt is not empty, show confirmation dialog
  if (item.type === 'prompt' && promptText.value.trim() && promptText.value.trim() != item.prompt.trim()) {
    await appStore.showPromptReplaceDialog({
      currentPrompt: promptText.value,
      newPrompt: item.prompt,
      onConfirm: () => {
        promptText.value = item.prompt
        selectedItem.value = item
      },
      onCancel: () => {
        // Just select the item without replacing the prompt
        selectedItem.value = item
      }
    })
  } else {
    // If it's an image item or prompt is empty, just select normally
    selectedItem.value = item
    // If it's a prompt item and current prompt is empty, fill it
    if (item.type === 'prompt' && !promptText.value.trim()) {
      promptText.value = item.prompt
    }
  }
}

const generateImage = async () => {
  if (!promptText.value.trim()) return

  // Add prompt to timeline immediately (optimistic UI)
  const promptItem: TimelineItem = {
    id: v4(),
    type: 'prompt',
    prompt: promptText.value,
    timestamp: new Date()
  }
  timelineItems.value.push(promptItem)

  try {
    const idParam = (route.params as any).id as string
    const payload:GenerateImageDto = { 
      prompt: promptText.value,
      generationType: uploadedImages.value.length > 0 ? GenerationType.ImageToImage : GenerationType.TextToImage,
      inputImageIds: uploadedImages.value.map(img => img.id)
    }
    isGenerating.value = true
    const result = await convoApi.generateImage(idParam, payload)

    // Try to map returned image dto or reload
    const image =  result.outputImage;
    if (image) {
      const imageItem: TimelineItem = {
        id: v4(),
        type: 'image',
        prompt: promptText.value,
        timestamp: new Date(),
        image
      }
      timelineItems.value.push(imageItem)
      selectedItem.value = imageItem
    } else {
      // If no direct image returned, reload conversation
      await loadConversation(idParam)
    }
  } catch (e) {
    console.error('Generate failed', e)
  } finally {
    promptText.value = ''
    isGenerating.value = false
  }
}

const triggerUpload = async (file: File) => {
  const res = await convoApi.uploadImage(file)
  if (res) {
    uploadedImages.value.push(res)
  }
}

const removeImage = (index: number) => {
  uploadedImages.value.splice(index, 1)
}

const downloadImage = () => {
  // Implementation for image download
  console.log('Download image')
}

const shareImage = () => {
  // Implementation for image sharing
  console.log('Share image')
}

const deleteImage = () => {
  // Implementation for image deletion
  console.log('Delete image')
}

const addImageReference = () => {
  if (selectedItem.value?.image) {
    addToReference(selectedItem.value.image)
  }
}

const addTimelineReference = (item: TimelineItem) => {
  if (item.image) {
    addToReference(item.image)
  }
}

const addToReference = (image: ImageDto) => {
  // Check if image is already in references
  const exists = uploadedImages.value.find(img => img.id === image.id)
  if (!exists) {
    uploadedImages.value.push(image)
  }
}

// moved into components where needed
</script>

<style scoped>
.conversation-page {
  height: 100%;
  display: flex;
  flex-direction: column;
}

.conversation-container {
  flex: 1;
  height: 100%;
  padding: 24px;
}

.timeline-sidebar,
.upload-sidebar {
  height: 100%;
}

.main-content {
  display: flex;
  flex-direction: column;
  height: 100%;
}

.content-area {
  display: flex;
  flex-direction: column;
  height: 100%;
}

.image-display-card {
  flex: 1;
  min-height: 400px;
}

.image-container {
  height: 500px;
  position: relative;
}

.placeholder-area {
  height: 500px;
}

.timeline-item {
  cursor: pointer;
}

.timeline-card {
  transition: all 0.2s ease;
  border: 2px solid transparent;
}

.timeline-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.timeline-card-active {
  border-color: rgb(var(--v-theme-primary));
  background-color: rgba(var(--v-theme-primary), 0.05);
}

.upload-btn:hover {
  transform: scale(1.05);
}

.uploaded-image-card {
  transition: transform 0.2s ease;
}

.uploaded-image-card:hover {
  transform: translateY(-1px);
}

.prompt-card {
  margin-top: auto;
}

/* Responsive adjustments */
@media (max-width: 960px) {
  .conversation-container {
    height: auto;
  }

  .main-content,
  .timeline-sidebar,
  .upload-sidebar {
    height: auto;
  }

  .image-container,
  .placeholder-area {
    height: 300px;
  }
}

/* input-area styles moved to InputPanel component */
</style>

<route lang="yaml">
meta:
  title: Conversation
  Layout: ConversationLayout
</route>