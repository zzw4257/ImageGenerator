<template>
  <v-container class="pa-8">
    <v-tabs v-model="tab">
      <v-tab value="gen">Generations</v-tab>
      <v-tab value="tx">Transactions</v-tab>
    </v-tabs>

    <div v-if="tab === 'gen'">
      <!-- 筛选工具栏 -->
      <v-card class="mb-4" elevation="1">
        <v-card-text class="py-4">
          <v-row>
            <v-col cols="12" md="3">
              <v-text-field
                v-model="filterOptions.keyword"
                label="搜索关键词"
                prepend-inner-icon="mdi-magnify"
                variant="outlined"
                density="compact"
                clearable
                @input="applyFilters"
              />
            </v-col>
            <v-col cols="12" md="2">
              <v-select
                v-model="filterOptions.status"
                :items="statusOptions"
                label="状态"
                variant="outlined"
                density="compact"
                clearable
                @update:modelValue="applyFilters"
              />
            </v-col>
            <v-col cols="12" md="2">
              <v-select
                v-model="filterOptions.provider"
                :items="providerOptions"
                label="提供商"
                variant="outlined"
                density="compact"
                clearable
                @update:modelValue="applyFilters"
              />
            </v-col>
            <v-col cols="12" md="2">
              <v-text-field
                v-model="filterOptions.dateFrom"
                label="开始日期"
                type="date"
                variant="outlined"
                density="compact"
                @update:modelValue="applyFilters"
              />
            </v-col>
            <v-col cols="12" md="2">
              <v-text-field
                v-model="filterOptions.dateTo"
                label="结束日期"
                type="date"
                variant="outlined"
                density="compact"
                @update:modelValue="applyFilters"
              />
            </v-col>
            <v-col cols="12" md="1">
              <v-btn
                color="primary"
                variant="outlined"
                @click="resetFilters"
                :disabled="!hasActiveFilters"
              >
                重置
              </v-btn>
            </v-col>
          </v-row>
        </v-card-text>
      </v-card>

      <!-- 显示生成历史记录 -->
      <v-row>
        <v-col v-for="record in paginatedGenerations" :key="record.id" cols="12" md="6" lg="4">
          <v-card class="pa-4 mb-3" elevation="2">
            <div class="d-flex align-start">
              <v-avatar rounded size="64" class="mr-4">
                <v-img :src="record.thumbnail" />
              </v-avatar>
              <div class="flex-grow-1">
                <div class="text-body-1 font-weight-medium mb-2">
                  {{ truncatePrompt(record.prompt) }}
                </div>
                <div class="text-caption text-grey-600 mb-2">
                  {{ formatDateTime(record.createdAt) }}
                </div>
                <div class="d-flex align-center">
                  <v-chip 
                    :color="getStatusColor(record.status)" 
                    size="small" 
                    class="mr-2"
                  >
                    {{ getStatusText(record.status) }}
                  </v-chip>
                  <v-chip 
                    v-if="record.provider" 
                    size="small" 
                    variant="outlined"
                  >
                    {{ record.provider }}
                  </v-chip>
                </div>
              </div>
            </div>
          </v-card>
        </v-col>
      </v-row>
      
      <!-- 分页 -->
      <v-pagination 
        v-model="genPageUI" 
        :length="Math.ceil(filteredGenerations.length / pageSize)" 
        @update:modelValue="handlePageChange" 
      />
    </div>

    <div v-else>
      <v-row>
        <v-col v-for="t in transactions" :key="t.id" cols="12">
          <v-list-item>
            <v-list-item-content>
              <v-list-item-title>{{ t.type }} {{ t.amount }}</v-list-item-title>
              <v-list-item-subtitle>{{ t.description }}</v-list-item-subtitle>
            </v-list-item-content>
            <v-list-item-action>{{ formatDateTime(t.createdAt) }}</v-list-item-action>
          </v-list-item>
          <v-divider />
        </v-col>
      </v-row>
      <v-pagination v-model="txPageUI" :length="txPagination.TotalPages || 1" @update:modelValue="loadTransactions" />
    </div>
  </v-container>
</template>

<script lang="ts" setup>
import { ref, onMounted, computed } from 'vue'
import { useHistoryStore } from '@/stores/history'
import { listTransactions } from '@/services/wallet'
import type { GenerationRecordDto } from '@/types/api'

const tab = ref<'gen'|'tx'>('gen')

// generations
const historyStore = useHistoryStore()
const generations = ref<GenerationRecordDto[]>([])
const genPagination = ref({ TotalPages: 1 })
const genPageUI = ref(1)
const pageSize = ref(12)

// 筛选选项
const filterOptions = ref({
  keyword: '',
  status: null as number | null,
  provider: null as string | null,
  dateFrom: '',
  dateTo: ''
})

// 筛选选项数据
const statusOptions = [
  { title: '等待中', value: 0 },
  { title: '处理中', value: 1 },
  { title: '已完成', value: 2 },
  { title: '失败', value: 3 }
]

const providerOptions = [
  { title: 'Stub', value: 'Stub' },
  { title: 'Qwen', value: 'Qwen' },
  { title: 'Flux', value: 'Flux' },
  { title: 'OpenAI', value: 'OpenAI' },
  { title: 'Gemini', value: 'Gemini' }
]

// 筛选后的生成记录
const filteredGenerations = computed(() => {
  let filtered = [...generations.value]

  // 关键词搜索
  if (filterOptions.value.keyword) {
    const keyword = filterOptions.value.keyword.toLowerCase()
    filtered = filtered.filter(record => 
      record.prompt.toLowerCase().includes(keyword)
    )
  }

  // 状态筛选
  if (filterOptions.value.status !== null) {
    filtered = filtered.filter(record => record.status === filterOptions.value.status)
  }

  // 提供商筛选
  if (filterOptions.value.provider) {
    filtered = filtered.filter(record => record.provider === filterOptions.value.provider)
  }

  // 时间范围筛选
  if (filterOptions.value.dateFrom) {
    const fromDate = new Date(filterOptions.value.dateFrom)
    filtered = filtered.filter(record => new Date(record.createdAt) >= fromDate)
  }

  if (filterOptions.value.dateTo) {
    const toDate = new Date(filterOptions.value.dateTo)
    toDate.setHours(23, 59, 59, 999) // 包含整天
    filtered = filtered.filter(record => new Date(record.createdAt) <= toDate)
  }

  return filtered
})

// 分页后的生成记录
const paginatedGenerations = computed(() => {
  const start = (genPageUI.value - 1) * pageSize.value
  const end = start + pageSize.value
  return filteredGenerations.value.slice(start, end)
})

// 计算属性：是否有活跃的筛选条件
const hasActiveFilters = computed(() => {
  return filterOptions.value.keyword !== '' ||
         filterOptions.value.status !== null ||
         filterOptions.value.provider !== null ||
         filterOptions.value.dateFrom !== '' ||
         filterOptions.value.dateTo !== ''
})

async function loadGenerations() {
  await historyStore.fetchGenerations(genPageUI.value - 1, 12)
  generations.value = historyStore.generations
  genPagination.value = historyStore.genPagination
}

// transactions
const transactions = ref<any[]>([])
const txPagination = ref({ TotalPages: 1 })
const txPageUI = ref(1)

async function loadTransactions() {
  const { items, pagination } = await listTransactions(txPageUI.value - 1, 12)
  transactions.value = items
  txPagination.value = pagination
}

// 筛选函数
function applyFilters() {
  // 筛选逻辑在computed中处理
  genPageUI.value = 1 // 重置到第一页
}

// 分页处理函数
function handlePageChange(page: number) {
  genPageUI.value = page
}

// 重置筛选
function resetFilters() {
  filterOptions.value = {
    keyword: '',
    status: null,
    provider: null,
    dateFrom: '',
    dateTo: ''
  }
  genPageUI.value = 1
}

// 工具函数
function truncatePrompt(prompt: string, maxLength = 50) {
  return prompt.length > maxLength ? prompt.substring(0, maxLength) + '...' : prompt
}

function formatDateTime(date: string) {
  return new Date(date).toLocaleString('zh-CN', {
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  })
}

function getStatusColor(status: number) {
  switch (status) {
    case 0: return 'orange' // Pending
    case 1: return 'blue'   // Processing
    case 2: return 'green'  // Completed
    case 3: return 'red'     // Failed
    default: return 'grey'
  }
}

function getStatusText(status: number) {
  switch (status) {
    case 0: return '等待中'
    case 1: return '处理中'
    case 2: return '已完成'
    case 3: return '失败'
    default: return '未知'
  }
}

onMounted(() => { 
  loadGenerations()
  loadTransactions() 
})
</script>

<route lang="yaml">
meta:
  title: History
</route>