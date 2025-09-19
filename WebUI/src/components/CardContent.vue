<template>
  <v-card-text class="pa-4">
    <div class="d-flex flex-column">
      <!-- Title -->
      <h3 
        class="text-h6 font-weight-bold mb-2 text-truncate card-title"
        :class="{ 'title-hover': isHovered }"
      >
        {{ title }}
      </h3>
      
      <!-- Last Message -->
      <p 
        class="text-body-2 text-grey-darken-1 mb-3 text-truncate card-message"
        :title="lastMessage"
      >
        {{ lastMessage }}
      </p>
      
      <!-- Footer with timestamp and action indicator -->
      <div class="d-flex align-center justify-space-between">
        <CardTimestamp 
          :timestamp="timestamp"
          :format="timestampFormat"
        />
        
        <v-icon
          :color="actionIconColor"
          :size="actionIconSize"
          class="action-icon"
          :class="{ 'icon-hover': isHovered }"
        >
          {{ actionIcon }}
        </v-icon>
      </div>

      <!-- Optional metadata chips -->
      <div v-if="showMetadata && metadata.length > 0" class="mt-3">
        <v-chip
          v-for="(meta, index) in metadata"
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
</template>

<script lang="ts" setup>
import CardTimestamp from './CardTimestamp.vue'

interface MetadataItem {
  label: string
  value: string | number
}

interface Props {
  title: string
  lastMessage: string
  timestamp: Date
  isHovered?: boolean
  actionIcon?: string
  actionIconColor?: string
  actionIconSize?: string | number
  timestampFormat?: 'relative' | 'absolute' | 'short'
  showMetadata?: boolean
  metadata?: string[]
}

withDefaults(defineProps<Props>(), {
  isHovered: false,
  actionIcon: 'mdi-chevron-right',
  actionIconColor: 'grey-lighten-1',
  actionIconSize: 'small',
  timestampFormat: 'relative',
  showMetadata: false,
  metadata: () => []
})
</script>

<style scoped>
.card-title {
  transition: color 0.2s ease;
}

.title-hover {
  color: rgb(var(--v-theme-primary)) !important;
}

.card-message {
  transition: opacity 0.2s ease;
  line-height: 1.4;
}

.action-icon {
  transition: all 0.2s ease;
}

.icon-hover {
  transform: translateX(2px);
  color: rgb(var(--v-theme-primary)) !important;
}

/* Ensure text doesn't overflow */
.text-truncate {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

/* Better spacing for metadata */
.v-chip {
  font-size: 0.75rem;
}
</style>