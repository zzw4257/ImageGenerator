<template>
  <v-container class="py-6">
    <v-card elevation="2" rounded="xl">
      <v-card-title class="pa-6 d-flex align-center">
        <v-icon class="mr-3">mdi-heart</v-icon>
        Favorites
      </v-card-title>
      <v-divider />
      <v-card-text>
        <div v-if="loading" class="text-center py-8">
          <v-progress-circular color="primary" indeterminate />
        </div>
        <div v-else>
          <v-row>
            <v-col
              v-for="img in favoriteImages"
              :key="img.id"
              cols="12"
              lg="3"
              md="4"
              sm="6"
            >
              <v-card elevation="1" rounded="xl">
                <SmoothPicture
                  class="rounded-t-xl"
                  cover
                  height="220"
                  :url="`/${img.imagePath}`"
                />
                <v-card-actions>
                  <v-btn
                    icon="mdi-share"
                    variant="text"
                    @click="shareImage(img)"
                  />
                  <v-spacer />
                  <v-btn
                    :color="img.isFavorite ? 'error' : ''"
                    :icon="img.isFavorite ? 'mdi-heart' : 'mdi-heart-outline'"
                    variant="text"
                    @click="toggleFavorite(img)"
                  />
                </v-card-actions>
              </v-card>
            </v-col>
          </v-row>
          <div class="d-flex justify-center mt-4">
            <v-pagination
              v-model="pageNumberUI"
              :length="pagination.TotalPages"
              rounded="circle"
              total-visible="7"
              @update:model-value="onPageChange"
            />
          </div>
          <div v-if="favoriteImages.length === 0" class="text-center py-12">
            <v-icon
              class="mb-4"
              color="grey-lighten-2"
              size="80"
            >mdi-heart-outline</v-icon>
            <h3 class="text-h6 mb-2">No favorites yet</h3>
            <p class="text-body-2 text-grey-darken-1 mb-4">
              Mark images as favorite to see them here.
            </p>
          </div>
        </div>
      </v-card-text>
    </v-card>
  </v-container>
</template>

<script lang="ts" setup>
/**
 * A page that displays the user's favorite images.
 */
  import type { ImageDto } from '@/types/api'
  import { onMounted, ref } from 'vue'

  import {
    addFavorite,
    listFavorites,
    removeFavorite,
  } from '@/services/favorite'
  import { useNotificationStore } from '@/stores/notification'

  const notificationStore = useNotificationStore()

  const loading = ref(false)
  const favoriteImages = ref<ImageDto[]>([])
  const pagination = ref({
    TotalCount: 0,
    PageSize: 24,
    PageNumber: 0,
    TotalPages: 0,
  })
  const pageNumberUI = ref(1)
  const pageSize = ref(24)

  async function loadFavorites () {
    loading.value = true
    try {
      const { items, pagination: meta } = await listFavorites(
        pageNumberUI.value - 1,
        pageSize.value,
      )
      favoriteImages.value = items
      pagination.value = meta
    } catch {
      notificationStore.error('Failed to load favorites')
    } finally {
      loading.value = false
    }
  }

  function onPageChange () {
    loadFavorites()
  }

  async function toggleFavorite (img: ImageDto) {
    const original = img.isFavorite
    try {
      // optimistic toggle
      img.isFavorite = !img.isFavorite
      if (img.isFavorite) {
        await addFavorite(img.id)
        notificationStore.success('Added to favorites')
      } else {
        await removeFavorite(img.id)
        notificationStore.info('Removed from favorites')
      }
    } catch {
      // rollback
      img.isFavorite = original
      notificationStore.error('Failed to update favorite')
    }
  }

  async function shareImage (img: ImageDto) {
    try {
      await navigator.clipboard.writeText(
        `${window.location.origin}/${img.imagePath}`,
      )
      notificationStore.success('Image URL copied to clipboard', {
        icon: 'mdi-content-copy',
      })
    } catch {
      notificationStore.error('Failed to copy image URL')
    }
  }

  onMounted(loadFavorites)
</script>

<route lang="yaml">
meta:
  title: Favorites
</route>
