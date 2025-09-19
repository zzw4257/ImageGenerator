<template>
  <div class="conversation-layout">
    <!-- Header -->
    <v-app-bar
      elevation="0"
      color="transparent"
      class="conversation-header px-6 py-2"
      height="72"
    >
      <!-- Back Button -->
      <template v-slot:prepend>
        <v-btn
          icon
          variant="text"
          @click="goBack"
          class="mr-2"
        >
          <v-icon>mdi-arrow-left</v-icon>
        </v-btn>
      </template>
      
      <!-- Title Section -->
      <div class="d-flex align-center flex-grow-1">
        <div class="conversation-title-section">
          <v-app-bar-title class="text-h5 font-weight-bold">
            {{ conversationTitle }}
          </v-app-bar-title>
          <div v-if="conversationSubtitle" class="text-caption text-grey-darken-1">
            {{ conversationSubtitle }}
          </div>
        </div>
      </div>
      
      <!-- Actions -->
      <template v-slot:append>
        <div class="d-flex align-center gap-2">
          <!-- Status Indicator -->
          <v-chip
            v-if="conversationStatus"
            :color="statusColor"
            size="small"
            variant="outlined"
            class="mr-2"
          >
            <v-icon start :icon="statusIcon" size="small" />
            {{ conversationStatus }}
          </v-chip>

          <!-- Action Buttons -->
          <v-btn
            v-for="action in headerActions"
            :key="action.key"
            :color="action.color || 'primary'"
            :variant="action.variant || 'outlined'"
            :icon="action.icon"
            :size="action.size || 'default'"
            rounded="xl"
            :class="action.class"
            @click="handleHeaderAction(action.key)"
          >
            <v-icon v-if="action.icon && !action.text">{{ action.icon }}</v-icon>
            <template v-if="action.text">
              <v-icon v-if="action.icon" start>{{ action.icon }}</v-icon>
              {{ action.text }}
            </template>
          </v-btn>
        </div>
      </template>
    </v-app-bar>

    <!-- Main Content Area -->
    <v-main class="conversation-main">
      <v-container fluid class="conversation-container fill-height pa-0">
        <router-view v-slot="{ Component }">
          <transition name="conversation-transition" mode="out-in">
            <component :is="Component" />
          </transition>
        </router-view>
      </v-container>
    </v-main>

    <!-- Floating Controls -->
    <div class="floating-controls">
      <!-- Quick Actions -->
      <v-speed-dial
        v-if="showSpeedDial"
        v-model="speedDialOpen"
        location="bottom end"
      >
        <template v-slot:activator="{ props: activatorProps }">
          <v-fab
            v-bind="activatorProps"
            :icon="speedDialOpen ? 'mdi-close' : 'mdi-dots-vertical'"
            color="secondary"
            size="default"
          />
        </template>

        <v-btn
          v-for="quickAction in quickActions"
          :key="quickAction.key"
          :icon="quickAction.icon"
          :color="quickAction.color"
          size="small"
          @click="handleQuickAction(quickAction.key)"
        />
      </v-speed-dial>

      <!-- Generation Status -->
      <v-card
        v-if="showGenerationStatus"
        class="generation-status"
        elevation="4"
        rounded="xl"
      >
        <v-card-text class="pa-3">
          <div class="d-flex align-center">
            <v-progress-circular
              :indeterminate="generationProgress < 0"
              :model-value="generationProgress"
              size="24"
              width="3"
              color="primary"
              class="mr-3"
            />
            <div class="flex-grow-1">
              <div class="text-body-2 font-weight-medium">
                {{ generationStatusText }}
              </div>
              <div v-if="generationSubtext" class="text-caption text-grey-darken-1">
                {{ generationSubtext }}
              </div>
            </div>
            <v-btn
              icon
              size="x-small"
              variant="text"
              @click="cancelGeneration"
            >
              <v-icon>mdi-close</v-icon>
            </v-btn>
          </div>
        </v-card-text>
      </v-card>
    </div>

    <!-- Snackbar for notifications -->
    <v-snackbar
      v-model="showSnackbar"
      :timeout="snackbarTimeout"
      :color="snackbarColor"
      rounded="xl"
      class="mb-4"
    >
      {{ snackbarMessage }}
      
      <template v-slot:actions>
        <v-btn
          color="white"
          variant="text"
          @click="showSnackbar = false"
        >
          Close
        </v-btn>
      </template>
    </v-snackbar>
  </div>
</template>

<script lang="ts" setup>
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'

interface HeaderAction {
  key: string
  text?: string
  icon?: string
  color?: string
  variant?: 'text' | 'outlined' | 'flat' | 'elevated' | 'tonal' | 'plain'
  size?: 'x-small' | 'small' | 'default' | 'large' | 'x-large'
  class?: string
}

interface QuickAction {
  key: string
  icon: string
  color?: string
}

interface Props {
  conversationTitle?: string
  conversationSubtitle?: string
  conversationStatus?: string
  headerActions?: HeaderAction[]
  quickActions?: QuickAction[]
  showSpeedDial?: boolean
  showGenerationStatus?: boolean
  generationProgress?: number
  generationStatusText?: string
  generationSubtext?: string
}

const props = withDefaults(defineProps<Props>(), {
  conversationTitle: 'Conversation',
  headerActions: () => [
    {
      key: 'share',
      text: 'Share',
      icon: 'mdi-share',
      variant: 'outlined',
      class: 'mr-2'
    },
    {
      key: 'download',
      text: 'Download',
      icon: 'mdi-download'
    }
  ],
  quickActions: () => [
    { key: 'save', icon: 'mdi-content-save', color: 'primary' },
    { key: 'copy', icon: 'mdi-content-copy', color: 'secondary' },
    { key: 'delete', icon: 'mdi-delete', color: 'error' }
  ],
  showSpeedDial: true,
  showGenerationStatus: false,
  generationProgress: -1,
  generationStatusText: 'Generating image...'
})

const emit = defineEmits<{
  headerAction: [key: string]
  quickAction: [key: string]
  cancelGeneration: []
}>()

const router = useRouter()

const speedDialOpen = ref(false)
const showSnackbar = ref(false)
const snackbarMessage = ref('')
const snackbarColor = ref('success')
const snackbarTimeout = ref(4000)

const statusColor = computed(() => {
  switch (props.conversationStatus?.toLowerCase()) {
    case 'active':
    case 'generating':
      return 'primary'
    case 'completed':
    case 'finished':
      return 'success'
    case 'error':
    case 'failed':
      return 'error'
    case 'paused':
    case 'waiting':
      return 'warning'
    default:
      return 'grey'
  }
})

const statusIcon = computed(() => {
  switch (props.conversationStatus?.toLowerCase()) {
    case 'active':
    case 'generating':
      return 'mdi-cog'
    case 'completed':
    case 'finished':
      return 'mdi-check'
    case 'error':
    case 'failed':
      return 'mdi-alert'
    case 'paused':
    case 'waiting':
      return 'mdi-pause'
    default:
      return 'mdi-circle'
  }
})

const goBack = () => {
  router.push('/')
}

const handleHeaderAction = (key: string) => {
  emit('headerAction', key)
}

const handleQuickAction = (key: string) => {
  speedDialOpen.value = false
  emit('quickAction', key)
}

const cancelGeneration = () => {
  emit('cancelGeneration')
}

const showNotification = (message: string, color = 'success', timeout = 4000) => {
  snackbarMessage.value = message
  snackbarColor.value = color
  snackbarTimeout.value = timeout
  showSnackbar.value = true
}

// Expose methods for parent components
defineExpose({
  showNotification
})
</script>

<style scoped>
.conversation-layout {
  height: 100vh;
  display: flex;
  flex-direction: column;
}

.conversation-header {
  border-bottom: 1px solid rgba(var(--v-theme-on-surface), 0.05);
  backdrop-filter: blur(10px);
  background: rgba(var(--v-theme-surface), 0.8) !important;
}

.conversation-title-section {
  flex-grow: 1;
}

.conversation-main {
  flex: 1;
  overflow: hidden;
}

.conversation-container {
  height: 100%;
}

.floating-controls {
  position: fixed;
  bottom: 24px;
  right: 24px;
  z-index: 10;
}

.generation-status {
  position: fixed;
  bottom: 24px;
  left: 50%;
  transform: translateX(-50%);
  min-width: 300px;
  max-width: 400px;
  z-index: 11;
}

/* Transition animations */
.conversation-transition-enter-active,
.conversation-transition-leave-active {
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}

.conversation-transition-enter-from {
  opacity: 0;
  transform: translateY(20px);
}

.conversation-transition-leave-to {
  opacity: 0;
  transform: translateY(-20px);
}

/* Responsive adjustments */
@media (max-width: 960px) {
  .conversation-header {
    padding-left: 16px;
    padding-right: 16px;
  }
  
  .floating-controls {
    bottom: 16px;
    right: 16px;
  }
  
  .generation-status {
    bottom: 16px;
    left: 16px;
    right: 16px;
    transform: none;
    max-width: none;
  }
}

@media (max-width: 600px) {
  .conversation-header :deep(.v-btn) {
    padding: 0 8px;
  }
  
  .conversation-header :deep(.v-btn .v-btn__content) {
    font-size: 0.875rem;
  }
}

/* Custom scrollbar for main content */
.conversation-main :deep(.v-main__wrap) {
  scrollbar-width: thin;
  scrollbar-color: rgba(var(--v-theme-on-surface), 0.2) transparent;
}

.conversation-main :deep(.v-main__wrap)::-webkit-scrollbar {
  width: 6px;
}

.conversation-main :deep(.v-main__wrap)::-webkit-scrollbar-thumb {
  background: rgba(var(--v-theme-on-surface), 0.2);
  border-radius: 3px;
}
</style>