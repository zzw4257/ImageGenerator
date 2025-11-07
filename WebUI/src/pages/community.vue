<template>
  <v-container fluid class="pa-8">
    <div class="d-flex justify-space-between align-center mb-6">
      <div>
        <h1 class="text-h4 font-weight-bold">创意社区</h1>
        <p class="text-body-1 text-grey-darken-1 mt-2">发现优秀创作者，分享创作心得</p>
      </div>
      <v-btn color="primary" prepend-icon="mdi-plus" rounded="xl">
        发布动态
      </v-btn>
    </div>

    <v-row>
      <!-- Left Sidebar -->
      <v-col cols="12" md="3">
        <!-- Leaderboard -->
        <v-card elevation="2" rounded="xl" class="mb-6">
          <v-card-title class="pa-4">
            <v-icon class="mr-2">mdi-trophy</v-icon>
            创作者排行榜
          </v-card-title>
          <v-divider />
          <v-card-text class="pa-0">
            <v-list>
              <v-list-item
                v-for="(creator, index) in topCreators"
                :key="creator.id"
                class="px-4 py-3"
              >
                <template #prepend>
                  <v-chip
                    :color="getRankColor(index)"
                    size="small"
                    variant="flat"
                    class="mr-3"
                  >
                    {{ index + 1 }}
                  </v-chip>
                  <v-avatar size="32">
                    <v-img v-if="creator.avatar" :src="creator.avatar" />
                    <v-icon v-else>mdi-account</v-icon>
                  </v-avatar>
                </template>
                <v-list-item-title class="text-body-2 font-weight-medium">
                  {{ creator.name }}
                </v-list-item-title>
                <v-list-item-subtitle>
                  {{ creator.score }} 积分
                </v-list-item-subtitle>
                <template #append>
                  <v-btn
                    :color="creator.isFollowing ? 'primary' : 'default'"
                    :variant="creator.isFollowing ? 'flat' : 'outlined'"
                    size="small"
                    @click="toggleFollow(creator)"
                  >
                    {{ creator.isFollowing ? '已关注' : '关注' }}
                  </v-btn>
                </template>
              </v-list-item>
            </v-list>
          </v-card-text>
        </v-card>

        <!-- Hot Tags -->
        <v-card elevation="2" rounded="xl">
          <v-card-title class="pa-4">
            <v-icon class="mr-2">mdi-fire</v-icon>
            热门标签
          </v-card-title>
          <v-divider />
          <v-card-text class="pa-4">
            <div class="d-flex flex-wrap gap-2">
              <v-chip
                v-for="tag in hotTags"
                :key="tag.name"
                size="small"
                variant="outlined"
                class="cursor-pointer"
                @click="filterByTag(tag.name)"
              >
                {{ tag.name }}
                <v-icon end size="12" color="error">mdi-fire</v-icon>
              </v-chip>
            </div>
          </v-card-text>
        </v-card>
      </v-col>

      <!-- Main Content -->
      <v-col cols="12" md="6">
        <!-- Filter Tabs -->
        <v-tabs v-model="activeTab" class="mb-6" color="primary">
          <v-tab value="latest">最新</v-tab>
          <v-tab value="hot">热门</v-tab>
          <v-tab value="following">关注</v-tab>
        </v-tabs>

        <!-- Posts Feed -->
        <div class="posts-feed">
          <v-card
            v-for="post in filteredPosts"
            :key="post.id"
            elevation="2"
            rounded="xl"
            class="mb-4"
          >
            <!-- Post Header -->
            <v-card-text class="pa-4 pb-0">
              <div class="d-flex align-center justify-space-between">
                <div class="d-flex align-center">
                  <v-avatar size="40" class="mr-3">
                    <v-img v-if="post.author.avatar" :src="post.author.avatar" />
                    <v-icon v-else>mdi-account</v-icon>
                  </v-avatar>
                  <div>
                    <div class="font-weight-medium">{{ post.author.name }}</div>
                    <div class="text-caption text-grey">{{ formatTime(post.createdAt) }}</div>
                  </div>
                </div>
                <v-btn icon size="small" variant="text">
                  <v-icon>mdi-dots-horizontal</v-icon>
                </v-btn>
              </div>
            </v-card-text>

            <!-- Post Content -->
            <v-card-text class="pa-4">
              <p class="text-body-1 mb-3">{{ post.content }}</p>
              
              <!-- Post Images -->
              <v-row v-if="post.images.length > 0" class="mb-3">
                <v-col
                  v-for="(image, index) in post.images.slice(0, 4)"
                  :key="index"
                  :cols="post.images.length === 1 ? 12 : 6"
                >
                  <v-img
                    :src="image"
                    aspect-ratio="1"
                    cover
                    rounded="lg"
                    class="cursor-pointer"
                    @click="viewImage(image)"
                  />
                </v-col>
              </v-row>

              <!-- Post Tags -->
              <div v-if="post.tags.length > 0" class="mb-3">
                <v-chip
                  v-for="tag in post.tags"
                  :key="tag"
                  size="small"
                  variant="outlined"
                  class="mr-1"
                >
                  {{ tag }}
                </v-chip>
              </div>
            </v-card-text>

            <!-- Post Actions -->
            <v-card-actions class="pa-4 pt-0">
              <v-btn
                :color="post.isLiked ? 'error' : 'default'"
                variant="text"
                @click="toggleLike(post)"
              >
                <v-icon start>{{ post.isLiked ? 'mdi-heart' : 'mdi-heart-outline' }}</v-icon>
                {{ post.likes }}
              </v-btn>
              <v-btn variant="text" @click="showComments(post)">
                <v-icon start>mdi-comment-outline</v-icon>
                {{ post.comments }}
              </v-btn>
              <v-btn variant="text">
                <v-icon start>mdi-share</v-icon>
                分享
              </v-btn>
              <v-spacer />
              <v-btn
                :color="post.isBookmarked ? 'warning' : 'default'"
                variant="text"
                @click="toggleBookmark(post)"
              >
                <v-icon>{{ post.isBookmarked ? 'mdi-bookmark' : 'mdi-bookmark-outline' }}</v-icon>
              </v-btn>
            </v-card-actions>
          </v-card>
        </div>

        <!-- Load More -->
        <div class="text-center mt-6">
          <v-btn variant="outlined" rounded="xl" @click="loadMore">
            加载更多
          </v-btn>
        </div>
      </v-col>

      <!-- Right Sidebar -->
      <v-col cols="12" md="3">
        <!-- Trending Topics -->
        <v-card elevation="2" rounded="xl" class="mb-6">
          <v-card-title class="pa-4">
            <v-icon class="mr-2">mdi-trending-up</v-icon>
            热门话题
          </v-card-title>
          <v-divider />
          <v-card-text class="pa-0">
            <v-list>
              <v-list-item
                v-for="topic in trendingTopics"
                :key="topic.id"
                class="px-4 py-3"
              >
                <v-list-item-title class="text-body-2 font-weight-medium">
                  #{{ topic.name }}
                </v-list-item-title>
                <v-list-item-subtitle>
                  {{ topic.posts }} 条讨论
                </v-list-item-subtitle>
              </v-list-item>
            </v-list>
          </v-card-text>
        </v-card>

        <!-- Suggested Users -->
        <v-card elevation="2" rounded="xl">
          <v-card-title class="pa-4">
            <v-icon class="mr-2">mdi-account-plus</v-icon>
            推荐关注
          </v-card-title>
          <v-divider />
          <v-card-text class="pa-0">
            <v-list>
              <v-list-item
                v-for="user in suggestedUsers"
                :key="user.id"
                class="px-4 py-3"
              >
                <template #prepend>
                  <v-avatar size="32">
                    <v-img v-if="user.avatar" :src="user.avatar" />
                    <v-icon v-else>mdi-account</v-icon>
                  </v-avatar>
                </template>
                <v-list-item-title class="text-body-2 font-weight-medium">
                  {{ user.name }}
                </v-list-item-title>
                <v-list-item-subtitle>
                  {{ user.followers }} 关注者
                </v-list-item-subtitle>
                <template #append>
                  <v-btn
                    color="primary"
                    size="small"
                    variant="outlined"
                    @click="followUser(user)"
                  >
                    关注
                  </v-btn>
                </template>
              </v-list-item>
            </v-list>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts" setup>
import { ref, computed } from 'vue'
import { useNotificationStore } from '@/stores/notification'

const notificationStore = useNotificationStore()

interface Creator {
  id: string
  name: string
  avatar?: string
  score: number
  isFollowing: boolean
}

interface Post {
  id: string
  author: {
    id: string
    name: string
    avatar?: string
  }
  content: string
  images: string[]
  tags: string[]
  likes: number
  comments: number
  isLiked: boolean
  isBookmarked: boolean
  createdAt: string
}

// State
const activeTab = ref('latest')

// Mock Data
const topCreators = ref<Creator[]>([
  {
    id: '1',
    name: '摄影大师',
    avatar: '/images/avatars/user1.png',
    score: 9856,
    isFollowing: false
  },
  {
    id: '2',
    name: 'UI设计师',
    avatar: '/images/avatars/user2.png',
    score: 8742,
    isFollowing: true
  },
  {
    id: '3',
    name: '插画师小王',
    score: 7321,
    isFollowing: false
  }
])

const hotTags = ref([
  { name: '产品摄影', count: 234 },
  { name: 'UI设计', count: 189 },
  { name: '动漫风格', count: 156 },
  { name: '商业插画', count: 134 }
])

const posts = ref<Post[]>([
  {
    id: '1',
    author: {
      id: '1',
      name: '摄影大师',
      avatar: '/images/avatars/user1.png'
    },
    content: '分享一组最新的产品摄影作品，使用了新的光线设置技巧，效果非常棒！',
    images: [
      '/images/examples/product1.jpg',
      '/images/examples/product2.jpg'
    ],
    tags: ['产品摄影', '商业摄影', '技巧分享'],
    likes: 234,
    comments: 45,
    isLiked: false,
    isBookmarked: false,
    createdAt: '2024-10-25T10:30:00Z'
  },
  {
    id: '2',
    author: {
      id: '2',
      name: 'UI设计师',
      avatar: '/images/avatars/user2.png'
    },
    content: '最新设计的移动端界面，采用了极简风格，用户体验很棒',
    images: [
      '/images/examples/ui1.jpg'
    ],
    tags: ['UI设计', '移动端', '极简风格'],
    likes: 189,
    comments: 32,
    isLiked: true,
    isBookmarked: true,
    createdAt: '2024-10-25T09:15:00Z'
  }
])

const trendingTopics = ref([
  { id: '1', name: 'AI绘画技巧', posts: 1234 },
  { id: '2', name: '产品摄影', posts: 892 },
  { id: '3', name: 'UI设计趋势', posts: 567 }
])

const suggestedUsers = ref([
  {
    id: '4',
    name: '概念艺术家',
    followers: 2341,
    avatar: '/images/avatars/user4.png'
  },
  {
    id: '5',
    name: '品牌设计师',
    followers: 1876
  }
])

// Computed
const filteredPosts = computed(() => {
  // TODO: Implement filtering logic based on activeTab
  return posts.value
})

// Methods
function getRankColor(index: number): string {
  const colors = ['warning', 'grey', 'orange']
  return colors[index] || 'grey'
}

function toggleFollow(creator: Creator) {
  creator.isFollowing = !creator.isFollowing
  notificationStore.success(creator.isFollowing ? '已关注' : '已取消关注')
}

function filterByTag(tag: string) {
  notificationStore.info(`筛选标签: ${tag}`)
}

function toggleLike(post: Post) {
  post.isLiked = !post.isLiked
  post.likes += post.isLiked ? 1 : -1
}

function toggleBookmark(post: Post) {
  post.isBookmarked = !post.isBookmarked
  notificationStore.success(post.isBookmarked ? '已收藏' : '已取消收藏')
}

function showComments(post: Post) {
  notificationStore.info('查看评论功能开发中...')
}

function viewImage(image: string) {
  window.open(image, '_blank')
}

function loadMore() {
  notificationStore.info('加载更多内容...')
}

function followUser(user: any) {
  notificationStore.success(`已关注 ${user.name}`)
}

function formatTime(dateString: string): string {
  const date = new Date(dateString)
  const now = new Date()
  const diff = now.getTime() - date.getTime()
  
  const minutes = Math.floor(diff / (1000 * 60))
  const hours = Math.floor(diff / (1000 * 60 * 60))
  const days = Math.floor(diff / (1000 * 60 * 60 * 24))
  
  if (minutes < 60) return `${minutes}分钟前`
  if (hours < 24) return `${hours}小时前`
  return `${days}天前`
}
</script>

<route lang="yaml">
meta:
  title: 创意社区
</route>

<style scoped>
.cursor-pointer {
  cursor: pointer;
}

.posts-feed {
  min-height: 600px;
}
</style>
