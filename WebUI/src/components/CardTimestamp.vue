<template>
  <span 
    class="card-timestamp text-caption text-grey-darken-1"
    :title="absoluteTime"
  >
    {{ displayTime }}
  </span>
</template>

<script lang="ts" setup>
import { computed } from 'vue'

interface Props {
  /** The timestamp to display. */
  timestamp: Date
  /** The format to display the timestamp in. */
  format?: 'relative' | 'absolute' | 'short'
  /** Whether to show an icon next to the timestamp. */
  showIcon?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  format: 'relative',
  showIcon: false
})

const displayTime = computed(() => {
  const now = new Date()
  const diff = now.getTime() - props.timestamp.getTime()
  
  switch (props.format) {
    case 'absolute':
      return props.timestamp.toLocaleDateString()
    
    case 'short':
      return props.timestamp.toLocaleDateString('en-US', { 
        month: 'short', 
        day: 'numeric' 
      })
    
    case 'relative':
    default:
      return formatRelativeTime(diff)
  }
})

const absoluteTime = computed(() => {
  return props.timestamp.toLocaleString()
})

const formatRelativeTime = (diff: number): string => {
  const days = Math.floor(diff / (1000 * 60 * 60 * 24))
  const hours = Math.floor(diff / (1000 * 60 * 60))
  const minutes = Math.floor(diff / (1000 * 60))
  
  if (days === 0) {
    if (hours === 0) {
      if (minutes < 1) {
        return 'Just now'
      } else if (minutes === 1) {
        return '1 minute ago'
      } else {
        return `${minutes} minutes ago`
      }
    } else if (hours === 1) {
      return '1 hour ago'
    } else {
      return `${hours} hours ago`
    }
  } else if (days === 1) {
    return 'Yesterday'
  } else if (days < 7) {
    return `${days} days ago`
  } else if (days < 30) {
    const weeks = Math.floor(days / 7)
    return weeks === 1 ? '1 week ago' : `${weeks} weeks ago`
  } else {
    return props.timestamp.toLocaleDateString()
  }
}
</script>

<style scoped>
.card-timestamp {
  transition: color 0.2s ease;
  cursor: help;
}

.card-timestamp:hover {
  color: rgb(var(--v-theme-on-surface)) !important;
}
</style>