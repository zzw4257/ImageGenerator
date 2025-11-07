<template>
  <v-container class="py-6 community-page">
    <v-row>
      <v-col cols="12">
        <div class="d-flex flex-column flex-md-row justify-space-between align-md-center mb-6 gap-4">
          <div>
            <h1 class="text-h4 font-weight-bold mb-2">社区创作者</h1>
            <p class="text-body-1 text-grey-darken-1">
              发现优秀的生成式创作者，关注他们的最新作品与项目灵感。
            </p>
          </div>
          <v-btn
            color="primary"
            prepend-icon="mdi-account-plus"
            rounded="lg"
            variant="flat"
            @click="exploreCreators"
          >
            我要加入社区
          </v-btn>
        </div>
      </v-col>
    </v-row>

    <v-row align="stretch" class="mb-8">
      <v-col cols="12" lg="8">
        <v-sheet class="pa-6" color="surface" elevation="2" rounded="xl">
          <div class="d-flex align-center justify-space-between mb-4">
            <h2 class="text-h5 font-weight-medium">创作者榜单</h2>
            <v-chip color="primary" variant="flat">
              Top {{ leaderboard.length }}
            </v-chip>
          </div>

          <v-row v-if="creatorsLoading" density="compact">
            <v-col v-for="index in 3" :key="`leaderboard-skeleton-${index}`" cols="12">
              <v-skeleton-loader
                class="mb-3"
                elevation="1"
                height="120"
                type="list-item-avatar-two-line, actions"
              />
            </v-col>
          </v-row>

          <template v-else>
            <v-row dense>
              <v-col
                v-for="creator in leaderboard"
                :key="creator.id"
                cols="12"
              >
                <v-card class="leaderboard-card px-4 py-4" flat rounded="xl">
                  <div class="d-flex align-center">
                    <div class="rank-badge mr-4" :class="`rank-${creator.rank}`">
                      {{ creator.rank }}
                    </div>
                    <v-avatar
                      class="mr-4 cursor-pointer"
                      rounded="lg"
                      size="64"
                      @click="openCreator(creator.id)"
                    >
                      <v-img :alt="creator.name" cover :src="creator.avatar" />
                    </v-avatar>
                    <div class="flex-grow-1">
                      <div class="d-flex align-center gap-2">
                        <span class="text-h6 font-weight-semibold cursor-pointer" @click="openCreator(creator.id)">
                          {{ creator.name }}
                        </span>
                        <v-chip color="primary" size="x-small" variant="flat">
                          {{ creator.city }}
                        </v-chip>
                      </div>
                      <div class="text-body-2 text-grey-darken-1 mb-2">
                        {{ creator.title }}
                      </div>
                      <div class="d-flex flex-wrap align-center gap-3 text-caption text-grey-darken-1">
                        <div class="d-flex align-center gap-1">
                          <v-icon color="primary" size="16">mdi-account-group</v-icon>
                          <span>{{ formatFollowers(creator.id) }} 关注者</span>
                        </div>
                        <div class="d-flex align-center gap-1">
                          <v-icon color="deep-purple-lighten-2" size="16">mdi-palette</v-icon>
                          <span>{{ creator.artworks }} 作品</span>
                        </div>
                        <div class="d-flex align-center gap-1">
                          <v-icon color="orange-lighten-2" size="16">mdi-chart-line-variant</v-icon>
                          <span>互动率 {{ formatPercent(creator.metrics.engagementRate) }}</span>
                        </div>
                      </div>
                    </div>
                    <v-btn
                      class="ml-4"
                      :color="isFollowing(creator.id) ? 'primary' : 'primary'"
                      rounded="lg"
                      :variant="isFollowing(creator.id) ? 'flat' : 'outlined'"
                      @click="toggleCreatorFollow(creator.id)"
                    >
                      {{ isFollowing(creator.id) ? '已关注' : '关注' }}
                    </v-btn>
                  </div>
                </v-card>
              </v-col>
            </v-row>
          </template>
        </v-sheet>
      </v-col>

      <v-col cols="12" lg="4">
        <v-sheet class="pa-6 fill-height" color="surface" elevation="2" rounded="xl">
          <div class="d-flex align-center justify-space-between mb-4">
            <h2 class="text-h6 font-weight-medium">趋势话题</h2>
            <!-- <v-btn color="primary" size="small" variant="text" @click="refreshTopics">
              <v-icon size="16" start>mdi-refresh</v-icon>
              更新
            </v-btn> -->
          </div>

          <v-skeleton-loader
            v-if="creatorsLoading"
            :loading="creatorsLoading"
            type="list-item-two-line"
          />

          <v-list v-else class="pa-0">
            <v-list-item
              v-for="topic in trendingTopics"
              :key="topic.id"
              class="mb-2 rounded-lg"
            >
              <template #prepend>
                <v-avatar
                  color="primary-lighten-4"
                  size="36"
                >
                  <v-icon :color="momentumColor(topic.momentum)">
                    {{ momentumIcon(topic.momentum) }}
                  </v-icon>
                </v-avatar>
              </template>

              <v-list-item-title class="font-weight-medium">
                {{ topic.name }}
              </v-list-item-title>

              <v-list-item-subtitle>
                {{ topic.posts }} 篇作品讨论
              </v-list-item-subtitle>

              <template #append>
                <v-chip
                  :color="momentumColor(topic.momentum)"
                  size="small"
                  variant="flat"
                >
                  {{ momentumLabel(topic.momentum) }}
                </v-chip>
              </template>
            </v-list-item>
          </v-list>
        </v-sheet>
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12">
        <v-sheet class="pa-6" color="surface" elevation="2" rounded="xl">
          <div class="d-flex flex-column flex-md-row align-md-center justify-space-between mb-4 gap-3">
            <div>
              <h2 class="text-h5 font-weight-medium mb-1">作品动态</h2>
              <p class="text-body-2 text-grey-darken-1">
                浏览社区每日精选的生成式作品与项目拆解。
              </p>
            </div>

            <v-tabs v-model="feedTab" class="feed-tabs" color="primary" grow>
              <v-tab value="latest">最新</v-tab>
              <v-tab value="popular">热门</v-tab>
            </v-tabs>
          </div>

          <v-row v-if="feedLoading" class="mt-2" dense>
            <v-col
              v-for="index in 4"
              :key="`feed-skeleton-${index}`"
              cols="12"
              lg="3"
              md="6"
            >
              <v-skeleton-loader class="mb-4" height="320" type="image, article" />
            </v-col>
          </v-row>

          <div v-else-if="feedItems.length === 0" class="py-10 text-center">
            <v-avatar class="mb-4" color="primary-lighten-4" size="64">
              <v-icon color="primary">mdi-package-variant</v-icon>
            </v-avatar>
            <div class="text-body-1 font-weight-medium mb-1">暂时没有内容</div>
            <div class="text-body-2 text-grey-darken-1">
              切换其他标签或稍后再来探索新的作品。
            </div>
          </div>

          <v-row v-else dense>
            <v-col
              v-for="item in feedItems"
              :key="item.id"
              cols="12"
              lg="3"
              md="6"
            >
              <v-card class="feed-card h-100" elevation="2" rounded="xl">
                <v-img
                  :alt="item.title"
                  class="rounded-t-xl"
                  cover
                  height="180"
                  :src="item.preview"
                />

                <v-card-text>
                  <div class="d-flex align-center justify-space-between mb-3">
                    <div class="d-flex align-center gap-2 cursor-pointer" @click="openCreator(item.creatorId)">
                      <v-avatar size="32">
                        <v-img
                          :alt="creatorName(item.creatorId)"
                          cover
                          :src="creatorAvatar(item.creatorId)"
                        />
                      </v-avatar>
                      <div>
                        <div class="text-body-2 font-weight-medium">
                          {{ creatorName(item.creatorId) }}
                        </div>
                        <div class="text-caption text-grey-darken-1">
                          {{ timeFromNow(item.createdAt) }}
                        </div>
                      </div>
                    </div>

                    <!-- <v-btn
                      class="text-caption"
                      :color="isFollowing(item.creatorId) ? 'primary' : 'primary'"
                      size="small"
                      :variant="isFollowing(item.creatorId) ? 'flat' : 'outlined'"
                      @click="toggleCreatorFollow(item.creatorId)"
                    >
                      {{ isFollowing(item.creatorId) ? '已关注' : '关注' }}
                    </v-btn> -->
                  </div>

                  <div class="mb-2 text-body-1 font-weight-medium">
                    {{ item.title }}
                  </div>
                  <p class="text-body-2 text-grey-darken-1 mb-3">
                    {{ item.caption }}
                  </p>

                  <div class="d-flex flex-wrap gap-2 mb-4">
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

                  <div class="d-flex justify-space-between text-caption text-grey-darken-1">
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
            </v-col>
          </v-row>
        </v-sheet>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts" setup>
  import type {
    CommunityCreator,
    CommunityFeedItem,
    CommunityFeedTab,
    TrendingTopic,
  } from '@/types/community'
  import { computed, onMounted, ref, watch } from 'vue'
  import { useRouter } from 'vue-router'
  import { useCommunityFollow } from '@/composables/useCommunityFollow'
  import {
    getCommunityCreators,
    getCommunityFeed,
    getTrendingTopics,
  } from '@/mock/community'

  const router = useRouter()
  const { isFollowing, toggleFollow } = useCommunityFollow()

  const creators = ref<CommunityCreator[]>([])
  const trendingTopics = ref<TrendingTopic[]>([])
  const feedItems = ref<CommunityFeedItem[]>([])
  const feedTab = ref<CommunityFeedTab>('latest')
  const creatorsLoading = ref(true)
  const feedLoading = ref(true)
  const numberFormatter = new Intl.NumberFormat('zh-CN', {
    notation: 'compact',
    compactDisplay: 'short',
  })

  const leaderboard = computed(() => {
    return creators.value
      .toSorted((a, b) => b.followers - a.followers)
      .slice(0, 3)
      .map((creator, index) => ({
        ...creator,
        rank: index + 1,
      }))
  })

  const creatorMap = computed(() => {
    return creators.value.reduce<Record<string, CommunityCreator>>((result, creator) => {
      result[creator.id] = creator
      return result
    }, {})
  })

  function formatFollowers (creatorId: string) {
    const creator = creatorMap.value[creatorId]
    if (!creator) return '0'
    const base = creator.followers
    const delta = isFollowing(creatorId) ? 1 : 0
    return numberFormatter.format(base + delta)
  }

  function formatPercent (value: number) {
    return `${Math.round(value * 100)}%`
  }

  function momentumIcon (momentum: TrendingTopic['momentum']) {
    if (momentum === 'rising') return 'mdi-trending-up'
    if (momentum === 'steady') return 'mdi-trending-neutral'
    return 'mdi-sparkles'
  }

  function momentumColor (momentum: TrendingTopic['momentum']) {
    if (momentum === 'rising') return 'primary'
    if (momentum === 'steady') return 'deep-purple-lighten-2'
    return 'orange-accent-2'
  }

  function momentumLabel (momentum: TrendingTopic['momentum']) {
    if (momentum === 'rising') return '热度上升'
    if (momentum === 'steady') return '稳定讨论'
    return '新话题'
  }

  const creatorName = (creatorId: string) => creatorMap.value[creatorId]?.name ?? '匿名创作者'
  const creatorAvatar = (creatorId: string) => creatorMap.value[creatorId]?.avatar ?? ''

  function timeFromNow (createdAt: Date) {
    const now = Date.now()
    const diffMs = now - createdAt.getTime()
    const diffHours = Math.floor(diffMs / (1000 * 60 * 60))
    if (diffHours < 1) return '刚刚'
    if (diffHours < 24) return `${diffHours} 小时前`
    const diffDays = Math.floor(diffHours / 24)
    if (diffDays < 7) return `${diffDays} 天前`
    const diffWeeks = Math.floor(diffDays / 7)
    return `${diffWeeks} 周前`
  }

  function toggleCreatorFollow (creatorId: string) {
    toggleFollow(creatorId)
  }

  function openCreator (creatorId: string) {
    router.push(`/community/${creatorId}`)
  }

  function exploreCreators () {
    router.push('/account')
  }

  async function refreshTopics () {
    creatorsLoading.value = true
    await new Promise(resolve => setTimeout(resolve, 240))
    trendingTopics.value = getTrendingTopics()
    creatorsLoading.value = false
  }

  async function loadCreators () {
    creatorsLoading.value = true
    await new Promise(resolve => setTimeout(resolve, 280))
    creators.value = getCommunityCreators()
    trendingTopics.value = getTrendingTopics()
    creatorsLoading.value = false
  }

  async function loadFeed (tab: CommunityFeedTab) {
    feedLoading.value = true
    await new Promise(resolve => setTimeout(resolve, 220))
    feedItems.value = getCommunityFeed(tab)
    feedLoading.value = false
  }

  watch(feedTab, async newTab => {
    await loadFeed(newTab)
  })

  onMounted(async () => {
    await loadCreators()
    await loadFeed(feedTab.value)
  })
</script>

<style scoped>
.community-page .leaderboard-card {
  background:
    linear-gradient(135deg, rgba(var(--v-theme-primary), 0.08), transparent),
    rgb(var(--v-theme-surface));
  border: 1px solid rgba(var(--v-theme-primary), 0.08);
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}

.community-page .leaderboard-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 10px 30px rgba(31, 38, 135, 0.11);
}

.rank-badge {
  width: 40px;
  height: 40px;
  border-radius: 12px;
  display: grid;
  place-items: center;
  font-weight: 600;
  color: rgb(var(--v-theme-on-primary));
}

.rank-1 {
  background: linear-gradient(135deg, #ff9800, #ffc107);
}

.rank-2 {
  background: linear-gradient(135deg, #9e9e9e, #cfd8dc);
}

.rank-3 {
  background: linear-gradient(135deg, #8d6e63, #bcaaa4);
}

.feed-card {
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}

.feed-card:hover {
  transform: translateY(-3px);
  box-shadow: 0 12px 30px rgba(0, 0, 0, 0.16);
}

.cursor-pointer {
  cursor: pointer;
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
</style>

<route lang="yaml">
meta:
  layout: default
  title: 社区
</route>
