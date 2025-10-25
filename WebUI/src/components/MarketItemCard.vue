<template>
  <v-card class="market-item-card" elevation="2" rounded="xl" @click="$emit('purchase', item)">
    <!-- Cover Image -->
    <div class="position-relative">
      <v-img
        :src="item.coverUrl || '/images/default-preset.png'"
        aspect-ratio="16/9"
        cover
        class="rounded-t-xl"
      />
      
      <!-- Type Badge -->
      <v-chip
        class="position-absolute type-badge"
        :color="getTypeColor(item.type)"
        size="small"
        variant="flat"
      >
        {{ getTypeText(item.type) }}
      </v-chip>

      <!-- Verified Badge -->
      <v-icon
        v-if="item.isVerified"
        class="position-absolute verified-badge"
        color="success"
        size="20"
      >
        mdi-check-decagram
      </v-icon>

      <!-- Favorite Button -->
      <v-btn
        class="position-absolute favorite-btn"
        :color="item.isFavorite ? 'error' : 'white'"
        icon
        size="small"
        variant="flat"
        @click.stop="$emit('favorite', item)"
      >
        <v-icon>{{ item.isFavorite ? 'mdi-heart' : 'mdi-heart-outline' }}</v-icon>
      </v-btn>
    </div>

    <!-- Content -->
    <v-card-text class="pa-4">
      <!-- Title and Description -->
      <div class="mb-3">
        <h3 class="text-h6 font-weight-medium mb-1 text-truncate">{{ item.title }}</h3>
        <p class="text-body-2 text-grey-darken-1 line-clamp-2">{{ item.description }}</p>
      </div>

      <!-- Author Info -->
      <div class="d-flex align-center mb-3">
        <v-avatar size="24" class="mr-2">
          <v-img v-if="item.author.avatar" :src="item.author.avatar" />
          <v-icon v-else>mdi-account</v-icon>
        </v-avatar>
        <span class="text-body-2 text-grey-darken-1">{{ item.author.name }}</span>
        <v-spacer />
        <div class="d-flex align-center">
          <v-icon size="14" color="amber">mdi-star</v-icon>
          <span class="text-caption ml-1">{{ item.author.rating }}</span>
        </div>
      </div>

      <!-- Stats -->
      <div class="d-flex align-center justify-space-between mb-3">
        <div class="d-flex align-center">
          <v-icon size="16" color="grey">mdi-download</v-icon>
          <span class="text-caption ml-1 mr-3">{{ item.stats.downloads }}</span>
          <v-icon size="16" color="grey">mdi-star</v-icon>
          <span class="text-caption ml-1 mr-1">{{ item.stats.rating }}</span>
          <span class="text-caption text-grey">({{ item.stats.reviews }})</span>
        </div>
      </div>

      <!-- Tags -->
      <div class="mb-3">
        <v-chip
          v-for="tag in item.tags.slice(0, 3)"
          :key="tag"
          size="x-small"
          variant="outlined"
          class="mr-1 mb-1"
        >
          {{ tag }}
        </v-chip>
      </div>

      <!-- Price -->
      <div class="d-flex align-center justify-space-between">
        <div class="d-flex align-center">
            <!-- Post Actions -->
            <v-card-actions class="pa-4 pt-0">
              <div class="d-flex align-center gap-2 flex-grow-1">
                <v-btn
                  :color="item.isLiked ? 'error' : 'default'"
                  variant="text"
                  size="small"
                  @click="$emit('like', item)"
                >
                  <v-icon start size="16">{{ item.isLiked ? 'mdi-heart' : 'mdi-heart-outline' }}</v-icon>
                  {{ item.stats.likes }}
                </v-btn>
                <v-btn
                  :color="item.isDisliked ? 'error' : 'default'"
                  variant="text"
                  size="small"
                  @click="$emit('dislike', item)"
                >
                  <v-icon start size="16">{{ item.isDisliked ? 'mdi-thumb-down' : 'mdi-thumb-down-outline' }}</v-icon>
                  {{ item.stats.dislikes }}
                </v-btn>
              </div>
              <v-btn
                :color="item.price === 0 ? 'success' : 'primary'"
                variant="flat"
                size="small"
                @click="$emit('purchase', item)"
              >
                <v-icon start size="16">mdi-play</v-icon>
                {{ item.price === 0 ? '获取' : '购买' }}
              </v-btn>
            </v-card-actions>
        </div>
      </div>
    </v-card-text>
  </v-card>
</template>

<script lang="ts" setup>
interface MarketItem {
  id: string
  title: string
  description: string
  type: 'template' | 'workflow' | 'workspace' | 'model'
  category: string
  price: number
  originalPrice?: number
  coverUrl: string
  author: {
    id: string
    name: string
    avatar?: string
    rating: number
  }
  stats: {
    downloads: number
    rating: number
    reviews: number
    likes: number
    dislikes: number
  }
  tags: string[]
  createdAt: string
  isFavorite: boolean
  isVerified: boolean
  isLiked: boolean
  isDisliked: boolean
  prompt?: string
  difficulty?: 'beginner' | 'intermediate' | 'advanced'
}

defineProps<{
  item: MarketItem
}>()

defineEmits<{
  purchase: [item: MarketItem]
  favorite: [item: MarketItem]
  like: [item: MarketItem]
  dislike: [item: MarketItem]
}>()

// Methods
function getTypeColor(type: string): string {
  const colors = {
    template: 'info',
    workflow: 'primary',
    workspace: 'success',
    model: 'warning'
  }
  return colors[type as keyof typeof colors] || 'grey'
}

function getTypeText(type: string): string {
  const texts = {
    template: '模板',
    workflow: '工作流',
    workspace: '工作空间',
    model: '模型'
  }
  return texts[type as keyof typeof texts] || type
}
</script>

<style scoped>
.market-item-card {
  cursor: pointer;
  transition: all 0.3s ease;
  height: 100%;
}

.market-item-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 25px rgba(0,0,0,0.15);
}

.type-badge {
  top: 12px;
  left: 12px;
}

.verified-badge {
  top: 12px;
  right: 50px;
}

.favorite-btn {
  top: 12px;
  right: 12px;
}

.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  line-clamp: 2;
  overflow: hidden;
}
</style>
