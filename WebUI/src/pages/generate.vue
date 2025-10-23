<template>
  <v-container class="pa-0" fluid>
    <v-row no-gutters>
      <!-- 左侧控制栏 -->
      <v-col class="bg-grey-lighten-4" cols="12" md="3">
        <v-container class="py-6">
          <v-card elevation="1" rounded="xl">
            <v-card-text class="pa-6">
              <h2 class="text-h6 font-weight-bold mb-4">生成设置</h2>

              <!-- Prompt 输入 -->
              <v-textarea
                v-model="form.prompt"
                class="mb-4"
                label="Prompt"
                placeholder="描述您想要生成的图像..."
                rows="4"
                variant="outlined"
              />

              <!-- 模型选择 -->
              <v-select
                v-model="form.model"
                class="mb-4"
                :items="modelOptions"
                label="模型/提供商"
                variant="outlined"
              />

              <!-- 分辨率 -->
              <v-select
                v-model="form.resolution"
                class="mb-4"
                :items="resolutionOptions"
                label="分辨率"
                variant="outlined"
              />

              <!-- 数量 -->
              <v-text-field
                v-model="form.quantity"
                class="mb-4"
                disabled
                label="数量"
                max="1"
                min="1"
                type="number"
                variant="outlined"
              />

              <!-- 风格 -->
              <v-select
                v-model="form.style"
                class="mb-4"
                clearable
                :items="styleOptions"
                label="风格 (可选)"
                variant="outlined"
              />

              <!-- 费用显示 -->
              <v-card
                class="pa-4 rounded-lg mb-4"
                color="primary-lighten-5"
                variant="flat"
              >
                <div class="d-flex align-center justify-space-between">
                  <span class="text-body-1 font-weight-medium">预计消耗</span>
                  <div class="d-flex align-center">
                    <v-icon class="mr-1" color="primary">mdi-flash</v-icon>
                    <span class="text-h6 font-weight-bold text-primary">
                      {{ calculatedCredits }} Credits
                    </span>
                  </div>
                </div>
              </v-card>

              <!-- 生成按钮 -->
              <v-btn
                block
                class="font-weight-bold"
                :color="generateBtnColor"
                :disabled="!form.prompt"
                :loading="generating"
                size="large"
                @click="generateImage"
              >
                <template #prepend>
                  <v-icon>mdi-auto-fix</v-icon>
                </template>
                生成图像
              </v-btn>
            </v-card-text>
          </v-card>
        </v-container>
      </v-col>

      <!-- 中间内容区 -->
      <v-col cols="12" md="6">
        <v-container class="py-6">
          <!-- 步骤提示 -->
          <v-row class="mb-8">
            <v-col
              v-for="(step, index) in steps"
              :key="step.title"
              cols="4"
            >
              <v-card
                class="text-center pa-4 rounded-xl"
                :color="step.active ? 'primary' : 'grey-lighten-4'"
                height="120"
                variant="flat"
              >
                <v-icon
                  class="mb-2"
                  :color="step.active ? 'white' : 'grey'"
                  size="40"
                >
                  {{ step.icon }}
                </v-icon>
                <h4
                  :class="[
                    'text-body-1 font-weight-medium',
                    step.active ? 'text-white' : 'text-grey'
                  ]"
                >
                  {{ step.title }}
                </h4>
                <p
                  :class="[
                    'text-caption',
                    step.active ? 'text-white' : 'text-grey-darken-1'
                  ]"
                >
                  {{ step.description }}
                </p>
              </v-card>
            </v-col>
          </v-row>

          <!-- 生成结果 -->
          <div v-if="generating" class="text-center py-12">
            <v-progress-circular
              class="mb-4"
              color="primary"
              indeterminate
              size="64"
              width="4"
            />
            <h3 class="text-h6 mb-2">正在生成图像...</h3>
            <p class="text-body-2 text-grey-darken-1">
              这可能需要几分钟时间，请耐心等待
            </p>
          </div>

          <div v-else-if="currentResult" class="text-center">
            <v-card class="mb-4" elevation="2" rounded="xl">
              <v-img
                :alt="currentResult.prompt"
                class="rounded-t-xl"
                contain
                max-height="500"
                :src="currentResult.imageUrl"
              />
              <v-card-actions class="pa-4">
                <v-btn
                  variant="outlined"
                  @click="downloadImage(currentResult)"
                >
                  <v-icon start>mdi-download</v-icon>
                  下载
                </v-btn>
                <v-spacer />
                <v-btn
                  :color="currentResult.isFavorite ? 'error' : 'default'"
                  variant="text"
                  @click="toggleFavorite(currentResult)"
                >
                  <v-icon>
                    {{ currentResult.isFavorite ? 'mdi-heart' : 'mdi-heart-outline' }}
                  </v-icon>
                </v-btn>
              </v-card-actions>
            </v-card>
          </div>

          <div v-else class="text-center py-12">
            <v-icon
              class="mb-4"
              color="grey-lighten-2"
              size="80"
            >mdi-image-outline</v-icon>
            <h3 class="text-h6 mb-2">等待生成</h3>
            <p class="text-body-2 text-grey-darken-1">
              配置参数后点击生成按钮创建您的第一张图像
            </p>
          </div>
        </v-container>
      </v-col>

      <!-- 右侧边栏 -->
      <v-col class="bg-grey-lighten-3" cols="12" md="3">
        <v-container class="py-6">
          <!-- 钱包余额 -->
          <v-card class="mb-6" elevation="1" rounded="xl">
            <v-card-text class="pa-4">
              <div class="d-flex align-center justify-space-between mb-2">
                <span class="text-body-1 font-weight-medium">钱包余额</span>
                <v-chip color="primary" variant="flat">
                  <v-icon start>mdi-wallet</v-icon>
                  {{ walletBalance }} Credits
                </v-chip>
              </div>
              <v-btn
                block
                size="small"
                variant="outlined"
                @click="grantCredits"
              >
                <v-icon start>mdi-plus</v-icon>
                获取 Credits
              </v-btn>
            </v-card-text>
          </v-card>

          <!-- 最近任务 -->
          <v-card elevation="1" rounded="xl">
            <v-card-title class="pa-4">
              <v-icon start>mdi-history</v-icon>
              最近任务
            </v-card-title>
            <v-divider />
            <v-card-text class="pa-0">
              <v-list>
                <v-list-item
                  v-for="task in recentTasks"
                  :key="task.id"
                  class="px-4 py-3"
                >
                  <template #prepend>
                    <v-avatar rounded size="40">
                      <v-img :src="task.thumbnail" />
                    </v-avatar>
                  </template>

                  <v-list-item-title class="text-body-2">
                    {{ truncatePrompt(task.prompt) }}
                  </v-list-item-title>

                  <v-list-item-subtitle class="d-flex align-center mt-1">
                    <v-chip
                      :color="getStatusColor(task.status)"
                      size="x-small"
                      variant="flat"
                    >
                      {{ task.status }}
                    </v-chip>
                    <span class="text-caption text-grey ms-2">
                      {{ formatTime(task.createdAt) }}
                    </span>
                  </v-list-item-subtitle>
                </v-list-item>
              </v-list>
            </v-card-text>
          </v-card>
        </v-container>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts" setup>
  import { computed, onMounted, ref } from 'vue'
  import { useRoute } from 'vue-router'
  import { usePresetStore } from '@/stores/presets'
import { generateImage as generateImageApi, pollTaskStatus, type GenerateRequestDto, type GenerateTaskStatusDto } from '@/services/generate'
import { getBalance, grantCredits as grantCreditsApi } from '@/services/wallet'
import { createConversation } from '@/services/conversation'
import { useNotificationStore } from '@/stores/notification'

  interface GenerationForm {
    prompt: string
    model: string
    resolution: string
    quantity: number
    style?: string
  }

  interface GenerationResult {
    id: string
    prompt: string
    imageUrl: string
    isFavorite: boolean
    createdAt: Date
  }

  interface Task {
    id: string
    prompt: string
    thumbnail: string
    status: 'completed' | 'processing' | 'failed'
    createdAt: Date
  }

  const route = useRoute()
  const presetStore = usePresetStore()
  const notificationStore = useNotificationStore()

  const form = ref<GenerationForm>({
    prompt: '',
    model: 'Stub',
    resolution: '1024x1024',
    quantity: 1,
    style: '',
  })

  const generating = ref(false)
  const currentResult = ref<GenerationResult | null>(null)
  const walletBalance = ref(0)
  const recentTasks = ref<Task[]>([])
  const currentTaskId = ref<string | null>(null)

  const modelOptions = [
    { title: 'Stub', value: 'Stub' },
    { title: 'Qwen', value: 'Qwen' },
    { title: 'Flux', value: 'Flux' },
  ]

  const resolutionOptions = [
    '512x512',
    '768x768',
    '1024x1024',
    '768x1024',
    '1024x768',
  ]

  const styleOptions = [
    '真实',
    '卡通',
    '油画',
    '水彩',
    '像素艺术',
    '赛博朋克',
    '极简',
  ]

  const steps = ref([
    {
      title: '配置参数',
      description: '设置提示词和模型',
      icon: 'mdi-cog',
      active: true,
    },
    {
      title: '生成图像',
      description: 'AI 正在创作',
      icon: 'mdi-auto-fix',
      active: false,
    },
    {
      title: '获取结果',
      description: '下载或收藏',
      icon: 'mdi-image-check',
      active: false,
    },
  ])

  const calculatedCredits = computed(() => {
    const baseCredits = form.value.model === 'Flux'
      ? 2
      : (form.value.model === 'Qwen' ? 3 : 1)
    return baseCredits * form.value.quantity
  })

  const generateBtnColor = computed(() => {
    return form.value.prompt ? '#22c55e' : 'grey'
  })

  function truncatePrompt (prompt: string, length = 30) {
    return prompt.length > length ? prompt.slice(0, Math.max(0, length)) + '...' : prompt
  }

  function getStatusColor (status: string) {
    const colors = {
      completed: 'success',
      processing: 'warning',
      failed: 'error',
    }
    return colors[status as keyof typeof colors] || 'grey'
  }

  function formatTime (date: Date) {
    return new Date(date).toLocaleTimeString('zh-CN', {
      hour: '2-digit',
      minute: '2-digit',
    })
  }

  async function generateImage () {
    if (!form.value.prompt) return

    generating.value = true
    if (steps.value[1]) steps.value[1].active = true
    currentTaskId.value = null

    try {
      // 先创建conversation
      const conversation = await createConversation()
      
      // 构建生成参数
      const params = {
        resolution: form.value.resolution,
        style: form.value.style,
        quantity: form.value.quantity
      }

      const request: GenerateRequestDto = {
        conversationId: conversation.id,
        prompt: form.value.prompt,
        provider: form.value.model,
        params: JSON.stringify(params),
        quality: 'standard',
        style: 'vivid'
      }

      // 提交生成任务
      const response = await generateImageApi(request)
      currentTaskId.value = response.taskId

      // 轮询任务状态
      await pollTaskStatus(
        response.taskId,
        (status: GenerateTaskStatusDto) => {
          console.log('Task status update:', status)
        }
      )

      // 任务完成
      const finalStatus = await pollTaskStatus(response.taskId)
      
      if (finalStatus.status === 2 && finalStatus.imageUrl) {
        currentResult.value = {
          id: finalStatus.taskId,
          prompt: finalStatus.prompt || form.value.prompt,
          imageUrl: finalStatus.imageUrl,
          isFavorite: false,
          createdAt: new Date(finalStatus.createdAt),
        }

        // 添加到最近任务
        recentTasks.value.unshift({
          id: finalStatus.taskId,
          prompt: finalStatus.prompt || form.value.prompt,
          thumbnail: finalStatus.imageUrl,
          status: 'completed',
          createdAt: new Date(finalStatus.createdAt),
        })

        if (steps.value[2]) steps.value[2].active = true
        notificationStore.success('图像生成成功！')
        
        // 刷新余额
        await loadWalletBalance()
      } else {
        throw new Error(finalStatus.error || '生成失败')
      }
    } catch (error: any) {
      console.error('生成失败:', error)
      notificationStore.error(error?.message || '图像生成失败')
    } finally {
      generating.value = false
      currentTaskId.value = null
    }
  }

  function downloadImage (result: GenerationResult) {
    const link = document.createElement('a')
    link.href = result.imageUrl
    link.download = `ai-image-${result.id}.jpg`
    link.click()
  }

  function toggleFavorite (result: GenerationResult) {
    result.isFavorite = !result.isFavorite
  }

  async function grantCredits () {
    try {
      await grantCreditsApi(10, '开发测试发放')
      await loadWalletBalance()
      notificationStore.success('Credits 发放成功！')
    } catch (error: any) {
      console.error('发放 Credits 失败:', error)
      notificationStore.error(error?.message || '发放 Credits 失败')
    }
  }

  async function loadWalletBalance() {
    try {
      const balance = await getBalance()
      walletBalance.value = balance.balance
    } catch (error) {
      console.error('加载余额失败:', error)
    }
  }

  function loadRecentTasks () {
    // 模拟加载最近任务 - 后续可以从API获取
    recentTasks.value = [
      {
        id: '1',
        prompt: 'A beautiful sunset over mountains',
        thumbnail: '/images/generated/thumb1.jpg',
        status: 'completed',
        createdAt: new Date(Date.now() - 1000 * 60 * 5),
      },
      {
        id: '2',
        prompt: 'Abstract geometric patterns in blue',
        thumbnail: '/images/generated/thumb2.jpg',
        status: 'completed',
        createdAt: new Date(Date.now() - 1000 * 60 * 30),
      },
    ]
  }

  onMounted(async () => {
    loadRecentTasks()
    await loadWalletBalance()

    const presetStore = usePresetStore()
    // 优先从 store/session 恢复已选择的 preset
    presetStore.restoreSelectedFromSession()
    const selected = presetStore.selectedPreset

    const q = route.query
    const safeDecode = (v?: unknown) => {
      if (!v) return undefined
      try { return decodeURIComponent(String(v)) } catch { return String(v) }
    }
    const safeParseJson = (s?: string) => {
      if (!s) return undefined
      try { return JSON.parse(s) } catch { return undefined }
    }

    if (selected) {
      form.value.prompt = selected.prompt || form.value.prompt
      form.value.model = selected.provider || form.value.model
      
      // 解析 defaultParams
      if (selected.defaultParams) {
        try {
          const params = JSON.parse(selected.defaultParams)
          form.value.resolution = params.resolution || form.value.resolution
          form.value.style = params.style || form.value.style
        } catch (e) {
          console.warn('Failed to parse defaultParams:', selected.defaultParams)
        }
      }
      return
    }

    // 若 store 没有 preset，则继续解析 query（兼容多种格式）
    if (q.params) {
      const decoded = safeDecode(q.params)
      const params = safeParseJson(decoded)
      if (params) {
        form.value.prompt = params.prompt || form.value.prompt
        form.value.model = params.provider || params.model || form.value.model
        form.value.resolution = params.resolution || form.value.resolution
        form.value.style = params.style || form.value.style
      }
    } else if (q.preset) {
      let presetObj: any = undefined
      const decoded = safeDecode(q.preset)
      presetObj = safeParseJson(decoded) || (typeof q.preset === 'object' ? q.preset : undefined)
      if (presetObj) {
        form.value.prompt = presetObj.prompt || form.value.prompt
        form.value.model = presetObj.provider || presetObj.model || form.value.model
        form.value.resolution = presetObj.params?.resolution || presetObj.resolution || form.value.resolution
        form.value.style = presetObj.params?.style || presetObj.style || form.value.style
      }
    } else {
      if (q.prompt) form.value.prompt = safeDecode(q.prompt) || form.value.prompt
      if (q.model) form.value.model = safeDecode(q.model) || form.value.model
      if (q.resolution) form.value.resolution = safeDecode(q.resolution) || form.value.resolution
      if (q.style) form.value.style = safeDecode(q.style) || form.value.style
    }
  })
</script>

<route lang="yaml">
meta:
  title: 生成图像
</route>
