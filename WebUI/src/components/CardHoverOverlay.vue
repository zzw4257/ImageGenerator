<template>
  <v-overlay
    :model-value="show"
    contained
    class="align-center justify-center hover-overlay"
    :opacity="opacity"
    :z-index="zIndex"
  >
    <div class="text-center pa-4 overlay-content">
      <!-- Primary Icon -->
      <v-icon
        :size="iconSize"
        :color="iconColor"
        class="mb-2 overlay-icon"
      >
        {{ icon }}
      </v-icon>
      
      <!-- Primary Text -->
      <div 
        class="overlay-title font-weight-bold mb-1"
        :class="titleClass"
      >
        {{ title }}
      </div>
      
      <!-- Secondary Text -->
      <div 
        v-if="subtitle"
        class="overlay-subtitle"
        :class="subtitleClass"
      >
        {{ subtitle }}
      </div>

      <!-- Custom content slot -->
      <div v-if="$slots.default" class="overlay-custom mt-2">
        <slot></slot>
      </div>

      <!-- Action buttons -->
      <div v-if="showActions" class="overlay-actions mt-3">
        <v-btn
          v-for="action in actions"
          :key="action.key"
          :color="action.color || 'white'"
          :variant="action.variant || 'outlined'"
          :size="action.size || 'small'"
          class="mx-1"
          @click.stop="$emit('action', action.key)"
        >
          <v-icon v-if="action.icon" :start="!!action.text">
            {{ action.icon }}
          </v-icon>
          {{ action.text }}
        </v-btn>
      </div>
    </div>
  </v-overlay>
</template>

<script lang="ts" setup>
/**
 * Represents an action button in the overlay.
 */
interface Action {
  /** The unique key for the action. */
  key: string
  /** The text to display on the button. */
  text?: string
  /** The icon to display on the button. */
  icon?: string
  /** The color of the button. */
  color?: string
  /** The variant of the button. */
  variant?: 'text' | 'outlined' | 'flat' | 'elevated' | 'tonal' | 'plain'
  /** The size of the button. */
  size?: 'x-small' | 'small' | 'default' | 'large' | 'x-large'
}

interface Props {
  /** Whether to show the overlay. */
  show: boolean
  /** The main title text. */
  title: string
  /** The subtitle text. */
  subtitle?: string
  /** The icon to display. */
  icon?: string
  /** The size of the icon. */
  iconSize?: string | number
  /** The color of the icon. */
  iconColor?: string
  /** Custom CSS class for the title. */
  titleClass?: string
  /** Custom CSS class for the subtitle. */
  subtitleClass?: string
  /** The opacity of the overlay. */
  opacity?: number
  /** The z-index of the overlay. */
  zIndex?: number
  /** Whether to show the action buttons. */
  showActions?: boolean
  /** The list of action buttons to display. */
  actions?: Action[]
}

withDefaults(defineProps<Props>(), {
  icon: 'mdi-eye',
  iconSize: 48,
  iconColor: 'white',
  titleClass: 'text-h6 text-white',
  subtitleClass: 'text-body-2 text-white',
  opacity: 0.3,
  zIndex: 2,
  showActions: false,
  actions: () => []
})

defineEmits<{
  action: [key: string]
}>()
</script>

<style scoped>
.hover-overlay {
  border-radius: inherit;
  backdrop-filter: blur(2px);
}

.overlay-content {
  position: relative;
  max-width: 200px;
}

.overlay-icon {
  filter: drop-shadow(0 2px 4px rgba(0, 0, 0, 0.3));
  animation: pulse 2s infinite;
}

.overlay-title {
  text-shadow: 0 1px 3px rgba(0, 0, 0, 0.5);
}

.overlay-subtitle {
  text-shadow: 0 1px 2px rgba(0, 0, 0, 0.5);
  opacity: 0.9;
}

.overlay-actions {
  animation: slideUp 0.3s ease-out;
}

@keyframes pulse {
  0%, 100% {
    transform: scale(1);
  }
  50% {
    transform: scale(1.1);
  }
}

@keyframes slideUp {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* Responsive adjustments */
@media (max-width: 600px) {
  .overlay-content {
    max-width: 150px;
    padding: 12px;
  }
  
  .overlay-icon {
    font-size: 36px !important;
  }
  
  .overlay-title {
    font-size: 1rem !important;
  }
  
  .overlay-subtitle {
    font-size: 0.875rem !important;
  }
}
</style>