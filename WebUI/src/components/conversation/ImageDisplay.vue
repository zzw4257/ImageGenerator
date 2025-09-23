<template>
  <div class="content-area">
    <v-card rounded="xl" elevation="2" class="image-display-card">
      <div class="image-container">
        <SmoothPicture v-if="item && item.image?.imagePath" :url="`/${item.image.imagePath}`" height="500" cover class="rounded-xl">
          <template #placeholder>
            <div class="d-flex align-center justify-center fill-height">
              <v-progress-circular color="primary" indeterminate />
            </div>
          </template>
        </SmoothPicture>

        <div v-else class="d-flex align-center justify-center fill-height placeholder-area">
          <div class="text-center">
            <v-icon size="120" class="mb-4">mdi-image-outline</v-icon>
            <h3 class="text-h5 mb-2">Select an image from timeline</h3>
            <p class="text-body-1">Or generate a new one below</p>
          </div>
        </div>
      </div>

    <v-card-actions v-if="item?.image?.imagePath" class="pa-4">
        <v-chip color="primary" variant="outlined" size="small" prepend-icon="mdi-clock">
          {{ formatTime(item.timestamp) }}
        </v-chip>
        <v-spacer />
        <v-btn variant="tonal" color="primary" size="small" prepend-icon="mdi-image-plus" @click="$emit('add-reference')">
          Add as Reference
        </v-btn>
        <v-btn icon variant="text" @click="$emit('download')"><v-icon>mdi-download</v-icon></v-btn>
        <v-btn icon variant="text" @click="$emit('share')"><v-icon>mdi-share</v-icon></v-btn>
        <v-btn icon variant="text" @click="$emit('delete')"><v-icon>mdi-delete</v-icon></v-btn>
      </v-card-actions>
    </v-card>
  </div>
</template>

<script lang="ts" setup>
import type { TimelineItem } from '@/types/ui'

defineProps<{ item: TimelineItem | null }>()

const formatTime = (date: Date): string => {
  return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
}
</script>

<style scoped>
.content-area { display: flex; flex-direction: column; height: 100%; }
.image-display-card { flex: 1; min-height: 400px; }
.image-container { height: 500px; position: relative; }
.placeholder-area { height: 500px; }
</style>
