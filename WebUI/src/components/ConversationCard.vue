<template>
  <v-card
    class="conversation-card"
    :class="{ 'card-hover': isHovered }"
    rounded="xl"
    elevation="2"
    hover
    @mouseenter="handleMouseEnter"
    @mouseleave="handleMouseLeave"
    @click="handleClick"
  >
    <!-- Image Thumbnail Section -->
    <CardImageThumbnail
      :thumbnail="conversation.thumbnail"
      :image-count="conversation.imageCount"
      :is-hovered="isHovered"
      :height="thumbnailHeight"
      :show-badge="showImageCount"
    />

    <!-- Card Content Section -->
    <!-- <CardContent
      :title="conversation.title"
      :last-message="conversation.lastMessage"
      :timestamp="conversation.timestamp"
      :is-hovered="isHovered"
      :metadata="cardMetadata"
      :show-metadata="showMetadata"
    /> -->

    <v-card-text class="pa-4">
    <div class="d-flex flex-column">
      <!-- Title -->
      <h3 
        class="text-h6 font-weight-bold mb-2 text-truncate card-title"
        :class="{ 'title-hover': isHovered }"
      >
        {{ conversation.title }}
      </h3>
      
      <!-- Last Message -->
      <p 
        class="text-body-2 text-grey-darken-1 mb-3 text-truncate card-message"
        :title="conversation.title"
      >
        {{ conversation.title }}
      </p>
      
      <!-- Footer with timestamp and action indicator -->
      <div class="d-flex align-center justify-space-between">
        <CardTimestamp 
          :timestamp="conversation.timestamp"
          format="relative"
        />
        
        <v-icon
          size="small"
          class="action-icon"
          :class="{ 'icon-hover': isHovered }"
        >
          mdi-chevron-right
        </v-icon>
      </div>

      <!-- Optional metadata chips -->
      <div v-if="showMetadata && cardMetadata.length > 0" class="mt-3">
        <v-chip
          v-for="(meta, index) in cardMetadata"
          :key="index"
          size="x-small"
          variant="outlined"
          class="mr-1 mb-1"
        >
          {{ meta }}
        </v-chip>
      </div>
    </div>
  </v-card-text>

    <!-- Hover Overlay -->
    <CardHoverOverlay
      :show="showHoverOverlay"
      :title="overlayTitle"
      :subtitle="overlaySubtitle"
      :icon="overlayIcon"
      :show-actions="showOverlayActions"
      :actions="overlayActions"
      @action="handleOverlayAction"
    />
  </v-card>
</template>

<script lang="ts" setup>
import { ref, computed } from 'vue'
import CardImageThumbnail from './CardImageThumbnail.vue'
import CardHoverOverlay from './CardHoverOverlay.vue'

interface Conversation {
  id: string
  title: string
  thumbnail: string
  lastMessage: string
  timestamp: Date
  imageCount: number
}

interface OverlayAction {
  key: string
  text?: string
  icon?: string
  color?: string
  variant?: 'text' | 'outlined' | 'flat' | 'elevated' | 'tonal' | 'plain'
  size?: 'x-small' | 'small' | 'default' | 'large' | 'x-large'
}

interface Props {
  conversation: Conversation
  thumbnailHeight?: number | string
  showImageCount?: boolean
  showMetadata?: boolean
  showHoverOverlay?: boolean
  showOverlayActions?: boolean
  overlayTitle?: string
  overlaySubtitle?: string
  overlayIcon?: string
  overlayActions?: OverlayAction[]
}

const props = withDefaults(defineProps<Props>(), {
  thumbnailHeight: 200,
  showImageCount: true,
  showMetadata: false,
  showHoverOverlay: true,
  showOverlayActions: false,
  overlayTitle: 'View Conversation',
  overlayIcon: 'mdi-eye',
  overlayActions: () => [],
})

const emit = defineEmits<{
  click: []
  overlayAction: [key: string]
  mouseenter: []
  mouseleave: []
}>()

const isHovered = ref(false)

const cardMetadata = computed(() => {
  if (!props.showMetadata) return []
  
  return [
    `${props.conversation.imageCount} images`,
    formatDate(props.conversation.timestamp)
  ]
})

const overlaySubtitle = computed(() => {
  return props.overlaySubtitle || `${props.conversation.imageCount} images generated`
})

const showHoverOverlay = computed(() => {
  return props.showHoverOverlay && isHovered.value
})

const handleMouseEnter = () => {
  isHovered.value = true
  emit('mouseenter')
}

const handleMouseLeave = () => {
  isHovered.value = false
  emit('mouseleave')
}

const handleClick = () => {
  emit('click')
}

const handleOverlayAction = (key: string) => {
  emit('overlayAction', key)
}

const formatDate = (date: Date): string => {
  const now = new Date()
  const diff = now.getTime() - date.getTime()
  const days = Math.floor(diff / (1000 * 60 * 60 * 24))
  
  if (days === 0) {
    return 'Today'
  } else if (days === 1) {
    return 'Yesterday'
  } else if (days < 7) {
    return `${days} days ago`
  } else {
    return date.toLocaleDateString()
  }
}
</script>

<style scoped>
.conversation-card {
  cursor: pointer;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  position: relative;
  overflow: hidden;
}

.conversation-card:hover {
  transform: translateY(-4px) scale(1.02);
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.15) !important;
}

/* Ensure consistent border radius */
.conversation-card {
  border-radius: 24px !important;
}

/* Smooth transitions for all child components */
.conversation-card :deep(.v-card-text) {
  transition: all 0.2s ease;
}

/* Focus states for accessibility */
.conversation-card:focus-visible {
  outline: 2px solid rgb(var(--v-theme-primary));
  outline-offset: 2px;
}

/* Loading states */
.conversation-card.loading {
  opacity: 0.7;
  pointer-events: none;
}

/* Responsive adjustments */
@media (max-width: 600px) {
  .conversation-card:hover {
    transform: translateY(-2px) scale(1.01);
  }
}
</style>
