<template>
  <v-dialog v-model="dialog" max-width="700" persistent>
    <v-card rounded="xl">
      <v-card-title class="pa-6 pb-0">
        <h2 class="text-h5 font-weight-bold">分享模板</h2>
        <p class="text-body-2 text-grey-darken-1 mt-1">分享您的优质提示词模板，帮助更多创作者</p>
      </v-card-title>

      <v-card-text class="pa-6">
        <v-form ref="formRef" v-model="formValid">
          <!-- Basic Info -->
          <v-row>
            <v-col cols="12">
              <v-text-field
                v-model="form.title"
                label="模板标题 *"
                variant="outlined"
                :rules="[rules.required]"
                counter="50"
                maxlength="50"
              />
            </v-col>
            <v-col cols="12">
              <v-textarea
                v-model="form.description"
                label="模板描述 *"
                variant="outlined"
                :rules="[rules.required]"
                counter="200"
                maxlength="200"
                rows="3"
              />
            </v-col>
          </v-row>

          <!-- Category and Difficulty -->
          <v-row>
            <v-col cols="12" md="6">
              <v-select
                v-model="form.category"
                :items="categoryOptions"
                label="分类 *"
                variant="outlined"
                :rules="[rules.required]"
              />
            </v-col>
            <v-col cols="12" md="6">
              <v-select
                v-model="form.difficulty"
                :items="difficultyOptions"
                label="难度等级 *"
                variant="outlined"
                :rules="[rules.required]"
              />
            </v-col>
          </v-row>

          <!-- Prompt Template -->
          <v-textarea
            v-model="form.prompt"
            label="提示词模板 *"
            variant="outlined"
            :rules="[rules.required, rules.promptLength]"
            counter="1000"
            maxlength="1000"
            rows="6"
            placeholder="使用 [变量名] 来标记可替换的部分，例如：[PRODUCT], [COLOR], [STYLE] 等"
          />

          <!-- Variables Help -->
          <v-alert type="info" variant="tonal" class="mb-4">
            <div class="d-flex align-center">
              <v-icon class="mr-2">mdi-lightbulb</v-icon>
              <div>
                <div class="font-weight-medium">变量使用提示</div>
                <div class="text-body-2">
                  使用方括号 [变量名] 来标记可替换的部分，让其他用户可以轻松自定义模板
                </div>
              </div>
            </div>
          </v-alert>

          <!-- Tags -->
          <v-combobox
            v-model="form.tags"
            label="标签"
            variant="outlined"
            multiple
            chips
            closable-chips
            placeholder="输入标签并按回车添加"
            hint="最多添加 8 个标签"
            :rules="[rules.maxTags]"
          />

          <!-- Example Images -->
          <div class="mb-4">
            <h4 class="text-h6 mb-3">示例图片 (可选)</h4>
            <v-file-input
              v-model="form.exampleFiles"
              label="上传示例图片"
              variant="outlined"
              accept="image/*"
              multiple
              prepend-icon="mdi-camera"
              :rules="[rules.maxFiles]"
            />
            <p class="text-caption text-grey mt-1">最多上传 3 张示例图片，帮助其他用户了解模板效果</p>
          </div>

          <!-- Preview -->
          <div v-if="form.prompt" class="mb-4">
            <h4 class="text-h6 mb-3">模板预览</h4>
            <v-card variant="outlined" rounded="lg">
              <v-card-text class="pa-4">
                <div class="d-flex align-center justify-space-between mb-2">
                  <span class="text-body-2 font-weight-medium">处理后的提示词</span>
                  <v-btn
                    size="small"
                    variant="text"
                    @click="generatePreview"
                  >
                    <v-icon start size="16">mdi-refresh</v-icon>
                    重新生成
                  </v-btn>
                </div>
                <p class="text-body-2 text-grey-darken-2">{{ processedPrompt }}</p>
              </v-card-text>
            </v-card>
          </div>

          <!-- Terms -->
          <v-checkbox
            v-model="form.agreeTerms"
            :rules="[rules.required]"
            color="primary"
          >
            <template #label>
              我确认此模板为原创内容，同意 <a href="#" class="text-primary">模板分享协议</a>
            </template>
          </v-checkbox>
        </v-form>
      </v-card-text>

      <v-card-actions class="pa-6 pt-0">
        <v-spacer />
        <v-btn
          variant="outlined"
          @click="dialog = false"
        >
          取消
        </v-btn>
        <v-btn
          color="primary"
          :loading="submitting"
          :disabled="!formValid"
          @click="submitTemplate"
        >
          分享模板
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts" setup>
import { ref, computed } from 'vue'
import { useNotificationStore } from '@/stores/notification'

const notificationStore = useNotificationStore()

// Props & Emits
const props = defineProps<{
  modelValue: boolean
}>()

const emit = defineEmits<{
  'update:modelValue': [value: boolean]
  created: []
}>()

// State
const formRef = ref()
const formValid = ref(false)
const submitting = ref(false)

const form = ref({
  title: '',
  description: '',
  category: '',
  difficulty: '',
  prompt: '',
  tags: [] as string[],
  exampleFiles: null as File[] | null,
  agreeTerms: false
})

// Computed
const dialog = computed({
  get: () => props.modelValue,
  set: (value) => emit('update:modelValue', value)
})

const processedPrompt = computed(() => {
  if (!form.value.prompt) return ''
  
  // Replace variables with example values for preview
  const examples: Record<string, string> = {
    'PRODUCT': '无线耳机',
    'COLOR': '黑色',
    'STYLE': '现代简约',
    'BACKGROUND': '白色背景',
    'LIGHTING': '柔和自然光',
    'ANGLE': '45度角',
    'CAMERA': 'Canon EOS R5',
    'PLATFORM': '移动端',
    'UI_TYPE': '登录界面',
    'CHARACTER_TYPE': '魔法师',
    'HAIR_COLOR': '银色',
    'EYE_COLOR': '蓝色'
  }
  
  let processed = form.value.prompt
  Object.entries(examples).forEach(([key, value]) => {
    processed = processed.replace(new RegExp(`\\[${key}\\]`, 'g'), value)
  })
  
  return processed
})

// Options
const categoryOptions = [
  { title: '摄影', value: 'photography' },
  { title: '设计', value: 'design' },
  { title: '插画', value: 'illustration' },
  { title: '文字设计', value: 'typography' },
  { title: '其他', value: 'other' }
]

const difficultyOptions = [
  { title: '初级 - 适合新手使用', value: 'beginner' },
  { title: '中级 - 需要一定经验', value: 'intermediate' },
  { title: '高级 - 需要专业知识', value: 'advanced' }
]

// Validation Rules
const rules = {
  required: (value: any) => !!value || '此字段为必填项',
  promptLength: (value: string) => value.length >= 20 || '提示词模板至少需要 20 个字符',
  maxTags: (value: string[]) => value.length <= 8 || '最多只能添加 8 个标签',
  maxFiles: (value: File[] | null) => !value || value.length <= 3 || '最多只能上传 3 个文件'
}

// Methods
function generatePreview() {
  // Trigger reactivity to regenerate preview
  const currentPrompt = form.value.prompt
  form.value.prompt = ''
  setTimeout(() => {
    form.value.prompt = currentPrompt
  }, 0)
}

async function submitTemplate() {
  if (!formValid.value) return

  submitting.value = true
  try {
    // Mock API call
    await new Promise(resolve => setTimeout(resolve, 2000))
    
    notificationStore.success('模板分享成功！审核通过后将会显示在模板库中。')
    emit('created')
    dialog.value = false
    resetForm()
  } catch (error) {
    notificationStore.error('分享失败，请重试')
  } finally {
    submitting.value = false
  }
}

function resetForm() {
  form.value = {
    title: '',
    description: '',
    category: '',
    difficulty: '',
    prompt: '',
    tags: [],
    exampleFiles: null,
    agreeTerms: false
  }
}
</script>
