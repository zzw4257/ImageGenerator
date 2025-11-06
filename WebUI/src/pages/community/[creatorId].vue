<template>
  <v-container class="py-6">
    <template v-if="loading">
      <v-row>
        <v-col cols="12">
          <v-skeleton-loader class="mb-4" height="280" type="image" />
        </v-col>
        <v-col cols="12" md="8">
          <v-skeleton-loader class="mb-4" height="180" type="article" />
        </v-col>
        <v-col cols="12" md="4">
          <v-skeleton-loader class="mb-4" height="180" type="list-item-two-line" />
        </v-col>
      </v-row>
    </template>

    <template v-else-if="!creator">
      <v-row>
        <v-col cols="12">
          <v-sheet class="py-12 px-6 text-center" elevation="2" rounded="xl">
            <v-avatar class="mb-4" color="primary-lighten-4" size="72">
              <v-icon color="primary" size="36">mdi-alert-circle</v-icon>
            </v-avatar>
            <div class="text-h5 font-weight-medium mb-2">未找到创作者</div>
            <p class="text-body-2 text-grey-darken-1 mb-6">
              该创作者可能暂未加入社区，或链接已失效。
            </p>
            <v-btn color="primary" rounded="lg" @click="goBack">
              返回社区首页
            </v-btn>
          </v-sheet>
        </v-col>
      </v-row>
    </template>

    <template v-else>
      <v-row>
        <v-col cols="12">
          <v-card class="hero-card" elevation="2" rounded="xl">
            <v-img
              :alt="creator.name"
              class="hero-cover"
              cover
              :src="creator.highlightImage"
            >
              <div class="hero-overlay" />
            </v-img>
            <v-card-text class="hero-content">
              <div class="d-flex flex-column flex-md-row align-md-center gap-4">
                <div class="d-flex align-center gap-4">
                  <v-avatar rounded="lg" size="88">
                    <v-img :alt="creator.name" cover :src="creator.avatar" />
                  </v-avatar>
                  <div>
                    <div class="d-flex align-center gap-2 mb-1">
                      <h1 class="text-h4 font-weight-bold mb-0">
                        {{ creator.name }}
                      </h1>
                      <v-chip color="primary" size="small" variant="flat">
                        {{ creator.city }}
                      </v-chip>
                    </div>
                    <div class="text-body-1 text-grey-lighten-4 mb-2">
                      {{ creator.title }}
                    </div>
                    <div class="text-body-2 text-grey-lighten-3">
                      {{ creator.bio }}
                    </div>
                  </div>
                </div>

                <div class="d-flex flex-column flex-sm-row gap-3 ml-auto">
                  <v-btn
                    :color="isFollowing(creator.id) ? 'primary' : 'primary'"
                    prepend-icon="mdi-account-plus"
                    rounded="lg"
                    :variant="isFollowing(creator.id) ? 'flat' : 'outlined'"
                    @click="toggleCreatorFollow(creator.id)"
                  >
                    {{ isFollowing(creator.id) ? '已关注' : '关注创作者' }}
                  </v-btn>
                  <v-btn
                    color="surface"
                    prepend-icon="mdi-share-variant"
                    rounded="lg"
                    variant="tonal"
                    @click="shareProfile"
                  >
                    分享档案
                  </v-btn>
                </div>
              </div>

              <v-divider class="my-4" color="white" />

              <div class="d-flex flex-wrap gap-6 text-white">
                <div>
                  <div class="text-subtitle-1 font-weight-semibold">
                    {{ formatFollowers(creator) }}
                  </div>
                  <div class="text-caption text-white text-opacity-70">
                    关注者
                  </div>
                </div>
                <div>
                  <div class="text-subtitle-1 font-weight-semibold">
                    {{ creator.artworks }}
                  </div>
                  <div class="text-caption text-white text-opacity-70">
                    已发布作品
                  </div>
                </div>
                <div>
                  <div class="text-subtitle-1 font-weight-semibold">
                    {{ formatPercent(creator.metrics.engagementRate) }}
                  </div>
                  <div class="text-caption text-white text-opacity-70">
                    互动率
                  </div>
                </div>
                <div>
                  <div class="text-subtitle-1 font-weight-semibold">
                    {{ formatPercent(creator.metrics.clientSatisfaction) }}
                  </div>
                  <div class="text-caption text-white text-opacity-70">
                    客户满意度
                  </div>
                </div>
              </div>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>

      <v-row class="mt-6" dense>
        <v-col cols="12" md="8">
          <v-sheet class="pa-6 h-100" color="surface" elevation="2" rounded="xl">
            <div class="d-flex align-center justify-space-between mb-4">
              <h2 class="text-h6 font-weight-medium">精选拆解</h2>
              <v-chip color="primary" size="small" variant="flat">
                最新更新 {{ timeFromNow(feedItems[0]?.createdAt) }}
              </v-chip>
            </div>

            <v-row v-if="feedLoading" dense>
              <v-col v-for="index in 3" :key="`detail-feed-skeleton-${index}`" cols="12">
                <v-skeleton-loader class="mb-4" height="220" type="image, list-item-two-line" />
              </v-col>
            </v-row>

            <div v-else-if="feedItems.length === 0" class="py-6 text-center text-grey-darken-1">
              <v-avatar class="mb-3" color="primary-lighten-4" size="56">
                <v-icon color="primary">mdi-brush</v-icon>
              </v-avatar>
              <div class="text-body-1 font-weight-medium mb-1">
                还没有作品动态
              </div>
              <div class="text-body-2">
                关注后第一时间获取创作者的最新案例拆解。
              </div>
            </div>

            <v-timeline
              v-else
              align="start"
              density="compact"
              line-color="primary"
            >
              <v-timeline-item
                v-for="item in feedItems"
                :key="item.id"
                dot-color="primary"
                size="small"
              >
                <template #opposite>
                  <div class="text-caption text-grey-darken-1">
                    {{ timeFromNow(item.createdAt) }}
                  </div>
                </template>

                <v-card class="mb-4" elevation="2" rounded="xl">
                  <v-img
                    :alt="item.title"
                    class="rounded-t-xl"
                    cover
                    height="160"
                    :src="item.preview"
                  />
                  <v-card-text>
                    <div class="text-subtitle-1 font-weight-medium mb-2">
                      {{ item.title }}
                    </div>
                    <p class="text-body-2 text-grey-darken-1 mb-3">
                      {{ item.caption }}
                    </p>
                    <div class="d-flex flex-wrap gap-2 mb-3">
                      <v-chip
                        v-for="tag in item.tags"
                        :key="tag"
                        color="primary-lighten-4"
                        size="x-small"
                        variant="flat"
                      >
                        #{{ tag }}
                      </v-chip>
                    </div>
                    <div class="d-flex gap-4 text-caption text-grey-darken-1">
                      <div class="d-flex align-center gap-1">
                        <v-icon color="primary" size="16">mdi-thumb-up</v-icon>
                        <span>{{ item.likes }}</span>
                      </div>
                      <div class="d-flex align-center gap-1">
                        <v-icon color="primary" size="16">mdi-bookmark</v-icon>
                        <span>{{ item.saves }}</span>
                      </div>
                      <div class="d-flex align-center gap-1">
                        <v-icon color="primary" size="16">mdi-share-variant</v-icon>
                        <span>{{ item.shares }}</span>
                      </div>
                    </div>
                  </v-card-text>
                </v-card>
              </v-timeline-item>
            </v-timeline>
          </v-sheet>
        </v-col>

        <v-col cols="12" md="4">
          <v-sheet class="pa-6 mb-6" color="surface" elevation="2" rounded="xl">
            <h2 class="text-h6 font-weight-medium mb-4">擅长领域</h2>
            <div class="d-flex flex-wrap gap-2">
              <v-chip
                v-for="topic in creator.topics"
                :key="topic"
                color="primary"
                size="small"
                variant="tonal"
              >
                #{{ topic }}
              </v-chip>
            </div>
          </v-sheet>

          <v-sheet class="pa-6 mb-6" color="surface" elevation="2" rounded="xl">
            <h2 class="text-h6 font-weight-medium mb-3">近期合作</h2>
            <v-list class="pa-0">
              <v-list-item
                v-for="project in creator.recentProjects"
                :key="project.id"
                class="rounded-lg mb-2"
              >
                <template #prepend>
                  <v-avatar rounded="lg" size="48">
                    <v-img :alt="project.title" cover :src="project.thumbnail" />
                  </v-avatar>
                </template>

                <v-list-item-title class="font-weight-medium">
                  {{ project.title }}
                </v-list-item-title>
                <v-list-item-subtitle class="d-flex align-center gap-1">
                  <v-icon color="primary" size="16">mdi-thumb-up</v-icon>
                  {{ project.likes }} 喜欢
                </v-list-item-subtitle>
              </v-list-item>
            </v-list>
          </v-sheet>

          <v-sheet class="pa-6" color="surface" elevation="2" rounded="xl">
            <h2 class="text-h6 font-weight-medium mb-3">创作者亮点</h2>
            <ul class="pl-4 text-body-2 text-grey-darken-1">
              <li v-for="achievement in creator.achievements" :key="achievement" class="mb-2">
                {{ achievement }}
              </li>
            </ul>
          </v-sheet>
        </v-col>
      </v-row>
    </template>
  </v-container>
</template>

<script lang="ts" setup>
  import type { CommunityCreator, CommunityFeedItem } from '@/types/community'
  import { computed, onMounted, ref, watch } from 'vue'
  import { useRoute, useRouter } from 'vue-router'
  import { useCommunityFollow } from '@/composables/useCommunityFollow'
  import {
    findCommunityCreatorById,
    getCreatorFeed,
  } from '@/mock/community'

  const route = useRoute()
  const router = useRouter()
  const { isFollowing, toggleFollow } = useCommunityFollow()

  const creator = ref<CommunityCreator | null>(null)
  const feedItems = ref<CommunityFeedItem[]>([])
  const loading = ref(true)
  const feedLoading = ref(true)
  const numberFormatter = new Intl.NumberFormat('zh-CN', {
    notation: 'compact',
    compactDisplay: 'short',
  })

  const creatorId = computed(() => String(route.params.creatorId ?? ''))

  const formatPercent = (value: number) => `${Math.round(value * 100)}%`

  function formatFollowers (currentCreator: CommunityCreator) {
    const base = currentCreator.followers
    const delta = isFollowing(currentCreator.id) ? 1 : 0
    return numberFormatter.format(base + delta)
  }

  function timeFromNow (createdAt?: Date) {
    if (!createdAt) return ''
    const now = Date.now()
    const diffMs = now - createdAt.getTime()
    const diffHours = Math.floor(diffMs / (1000 * 60 * 60))
    if (diffHours < 1) return '刚刚更新'
    if (diffHours < 24) return `${diffHours} 小时前`
    const diffDays = Math.floor(diffHours / 24)
    if (diffDays < 7) return `${diffDays} 天前`
    const diffWeeks = Math.floor(diffDays / 7)
    return `${diffWeeks} 周前`
  }

  function toggleCreatorFollow (creatorIdValue: string) {
    toggleFollow(creatorIdValue)
  }

  function shareProfile () {
    navigator.clipboard?.writeText(window.location.href).catch(() => undefined)
  }

  function goBack () {
    router.push('/community')
  }

  async function loadCreator () {
    loading.value = true
    const id = creatorId.value
    await new Promise(resolve => setTimeout(resolve, 260))
    const result = findCommunityCreatorById(id)
    creator.value = result ?? null
    loading.value = false
  }

  async function loadFeed () {
    feedLoading.value = true
    await new Promise(resolve => setTimeout(resolve, 220))
    feedItems.value = creator.value ? getCreatorFeed(creator.value.id) : []
    feedLoading.value = false
  }

  watch(creatorId, async () => {
    await loadCreator()
    await loadFeed()
  })

  onMounted(async () => {
    await loadCreator()
    await loadFeed()
  })
</script>

<style scoped>
.hero-card {
  position: relative;
  overflow: hidden;
  color: white;
}

.hero-cover {
  height: 320px;
}

.hero-overlay {
  position: absolute;
  inset: 0;
  background: linear-gradient(180deg, rgba(0, 0, 0, 0.55) 5%, rgba(0, 0, 0, 0.8) 72%, rgba(0, 0, 0, 0.92) 100%);
}

.hero-content {
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  padding: 24px;
  backdrop-filter: blur(4px);
}

.gap-1 {
  gap: 4px;
}

.gap-2 {
  gap: 8px;
}

.gap-3 {
  gap: 12px;
}

.gap-4 {
  gap: 16px;
}

.gap-6 {
  gap: 24px;
}
</style>

<route lang="yaml">
meta:
  layout: default
  title: 创作者档案
</route>
