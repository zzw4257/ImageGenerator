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
                  {{ transaction.description }}
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
                      transaction.type === 'income' ? 'text-success' : 'text-error'
                    ]"
                  >
                    {{ transaction.type === 'income' ? '+' : '-' }}{{ transaction.amount }}
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
            />
          </v-card-actions>
        </v-card>
      </v-col>

      <!-- 生成历史 -->
      <v-col cols="12" lg="6">
        <v-card elevation="2" rounded="xl">
          <v-card-title class="d-flex align-center">
            <v-icon class="mr-2">mdi-history</v-icon>
            生成历史
          </v-card-title>
          <v-divider />
          <v-card-text class="pa-0">
            <v-list>
              <v-list-item
                v-for="task in generationHistory"
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
                    {{ task.statusText }}
                  </v-chip>
                  <span class="text-caption text-grey">
                    {{ formatDateTime(task.createdAt) }}
                  </span>
                  <span class="text-caption font-weight-medium">
                    • {{ task.credits }} Credits
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
              v-model="historyPage"
              :length="historyTotalPages"
              rounded="circle"
              total-visible="5"
            />
          </v-card-actions>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts" setup>
  import type { GenerationTaskDto, TransactionDto } from '@/types/api'

  import { onMounted, ref } from 'vue'

  const walletBalance = ref(25)
  const granting = ref(false)

  const transactions = ref<TransactionDto[]>([])
  const generationHistory = ref<GenerationTaskDto[]>([])

  const transactionPage = ref(1)
  const transactionTotalPages = ref(3)
  const historyPage = ref(1)
  const historyTotalPages = ref(2)

  function getTransactionColor (type: string) {
    return type === 'income' ? 'green-lighten-5' : 'red-lighten-5'
  }

  function getTransactionIconColor (type: string) {
    return type === 'income' ? 'green' : 'red'
  }

  function getTransactionIcon (type: string) {
    return type === 'income' ? 'mdi-plus-circle' : 'mdi-minus-circle'
  }

  function getStatusColor (status: string) {
    const colors = {
      completed: 'success',
      processing: 'warning',
      failed: 'error',
    }
    return colors[status as keyof typeof colors] || 'grey'
  }

  function truncatePrompt (prompt: string, length = 40) {
    return prompt.length > length ? prompt.slice(0, Math.max(0, length)) + '...' : prompt
  }

  function formatDate (date: Date) {
    return new Date(date).toLocaleDateString('zh-CN')
  }

  function formatDateTime (date: Date) {
    return new Date(date).toLocaleString('zh-CN', {
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    })
  }

  async function grantCredits () {
    granting.value = true
    try {
      // 调用发放 API
      await new Promise(resolve => setTimeout(resolve, 1000))
      walletBalance.value += 10

      // 添加交易记录
      transactions.value.unshift({
        id: Date.now().toString(),
        type: 'income',
        amount: 10,
        description: '开发测试发放',
        createdAt: new Date(),
      })
    } catch (error) {
      console.error('发放 Credits 失败:', error)
    } finally {
      granting.value = false
    }
  }

  function viewResult (task: GenerationTaskDto) {
    // 查看生成结果
    console.log('查看任务:', task.id)
  }

  function loadTransactions () {
    transactions.value = [
      {
        id: '1',
        type: 'expense',
        amount: 2,
        description: '图像生成 - 梦幻风景',
        createdAt: new Date(Date.now() - 1000 * 60 * 30),
      },
      {
        id: '2',
        type: 'income',
        amount: 10,
        description: 'Credits 充值',
        createdAt: new Date(Date.now() - 1000 * 60 * 60 * 2),
      },
      {
        id: '3',
        type: 'expense',
        amount: 3,
        description: '图像生成 - 肖像艺术',
        createdAt: new Date(Date.now() - 1000 * 60 * 60 * 5),
      },
      {
        id: '4',
        type: 'expense',
        amount: 1,
        description: '图像生成 - 抽象艺术',
        createdAt: new Date(Date.now() - 1000 * 60 * 60 * 24),
      },
    //   {
    //     id: '5',
    //     type: 'expense',
    //     amount: 2,
    //     description: '图像生成 - 产品设计',
    //     createdAt: new Date(Date.now() - 1000 * 60 * 60 * 48),
    //   },
    //   {
    //     id: '6',
    //     type: 'income',
    //     amount: 15,
    //     description: 'Credits 充值',
    //     createdAt: new Date(Date.now() - 1000 * 60 * 60 * 72),
    //   },
    //   {
    //     id: '7',
    //     type: 'expense',
    //     amount: 4,
    //     description: '图像生成 - 科幻场景',
    //     createdAt: new Date(Date.now() - 1000 * 60 * 60 * 96),
    //   },
    ]
  }

  function loadGenerationHistory () {
    generationHistory.value = [
      {
        id: '1',
        prompt: 'A beautiful sunset over mountains with reflective lake, fantasy art style',
        thumbnail: '/images/history/1-thumb.jpg',
        status: 'completed',
        statusText: '已完成',
        credits: 2,
        createdAt: new Date(Date.now() - 1000 * 60 * 30),
      },
      {
        id: '2',
        prompt: 'Professional portrait of a business person in suit, cinematic lighting',
        thumbnail: '/images/history/2-thumb.jpg',
        status: 'completed',
        statusText: '已完成',
        credits: 3,
        createdAt: new Date(Date.now() - 1000 * 60 * 60 * 2),
      },
      {
        id: '3',
        prompt: 'Abstract geometric patterns with vibrant colors and modern design',
        thumbnail: '/images/history/3-thumb.jpg',
        status: 'completed',
        statusText: '已完成',
        credits: 1,
        createdAt: new Date(Date.now() - 1000 * 60 * 60 * 5),
      },
      {
        id: '4',
        prompt: 'Product design visualization for a modern smartphone, clean background',
        thumbnail: '/images/history/4-thumb.jpg',
        status: 'processing',
        statusText: '生成中',
        credits: 2,
        createdAt: new Date(Date.now() - 1000 * 60 * 60 * 24),
      },
    ]
  }

  onMounted(() => {
    loadTransactions()
    loadGenerationHistory()
  })
</script>

<route lang="yaml">
meta:
  title: 账户与历史
</route>
