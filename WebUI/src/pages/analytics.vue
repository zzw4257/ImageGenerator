<template>
  <v-container fluid class="pa-8">
    <div class="d-flex justify-space-between align-center mb-6">
      <div>
        <h1 class="text-h4 font-weight-bold">数据分析仪表盘</h1>
        <p class="text-body-1 text-grey-darken-1 mt-2">深度洞察您的 AIGC 使用数据和市场趋势</p>
      </div>
      <div class="d-flex gap-2">
        <v-btn variant="outlined" prepend-icon="mdi-download">
          导出报告
        </v-btn>
        <v-btn color="primary" prepend-icon="mdi-refresh" @click="refreshData">
          刷新数据
        </v-btn>
      </div>
    </div>

    <!-- Time Range Selector -->
    <v-card class="mb-6" variant="outlined">
      <v-card-text class="pa-4">
        <v-row align="center">
          <v-col cols="12" md="6">
            <v-btn-toggle v-model="timeRange" mandatory color="primary" rounded="xl">
              <v-btn value="7d">近7天</v-btn>
              <v-btn value="30d">近30天</v-btn>
              <v-btn value="90d">近90天</v-btn>
              <v-btn value="1y">近1年</v-btn>
            </v-btn-toggle>
          </v-col>
          <v-col cols="12" md="6" class="text-right">
            <span class="text-caption text-grey">最后更新：{{ lastUpdated }}</span>
          </v-col>
        </v-row>
      </v-card-text>
    </v-card>

    <!-- Key Metrics -->
    <v-row class="mb-6">
      <v-col v-for="metric in keyMetrics" :key="metric.title" cols="12" sm="6" md="3">
        <v-card class="pa-4" :color="metric.color" variant="tonal">
          <div class="d-flex align-center justify-space-between">
            <div>
              <p class="text-caption text-grey-darken-1 mb-1">{{ metric.title }}</p>
              <h2 class="text-h4 font-weight-bold">{{ metric.value }}</h2>
              <div class="d-flex align-center mt-2">
                <v-icon 
                  :color="metric.trend > 0 ? 'success' : 'error'" 
                  size="16"
                >
                  {{ metric.trend > 0 ? 'mdi-trending-up' : 'mdi-trending-down' }}
                </v-icon>
                <span 
                  class="text-caption ml-1"
                  :class="metric.trend > 0 ? 'text-success' : 'text-error'"
                >
                  {{ Math.abs(metric.trend) }}%
                </span>
              </div>
            </div>
            <v-icon :color="metric.color" size="40">{{ metric.icon }}</v-icon>
          </div>
        </v-card>
      </v-col>
    </v-row>

    <v-row>
      <!-- Prompt Popularity Chart -->
      <v-col cols="12" lg="8">
        <v-card elevation="2" rounded="xl">
          <v-card-title class="d-flex align-center">
            <v-icon class="mr-2">mdi-chart-line</v-icon>
            提示词热度趋势
          </v-card-title>
          <v-divider />
          <v-card-text class="pa-6">
            <div class="chart-container" style="height: 300px;">
              <div class="d-flex align-center justify-center h-100">
                <div class="text-center">
                  <v-icon size="80" color="grey-lighten-2" class="mb-4">mdi-chart-line</v-icon>
                  <p class="text-body-2 text-grey">提示词热度趋势图占位</p>
                </div>
              </div>
            </div>
          </v-card-text>
        </v-card>
      </v-col>

      <!-- Top Prompts -->
      <v-col cols="12" lg="4">
        <v-card elevation="2" rounded="xl">
          <v-card-title class="d-flex align-center">
            <v-icon class="mr-2">mdi-fire</v-icon>
            热门提示词
          </v-card-title>
          <v-divider />
          <v-card-text class="pa-0">
            <v-list>
              <v-list-item
                v-for="(prompt, index) in topPrompts"
                :key="prompt.id"
                class="px-4 py-3"
              >
                <template #prepend>
                  <v-chip
                    :color="index < 3 ? 'primary' : 'grey'"
                    size="small"
                    variant="flat"
                  >
                    {{ index + 1 }}
                  </v-chip>
                </template>
                <v-list-item-title class="text-body-2 font-weight-medium">
                  {{ truncateText(prompt.text, 30) }}
                </v-list-item-title>
                <v-list-item-subtitle>
                  {{ prompt.category }} • {{ prompt.usage }}次使用
                </v-list-item-subtitle>
                <template #append>
                  <v-chip size="x-small" variant="outlined">
                    {{ prompt.successRate }}%
                  </v-chip>
                </template>
              </v-list-item>
            </v-list>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <v-row class="mt-6">
      <!-- API Performance -->
      <v-col cols="12" md="6">
        <v-card elevation="2" rounded="xl">
          <v-card-title class="d-flex align-center">
            <v-icon class="mr-2">mdi-speedometer</v-icon>
            API 性能监控
          </v-card-title>
          <v-divider />
          <v-card-text class="pa-6">
            <div class="mb-4">
              <div class="d-flex justify-space-between align-center mb-2">
                <span class="text-body-2">平均响应时间</span>
                <span class="font-weight-bold">{{ apiMetrics.avgResponseTime }}ms</span>
              </div>
              <v-progress-linear
                :model-value="(apiMetrics.avgResponseTime / 5000) * 100"
                color="primary"
                height="8"
                rounded
              />
            </div>

            <div class="mb-4">
              <div class="d-flex justify-space-between align-center mb-2">
                <span class="text-body-2">成功率</span>
                <span class="font-weight-bold text-success">{{ apiMetrics.successRate }}%</span>
              </div>
              <v-progress-linear
                :model-value="apiMetrics.successRate"
                color="success"
                height="8"
                rounded
              />
            </div>

            <div class="mb-4">
              <div class="d-flex justify-space-between align-center mb-2">
                <span class="text-body-2">错误率</span>
                <span class="font-weight-bold text-error">{{ apiMetrics.errorRate }}%</span>
              </div>
              <v-progress-linear
                :model-value="apiMetrics.errorRate"
                color="error"
                height="8"
                rounded
              />
            </div>

            <!-- Provider Performance -->
            <v-divider class="my-4" />
            <h4 class="text-h6 mb-3">Provider 性能对比</h4>
            <div v-for="provider in providerMetrics" :key="provider.name" class="mb-3">
              <div class="d-flex justify-space-between align-center mb-1">
                <span class="text-body-2">{{ provider.name }}</span>
                <span class="text-caption">{{ provider.responseTime }}ms</span>
              </div>
              <v-progress-linear
                :model-value="(provider.responseTime / 3000) * 100"
                :color="getProviderColor(provider.name)"
                height="6"
                rounded
              />
            </div>
          </v-card-text>
        </v-card>
      </v-col>

      <!-- Revenue Analytics -->
      <v-col cols="12" md="6">
        <v-card elevation="2" rounded="xl">
          <v-card-title class="d-flex align-center">
            <v-icon class="mr-2">mdi-currency-usd</v-icon>
            收益分析
          </v-card-title>
          <v-divider />
          <v-card-text class="pa-6">
            <!-- Revenue Summary -->
            <div class="text-center mb-4">
              <h2 class="text-h3 font-weight-bold text-primary">{{ revenueMetrics.total }}</h2>
              <p class="text-body-2 text-grey">总收益 (Credits)</p>
            </div>

            <!-- Revenue Breakdown -->
            <div class="mb-4">
              <h4 class="text-h6 mb-3">收益来源</h4>
              <div v-for="source in revenueMetrics.sources" :key="source.name" class="mb-3">
                <div class="d-flex justify-space-between align-center mb-1">
                  <span class="text-body-2">{{ source.name }}</span>
                  <span class="font-weight-medium">{{ source.amount }} Credits</span>
                </div>
                <v-progress-linear
                  :model-value="(source.amount / revenueMetrics.total) * 100"
                  :color="source.color"
                  height="8"
                  rounded
                />
              </div>
            </div>

            <!-- Transaction Fees -->
            <v-divider class="my-4" />
            <div class="d-flex justify-space-between align-center">
              <span class="text-body-2">平台抽佣</span>
              <div class="text-right">
                <div class="font-weight-bold">{{ revenueMetrics.platformFee }} Credits</div>
                <div class="text-caption text-grey">{{ revenueMetrics.feeRate }}% 费率</div>
              </div>
            </div>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <v-row class="mt-6">
      <!-- User Behavior -->
      <v-col cols="12" lg="6">
        <v-card elevation="2" rounded="xl">
          <v-card-title class="d-flex align-center">
            <v-icon class="mr-2">mdi-account-group</v-icon>
            用户行为分析
          </v-card-title>
          <v-divider />
          <v-card-text class="pa-6">
            <div class="chart-container" style="height: 250px;">
              <canvas ref="userBehaviorChart"></canvas>
            </div>
          </v-card-text>
        </v-card>
      </v-col>

      <!-- Market Insights -->
      <v-col cols="12" lg="6">
        <v-card elevation="2" rounded="xl">
          <v-card-title class="d-flex align-center">
            <v-icon class="mr-2">mdi-lightbulb</v-icon>
            市场洞察
          </v-card-title>
          <v-divider />
          <v-card-text class="pa-6">
            <v-alert
              v-for="insight in marketInsights"
              :key="insight.id"
              :type="insight.type as 'success' | 'error' | 'warning' | 'info'"
              variant="tonal"
              class="mb-2"
            >
              <div class="d-flex align-center">
                <v-icon class="mr-2">{{ insight.icon }}</v-icon>
                <div>
                  <div class="font-weight-medium">{{ insight.title }}</div>
                  <div class="text-body-2">{{ insight.description }}</div>
                </div>
              </div>
            </v-alert>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts" setup>
import { ref, onMounted, nextTick } from 'vue'
import { useNotificationStore } from '@/stores/notification'

const notificationStore = useNotificationStore()

// State
const timeRange = ref('30d')
const lastUpdated = ref(new Date().toLocaleString('zh-CN'))
const promptTrendChart = ref<HTMLCanvasElement>()
const userBehaviorChart = ref<HTMLCanvasElement>()

// Key Metrics
const keyMetrics = ref([
  {
    title: '总生成次数',
    value: '12,543',
    trend: 15.3,
    icon: 'mdi-image-multiple',
    color: 'primary'
  },
  {
    title: '活跃用户',
    value: '2,847',
    trend: 8.7,
    icon: 'mdi-account-group',
    color: 'success'
  },
  {
    title: '平台收益',
    value: '45,231',
    trend: 23.1,
    icon: 'mdi-currency-usd',
    color: 'warning'
  },
  {
    title: '成功率',
    value: '94.2%',
    trend: -2.1,
    icon: 'mdi-check-circle',
    color: 'info'
  }
])

// Top Prompts
const topPrompts = ref([
  {
    id: '1',
    text: 'A professional product photo of wireless earbuds on white background',
    category: '产品摄影',
    usage: 1247,
    successRate: 96
  },
  {
    id: '2',
    text: 'Minimalist logo design for tech startup',
    category: '设计',
    usage: 892,
    successRate: 94
  },
  {
    id: '3',
    text: 'Anime character in cyberpunk style',
    category: '插画',
    usage: 756,
    successRate: 91
  },
  {
    id: '4',
    text: 'Modern UI interface mockup',
    category: 'UI设计',
    usage: 634,
    successRate: 89
  },
  {
    id: '5',
    text: 'Realistic portrait photography',
    category: '人像',
    usage: 523,
    successRate: 87
  }
])

// API Metrics
const apiMetrics = ref({
  avgResponseTime: 1247,
  successRate: 94.2,
  errorRate: 5.8
})

// Provider Metrics
const providerMetrics = ref([
  { name: 'Qwen', responseTime: 1100 },
  { name: 'Flux', responseTime: 1350 },
  { name: 'OpenAI', responseTime: 2100 },
  { name: 'Stub', responseTime: 50 }
])

// Revenue Metrics
const revenueMetrics = ref({
  total: 45231,
  platformFee: 4523,
  feeRate: 10,
  sources: [
    { name: 'API 调用', amount: 28500, color: 'primary' },
    { name: '市场交易', amount: 12400, color: 'success' },
    { name: '订阅服务', amount: 4331, color: 'warning' }
  ]
})

// Market Insights
const marketInsights = ref([
  {
    id: '1',
    type: 'success',
    icon: 'mdi-trending-up',
    title: '产品摄影需求激增',
    description: '产品摄影类提示词使用量较上月增长 45%，建议增加相关模板'
  },
  {
    id: '2',
    type: 'info',
    icon: 'mdi-clock',
    title: '峰值时段优化建议',
    description: '14:00-16:00 为使用高峰期，建议在此时段提供更多计算资源'
  },
  {
    id: '3',
    type: 'warning',
    icon: 'mdi-alert',
    title: 'API 响应时间波动',
    description: 'OpenAI Provider 响应时间较不稳定，建议考虑负载均衡策略'
  }
])

// Methods
function truncateText(text: string, length: number): string {
  return text.length > length ? text.substring(0, length) + '...' : text
}

function getProviderColor(provider: string): string {
  const colors: Record<string, string> = {
    'Qwen': 'primary',
    'Flux': 'success',
    'OpenAI': 'warning',
    'Stub': 'info'
  }
  return colors[provider] || 'grey'
}

async function refreshData() {
  try {
    // Mock API call
    await new Promise(resolve => setTimeout(resolve, 1000))
    lastUpdated.value = new Date().toLocaleString('zh-CN')
    notificationStore.success('数据已刷新')
  } catch (error) {
    notificationStore.error('刷新失败')
  }
}

function initCharts() {
  nextTick(() => {
    // Mock chart initialization
    // In real implementation, use Chart.js or similar library
    if (promptTrendChart.value) {
      const ctx = promptTrendChart.value.getContext('2d')
      if (ctx) {
        ctx.fillStyle = '#1976d2'
        ctx.fillRect(10, 10, 100, 50)
        ctx.fillStyle = '#ffffff'
        ctx.font = '14px Arial'
        ctx.fillText('提示词趋势图', 20, 35)
      }
    }

    if (userBehaviorChart.value) {
      const ctx = userBehaviorChart.value.getContext('2d')
      if (ctx) {
        ctx.fillStyle = '#4caf50'
        ctx.fillRect(10, 10, 100, 50)
        ctx.fillStyle = '#ffffff'
        ctx.font = '14px Arial'
        ctx.fillText('用户行为图', 20, 35)
      }
    }
  })
}

onMounted(() => {
  initCharts()
})
</script>

<route lang="yaml">
meta:
  title: 数据分析
</route>

<style scoped>
.chart-container {
  position: relative;
  width: 100%;
}

.chart-container canvas {
  width: 100% !important;
  height: 100% !important;
}
</style>
