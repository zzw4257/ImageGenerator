<template>
  <div class="content-area">
    <v-card rounded="xl" elevation="2" class="image-display-card">
      <div class="image-container">
        <SmoothPicture v-if="currentImage?.imagePath" :url="`/${currentImage.imagePath}`" height="500" cover
          class="rounded-xl">
          <template #placeholder>
            <div class="d-flex align-center justify-center fill-height">
              <v-progress-circular color="primary" indeterminate />
            </div>
          </template>
          <template #default>
            <v-btn @click.stop="() => currentImage && toggleFavorite(currentImage)" variant="outlined" icon class="favorite-btn">
              <v-icon v-if="currentImage?.isFavorite" color="error">mdi-heart</v-icon>
              <v-icon v-else>mdi-heart-outline</v-icon>
            </v-btn>
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

      <v-card-actions v-if="currentImage?.imagePath" class="pa-4">
        <v-chip v-if="item" color="primary" variant="outlined" size="small" prepend-icon="mdi-clock">
          {{ formatTime(item.timestamp) }}
        </v-chip>
        <v-spacer />

        <!-- Pagination controls -->
        <div class="d-flex align-center mr-2">
          <v-btn icon variant="text" density="comfortable" :disabled="totalImages <= 1 || currentIndex === 0" @click="prevImage">
            <v-icon>mdi-chevron-left</v-icon>
          </v-btn>
          <div class="mx-2 text-body-2">{{ currentIndex + 1 }} / {{ totalImages }}</div>
          <v-btn icon variant="text" density="comfortable" :disabled="totalImages <= 1 || currentIndex >= totalImages - 1" @click="nextImage">
            <v-icon>mdi-chevron-right</v-icon>
          </v-btn>
        </div>

        <v-btn variant="tonal" color="primary" size="small" prepend-icon="mdi-image-plus"
          @click="$emit('add-reference')">
          Add as Reference
        </v-btn>
        <v-btn icon variant="text" @click="downloadImage"><v-icon>mdi-download</v-icon></v-btn>
        <v-btn icon variant="text" @click="shareImage"><v-icon>mdi-share</v-icon></v-btn>
      </v-card-actions>
    </v-card>
  </div>
</template>

<script lang="ts" setup>
import { computed, ref, watch } from 'vue'
import { addFavorite, removeFavorite } from '@/services/favorite'
import type { ImageDto } from '@/types/api'
import type { TimelineItem } from '@/types/ui'
import { useNotificationStore } from '@/stores/notification';

const props = defineProps<{ item: TimelineItem | null }>()

const notificationStore = useNotificationStore();
const formatTime = (date: Date): string => {
  return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
}

const images = computed<ImageDto[]>(() => props.item?.image ?? [])
const totalImages = computed(() => images.value.length)
const currentIndex = ref(0)
watch(() => props.item?.id, () => { currentIndex.value = 0 })
watch(images, (arr) => {
  if (currentIndex.value > (arr.length - 1)) currentIndex.value = 0
})
const currentImage = computed<ImageDto | undefined>(() => images.value[currentIndex.value])

const prevImage = () => {
  if (currentIndex.value > 0) currentIndex.value -= 1
}
const nextImage = () => {
  if (currentIndex.value < totalImages.value - 1) currentIndex.value += 1
}

const toggleFavorite = async (img: ImageDto) => {
  const original = img.isFavorite
  try {
    // optimistic toggle
    img.isFavorite = !img.isFavorite
    if (img.isFavorite) {
      await addFavorite(img.id)
    } else {
      await removeFavorite(img.id)
    }
  } catch (e) {
    // rollback
    img.isFavorite = original
    notificationStore.error('Failed to update favorite')
  }
}

const downloadImage = () => {
  if (!currentImage?.value) return
  const link = document.createElement('a')
  link.href = `/${currentImage.value.imagePath}`
  link.download = currentImage.value.imagePath.split('/').pop() || 'image'
  document.body.appendChild(link)
  link.click()
  document.body.removeChild(link)
}

const shareImage = async () => {
  if (!currentImage?.value) return
  try {
    await navigator.clipboard.writeText(`${window.location.origin}/${currentImage.value.imagePath}`)
    notificationStore.success('Image URL copied to clipboard', { icon: 'mdi-content-copy' })
  } catch (error) {
    notificationStore.error('Failed to copy image URL')
  }
}
</script>

<style scoped>
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

.favorite-btn {
  position: absolute;
  top: 10px;
  right: 10px;
  z-index: 10;
  background-color: rgba(var(--v-theme-surface), 0.7);
}
</style>
