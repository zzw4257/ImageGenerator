<template>
  <v-container class="pa-8" fluid>
    <div class="d-flex justify-space-between align-center mb-6">
      <div>
        <h1 class="text-h4 font-weight-bold">创意市场</h1>
        <p class="text-body-1 text-grey-darken-1 mt-2">发现和交易优质工作流、工作空间</p>
      </div>
      <v-btn
        v-if="canCreate"
        color="primary"
        prepend-icon="mdi-plus"
        rounded="xl"
        size="large"
        @click="showCreateDialog = true"
      >
        发布创意
      </v-btn>
    </div>

    <!-- Filter Tabs -->
    <v-tabs v-model="activeTab" class="mb-6" color="primary">
      <v-tab value="all">全部</v-tab>
      <v-tab value="templates">提示词模板</v-tab>
      <v-tab value="workflows">工作流</v-tab>
      <v-tab value="workspaces">工作空间</v-tab>
      <v-tab value="models">模型市场</v-tab>
      <v-tab value="my-listings">我的挂单</v-tab>
    </v-tabs>

    <!-- Filter Bar -->
    <v-row class="mb-6">
      <v-col cols="12" md="3">
        <v-select
          v-model="filters.category"
          clearable
          :items="categoryOptions"
          label="分类"
          variant="outlined"
        />
      </v-col>
      <v-col cols="12" md="3">
        <v-select
          v-model="filters.priceRange"
          clearable
          :items="priceRangeOptions"
          label="价格区间"
          variant="outlined"
        />
      </v-col>
      <v-col cols="12" md="3">
        <v-select
          v-model="filters.sortBy"
          :items="sortOptions"
          label="排序方式"
          variant="outlined"
        />
      </v-col>
      <v-col cols="12" md="3">
        <v-text-field
          v-model="filters.search"
          clearable
          label="搜索"
          prepend-inner-icon="mdi-magnify"
          variant="outlined"
        />
      </v-col>
    </v-row>

    <!-- Loading State -->
    <div v-if="loading" class="text-center py-12">
      <v-progress-circular color="primary" indeterminate size="64" />
      <p class="text-body-2 mt-4">加载市场数据中...</p>
    </div>

    <!-- Market Items Grid -->
    <v-row v-else>
      <!-- Hot Ranking Sidebar -->
      <v-col v-if="activeTab === 'all'" cols="12" md="3">
        <v-card class="mb-6" elevation="2" rounded="xl">
          <v-card-title class="pa-4">
            <v-icon class="mr-2">mdi-fire</v-icon>
            热度排行榜
          </v-card-title>
          <v-divider />
          <v-card-text class="pa-0">
            <v-list>
              <v-list-item
                v-for="(item, index) in hotRanking"
                :key="item.id"
                class="px-4 py-3"
              >
                <template #prepend>
                  <v-chip
                    class="mr-3"
                    :color="getRankColor(index)"
                    size="small"
                    variant="flat"
                  >
                    {{ index + 1 }}
                  </v-chip>
                  <v-avatar size="32">
                    <v-img :src="item.coverUrl || '/images/default-preset.png'" />
                  </v-avatar>
                </template>
                <v-list-item-title class="text-body-2 font-weight-medium">
                  {{ truncateTitle(item.title) }}
                </v-list-item-title>
                <v-list-item-subtitle>
                  {{ item.stats.downloads }} 下载
                </v-list-item-subtitle>
                <template #append>
                  <v-chip :color="getTypeColor(item.type)" size="x-small" variant="flat">
                    {{ getTypeText(item.type) }}
                  </v-chip>
                </template>
              </v-list-item>
            </v-list>
          </v-card-text>
        </v-card>
      </v-col>

      <!-- Main Content -->
      <v-col :cols="activeTab === 'all' ? 9 : 12">
        <v-row>
          <v-col
            v-for="item in filteredItems"
            :key="item.id"
            cols="12"
            :lg="activeTab === 'all' ? 4 : 3"
            :md="activeTab === 'all' ? 6 : 4"
            sm="6"
          >
            <MarketItemCard :item="item" @favorite="toggleFavorite" @purchase="handlePurchase" />
          </v-col>
        </v-row>
      </v-col>
    </v-row>

    <!-- Empty State -->
    <div v-if="!loading && filteredItems.length === 0" class="text-center py-12">
      <v-icon class="mb-4" color="grey-lighten-2" size="80">mdi-store-outline</v-icon>
      <h3 class="text-h6 mb-2">暂无商品</h3>
      <p class="text-body-2 text-grey-darken-1">调整筛选条件或成为第一个发布者</p>
    </div>

    <!-- Pagination -->
    <div v-if="totalPages > 1" class="d-flex justify-center mt-6">
      <v-pagination
        v-model="currentPage"
        :length="totalPages"
        rounded="circle"
        :total-visible="7"
        @update:model-value="loadMarketItems"
      />
    </div>

    <!-- Create Listing Dialog -->
    <CreateListingDialog
      v-model="showCreateDialog"
      @created="onListingCreated"
    />

    <!-- Purchase Dialog -->
    <PurchaseDialog
      v-model="showPurchaseDialog"
      :item="selectedItem"
      @confirmed="onPurchaseConfirmed"
    />
  </v-container>
</template>

<script lang="ts" setup>
  import { computed, onMounted, ref, watch } from 'vue'
  import CreateListingDialog from '@/components/CreateListingDialog.vue'
  import MarketItemCard from '@/components/MarketItemCard.vue'
  import PurchaseDialog from '@/components/PurchaseDialog.vue'
  import { useAuth } from '@/composables/useAuth'
  import { useNotificationStore } from '@/stores/notification'

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
    prompt?: string // For templates
    difficulty?: 'beginner' | 'intermediate' | 'advanced' // For templates
  }

  const role = useAuth()
  const notificationStore = useNotificationStore()

  // State
  const loading = ref(false)
  const activeTab = ref('all')
  const currentPage = ref(1)
  const totalPages = ref(1)
  const showCreateDialog = ref(false)
  const canCreate = computed(() => role.value >= 2)
  const showPurchaseDialog = ref(false)
  const selectedItem = ref<MarketItem | null>(null)

  // Filters
  const filters = ref({
    category: null as string | null,
    priceRange: null as string | null,
    sortBy: 'latest',
    search: '',
  })

  // Mock data
  const marketItems = ref<MarketItem[]>([
    // Templates
    {
      id: 't1',
      title: '产品摄影专业模板',
      description: '适用于电商产品拍摄的专业提示词模板，包含光线、角度、背景等完整设置',
      type: 'template',
      category: 'photography',
      price: 5,
      coverUrl: '/images/presets/product-shot.png',
      author: {
        id: 'user1',
        name: '摄影师小王',
        avatar: '/images/avatars/user1.png',
        rating: 4.8,
      },
      stats: {
        downloads: 1567,
        rating: 4.8,
        reviews: 234,
        likes: 234,
        dislikes: 12,
      },
      tags: ['产品摄影', '电商', '专业', '商业'],
      createdAt: '2024-10-20',
      isFavorite: false,
      isVerified: true,
      isLiked: false,
      isDisliked: false,
      prompt: 'Professional product photography of [PRODUCT], shot with [CAMERA] on [BACKGROUND]. Lighting: [LIGHTING_SETUP]. Angle: [ANGLE]. Style: [STYLE]. High resolution, sharp focus, commercial quality.',
      difficulty: 'intermediate',
    },
    {
      id: 't2',
      title: 'UI界面设计模板',
      description: '现代化UI界面设计的提示词模板，适用于网站、App等界面设计',
      type: 'template',
      category: 'design',
      price: 3,
      coverUrl: '/images/presets/minimal.png',
      author: {
        id: 'user2',
        name: 'UI设计师',
        avatar: '/images/avatars/user2.png',
        rating: 4.6,
      },
      stats: {
        downloads: 892,
        rating: 4.6,
        reviews: 189,
        likes: 189,
        dislikes: 8,
      },
      tags: ['UI设计', '界面', '现代', '简洁'],
      createdAt: '2024-10-18',
      isFavorite: true,
      isVerified: true,
      isLiked: true,
      isDisliked: false,
      prompt: 'Modern [UI_TYPE] design for [PLATFORM], featuring [COLOR_SCHEME] color palette. Layout: [LAYOUT_STYLE]. Components: [COMPONENTS]. Design system: [DESIGN_SYSTEM]. Clean, minimal, user-friendly interface.',
      difficulty: 'beginner',
    },
    // Workflows
    {
      id: 'w1',
      title: '电商产品摄影工作流',
      description: '专业的产品摄影工作流，包含光线设置、角度调整和后期处理步骤',
      type: 'workflow',
      category: 'photography',
      price: 15,
      originalPrice: 25,
      coverUrl: '/images/presets/product-shot.png',
      author: {
        id: 'user1',
        name: '摄影大师',
        avatar: '/images/avatars/user1.png',
        rating: 4.8,
      },
      stats: {
        downloads: 234,
        rating: 4.7,
        reviews: 45,
        likes: 156,
        dislikes: 3,
      },
      tags: ['电商', '产品摄影', '专业'],
      createdAt: '2024-10-20',
      isFavorite: false,
      isVerified: true,
    },
    // Workspaces
    {
      id: 's1',
      title: 'UI设计工作空间',
      description: '完整的UI设计工作空间，包含组件库、设计规范和模板',
      type: 'workspace',
      category: 'design',
      price: 30,
      coverUrl: '/images/presets/minimal.png',
      author: {
        id: 'user2',
        name: 'UI设计师',
        rating: 4.9,
      },
      stats: {
        downloads: 156,
        rating: 4.8,
        reviews: 28,
        likes: 89,
        dislikes: 2,
      },
      tags: ['UI', '设计', '组件库'],
      createdAt: '2024-10-18',
      isFavorite: true,
      isVerified: true,
    },
    // Models
    {
      id: 'm1',
      title: 'Anime Style LoRA',
      description: '专业训练的动漫风格 LoRA 模型，适用于角色设计和插画创作',
      type: 'model',
      category: 'illustration',
      price: 50,
      originalPrice: 80,
      coverUrl: '/images/presets/comic.png',
      author: {
        id: 'user3',
        name: '模型训练师',
        rating: 4.7,
      },
      stats: {
        downloads: 89,
        rating: 4.5,
        reviews: 12,
        likes: 67,
        dislikes: 5,
      },
      tags: ['动漫', 'LoRA', '模型', '插画'],
      createdAt: '2024-10-15',
      isFavorite: false,
      isVerified: true,
      isLiked: false,
      isDisliked: false,
    },
  ])

  // Options
  const categoryOptions = [
    { title: '摄影', value: 'photography' },
    { title: '设计', value: 'design' },
    { title: '插画', value: 'illustration' },
    { title: '文字设计', value: 'typography' },
    { title: '其他', value: 'other' },
  ]

  const priceRangeOptions = [
    { title: '免费', value: 'free' },
    { title: '1-10 Credits', value: '1-10' },
    { title: '11-30 Credits', value: '11-30' },
    { title: '31+ Credits', value: '31+' },
  ]

  const sortOptions = [
    { title: '最新发布', value: 'latest' },
    { title: '最受欢迎', value: 'popular' },
    { title: '价格从低到高', value: 'price-asc' },
    { title: '价格从高到低', value: 'price-desc' },
    { title: '评分最高', value: 'rating' },
  ]

  // Hot Ranking Data
  const hotRanking = computed(() => {
    return marketItems.value
      .slice()
      .sort((a, b) => b.stats.downloads - a.stats.downloads)
      .slice(0, 10)
  })

  // Computed
  const filteredItems = computed(() => {
    let items = marketItems.value

    // Filter by tab
    switch (activeTab.value) {
      case 'templates': {
        items = items.filter(item => item.type === 'template')

        break
      }
      case 'workflows': {
        items = items.filter(item => item.type === 'workflow')

        break
      }
      case 'workspaces': {
        items = items.filter(item => item.type === 'workspace')

        break
      }
      case 'models': {
        items = items.filter(item => item.type === 'model')

        break
      }
      case 'my-listings': {
        // TODO: Filter by current user's listings
        items = []

        break
      }
    // No default
    }

    // Apply filters
    if (filters.value.category) {
      items = items.filter(item => item.category === filters.value.category)
    }

    if (filters.value.priceRange) {
      const range = filters.value.priceRange
      switch (range) {
        case 'free': {
          items = items.filter(item => item.price === 0)

          break
        }
        case '1-10': {
          items = items.filter(item => item.price >= 1 && item.price <= 10)

          break
        }
        case '11-30': {
          items = items.filter(item => item.price >= 11 && item.price <= 30)

          break
        }
        case '31+': {
          items = items.filter(item => item.price >= 31)

          break
        }
      // No default
      }
    }

    if (filters.value.search) {
      const search = filters.value.search.toLowerCase()
      items = items.filter(item =>
        item.title.toLowerCase().includes(search)
        || item.description.toLowerCase().includes(search)
        || item.tags.some(tag => tag.toLowerCase().includes(search)),
      )
    }

    // Sort items
    const sortBy = filters.value.sortBy
    switch (sortBy) {
      case 'popular': {
        items.sort((a, b) => b.stats.downloads - a.stats.downloads)

        break
      }
      case 'price-asc': {
        items.sort((a, b) => a.price - b.price)

        break
      }
      case 'price-desc': {
        items.sort((a, b) => b.price - a.price)

        break
      }
      case 'rating': {
        items.sort((a, b) => b.stats.rating - a.stats.rating)

        break
      }
      default: {
        // latest
        items.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
      }
    }

    return items
  })

  // Methods
  async function loadMarketItems () {
    loading.value = true
    try {
      // TODO: Replace with actual API call
      await new Promise(resolve => setTimeout(resolve, 1000))
      // Mock pagination
      totalPages.value = Math.ceil(marketItems.value.length / 12)
    } catch {
      notificationStore.error('加载市场数据失败')
    } finally {
      loading.value = false
    }
  }

  function handlePurchase (item: MarketItem) {
    selectedItem.value = item
    showPurchaseDialog.value = true
  }

  function toggleFavorite (item: MarketItem) {
    item.isFavorite = !item.isFavorite
    notificationStore.success(item.isFavorite ? '已收藏' : '已取消收藏')
  }

  function getRankColor (index: number): string {
    const colors = ['warning', 'grey', 'orange']
    return colors[index] || 'grey'
  }

  function truncateTitle (title: string): string {
    return title.length > 15 ? title.slice(0, 15) + '...' : title
  }

  function getTypeColor (type: string): string {
    const colors = {
      template: 'info',
      workflow: 'primary',
      workspace: 'success',
      model: 'warning',
    }
    return colors[type as keyof typeof colors] || 'grey'
  }

  function getTypeText (type: string): string {
    const texts = {
      template: '模板',
      workflow: '工作流',
      workspace: '工作空间',
      model: '模型',
    }
    return texts[type as keyof typeof texts] || type
  }

  function onListingCreated () {
    notificationStore.success('发布成功！等待系统审核...')
    loadMarketItems()
  }

  function onPurchaseConfirmed (item: MarketItem) {
    notificationStore.success(`成功购买 "${item.title}"！`)
  // TODO: Add to user's library
  }

  // Watch filters
  watch(filters, () => {
    currentPage.value = 1
  }, { deep: true })

  watch(activeTab, () => {
    currentPage.value = 1
  })

  onMounted(() => {
    loadMarketItems()
  })
</script>

<route lang="yaml">
meta:
  title: 创意市场
</route>
