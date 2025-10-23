<template>
  <v-container class="py-6">
    <v-row>
      <v-col cols="12">
        <h1 class="text-h4 font-weight-bold mb-2">账户与历史</h1>
        <p class="text-body-1 text-grey-darken-1">管理您的余额和查看历史记录</p>
      </v-col>
    </v-row>

    <!-- 余额卡片 -->
    <v-row class="mb-6">
      <v-col cols="12" md="6">
        <v-card class="pa-6 text-white" color="primary" rounded="xl" variant="flat">
          <div class="d-flex align-center justify-space-between mb-4">
            <div>
              <h2 class="text-h5 font-weight-bold">钱包余额</h2>
              <p class="text-body-2 opacity-75">当前可用的 Credits</p>
            </div>
            <v-icon color="white" size="48">mdi-wallet</v-icon>
          </div>

          <div class="d-flex align-end justify-space-between">
            <div class="d-flex align-end">
              <span class="text-h3 font-weight-bold mr-2">{{ walletBalance }}</span>
              <span class="text-h6 font-weight-medium mb-2">Credits</span>
            </div>

            <v-btn
              color="white"
              :loading="granting"
              variant="flat"
              @click="grantCredits"
            >
              <v-icon start>mdi-plus</v-icon>
              一键发放
            </v-btn>
          </div>
        </v-card>
      </v-col>
    </v-row>

    <v-row>
      <!-- 交易流水 -->
      <v-col class="mb-6" cols="12" lg="6">
        <v-card elevation="2" rounded="xl">
          <v-card-title class="d-flex align-center">
            <v-icon class="mr-2">mdi-credit-card-outline</v-icon>
            交易流水
          </v-card-title>
          <v-divider />
          <v-card-text class="pa-0">
            <v-list>
              <v-list-item
                v-for="transaction in transactions"
                :key="transaction.id"
                class="px-4 py-3"
              >
                <template #prepend>
                  <v-avatar
                    :color="getTransactionColor(transaction.type)"
                    rounded
                    size="40"
                    variant="flat"
                  >
                    <v-icon :color="getTransactionIconColor(transaction.type)">
                      {{ getTransactionIcon(transaction.type) }}
                    </v-icon>
                  </v-avatar>
                </template>

                <v-list-item-title class="text-body-1 font-weight-medium">
                  {{ getTransactionDescription(transaction.type, transaction.description) }}
                </v-list-item-title>

                <v-list-item-subtitle class="d-flex align-center mt-1">
                  <span class="text-caption text-grey">
                    {{ formatDate(transaction.createdAt) }}
                  </span>
                </v-list-item-subtitle>

                <template #append>
                  <span
                    :class="[
                      'text-body-1 font-weight-bold',
                      transaction.type === 'Recharge' || transaction.type === 'Earn' ? 'text-success' : 'text-error'
                    ]"
                  >
                    {{ transaction.type === 'Recharge' || transaction.type === 'Earn' ? '+' : '-' }}{{ transaction.amount }}
                  </span>
                </template>
              </v-list-item>
            </v-list>
          </v-card-text>

          <v-card-actions class="pa-4">
            <v-pagination
              v-model="transactionPage"
              :length="transactionTotalPages"
              rounded="circle"
              total-visible="5"
              @update:modelValue="handleTransactionPageChange"
            />
          </v-card-actions>
        </v-card>
      </v-col>

      <!-- 生成历史 -->
      <v-col cols="12" lg="6">
        <v-card elevation="2" rounded="xl">
          <v-card-title class="d-flex align-center justify-space-between">
            <div class="d-flex align-center">
              <v-icon class="mr-2">mdi-history</v-icon>
              生成历史
              <v-chip
                v-if="hasActiveFilters"
                color="primary"
                size="small"
                class="ml-2"
              >
                已筛选
              </v-chip>
            </div>
            <div class="d-flex align-center">
              <v-btn
                v-if="hasActiveFilters"
                icon
                size="small"
                variant="text"
                @click="resetFilters"
                class="mr-1"
              >
                <v-icon>mdi-close</v-icon>
              </v-btn>
              <v-btn
                icon
                size="small"
                variant="text"
                @click="showFilterDialog = true"
              >
                <v-icon>mdi-filter</v-icon>
              </v-btn>
            </div>
          </v-card-title>
          <v-divider />
          <v-card-text class="pa-0">
            <v-list>
              <v-list-item
                v-for="task in currentHistory"
                :key="task.id"
                class="px-4 py-3"
              >
                <template #prepend>
                  <v-avatar rounded size="48">
                    <v-img :src="task.thumbnail" />
                  </v-avatar>
                </template>

                <v-list-item-title class="text-body-1 font-weight-medium mb-1">
                  {{ truncatePrompt(task.prompt) }}
                </v-list-item-title>

                <v-list-item-subtitle class="d-flex align-center flex-wrap gap-2">
                  <v-chip
                    :color="getStatusColor(task.status)"
                    size="x-small"
                    variant="flat"
                  >
                    {{ getStatusText(task.status) }}
                  </v-chip>
                  <span class="text-caption text-grey">
                    {{ formatDateTime(task.createdAt) }}
                  </span>
                  <span class="text-caption font-weight-medium">
                    • {{ task.credits || 0 }} Credits
                  </span>
                </v-list-item-subtitle>

                <template #append>
                  <v-btn
                    icon
                    size="small"
                    variant="text"
                    @click="viewResult(task)"
                  >
                    <v-icon>mdi-eye-outline</v-icon>
                  </v-btn>
                </template>
              </v-list-item>
            </v-list>
          </v-card-text>

          <v-card-actions class="pa-4">
            <v-pagination
              v-model="currentHistoryPage"
              :length="currentHistoryTotalPages"
              rounded="circle"
              total-visible="5"
            />
          </v-card-actions>
        </v-card>
      </v-col>
    </v-row>

    <!-- 筛选对话框 -->
    <v-dialog v-model="showFilterDialog" max-width="500">
      <v-card rounded="xl">
        <v-card-title class="d-flex align-center">
          <v-icon class="mr-2">mdi-filter</v-icon>
          筛选历史记录
        </v-card-title>
        <v-divider />
        <v-card-text class="pa-6">
          <v-form>
            <!-- 状态筛选 -->
            <v-select
              v-model="filterOptions.status"
              :items="statusOptions"
              label="生成状态"
              variant="outlined"
              clearable
              class="mb-4"
            />

            <!-- Provider筛选 -->
            <v-select
              v-model="filterOptions.provider"
              :items="providerOptions"
              label="生成模型"
              variant="outlined"
              clearable
              class="mb-4"
            />

            <!-- 关键词搜索 -->
            <v-text-field
              v-model="filterOptions.keyword"
              label="搜索提示词"
              variant="outlined"
              clearable
              prepend-inner-icon="mdi-magnify"
              class="mb-4"
            />

            <!-- 时间范围 -->
            <v-row>
              <v-col cols="6">
                <v-text-field
                  v-model="filterOptions.dateFrom"
                  label="开始日期"
                  type="date"
                  variant="outlined"
                  clearable
                />
              </v-col>
              <v-col cols="6">
                <v-text-field
                  v-model="filterOptions.dateTo"
                  label="结束日期"
                  type="date"
                  variant="outlined"
                  clearable
                />
              </v-col>
            </v-row>
          </v-form>
        </v-card-text>
        <v-card-actions class="pa-6 pt-0">
          <v-spacer />
          <v-btn
            variant="text"
            @click="resetFilters"
          >
            重置
          </v-btn>
          <v-btn
            color="primary"
            variant="flat"
            @click="applyFilters"
          >
            应用筛选
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-container>
</template>

<script lang="ts" setup>
  import { onMounted, ref, computed } from 'vue'
  import { getBalance, listTransactions, grantCredits as grantCreditsApi } from '@/services/wallet'
  import { useHistoryStore } from '@/stores/history'
  import { useNotificationStore } from '@/stores/notification'
  import type { TransactionDto } from '@/services/wallet'

  const walletBalance = ref(0)
  const granting = ref(false)
  const loadingBalance = ref(false)
  const loadingTransactions = ref(false)
  const loadingHistory = ref(false)

  const transactions = ref<TransactionDto[]>([])
  const generationHistory = ref<any[]>([])
  const allGenerationHistory = ref<any[]>([]) // 存储所有历史记录用于筛选

  const transactionPage = ref(1)
  const transactionTotalPages = ref(1)
  const historyPage = ref(1)
  const historyTotalPages = ref(1)
  const filteredHistoryPage = ref(1)
  const filteredHistoryPageSize = ref(10) // 与原始分页保持一致

  const historyStore = useHistoryStore()
  const notificationStore = useNotificationStore()

  // 筛选相关状态
  const showFilterDialog = ref(false)
  const filterOptions = ref({
    status: null as string | null,
    provider: null as string | null,
    keyword: '',
    dateFrom: '',
    dateTo: ''
  })

  // 筛选选项
  const statusOptions = [
    { title: '全部', value: null },
    { title: '成功', value: 2 }, // Completed
    { title: '失败', value: 3 }, // Failed
    { title: '处理中', value: 1 }, // Processing
    { title: '等待中', value: 0 }, // Pending
    { title: '已取消', value: 4 } // Cancelled
  ]

  const providerOptions = [
    { title: '全部', value: null },
    { title: 'Stub', value: 'Stub' },
    { title: 'Qwen', value: 'Qwen' },
    { title: 'Flux', value: 'Flux' },
    { title: 'OpenAI', value: 'OpenAI' },
    { title: 'Gemini', value: 'Gemini' }
  ]

  // 筛选后的历史记录
  const filteredHistory = ref<any[]>([])

  // 计算属性：是否有活跃的筛选条件
  const hasActiveFilters = computed(() => {
    return filterOptions.value.status !== null ||
           filterOptions.value.provider !== null ||
           filterOptions.value.keyword !== '' ||
           filterOptions.value.dateFrom !== '' ||
           filterOptions.value.dateTo !== ''
  })

  // 计算属性：当前显示的历史记录（筛选后或原始）
  const currentHistory = computed(() => {
    if (hasActiveFilters.value && filteredHistory.value.length > 0) {
      // 如果有筛选条件且有筛选结果，显示筛选后的分页数据
      // 使用与原始分页相同的页面大小（10条记录）
      const start = (filteredHistoryPage.value - 1) * 10
      const end = start + 10
      return filteredHistory.value.slice(start, end)
    } else {
      // 否则显示原始数据
      return generationHistory.value
    }
  })

  // 计算属性：当前分页的总页数
  const currentHistoryTotalPages = computed(() => {
    if (hasActiveFilters.value && filteredHistory.value.length > 0) {
      // 如果有筛选条件且有筛选结果，基于筛选结果计算页数
      // 使用与原始分页相同的页面大小（10条记录）
      return Math.ceil(filteredHistory.value.length / 10)
    } else {
      // 否则使用原始分页页数
      return historyTotalPages.value
    }
  })

  // 计算属性：当前分页页码
  const currentHistoryPage = computed({
    get() {
      if (hasActiveFilters.value && filteredHistory.value.length > 0) {
        return filteredHistoryPage.value
      } else {
        return historyPage.value
      }
    },
    set(value: number) {
      if (hasActiveFilters.value && filteredHistory.value.length > 0) {
        filteredHistoryPage.value = value
      } else {
        historyPage.value = value
        loadGenerationHistory()
      }
    }
  })

  // 筛选函数
  async function applyFilters() {
    // 如果没有加载所有历史记录，先加载
    if (allGenerationHistory.value.length === 0) {
      try {
        allGenerationHistory.value = await historyStore.fetchAllGenerations()
      } catch (error) {
        console.error('加载所有历史记录失败:', error)
        notificationStore.error('加载历史记录失败，无法进行筛选')
        return
      }
    }

    let filtered = [...allGenerationHistory.value]

    // 状态筛选
    if (filterOptions.value.status !== null) {
      filtered = filtered.filter(task => task.status === filterOptions.value.status)
    }

    // Provider筛选
    if (filterOptions.value.provider) {
      filtered = filtered.filter(task => task.provider === filterOptions.value.provider)
    }

    // 关键词搜索
    if (filterOptions.value.keyword) {
      const keyword = filterOptions.value.keyword.toLowerCase()
      filtered = filtered.filter(task => 
        task.prompt.toLowerCase().includes(keyword)
      )
    }

    // 时间范围筛选
    if (filterOptions.value.dateFrom) {
      const fromDate = new Date(filterOptions.value.dateFrom)
      filtered = filtered.filter(task => new Date(task.createdAt) >= fromDate)
    }

    if (filterOptions.value.dateTo) {
      const toDate = new Date(filterOptions.value.dateTo)
      toDate.setHours(23, 59, 59, 999) // 包含整天
      filtered = filtered.filter(task => new Date(task.createdAt) <= toDate)
    }

    filteredHistory.value = filtered
    filteredHistoryPage.value = 1 // 重置筛选分页到第一页
    showFilterDialog.value = false
    
    // 计算筛选结果的总页数
    const totalPages = Math.ceil(filtered.length / 10)
    notificationStore.success(`筛选完成，找到 ${filtered.length} 条记录，共 ${totalPages} 页`)
  }

  // 重置筛选
  function resetFilters() {
    filterOptions.value = {
      status: null,
      provider: null,
      keyword: '',
      dateFrom: '',
      dateTo: ''
    }
    filteredHistory.value = []
    allGenerationHistory.value = [] // 清空所有历史记录缓存
    filteredHistoryPage.value = 1 // 重置筛选分页到第一页
    showFilterDialog.value = false
    notificationStore.info('筛选条件已重置')
  }

  function getTransactionColor (type: string) {
    return type === 'Recharge' || type === 'Earn' ? 'green-lighten-5' : 'red-lighten-5'
  }

  function getTransactionIconColor (type: string) {
    return type === 'Recharge' || type === 'Earn' ? 'green' : 'red'
  }

  function getTransactionIcon (type: string) {
    const icons = {
      Recharge: 'mdi-plus-circle',
      Consume: 'mdi-minus-circle',
      Earn: 'mdi-plus-circle',
      Refund: 'mdi-refresh'
    }
    return icons[type as keyof typeof icons] || 'mdi-circle'
  }

  function getTransactionDescription(type: string, description?: string) {
    const descriptions = {
      Recharge: 'Credits 充值',
      Consume: '图像生成消费',
      Earn: '获得 Credits',
      Refund: '退款'
    }
    return description || descriptions[type as keyof typeof descriptions] || type
  }

  function getStatusColor (status: number) {
    const colors = {
      0: 'orange', // Pending
      1: 'blue',   // Processing
      2: 'green',  // Completed
      3: 'red',    // Failed
    }
    return colors[status as keyof typeof colors] || 'grey'
  }

  function getStatusText (status: number) {
    const texts = {
      0: '等待中',
      1: '处理中', 
      2: '成功',
      3: '失败'
    }
    return texts[status as keyof typeof texts] || '未知'
  }

  function truncatePrompt (prompt: string | undefined, length = 40) {
    if (!prompt) return '无提示词'
    return prompt.length > length ? prompt.slice(0, Math.max(0, length)) + '...' : prompt
  }

  function formatDate (date: string) {
    return new Date(date).toLocaleDateString('zh-CN')
  }

  function formatDateTime (date: string) {
    // 统一处理时间格式，不添加'Z'后缀
    const dateObj = new Date(date)
    return dateObj.toLocaleString('zh-CN', {
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    })
  }

  async function loadWalletBalance() {
    loadingBalance.value = true
    try {
      const balance = await getBalance()
      walletBalance.value = balance.balance
    } catch (error: any) {
      console.error('加载余额失败:', error)
      notificationStore.error('加载余额失败')
    } finally {
      loadingBalance.value = false
    }
  }

  async function loadTransactions() {
    loadingTransactions.value = true
    try {
      const result = await listTransactions(transactionPage.value - 1, 10)
      transactions.value = result.items
      transactionTotalPages.value = result.pagination.TotalPages
    } catch (error: any) {
      console.error('加载交易流水失败:', error)
      notificationStore.error('加载交易流水失败')
    } finally {
      loadingTransactions.value = false
    }
  }

  async function loadGenerationHistory() {
    loadingHistory.value = true
    try {
      await historyStore.fetchGenerations(historyPage.value - 1, 10)
      generationHistory.value = historyStore.generations
      historyTotalPages.value = historyStore.genPagination.TotalPages
      // 初始化筛选数据为空，只有在应用筛选时才显示筛选结果
      filteredHistory.value = []
    } catch (error: any) {
      console.error('加载生成历史失败:', error)
      notificationStore.error('加载生成历史失败')
    } finally {
      loadingHistory.value = false
    }
  }

  async function grantCredits () {
    granting.value = true
    try {
      await grantCreditsApi(10)
      await loadWalletBalance()
      await loadTransactions()
      notificationStore.success('Credits 发放成功！')
    } catch (error: any) {
      console.error('发放 Credits 失败:', error)
      notificationStore.error(error?.message || '发放 Credits 失败')
    } finally {
      granting.value = false
    }
  }

  function viewResult (task: any) {
    // 查看生成结果
    console.log('查看任务:', task.id)
  }

  function handleTransactionPageChange(page: number) {
    transactionPage.value = page
    loadTransactions()
  }

  onMounted(async () => {
    await Promise.all([
      loadWalletBalance(),
      loadTransactions(),
      loadGenerationHistory()
    ])
  })
</script>

<route lang="yaml">
meta:
  title: 账户与历史
</route>
