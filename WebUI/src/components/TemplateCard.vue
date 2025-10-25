<template>
  <v-card elevation="2" rounded="xl" class="template-card h-100">
    <!-- Template Header -->
    <div class="position-relative">
      <v-img
        :src="template.examples[0] || '/images/default-template.png'"
        aspect-ratio="16/9"
        cover
        class="rounded-t-xl"
      />
      
      <!-- Difficulty Badge -->
      <v-chip
        class="position-absolute difficulty-badge"
        :color="getDifficultyColor(template.difficulty)"
        size="small"
        variant="flat"
      >
        {{ getDifficultyText(template.difficulty) }}
      </v-chip>

      <!-- Like Button -->
      <v-btn
        class="position-absolute like-btn"
        :color="template.isLiked ? 'error' : 'white'"
        icon
        size="small"
        variant="flat"
        @click.stop="$emit('like', template)"
      >
        <v-icon>{{ template.isLiked ? 'mdi-heart' : 'mdi-heart-outline' }}</v-icon>
      </v-btn>
    </div>

    <!-- Template Content -->
    <v-card-text class="pa-4">
      <!-- Title and Description -->
      <div class="mb-3">
        <h3 class="text-h6 font-weight-medium mb-1">{{ template.title }}</h3>
        <p class="text-body-2 text-grey-darken-1 line-clamp-2">{{ template.description }}</p>
      </div>

      <!-- Author Info -->
      <div class="d-flex align-center mb-3">
        <v-avatar size="24" class="mr-2">
          <v-img v-if="template.author.avatar" :src="template.author.avatar" />
          <v-icon v-else>mdi-account</v-icon>
        </v-avatar>
        <span class="text-body-2 text-grey-darken-1">{{ template.author.name }}</span>
        <v-spacer />
        <div class="d-flex align-center">
          <v-icon size="14" color="amber">mdi-star</v-icon>
          <span class="text-caption ml-1">{{ template.stats.rating }}</span>
        </div>
      </div>

      <!-- Stats -->
      <div class="d-flex align-center justify-space-between mb-3">
        <div class="d-flex align-center">
          <v-icon size="16" color="grey">mdi-heart</v-icon>
          <span class="text-caption ml-1 mr-3">{{ template.stats.likes }}</span>
          <v-icon size="16" color="grey">mdi-download</v-icon>
          <span class="text-caption ml-1">{{ template.stats.uses }}</span>
        </div>
      </div>

      <!-- Tags -->
      <div class="mb-3">
        <v-chip
          v-for="tag in template.tags.slice(0, 3)"
          :key="tag"
          size="x-small"
          variant="outlined"
          class="mr-1 mb-1"
        >
          {{ tag }}
        </v-chip>
      </div>

      <!-- Prompt Preview -->
      <v-card variant="tonal" color="grey-lighten-4" class="mb-3">
        <v-card-text class="pa-3">
          <div class="d-flex align-center justify-space-between mb-2">
            <span class="text-caption font-weight-medium text-grey-darken-1">提示词预览</span>
            <v-btn
              icon
              size="x-small"
              variant="text"
              @click="copyPrompt"
            >
              <v-icon size="14">mdi-content-copy</v-icon>
            </v-btn>
          </div>
          <p class="text-caption text-grey-darken-2 line-clamp-3">{{ template.prompt }}</p>
        </v-card-text>
      </v-card>

      <!-- Actions -->
      <div class="d-flex gap-2">
        <v-btn
          color="primary"
          variant="flat"
          size="small"
          class="flex-grow-1"
          @click="$emit('use', template)"
        >
          <v-icon start size="16">mdi-play</v-icon>
          使用模板
        </v-btn>
        <v-btn
          variant="outlined"
          size="small"
          icon
          @click="$emit('share', template)"
        >
          <v-icon size="16">mdi-share</v-icon>
        </v-btn>
      </div>
    </v-card-text>
  </v-card>
</template>

<script lang="ts" setup>
import { useNotificationStore } from '@/stores/notification'

interface Template {
  id: string
  title: string
  description: string
  prompt: string
  category: string
  difficulty: 'beginner' | 'intermediate' | 'advanced'
  tags: string[]
  author: {
    id: string
    name: string
    avatar?: string
  }
  stats: {
    likes: number
    uses: number
    rating: number
  }
  examples: string[]
  createdAt: string
  isLiked: boolean
}

const props = defineProps<{
  template: Template
}>()

const emit = defineEmits<{
  use: [template: Template]
  like: [template: Template]
  share: [template: Template]
}>()

const notificationStore = useNotificationStore()

function getDifficultyColor(difficulty: string): string {
  const colors = {
    beginner: 'success',
    intermediate: 'warning',
    advanced: 'error'
  }
  return colors[difficulty as keyof typeof colors] || 'grey'
}

function getDifficultyText(difficulty: string): string {
  const texts = {
    beginner: '初级',
    intermediate: '中级',
    advanced: '高级'
  }
  return texts[difficulty as keyof typeof texts] || difficulty
}

function copyPrompt() {
  navigator.clipboard.writeText(props.template.prompt).then(() => {
    notificationStore.success('提示词已复制到剪贴板')
  }).catch(() => {
    notificationStore.error('复制失败')
  })
}
</script>

<style scoped>
.template-card {
  cursor: pointer;
  transition: all 0.3s ease;
}

.template-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 25px rgba(0,0,0,0.12);
}

.difficulty-badge {
  top: 12px;
  left: 12px;
}

.like-btn {
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

.line-clamp-3 {
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
  line-clamp: 3;
  overflow: hidden;
}
</style>
