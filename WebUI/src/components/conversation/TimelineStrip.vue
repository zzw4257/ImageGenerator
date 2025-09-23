<template>
  <v-card rounded="xl" elevation="2" class="fill-height">
    <h3 class="text-h6 font-weight-bold ml-4 mt-4">Timeline</h3>
    <div class="timeline-wrapper pa-4">
      <v-timeline side="end" align="start" truncate-line="both" direction="horizontal">
      <v-timeline-item v-for="(item, index) in items.reverse()" :key="index" :dot-color="item.type === 'image' ? 'primary' : 'secondary'" class="timeline-item" @click="$emit('select', item)">
        <template #icon>
          <v-icon size="15px">{{ item.type === 'image' ? 'mdi-image' : 'mdi-message-text' }}</v-icon>
        </template>

        <v-card rounded="lg" elevation="1" class="timeline-card pa-3" :class="{ 'timeline-card-active': selected?.id === item.id }">
          <div class="text-caption text-grey-darken-1 mb-1">{{ formatTime(item.timestamp) }}</div>

          <div class="d-flex align-center">
            <v-avatar size="32" rounded="lg" class="mr-2"><v-img :src="`/${item.image?.imagePath}`" cover /></v-avatar>
            <div class="flex-grow-1 mr-4">
              <div class="text-body-2 font-weight-medium">{{ item.type === 'image' ? 'Image Generated' : 'Prompt' }}</div>
              <div class="text-caption text-grey-darken-1 text-truncate">{{ item.prompt }}</div>
            </div>
            <v-btn v-if="item.image?.imagePath" 
              icon="mdi-image-plus" 
              size="small" 
              variant="text" 
              density="compact"
              @click.stop="$emit('add-reference', item)"
              class="timeline-reference-btn">
            </v-btn>
          </div>
        </v-card>
      </v-timeline-item>
    </v-timeline>
    </div>
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
.timeline-card:hover .timeline-reference-btn { opacity: 1; }
.timeline-card-active { border-color: rgb(var(--v-theme-primary)); background-color: rgba(var(--v-theme-primary), 0.05); }
.timeline-reference-btn { opacity: 0; transition: opacity 0.2s ease; }

.timeline-wrapper{
  display: flex;
  flex-direction: column;
  flex-wrap: nowrap;
  overflow-x: auto;
}
</style>
