<template>
  <div class="conversation-page">
    <v-container fluid class="conversation-container">
      <v-row class="fill-height">
        <!-- Timeline Sidebar -->
        <v-col cols="12" md="3" class="timeline-sidebar">
          <v-card
            rounded="xl"
            elevation="2"
            class="fill-height pa-4"
          >
            <h3 class="text-h6 font-weight-bold mb-4">Timeline</h3>
            
            <v-timeline
              side="end"
              align="start"
              truncate-line="both"
              size="small"
            >
              <v-timeline-item
                v-for="(item, index) in timelineItems"
                :key="index"
                :dot-color="item.type === 'image' ? 'primary' : 'secondary'"
                size="small"
                class="timeline-item"
                @click="selectTimelineItem(item)"
              >
                <template v-slot:icon>
                  <v-icon size="small">
                    {{ item.type === 'image' ? 'mdi-image' : 'mdi-message-text' }}
                  </v-icon>
                </template>
                
                <v-card
                  rounded="lg"
                  elevation="1"
                  class="timeline-card pa-3"
                  :class="{ 'timeline-card-active': selectedItem?.id === item.id }"
                >
                  <div class="text-caption text-grey-darken-1 mb-1">
                    {{ formatTime(item.timestamp) }}
                  </div>
                  
                  <div v-if="item.type === 'image'" class="d-flex align-center">
                    <v-avatar size="32" rounded="lg" class="mr-2">
                      <v-img :src="item.thumbnail" cover></v-img>
                    </v-avatar>
                    <div class="flex-grow-1">
                      <div class="text-body-2 font-weight-medium">
                        Image Generated
                      </div>
                      <div class="text-caption text-grey-darken-1 text-truncate">
                        {{ item.prompt }}
                      </div>
                    </div>
                  </div>
                  
                  <div v-else>
                    <div class="text-body-2 font-weight-medium mb-1">
                      Prompt
                    </div>
                    <div class="text-caption text-grey-darken-1">
                      {{ item.prompt }}
                    </div>
                  </div>
                </v-card>
              </v-timeline-item>
            </v-timeline>
          </v-card>
        </v-col>

        <!-- Main Content Area -->
        <v-col cols="12" md="6" class="main-content">
          <div class="content-area">
            <!-- Current Image Display -->
            <v-card
              rounded="xl"
              elevation="2"
              class="image-display-card mb-6"
            >
              <div class="image-container">
                <v-img
                  v-if="selectedItem?.type === 'image'"
                  :src="selectedItem.imageUrl"
                  height="500"
                  cover
                  class="rounded-xl"
                >
                  <template v-slot:placeholder>
                    <div class="d-flex align-center justify-center fill-height">
                      <v-progress-circular
                        color="primary"
                        indeterminate
                      ></v-progress-circular>
                    </div>
                  </template>
                </v-img>
                
                <!-- Placeholder when no image selected -->
                <div
                  v-else
                  class="d-flex align-center justify-center fill-height placeholder-area"
                >
                  <div class="text-center">
                    <v-icon
                      size="120"
                      color="grey-lighten-2"
                      class="mb-4"
                    >
                      mdi-image-outline
                    </v-icon>
                    <h3 class="text-h5 text-grey-darken-1 mb-2">
                      Select an image from timeline
                    </h3>
                    <p class="text-body-1 text-grey-darken-1">
                      Or generate a new one below
                    </p>
                  </div>
                </div>
              </div>
              
              <!-- Image Actions -->
              <v-card-actions v-if="selectedItem?.type === 'image'" class="pa-4">
                <v-chip
                  color="primary"
                  variant="outlined"
                  size="small"
                  prepend-icon="mdi-clock"
                >
                  {{ formatTime(selectedItem.timestamp) }}
                </v-chip>
                
                <v-spacer></v-spacer>
                
                <v-btn
                  icon
                  variant="text"
                  @click="downloadImage"
                >
                  <v-icon>mdi-download</v-icon>
                </v-btn>
                
                <v-btn
                  icon
                  variant="text"
                  @click="shareImage"
                >
                  <v-icon>mdi-share</v-icon>
                </v-btn>
                
                <v-btn
                  icon
                  variant="text"
                  @click="deleteImage"
                >
                  <v-icon>mdi-delete</v-icon>
                </v-btn>
              </v-card-actions>
            </v-card>

            <!-- Prompt Input Area -->
            <v-card
              rounded="xl"
              elevation="2"
              class="prompt-card"
            >
              <v-card-text class="pa-6">
                <div class="d-flex align-start gap-4">
                  <v-textarea
                    v-model="promptText"
                    placeholder="Describe the image you want to generate..."
                    variant="outlined"
                    rounded="xl"
                    rows="3"
                    auto-grow
                    hide-details
                    class="flex-grow-1"
                    :loading="isGenerating"
                  ></v-textarea>
                  
                  <v-btn
                    color="primary"
                    size="large"
                    rounded="xl"
                    :loading="isGenerating"
                    :disabled="!promptText.trim()"
                    @click="generateImage"
                  >
                    <v-icon>mdi-send</v-icon>
                  </v-btn>
                </div>
                
                <!-- Quick Actions -->
                <div class="d-flex align-center mt-4 gap-2">
                  <v-chip
                    v-for="suggestion in promptSuggestions"
                    :key="suggestion"
                    size="small"
                    variant="outlined"
                    @click="promptText = suggestion"
                  >
                    {{ suggestion }}
                  </v-chip>
                </div>
              </v-card-text>
            </v-card>
          </div>
        </v-col>

        <!-- Upload Sidebar -->
        <v-col cols="12" md="3" class="upload-sidebar">
          <v-card
            rounded="xl"
            elevation="2"
            class="fill-height"
          >
            <!-- Upload Button -->
            <div class="pa-6 text-center">
              <v-btn
                color="secondary"
                size="x-large"
                rounded="xl"
                variant="outlined"
                class="upload-btn mb-4"
                @click="triggerUpload"
              >
                <v-icon size="32">mdi-plus</v-icon>
              </v-btn>
              
              <h3 class="text-h6 font-weight-bold mb-2">Upload Image</h3>
              <p class="text-body-2 text-grey-darken-1 mb-4">
                Upload a reference image to enhance your prompts
              </p>
              
              <input
                ref="fileInput"
                type="file"
                accept="image/*"
                multiple
                @change="handleFileUpload"
                style="display: none"
              >
            </div>

            <v-divider></v-divider>

            <!-- Uploaded Images -->
            <div class="pa-4">
              <h4 class="text-subtitle-1 font-weight-bold mb-3">Reference Images</h4>
              
              <div v-if="uploadedImages.length === 0" class="text-center py-6">
                <v-icon
                  size="48"
                  color="grey-lighten-2"
                  class="mb-2"
                >
                  mdi-image-plus
                </v-icon>
                <p class="text-caption text-grey-darken-1">
                  No images uploaded yet
                </p>
              </div>
              
              <div v-else class="uploaded-images">
                <v-card
                  v-for="(image, index) in uploadedImages"
                  :key="index"
                  rounded="lg"
                  elevation="1"
                  class="mb-3 uploaded-image-card"
                >
                  <v-img
                    :src="image.url"
                    height="80"
                    cover
                    class="rounded-t-lg"
                  ></v-img>
                  
                  <v-card-actions class="pa-2">
                    <span class="text-caption text-truncate flex-grow-1">
                      {{ image.name }}
                    </span>
                    <v-btn
                      icon
                      size="small"
                      variant="text"
                      @click="removeImage(index)"
                    >
                      <v-icon size="16">mdi-close</v-icon>
                    </v-btn>
                  </v-card-actions>
                </v-card>
              </div>
            </div>
          </v-card>
        </v-col>
      </v-row>
    </v-container>
  </div>
</template>

<script lang="ts" setup>
import { ref, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'

// Define layout for this page
defineOptions({
  layout: 'conversation'
})

interface TimelineItem {
  id: string
  type: 'prompt' | 'image'
  prompt: string
  timestamp: Date
  thumbnail?: string
  imageUrl?: string
}

interface UploadedImage {
  name: string
  url: string
  file: File
}

const router = useRouter()
const route = useRoute()

const promptText = ref('')
const isGenerating = ref(false)
const selectedItem = ref<TimelineItem | null>(null)
const timelineItems = ref<TimelineItem[]>([])
const uploadedImages = ref<UploadedImage[]>([])
const fileInput = ref<HTMLInputElement>()

const promptSuggestions = [
  'A beautiful landscape at sunset',
  'Abstract colorful art',
  'Fantasy character portrait',
  'Modern architecture',
  'Minimalist design'
]

// Mock timeline data
const mockTimelineItems: TimelineItem[] = [
  {
    id: '1',
    type: 'prompt',
    prompt: 'A beautiful sunset over mountains',
    timestamp: new Date('2024-01-15T10:30:00')
  },
  {
    id: '2',
    type: 'image',
    prompt: 'A beautiful sunset over mountains',
    timestamp: new Date('2024-01-15T10:31:00'),
    thumbnail: 'https://picsum.photos/100/100?random=1',
    imageUrl: 'https://picsum.photos/800/600?random=1'
  },
  {
    id: '3',
    type: 'prompt',
    prompt: 'Add more vibrant colors',
    timestamp: new Date('2024-01-15T10:35:00')
  },
  {
    id: '4',
    type: 'image',
    prompt: 'A beautiful sunset over mountains with vibrant colors',
    timestamp: new Date('2024-01-15T10:36:00'),
    thumbnail: 'https://picsum.photos/100/100?random=2',
    imageUrl: 'https://picsum.photos/800/600?random=2'
  }
]

onMounted(() => {
  // Load conversation data based on route params
  const conversationId = (route.params as any).id as string
  if (conversationId && conversationId !== 'new') {
    // Load existing conversation
    timelineItems.value = mockTimelineItems
    selectedItem.value = timelineItems.value.find(item => item.type === 'image') || null
  }
})

const selectTimelineItem = (item: TimelineItem) => {
  selectedItem.value = item
}

const generateImage = async () => {
  if (!promptText.value.trim()) return
  
  isGenerating.value = true
  
  // Add prompt to timeline
  const promptItem: TimelineItem = {
    id: Date.now().toString(),
    type: 'prompt',
    prompt: promptText.value,
    timestamp: new Date()
  }
  timelineItems.value.push(promptItem)
  
  // Simulate image generation
  setTimeout(() => {
    const imageItem: TimelineItem = {
      id: (Date.now() + 1).toString(),
      type: 'image',
      prompt: promptText.value,
      timestamp: new Date(),
      thumbnail: `https://picsum.photos/100/100?random=${Date.now()}`,
      imageUrl: `https://picsum.photos/800/600?random=${Date.now()}`
    }
    
    timelineItems.value.push(imageItem)
    selectedItem.value = imageItem
    promptText.value = ''
    isGenerating.value = false
  }, 3000)
}

const triggerUpload = () => {
  fileInput.value?.click()
}

const handleFileUpload = (event: Event) => {
  const files = (event.target as HTMLInputElement).files
  if (!files) return
  
  Array.from(files).forEach(file => {
    const reader = new FileReader()
    reader.onload = (e) => {
      uploadedImages.value.push({
        name: file.name,
        url: e.target?.result as string,
        file
      })
    }
    reader.readAsDataURL(file)
  })
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

const formatTime = (date: Date): string => {
  return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
}
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
  background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
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

.upload-btn {
  width: 80px;
  height: 80px;
  border: 2px dashed currentColor;
  transition: all 0.2s ease;
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
</style>