<template>
  <v-container class="py-6">
    <!-- Profile Header -->
    <v-row class="mb-6">
      <v-col cols="12">
        <v-card rounded="xl" elevation="2" color="primary" class="text-white">
          <v-card-text class="pa-8">
            <div class="d-flex align-center">
              <v-avatar size="80" class="mr-6">
                <v-img v-if="(profile as any)?.avatar" :src="(profile as any).avatar" />
                <v-icon v-else size="40">mdi-account</v-icon>
              </v-avatar>
              <div class="flex-grow-1">
                <h1 class="text-h4 font-weight-bold mb-2">{{ profile?.username || 'User' }}</h1>
                <p class="text-body-1 opacity-90 mb-3">{{ (profile as any)?.bio || '这个人很懒，什么都没留下...' }}</p>
                <div class="d-flex align-center gap-4">
                  <div class="text-center">
                    <div class="text-h6 font-weight-bold">{{ userStats.totalGenerations }}</div>
                    <div class="text-caption opacity-75">总生成数</div>
                  </div>
                  <div class="text-center">
                    <div class="text-h6 font-weight-bold">{{ userStats.followers }}</div>
                    <div class="text-caption opacity-75">关注者</div>
                  </div>
                  <div class="text-center">
                    <div class="text-h6 font-weight-bold">{{ userStats.following }}</div>
                    <div class="text-caption opacity-75">关注中</div>
                  </div>
                  <div class="text-center">
                    <div class="text-h6 font-weight-bold">{{ userStats.likes }}</div>
                    <div class="text-caption opacity-75">获赞数</div>
                  </div>
                </div>
              </div>
              <div class="text-right">
                <v-chip color="white" variant="flat" size="large" class="mb-2">
                  <v-icon start>mdi-star-circle</v-icon>
                  {{ profile?.credits ?? 0 }} Credits
                </v-chip>
                <br>
                <v-btn color="white" variant="outlined" @click="editProfile">
                  编辑资料
                </v-btn>
              </div>
            </div>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <v-row>
      <!-- Left Column - Stats & Activity -->
      <v-col cols="12" lg="8">
        <!-- Usage Analytics -->
        <v-card rounded="xl" elevation="2" class="mb-6">
          <v-card-title class="pa-6">
            <v-icon class="mr-3">mdi-chart-line</v-icon>
            使用统计
          </v-card-title>
          <v-divider />
          <v-card-text class="pa-6">
            <v-row>
              <v-col cols="12" md="6">
                <div class="text-center pa-4">
                  <div class="text-h4 font-weight-bold text-primary mb-2">{{ userStats.thisMonth }}</div>
                  <div class="text-body-2 text-grey">本月生成</div>
                  <v-progress-linear
                    :model-value="(userStats.thisMonth / userStats.monthlyGoal) * 100"
                    color="primary"
                    height="8"
                    rounded
                    class="mt-2"
                  />
                  <div class="text-caption text-grey mt-1">目标: {{ userStats.monthlyGoal }}</div>
                </div>
              </v-col>
              <v-col cols="12" md="6">
                <div class="text-center pa-4">
                  <div class="text-h4 font-weight-bold text-success mb-2">{{ userStats.successRate }}%</div>
                  <div class="text-body-2 text-grey">成功率</div>
                  <v-progress-linear
                    :model-value="userStats.successRate"
                    color="success"
                    height="8"
                    rounded
                    class="mt-2"
                  />
                  <div class="text-caption text-grey mt-1">行业平均: 94%</div>
                </div>
              </v-col>
            </v-row>
          </v-card-text>
        </v-card>

        <!-- Usage Trend Chart -->
        <v-card rounded="xl" elevation="2" class="mb-6">
          <v-card-title class="pa-6">
            <v-icon class="mr-3">mdi-chart-areaspline</v-icon>
            使用趋势
          </v-card-title>
          <v-divider />
          <v-card-text class="pa-6">
            <div style="height: 300px;" class="d-flex align-center justify-center">
              <div class="text-center">
                <v-icon size="80" color="grey-lighten-2" class="mb-4">mdi-chart-line</v-icon>
                <p class="text-body-2 text-grey">使用趋势图表占位</p>
              </div>
            </div>
          </v-card-text>
        </v-card>

        <!-- Recent Activity -->
        <v-card rounded="xl" elevation="2">
          <v-card-title class="pa-6">
            <v-icon class="mr-3">mdi-history</v-icon>
            最近活动
          </v-card-title>
          <v-divider />
          <v-card-text class="pa-0">
            <v-list>
              <v-list-item
                v-for="activity in recentActivities"
                :key="activity.id"
                class="px-6 py-3"
              >
                <template #prepend>
                  <v-avatar :color="activity.color" size="32">
                    <v-icon color="white">{{ activity.icon }}</v-icon>
                  </v-avatar>
                </template>
                <v-list-item-title class="text-body-2">{{ activity.title }}</v-list-item-title>
                <v-list-item-subtitle>{{ activity.time }}</v-list-item-subtitle>
              </v-list-item>
            </v-list>
          </v-card-text>
        </v-card>
      </v-col>

      <!-- Right Column - Profile Details -->
      <v-col cols="12" lg="4">
        <!-- Profile Info -->
        <v-card rounded="xl" elevation="2" class="mb-6">
          <v-card-title class="pa-6">
            <v-icon class="mr-3">mdi-account-details</v-icon>
            个人信息
          </v-card-title>
          <v-divider />
          <v-card-text class="pa-6">
            <v-list lines="two">
              <v-list-item prepend-icon="mdi-identifier" :title="profile?.id" subtitle="用户 ID" />
              <v-list-item prepend-icon="mdi-account" :title="profile?.username" subtitle="用户名" />
              <v-list-item prepend-icon="mdi-calendar" :title="formatDate(profile?.createdAt)" subtitle="注册时间" />
              <v-list-item prepend-icon="mdi-map-marker" title="北京, 中国" subtitle="位置" />
            </v-list>
          </v-card-text>
        </v-card>

        <!-- Achievements -->
        <v-card rounded="xl" elevation="2" class="mb-6">
          <v-card-title class="pa-6">
            <v-icon class="mr-3">mdi-trophy</v-icon>
            成就徽章
          </v-card-title>
          <v-divider />
          <v-card-text class="pa-6">
            <div class="d-flex flex-wrap gap-3">
              <v-tooltip
                v-for="achievement in achievements"
                :key="achievement.id"
                :text="achievement.description"
              >
                <template #activator="{ props }">
                  <v-avatar
                    v-bind="props"
                    :color="achievement.earned ? achievement.color : 'grey-lighten-2'"
                    size="48"
                    class="cursor-pointer"
                  >
                    <v-icon :color="achievement.earned ? 'white' : 'grey'">
                      {{ achievement.icon }}
                    </v-icon>
                  </v-avatar>
                </template>
              </v-tooltip>
            </div>
          </v-card-text>
        </v-card>

        <!-- Credits Management -->
        <v-card rounded="xl" elevation="2">
          <v-card-title class="pa-6">
            <v-icon class="mr-3">mdi-star-circle</v-icon>
            Credits 管理
          </v-card-title>
          <v-divider />
          <v-card-text class="pa-6">
            <div class="text-center mb-4">
              <div class="text-h3 font-weight-bold text-primary">{{ profile?.credits ?? 0 }}</div>
              <div class="text-body-2 text-grey">可用 Credits</div>
            </div>
            
            <v-btn
              color="primary"
              :loading="claiming"
              @click="onClaimCredits"
              :disabled="claimDisabled"
              block
              rounded="xl"
              class="mb-3"
            >
              <v-icon start>mdi-gift</v-icon>
              每日签到
            </v-btn>
            
            <v-btn
              variant="outlined"
              block
              rounded="xl"
              @click="$router.push('/recharge')"
            >
              <v-icon start>mdi-credit-card-plus</v-icon>
              充值 Credits
            </v-btn>
            
            <div class="text-caption text-grey text-center mt-3">
              上次签到: {{ formatDate(profile?.lastCreditClaimedAt) || '从未' }}
            </div>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts" setup>
import { onMounted, ref, computed } from 'vue'
import { useNotificationStore } from '@/stores/notification'

/**
 * A page that displays the user's profile information and allows them to claim daily credits.
 */
import { getProfile, claimCredits } from '@/services/profile'
import type { ProfileDto } from '@/types/api'

const notificationStore = useNotificationStore()

const profile = ref<ProfileDto | null>(null)
const loading = ref(false)
const claiming = ref(false)

// Mock user statistics
const userStats = ref({
  totalGenerations: 1247,
  followers: 234,
  following: 189,
  likes: 892,
  thisMonth: 67,
  monthlyGoal: 100,
  successRate: 96
})

// Mock recent activities
const recentActivities = ref([
  {
    id: '1',
    title: '生成了产品摄影图片',
    time: '2小时前',
    icon: 'mdi-camera',
    color: 'primary'
  },
  {
    id: '2',
    title: '购买了UI设计模板',
    time: '5小时前',
    icon: 'mdi-shopping',
    color: 'success'
  },
  {
    id: '3',
    title: '分享了工作流',
    time: '1天前',
    icon: 'mdi-share',
    color: 'info'
  }
])

// Mock achievements
const achievements = ref([
  {
    id: '1',
    icon: 'mdi-fire',
    color: 'orange',
    earned: true,
    description: '连续生成7天'
  },
  {
    id: '2',
    icon: 'mdi-star',
    color: 'yellow',
    earned: true,
    description: '获得100个赞'
  },
  {
    id: '3',
    icon: 'mdi-trophy',
    color: 'gold',
    earned: false,
    description: '成为顶级创作者'
  },
  {
    id: '4',
    icon: 'mdi-heart',
    color: 'red',
    earned: true,
    description: '收获1000个关注'
  }
])

const formatDate = (d?: string | null) => {
  if (!d) return ''
  const date = new Date(d)
  return date.toLocaleDateString() + ' ' + date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
}

const claimDisabled = computed(() => claiming.value || !profile.value)

const load = async () => {
  loading.value = true
  try {
    profile.value = await getProfile()
  } catch (e) {
    notificationStore.error('Failed to load profile')
  } finally {
    loading.value = false
  }
}

const onClaimCredits = async () => {
  claiming.value = true
  try {
    profile.value = await claimCredits()
    notificationStore.success('Credits claimed')
  } catch (e) {
    notificationStore.error('Failed to claim credits')
  } finally {
    claiming.value = false
  }
}

const editProfile = () => {
  notificationStore.info('编辑资料功能开发中...')
}

onMounted(load)
</script>

<route lang="yaml">
meta:
  title: Profile
</route>
