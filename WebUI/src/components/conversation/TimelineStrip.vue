<template>
  <v-card rounded="xl" elevation="2" class="fill-height pa-4">
    <h3 class="text-h6 font-weight-bold mb-4">Timeline</h3>
    <v-timeline side="end" align="start" truncate-line="both" size="small" direction="horizontal">
      <v-timeline-item v-for="(item, index) in items" :key="index" :dot-color="item.type === 'image' ? 'primary' : 'secondary'" size="small" class="timeline-item" @click="$emit('select', item)">
        <template #icon>
          <v-icon size="small">{{ item.type === 'image' ? 'mdi-image' : 'mdi-message-text' }}</v-icon>
        </template>

        <v-card rounded="lg" elevation="1" class="timeline-card pa-3" :class="{ 'timeline-card-active': selected?.id === item.id }">
          <div class="text-caption text-grey-darken-1 mb-1">{{ formatTime(item.timestamp) }}</div>

          <div class="d-flex align-center">
            <v-avatar size="32" rounded="lg" class="mr-2"><v-img :src="item.image?.imagePath" cover /></v-avatar>
            <div class="flex-grow-1">
              <div class="text-body-2 font-weight-medium">{{ item.type === 'image' ? 'Image Generated' : 'Prompt' }}</div>
              <div class="text-caption text-grey-darken-1 text-truncate">{{ item.prompt }}</div>
            </div>
          </div>
        </v-card>
      </v-timeline-item>
    </v-timeline>
  </v-card>
</template>

<script lang="ts" setup>
import type { TimelineItem } from '@/types/ui'

defineProps<{ items: TimelineItem[]; selected: TimelineItem | null }>()

const formatTime = (date: Date): string => date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
</script>

<style scoped>
.timeline-item { cursor: pointer; }
.timeline-card { transition: all 0.2s ease; border: 2px solid transparent; }
.timeline-card:hover { transform: translateY(-2px); box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1); }
.timeline-card-active { border-color: rgb(var(--v-theme-primary)); background-color: rgba(var(--v-theme-primary), 0.05); }
</style>
