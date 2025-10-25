<template>
  <v-dialog v-model="dialog" max-width="800" scrollable>
    <v-card v-if="template" rounded="xl">
      <v-card-title class="pa-6 pb-0">
        <div class="d-flex align-center justify-space-between">
          <div>
            <h2 class="text-h5 font-weight-bold">{{ template.title }}</h2>
            <div class="d-flex align-center mt-2">
              <v-chip
                :color="getDifficultyColor(template.difficulty)"
                size="small"
                variant="flat"
                class="mr-2"
              >
                {{ getDifficultyText(template.difficulty) }}
              </v-chip>
              <v-chip size="small" variant="outlined">
                {{ getCategoryText(template.category) }}
              </v-chip>
            </div>
          </div>
          <v-btn
            icon
            variant="text"
            @click="dialog = false"
          >
            <v-icon>mdi-close</v-icon>
          </v-btn>
        </div>
      </v-card-title>

      <v-card-text class="pa-6">
        <!-- Author and Stats -->
        <div class="d-flex align-center justify-space-between mb-4">
          <div class="d-flex align-center">
            <v-avatar size="32" class="mr-3">
              <v-img v-if="template.author.avatar" :src="template.author.avatar" />
              <v-icon v-else>mdi-account</v-icon>
            </v-avatar>
            <div>
              <div class="font-weight-medium">{{ template.author.name }}</div>
              <div class="text-caption text-grey">{{ formatDate(template.createdAt) }}</div>
            </div>
          </div>
          <div class="d-flex align-center gap-4">
            <div class="text-center">
              <div class="font-weight-bold">{{ template.stats.likes }}</div>
              <div class="text-caption text-grey">点赞</div>
            </div>
            <div class="text-center">
              <div class="font-weight-bold">{{ template.stats.uses }}</div>
              <div class="text-caption text-grey">使用</div>
            </div>
            <div class="text-center">
              <div class="font-weight-bold">{{ template.stats.rating }}</div>
              <div class="text-caption text-grey">评分</div>
            </div>
          </div>
        </div>

        <!-- Description -->
        <div class="mb-4">
          <h4 class="text-h6 mb-2">模板描述</h4>
          <p class="text-body-1">{{ template.description }}</p>
        </div>

        <!-- Tags -->
        <div class="mb-4">
          <h4 class="text-h6 mb-2">标签</h4>
          <div class="d-flex flex-wrap gap-2">
            <v-chip
              v-for="tag in template.tags"
              :key="tag"
              size="small"
              variant="outlined"
            >
              {{ tag }}
            </v-chip>
          </div>
        </div>

        <!-- Example Images -->
        <div v-if="template.examples.length > 0" class="mb-4">
          <h4 class="text-h6 mb-2">示例图片</h4>
          <v-row>
            <v-col
              v-for="(example, index) in template.examples"
              :key="index"
              cols="12"
              sm="6"
              md="4"
            >
              <v-img
                :src="example"
                aspect-ratio="1"
                cover
                rounded="lg"
                class="cursor-pointer"
                @click="viewImage(example)"
              />
            </v-col>
          </v-row>
        </div>

        <!-- Prompt Template -->
        <div class="mb-4">
          <div class="d-flex align-center justify-space-between mb-2">
            <h4 class="text-h6">提示词模板</h4>
            <v-btn
              size="small"
              variant="outlined"
              prepend-icon="mdi-content-copy"
              @click="copyPrompt"
            >
              复制
            </v-btn>
          </div>
          <v-card variant="outlined" rounded="lg">
            <v-card-text class="pa-4">
              <pre class="text-body-2 whitespace-pre-wrap">{{ template.prompt }}</pre>
            </v-card-text>
          </v-card>
        </div>

        <!-- Variables Guide -->
        <div v-if="extractedVariables.length > 0" class="mb-4">
          <h4 class="text-h6 mb-2">变量说明</h4>
          <v-card variant="tonal" color="info" rounded="lg">
            <v-card-text class="pa-4">
              <p class="text-body-2 mb-3">此模板包含以下可替换变量：</p>
              <div class="d-flex flex-wrap gap-2">
                <v-chip
                  v-for="variable in extractedVariables"
                  :key="variable"
                  size="small"
                  color="info"
                  variant="flat"
                >
                  {{ variable }}
                </v-chip>
              </div>
              <p class="text-caption text-grey mt-3">
                使用时请将这些变量替换为具体内容
              </p>
            </v-card-text>
          </v-card>
        </div>

        <!-- Usage Example -->
        <div class="mb-4">
          <h4 class="text-h6 mb-2">使用示例</h4>
          <v-card variant="outlined" rounded="lg">
            <v-card-text class="pa-4">
              <div class="text-body-2 mb-2 font-weight-medium">处理后的提示词：</div>
              <p class="text-body-2 text-grey-darken-2">{{ processedExample }}</p>
            </v-card-text>
          </v-card>
        </div>
      </v-card-text>

      <v-card-actions class="pa-6 pt-0">
        <v-spacer />
        <v-btn
          variant="outlined"
          @click="dialog = false"
        >
          关闭
        </v-btn>
        <v-btn
          color="primary"
          prepend-icon="mdi-play"
          @click="useTemplate"
        >
          使用模板
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts" setup>
import { computed } from 'vue'
import { useNotificationStore } from '@/stores/notification'

interface Template {
  id: string
  title: string
  description: string
  prompt: string
  category: string
  difficulty: 'beginner' | 'intermediate' | 'advanced'
  tags: string[]
  author: {
    id: string
    name: string
    avatar?: string
  }
  stats: {
    likes: number
    uses: number
    rating: number
  }
  examples: string[]
  createdAt: string
  isLiked: boolean
}

const props = defineProps<{
  modelValue: boolean
  template: Template | null
}>()

const emit = defineEmits<{
  'update:modelValue': [value: boolean]
  use: [template: Template]
}>()

const notificationStore = useNotificationStore()

// Computed
const dialog = computed({
  get: () => props.modelValue,
  set: (value) => emit('update:modelValue', value)
})

const extractedVariables = computed(() => {
  if (!props.template) return []
  
  const matches = props.template.prompt.match(/\[([^\]]+)\]/g)
  return matches ? [...new Set(matches)] : []
})

const processedExample = computed(() => {
  if (!props.template) return ''
  
  // Replace variables with example values
  const examples: Record<string, string> = {
    '[PRODUCT]': '无线蓝牙耳机',
    '[COLOR]': '深空灰',
    '[STYLE]': '现代简约',
    '[BACKGROUND]': '纯白色背景',
    '[LIGHTING]': '柔和工作室灯光',
    '[ANGLE]': '45度斜角',
    '[CAMERA]': 'Canon EOS R5',
    '[PLATFORM]': 'iOS应用',
    '[UI_TYPE]': '用户登录界面',
    '[CHARACTER_TYPE]': '年轻女法师',
    '[HAIR_COLOR]': '银白色',
    '[EYE_COLOR]': '深蓝色'
  }
  
  let processed = props.template.prompt
  Object.entries(examples).forEach(([key, value]) => {
    processed = processed.replace(new RegExp(key.replace(/[.*+?^${}()|[\]\\]/g, '\\$&'), 'g'), value)
  })
  
  return processed
})

// Methods
function getDifficultyColor(difficulty: string): string {
  const colors = {
    beginner: 'success',
    intermediate: 'warning',
    advanced: 'error'
  }
  return colors[difficulty as keyof typeof colors] || 'grey'
}

function getDifficultyText(difficulty: string): string {
  const texts = {
    beginner: '初级',
    intermediate: '中级',
    advanced: '高级'
  }
  return texts[difficulty as keyof typeof texts] || difficulty
}

function getCategoryText(category: string): string {
  const texts = {
    photography: '摄影',
    design: '设计',
    illustration: '插画',
    typography: '文字设计',
    other: '其他'
  }
  return texts[category as keyof typeof texts] || category
}

function formatDate(dateString: string): string {
  return new Date(dateString).toLocaleDateString('zh-CN')
}

function copyPrompt() {
  if (!props.template) return
  
  navigator.clipboard.writeText(props.template.prompt).then(() => {
    notificationStore.success('提示词已复制到剪贴板')
  }).catch(() => {
    notificationStore.error('复制失败')
  })
}

function viewImage(imageUrl: string) {
  // Open image in new tab or lightbox
  window.open(imageUrl, '_blank')
}

function useTemplate() {
  if (!props.template) return
  
  emit('use', props.template)
  dialog.value = false
}
</script>

<style scoped>
.whitespace-pre-wrap {
  white-space: pre-wrap;
  word-break: break-word;
}

.cursor-pointer {
  cursor: pointer;
}
</style>
