<template>
  <v-container fluid class="pa-8">
    <div class="d-flex justify-space-between align-center mb-6">
      <div>
        <h1 class="text-h4 font-weight-bold">模板分享</h1>
        <p class="text-body-1 text-grey-darken-1 mt-2">发现和分享优质的提示词模板</p>
      </div>
      <v-btn
        color="primary"
        size="large"
        rounded="xl"
        prepend-icon="mdi-plus"
        @click="showCreateDialog = true"
      >
        分享模板
      </v-btn>
    </div>

    <!-- Filter Bar -->
    <v-row class="mb-6">
      <v-col cols="12" md="3">
        <v-select
          v-model="filters.category"
          :items="categoryOptions"
          label="分类"
          variant="outlined"
          clearable
        />
      </v-col>
      <v-col cols="12" md="3">
        <v-select
          v-model="filters.difficulty"
          :items="difficultyOptions"
          label="难度"
          variant="outlined"
          clearable
        />
      </v-col>
      <v-col cols="12" md="3">
        <v-select
          v-model="filters.sortBy"
          :items="sortOptions"
          label="排序"
          variant="outlined"
        />
      </v-col>
      <v-col cols="12" md="3">
        <v-text-field
          v-model="filters.search"
          label="搜索模板"
          variant="outlined"
          prepend-inner-icon="mdi-magnify"
          clearable
        />
      </v-col>
    </v-row>

    <!-- Templates Grid -->
    <v-row>
      <v-col
        v-for="template in filteredTemplates"
        :key="template.id"
        cols="12"
        md="6"
        lg="4"
      >
        <TemplateCard 
          :template="template" 
          @use="useTemplate"
          @like="toggleLike"
          @share="shareTemplate"
        />
      </v-col>
    </v-row>

    <!-- Empty State -->
    <div v-if="filteredTemplates.length === 0" class="text-center py-12">
      <v-icon size="80" color="grey-lighten-2" class="mb-4">mdi-file-document-outline</v-icon>
      <h3 class="text-h6 mb-2">暂无模板</h3>
      <p class="text-body-2 text-grey-darken-1">调整筛选条件或成为第一个分享者</p>
    </div>

    <!-- Create Template Dialog -->
    <CreateTemplateDialog
      v-model="showCreateDialog"
      @created="onTemplateCreated"
    />

    <!-- Template Detail Dialog -->
    <TemplateDetailDialog
      v-model="showDetailDialog"
      :template="selectedTemplate"
      @use="useTemplate"
    />
  </v-container>
</template>

<script lang="ts" setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useNotificationStore } from '@/stores/notification'
import TemplateCard from '@/components/TemplateCard.vue'
import CreateTemplateDialog from '@/components/CreateTemplateDialog.vue'
import TemplateDetailDialog from '@/components/TemplateDetailDialog.vue'

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

const router = useRouter()
const notificationStore = useNotificationStore()

// State
const showCreateDialog = ref(false)
const showDetailDialog = ref(false)
const selectedTemplate = ref<Template | null>(null)

// Filters
const filters = ref({
  category: null as string | null,
  difficulty: null as string | null,
  sortBy: 'popular',
  search: ''
})

// Mock Templates Data
const templates = ref<Template[]>([
  {
    id: '1',
    title: '产品摄影专业模板',
    description: '适用于电商产品拍摄的专业提示词模板，包含光线、角度、背景等完整设置',
    prompt: 'Professional product photography of [PRODUCT], shot with [CAMERA] on [BACKGROUND]. Lighting: [LIGHTING_SETUP]. Angle: [ANGLE]. Style: [STYLE]. High resolution, sharp focus, commercial quality.',
    category: 'photography',
    difficulty: 'intermediate',
    tags: ['产品摄影', '电商', '专业', '商业'],
    author: {
      id: 'user1',
      name: '摄影师小王',
      avatar: '/images/avatars/user1.png'
    },
    stats: {
      likes: 234,
      uses: 1567,
      rating: 4.8
    },
    examples: [
      '/images/examples/product1.jpg',
      '/images/examples/product2.jpg',
      '/images/examples/product3.jpg'
    ],
    createdAt: '2024-10-20',
    isLiked: false
  },
  {
    id: '2',
    title: 'UI界面设计模板',
    description: '现代化UI界面设计的提示词模板，适用于网站、App等界面设计',
    prompt: 'Modern [UI_TYPE] design for [PLATFORM], featuring [COLOR_SCHEME] color palette. Layout: [LAYOUT_STYLE]. Components: [COMPONENTS]. Design system: [DESIGN_SYSTEM]. Clean, minimal, user-friendly interface.',
    category: 'design',
    difficulty: 'beginner',
    tags: ['UI设计', '界面', '现代', '简洁'],
    author: {
      id: 'user2',
      name: 'UI设计师',
      avatar: '/images/avatars/user2.png'
    },
    stats: {
      likes: 189,
      uses: 892,
      rating: 4.6
    },
    examples: [
      '/images/examples/ui1.jpg',
      '/images/examples/ui2.jpg'
    ],
    createdAt: '2024-10-18',
    isLiked: true
  },
  {
    id: '3',
    title: '动漫角色创作模板',
    description: '动漫风格角色设计模板，包含详细的外观、服装、表情描述',
    prompt: 'Anime character design: [CHARACTER_TYPE] with [HAIR_COLOR] hair and [EYE_COLOR] eyes. Outfit: [CLOTHING_STYLE]. Expression: [EMOTION]. Art style: [ANIME_STYLE]. Background: [BACKGROUND]. High quality anime artwork.',
    category: 'illustration',
    difficulty: 'advanced',
    tags: ['动漫', '角色设计', '插画', '二次元'],
    author: {
      id: 'user3',
      name: '动漫画师',
      avatar: '/images/avatars/user3.png'
    },
    stats: {
      likes: 156,
      uses: 634,
      rating: 4.7
    },
    examples: [
      '/images/examples/anime1.jpg',
      '/images/examples/anime2.jpg',
      '/images/examples/anime3.jpg'
    ],
    createdAt: '2024-10-15',
    isLiked: false
  }
])

// Options
const categoryOptions = [
  { title: '摄影', value: 'photography' },
  { title: '设计', value: 'design' },
  { title: '插画', value: 'illustration' },
  { title: '文字设计', value: 'typography' },
  { title: '其他', value: 'other' }
]

const difficultyOptions = [
  { title: '初级', value: 'beginner' },
  { title: '中级', value: 'intermediate' },
  { title: '高级', value: 'advanced' }
]

const sortOptions = [
  { title: '最受欢迎', value: 'popular' },
  { title: '最新发布', value: 'latest' },
  { title: '使用最多', value: 'most-used' },
  { title: '评分最高', value: 'highest-rated' }
]

// Computed
const filteredTemplates = computed(() => {
  let result = templates.value

  // Apply filters
  if (filters.value.category) {
    result = result.filter(t => t.category === filters.value.category)
  }

  if (filters.value.difficulty) {
    result = result.filter(t => t.difficulty === filters.value.difficulty)
  }

  if (filters.value.search) {
    const search = filters.value.search.toLowerCase()
    result = result.filter(t =>
      t.title.toLowerCase().includes(search) ||
      t.description.toLowerCase().includes(search) ||
      t.tags.some(tag => tag.toLowerCase().includes(search))
    )
  }

  // Apply sorting
  switch (filters.value.sortBy) {
    case 'latest':
      result.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
      break
    case 'most-used':
      result.sort((a, b) => b.stats.uses - a.stats.uses)
      break
    case 'highest-rated':
      result.sort((a, b) => b.stats.rating - a.stats.rating)
      break
    default: // popular
      result.sort((a, b) => b.stats.likes - a.stats.likes)
  }

  return result
})

// Methods
function useTemplate(template: Template) {
  // Navigate to generate page with template
  router.push({
    path: '/generate',
    query: {
      template: template.id,
      prompt: template.prompt
    }
  })
  notificationStore.success(`已应用模板: ${template.title}`)
}

function toggleLike(template: Template) {
  template.isLiked = !template.isLiked
  if (template.isLiked) {
    template.stats.likes++
    notificationStore.success('已点赞')
  } else {
    template.stats.likes--
    notificationStore.info('已取消点赞')
  }
}

function shareTemplate(template: Template) {
  // Copy template link to clipboard
  const url = `${window.location.origin}/templates/${template.id}`
  navigator.clipboard.writeText(url).then(() => {
    notificationStore.success('模板链接已复制到剪贴板')
  }).catch(() => {
    notificationStore.error('复制失败')
  })
}

function onTemplateCreated() {
  notificationStore.success('模板分享成功！')
  // Refresh templates list
  // TODO: Implement API call to refresh data
}

onMounted(() => {
  // Load templates from API
  // TODO: Implement API integration
})
</script>

<route lang="yaml">
meta:
  title: 模板分享
</route>
