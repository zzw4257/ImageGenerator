<template>
  <v-container fluid class="pa-8">
    <div class="text-center mb-8">
      <h1 class="text-h3 font-weight-bold mb-4">智能定价策略</h1>
      <p class="text-h6 text-grey-darken-1">动态定价 • 批量折扣 • 企业定制</p>
    </div>

    <!-- Pricing Strategy Toggle -->
    <v-row class="mb-8" justify="center">
      <v-col cols="12" md="8">
        <v-card variant="outlined" rounded="xl">
          <v-card-text class="pa-6">
            <v-btn-toggle v-model="pricingMode" mandatory color="primary" rounded="xl" class="w-100">
              <v-btn value="dynamic" size="large" class="flex-grow-1">
                <v-icon start>mdi-chart-line</v-icon>
                动态定价
              </v-btn>
              <v-btn value="bulk" size="large" class="flex-grow-1">
                <v-icon start>mdi-package-variant</v-icon>
                批量套餐
              </v-btn>
              <v-btn value="enterprise" size="large" class="flex-grow-1">
                <v-icon start>mdi-office-building</v-icon>
                企业定制
              </v-btn>
            </v-btn-toggle>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <!-- Dynamic Pricing -->
    <div v-if="pricingMode === 'dynamic'">
      <v-row class="mb-6">
        <v-col cols="12">
          <v-card elevation="2" rounded="xl">
            <v-card-title class="pa-6 pb-0">
              <div class="d-flex align-center">
                <v-icon class="mr-3" color="primary">mdi-robot</v-icon>
                <div>
                  <h2 class="text-h5 font-weight-bold">AI 智能定价</h2>
                  <p class="text-body-2 text-grey-darken-1 mt-1">基于市场需求和资源成本的实时定价</p>
                </div>
              </div>
            </v-card-title>
            <v-card-text class="pa-6">
              <v-row>
                <v-col cols="12" md="4">
                  <div class="text-center pa-4">
                    <v-icon size="48" color="success" class="mb-3">mdi-trending-down</v-icon>
                    <h3 class="text-h6 font-weight-bold text-success">低峰期折扣</h3>
                    <p class="text-body-2 text-grey-darken-1">00:00-08:00</p>
                    <v-chip color="success" variant="flat" class="mt-2">-20%</v-chip>
                  </div>
                </v-col>
                <v-col cols="12" md="4">
                  <div class="text-center pa-4">
                    <v-icon size="48" color="primary" class="mb-3">mdi-clock</v-icon>
                    <h3 class="text-h6 font-weight-bold">标准价格</h3>
                    <p class="text-body-2 text-grey-darken-1">08:00-18:00</p>
                    <v-chip color="primary" variant="flat" class="mt-2">基准价</v-chip>
                  </div>
                </v-col>
                <v-col cols="12" md="4">
                  <div class="text-center pa-4">
                    <v-icon size="48" color="warning" class="mb-3">mdi-trending-up</v-icon>
                    <h3 class="text-h6 font-weight-bold text-warning">高峰期加价</h3>
                    <p class="text-body-2 text-grey-darken-1">18:00-24:00</p>
                    <v-chip color="warning" variant="flat" class="mt-2">+15%</v-chip>
                  </div>
                </v-col>
              </v-row>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>

      <!-- Real-time Pricing -->
      <v-row>
        <v-col v-for="provider in dynamicPricing" :key="provider.name" cols="12" md="6" lg="3">
          <v-card elevation="2" rounded="xl" class="h-100">
            <v-card-text class="pa-6">
              <div class="d-flex align-center justify-space-between mb-4">
                <h3 class="text-h6 font-weight-bold">{{ provider.name }}</h3>
                <v-chip 
                  :color="provider.status === 'optimal' ? 'success' : provider.status === 'busy' ? 'warning' : 'error'"
                  size="small"
                  variant="flat"
                >
                  {{ getStatusText(provider.status) }}
                </v-chip>
              </div>
              
              <div class="mb-4">
                <div class="d-flex justify-space-between align-center mb-2">
                  <span class="text-body-2">当前价格</span>
                  <span class="text-h5 font-weight-bold text-primary">{{ provider.currentPrice }}</span>
                </div>
                <div class="d-flex justify-space-between align-center mb-2">
                  <span class="text-caption text-grey">基准价格</span>
                  <span class="text-caption text-decoration-line-through">{{ provider.basePrice }}</span>
                </div>
                <div class="d-flex justify-space-between align-center">
                  <span class="text-caption text-grey">节省</span>
                  <span class="text-caption text-success font-weight-medium">
                    {{ calculateSavings(provider.basePrice, provider.currentPrice) }}
                  </span>
                </div>
              </div>

              <v-progress-linear
                :model-value="provider.load"
                :color="provider.load > 80 ? 'error' : provider.load > 60 ? 'warning' : 'success'"
                height="8"
                rounded
                class="mb-2"
              />
              <p class="text-caption text-grey text-center">负载: {{ provider.load }}%</p>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>
    </div>

    <!-- Bulk Packages -->
    <div v-if="pricingMode === 'bulk'">
      <v-row>
        <v-col v-for="(pkg, index) in bulkPackages" :key="pkg.id" cols="12" md="6" lg="4">
          <v-card 
            elevation="2" 
            rounded="xl" 
            class="h-100 position-relative"
            :class="{ 'border-primary': pkg.popular }"
            :style="pkg.popular ? 'border: 2px solid rgb(var(--v-theme-primary))' : ''"
          >
            <v-chip
              v-if="pkg.popular"
              color="primary"
              class="position-absolute popular-badge"
              variant="flat"
            >
              最受欢迎
            </v-chip>

            <v-card-text class="pa-6 text-center">
              <v-icon :color="pkg.color" size="48" class="mb-4">{{ pkg.icon }}</v-icon>
              <h3 class="text-h5 font-weight-bold mb-2">{{ pkg.name }}</h3>
              <p class="text-body-2 text-grey-darken-1 mb-4">{{ pkg.description }}</p>
              
              <div class="mb-4">
                <div class="d-flex align-center justify-center mb-2">
                  <span class="text-h3 font-weight-bold text-primary">{{ pkg.credits }}</span>
                  <span class="text-h6 ml-2">Credits</span>
                </div>
                <div class="d-flex align-center justify-center">
                  <span class="text-h5 font-weight-bold">¥{{ pkg.price }}</span>
                  <span v-if="pkg.originalPrice" class="text-body-2 text-decoration-line-through text-grey ml-2">
                    ¥{{ pkg.originalPrice }}
                  </span>
                </div>
                <p class="text-caption text-success mt-1">
                  单价: ¥{{ (pkg.price / pkg.credits).toFixed(3) }}/Credit
                </p>
              </div>

              <v-list class="mb-4" density="compact">
                <v-list-item v-for="feature in pkg.features" :key="feature" class="px-0">
                  <template #prepend>
                    <v-icon color="success" size="16">mdi-check</v-icon>
                  </template>
                  <v-list-item-title class="text-body-2">{{ feature }}</v-list-item-title>
                </v-list-item>
              </v-list>

              <v-btn
                :color="pkg.popular ? 'primary' : 'default'"
                :variant="pkg.popular ? 'flat' : 'outlined'"
                size="large"
                rounded="xl"
                class="w-100"
                @click="purchasePackage(pkg)"
              >
                立即购买
              </v-btn>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>
    </div>

    <!-- Enterprise Solutions -->
    <div v-if="pricingMode === 'enterprise'">
      <v-row>
        <v-col cols="12" lg="8">
          <v-card elevation="2" rounded="xl">
            <v-card-title class="pa-6 pb-0">
              <div class="d-flex align-center">
                <v-icon class="mr-3" color="primary">mdi-office-building</v-icon>
                <div>
                  <h2 class="text-h5 font-weight-bold">企业定制方案</h2>
                  <p class="text-body-2 text-grey-darken-1 mt-1">为您的团队量身定制的 AIGC 解决方案</p>
                </div>
              </div>
            </v-card-title>
            <v-card-text class="pa-6">
              <v-row>
                <v-col v-for="solution in enterpriseSolutions" :key="solution.id" cols="12" md="6">
                  <v-card variant="outlined" rounded="lg" class="h-100">
                    <v-card-text class="pa-4">
                      <div class="d-flex align-center mb-3">
                        <v-icon :color="solution.color" class="mr-2">{{ solution.icon }}</v-icon>
                        <h4 class="text-h6 font-weight-bold">{{ solution.name }}</h4>
                      </div>
                      <p class="text-body-2 text-grey-darken-1 mb-3">{{ solution.description }}</p>
                      <v-list density="compact">
                        <v-list-item v-for="feature in solution.features" :key="feature" class="px-0 py-1">
                          <template #prepend>
                            <v-icon color="success" size="14">mdi-check-circle</v-icon>
                          </template>
                          <v-list-item-title class="text-body-2">{{ feature }}</v-list-item-title>
                        </v-list-item>
                      </v-list>
                    </v-card-text>
                  </v-card>
                </v-col>
              </v-row>
            </v-card-text>
          </v-card>
        </v-col>

        <v-col cols="12" lg="4">
          <v-card elevation="2" rounded="xl" color="primary" class="text-white">
            <v-card-text class="pa-6 text-center">
              <v-icon size="64" class="mb-4">mdi-handshake</v-icon>
              <h3 class="text-h5 font-weight-bold mb-4">联系我们</h3>
              <p class="text-body-1 mb-6 opacity-90">
                我们的企业解决方案专家将为您提供个性化的定价方案和技术支持
              </p>
              
              <v-btn
                color="white"
                variant="flat"
                size="large"
                rounded="xl"
                class="mb-4 w-100"
                @click="contactSales"
              >
                <v-icon start>mdi-phone</v-icon>
                联系销售
              </v-btn>
              
              <v-btn
                color="white"
                variant="outlined"
                size="large"
                rounded="xl"
                class="w-100"
                @click="requestDemo"
              >
                <v-icon start>mdi-play-circle</v-icon>
                预约演示
              </v-btn>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>
    </div>

    <!-- Pricing Calculator -->
    <v-row class="mt-8">
      <v-col cols="12">
        <v-card elevation="2" rounded="xl">
          <v-card-title class="pa-6 pb-0">
            <div class="d-flex align-center">
              <v-icon class="mr-3" color="primary">mdi-calculator</v-icon>
              <div>
                <h2 class="text-h5 font-weight-bold">成本计算器</h2>
                <p class="text-body-2 text-grey-darken-1 mt-1">预估您的使用成本</p>
              </div>
            </div>
          </v-card-title>
          <v-card-text class="pa-6">
            <v-row>
              <v-col cols="12" md="6">
                <v-form>
                  <v-row>
                    <v-col cols="12" md="6">
                      <v-text-field
                        v-model.number="calculator.dailyGenerations"
                        label="每日生成次数"
                        type="number"
                        variant="outlined"
                        min="1"
                      />
                    </v-col>
                    <v-col cols="12" md="6">
                      <v-select
                        v-model="calculator.primaryProvider"
                        :items="providerOptions"
                        label="主要使用模型"
                        variant="outlined"
                      />
                    </v-col>
                    <v-col cols="12" md="6">
                      <v-select
                        v-model="calculator.resolution"
                        :items="resolutionOptions"
                        label="图片分辨率"
                        variant="outlined"
                      />
                    </v-col>
                    <v-col cols="12" md="6">
                      <v-select
                        v-model="calculator.teamSize"
                        :items="teamSizeOptions"
                        label="团队规模"
                        variant="outlined"
                      />
                    </v-col>
                  </v-row>
                </v-form>
              </v-col>
              <v-col cols="12" md="6">
                <v-card variant="tonal" color="primary" rounded="lg">
                  <v-card-text class="pa-4">
                    <h4 class="text-h6 font-weight-bold mb-4">预估成本</h4>
                    <div class="mb-3">
                      <div class="d-flex justify-space-between align-center mb-1">
                        <span class="text-body-2">每日成本</span>
                        <span class="font-weight-medium">{{ calculatedCost.daily }} Credits</span>
                      </div>
                      <div class="d-flex justify-space-between align-center mb-1">
                        <span class="text-body-2">每月成本</span>
                        <span class="font-weight-medium">{{ calculatedCost.monthly }} Credits</span>
                      </div>
                      <div class="d-flex justify-space-between align-center mb-1">
                        <span class="text-body-2">年度成本</span>
                        <span class="font-weight-medium">{{ calculatedCost.yearly }} Credits</span>
                      </div>
                    </div>
                    <v-divider class="my-3" />
                    <div class="d-flex justify-space-between align-center">
                      <span class="text-h6 font-weight-bold">建议套餐</span>
                      <span class="text-h6 font-weight-bold text-primary">{{ recommendedPackage }}</span>
                    </div>
                  </v-card-text>
                </v-card>
              </v-col>
            </v-row>
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

// State
const pricingMode = ref('dynamic')

// Dynamic Pricing Data
const dynamicPricing = ref([
  {
    name: 'Qwen',
    currentPrice: '2.4 Credits',
    basePrice: '3.0 Credits',
    load: 45,
    status: 'optimal'
  },
  {
    name: 'Flux',
    currentPrice: '1.6 Credits',
    basePrice: '2.0 Credits',
    load: 68,
    status: 'busy'
  },
  {
    name: 'OpenAI',
    currentPrice: '4.5 Credits',
    basePrice: '4.0 Credits',
    load: 85,
    status: 'busy'
  },
  {
    name: 'Stub',
    currentPrice: '0 Credits',
    basePrice: '0 Credits',
    load: 15,
    status: 'optimal'
  }
])

// Bulk Packages
const bulkPackages = ref([
  {
    id: 'starter',
    name: '入门套餐',
    description: '适合个人用户和小团队',
    credits: 100,
    price: 29,
    originalPrice: null,
    color: 'primary',
    icon: 'mdi-rocket-launch',
    popular: false,
    features: [
      '100 Credits',
      '支持所有模型',
      '标准技术支持',
      '7天退款保证'
    ]
  },
  {
    id: 'professional',
    name: '专业套餐',
    description: '适合中小企业和专业团队',
    credits: 500,
    price: 129,
    originalPrice: 150,
    color: 'success',
    icon: 'mdi-briefcase',
    popular: true,
    features: [
      '500 Credits',
      '支持所有模型',
      '优先技术支持',
      '批量折扣',
      '使用分析报告',
      '30天退款保证'
    ]
  },
  {
    id: 'enterprise',
    name: '企业套餐',
    description: '适合大型企业和高频用户',
    credits: 2000,
    price: 449,
    originalPrice: 600,
    color: 'warning',
    icon: 'mdi-office-building',
    popular: false,
    features: [
      '2000 Credits',
      '支持所有模型',
      '专属客户经理',
      '最大批量折扣',
      '详细分析报告',
      'API 优先级',
      '90天退款保证'
    ]
  }
])

// Enterprise Solutions
const enterpriseSolutions = ref([
  {
    id: 'private-cloud',
    name: '私有云部署',
    description: '在您的基础设施上部署 Aetherflow',
    color: 'primary',
    icon: 'mdi-cloud-lock',
    features: [
      '完全数据隔离',
      '自定义模型集成',
      '本地化部署',
      '7x24 技术支持'
    ]
  },
  {
    id: 'api-integration',
    name: 'API 深度集成',
    description: '将 AIGC 能力无缝集成到您的系统',
    color: 'success',
    icon: 'mdi-api',
    features: [
      'RESTful API',
      'SDK 支持',
      'Webhook 回调',
      '技术文档支持'
    ]
  },
  {
    id: 'custom-model',
    name: '定制模型训练',
    description: '基于您的数据训练专属 AI 模型',
    color: 'warning',
    icon: 'mdi-brain',
    features: [
      '专属模型训练',
      '数据安全保障',
      '模型性能优化',
      '持续迭代更新'
    ]
  },
  {
    id: 'managed-service',
    name: '托管服务',
    description: '全托管的 AIGC 服务解决方案',
    color: 'info',
    icon: 'mdi-shield-check',
    features: [
      '全托管运维',
      '自动扩缩容',
      '监控告警',
      'SLA 保障'
    ]
  }
])

// Calculator
const calculator = ref({
  dailyGenerations: 50,
  primaryProvider: 'Qwen',
  resolution: '1024x1024',
  teamSize: '1-5人'
})

const providerOptions = [
  { title: 'Qwen (推荐)', value: 'Qwen' },
  { title: 'Flux', value: 'Flux' },
  { title: 'OpenAI', value: 'OpenAI' },
  { title: 'Stub (测试)', value: 'Stub' }
]

const resolutionOptions = [
  { title: '512x512', value: '512x512' },
  { title: '1024x1024 (推荐)', value: '1024x1024' },
  { title: '1024x768', value: '1024x768' },
  { title: '768x1024', value: '768x1024' }
]

const teamSizeOptions = [
  { title: '1-5人', value: '1-5人' },
  { title: '6-20人', value: '6-20人' },
  { title: '21-50人', value: '21-50人' },
  { title: '50+人', value: '50+人' }
]

// Computed
const calculatedCost = computed(() => {
  const providerCosts: Record<string, number> = {
    'Qwen': 2.5,
    'Flux': 1.8,
    'OpenAI': 4.2,
    'Stub': 0
  }
  
  const baseCost = providerCosts[calculator.value.primaryProvider] || 2.5
  const resolutionMultiplier = calculator.value.resolution === '1024x1024' ? 1.2 : 1.0
  const teamMultiplier = calculator.value.teamSize === '50+人' ? 0.8 : 1.0
  
  const dailyCost = Math.round(calculator.value.dailyGenerations * baseCost * resolutionMultiplier * teamMultiplier)
  
  return {
    daily: dailyCost,
    monthly: dailyCost * 30,
    yearly: dailyCost * 365
  }
})

const recommendedPackage = computed(() => {
  const monthlyCost = calculatedCost.value.monthly
  if (monthlyCost <= 100) return '入门套餐'
  if (monthlyCost <= 500) return '专业套餐'
  return '企业套餐'
})

// Methods
function getStatusText(status: string): string {
  const statusMap: Record<string, string> = {
    'optimal': '最优',
    'busy': '繁忙',
    'overload': '超载'
  }
  return statusMap[status] || status
}

function calculateSavings(basePrice: string, currentPrice: string): string {
  const base = parseFloat(basePrice.replace(' Credits', ''))
  const current = parseFloat(currentPrice.replace(' Credits', ''))
  if (base === 0) return '免费'
  const savings = ((base - current) / base * 100).toFixed(0)
  return `${savings}%`
}

function purchasePackage(pkg: any) {
  notificationStore.info(`正在跳转到 ${pkg.name} 购买页面...`)
  // TODO: Implement purchase flow
}

function contactSales() {
  notificationStore.info('正在连接销售团队...')
  // TODO: Implement contact sales
}

function requestDemo() {
  notificationStore.info('正在预约产品演示...')
  // TODO: Implement demo request
}
</script>

<route lang="yaml">
meta:
  title: 定价策略
</route>

<style scoped>
.popular-badge {
  top: -10px;
  right: 20px;
  z-index: 1;
}
</style>
