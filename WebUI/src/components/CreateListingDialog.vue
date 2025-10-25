<template>
  <v-dialog v-model="dialog" max-width="800" persistent>
    <v-card rounded="xl">
      <v-card-title class="pa-6 pb-0">
        <h2 class="text-h5 font-weight-bold">发布创意</h2>
        <p class="text-body-2 text-grey-darken-1 mt-1">分享你的工作流或工作空间，获得收益</p>
      </v-card-title>

      <v-card-text class="pa-6">
        <v-form ref="formRef" v-model="formValid">
          <!-- Type Selection -->
          <div class="mb-6">
            <h3 class="text-h6 mb-3">类型选择</h3>
            <v-btn-toggle v-model="form.type" mandatory color="primary" rounded="xl">
              <v-btn value="workflow" size="large">
                <v-icon start>mdi-workflow</v-icon>
                工作流
              </v-btn>
              <v-btn value="workspace" size="large">
                <v-icon start>mdi-folder-multiple</v-icon>
                工作空间
              </v-btn>
            </v-btn-toggle>
          </div>

          <!-- Basic Info -->
          <v-row>
            <v-col cols="12">
              <v-text-field
                v-model="form.title"
                label="标题 *"
                variant="outlined"
                :rules="[rules.required]"
                counter="50"
                maxlength="50"
              />
            </v-col>
            <v-col cols="12">
              <v-textarea
                v-model="form.description"
                label="描述 *"
                variant="outlined"
                :rules="[rules.required]"
                counter="200"
                maxlength="200"
                rows="3"
              />
            </v-col>
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
              <v-text-field
                v-model="form.tags"
                label="标签（用逗号分隔）"
                variant="outlined"
                placeholder="例如：摄影,产品,专业"
              />
            </v-col>
          </v-row>

          <!-- Cover Upload -->
          <div class="mb-6">
            <h3 class="text-h6 mb-3">封面图片</h3>
            <v-file-input
              v-model="form.coverFile"
              label="上传封面图片"
              variant="outlined"
              accept="image/*"
              prepend-icon="mdi-camera"
              @change="onCoverFileChange"
            />
            <v-img
              v-if="coverPreview"
              :src="coverPreview"
              aspect-ratio="16/9"
              max-height="200"
              class="mt-3 rounded-lg"
            />
          </div>

          <!-- Content Upload -->
          <div class="mb-6">
            <h3 class="text-h6 mb-3">{{ form.type === 'workflow' ? '工作流文件' : '工作空间文件' }}</h3>
            <v-file-input
              v-model="form.contentFile"
              :label="`上传${form.type === 'workflow' ? '工作流' : '工作空间'}文件`"
              variant="outlined"
              :accept="form.type === 'workflow' ? '.json,.yaml,.yml' : '.zip,.tar.gz'"
              prepend-icon="mdi-file-upload"
              :rules="[rules.required]"
            />
            <v-alert
              type="info"
              variant="tonal"
              class="mt-3"
            >
              {{ form.type === 'workflow' 
                ? '请上传 JSON 或 YAML 格式的工作流配置文件' 
                : '请上传包含完整工作空间的压缩包文件' }}
            </v-alert>
          </div>

          <!-- Pricing -->
          <div class="mb-6">
            <h3 class="text-h6 mb-3">定价设置</h3>
            <v-row>
              <v-col cols="12" md="6">
                <v-switch
                  v-model="form.isFree"
                  label="免费提供"
                  color="primary"
                  hide-details
                />
              </v-col>
              <v-col v-if="!form.isFree" cols="12" md="6">
                <v-text-field
                  v-model.number="form.price"
                  label="价格 (Credits)"
                  variant="outlined"
                  type="number"
                  min="1"
                  max="1000"
                  :rules="form.isFree ? [] : [rules.required, rules.minPrice]"
                  suffix="Credits"
                />
              </v-col>
            </v-row>
            
            <!-- AI Price Suggestion -->
            <v-card v-if="!form.isFree && aiPriceSuggestion" variant="tonal" color="info" class="mt-3">
              <v-card-text class="pa-4">
                <div class="d-flex align-center">
                  <v-icon class="mr-2">mdi-robot</v-icon>
                  <div>
                    <div class="font-weight-medium">AI 建议价格</div>
                    <div class="text-body-2">
                      基于内容复杂度和市场行情，建议定价 <strong>{{ aiPriceSuggestion }} Credits</strong>
                    </div>
                  </div>
                  <v-spacer />
                  <v-btn
                    size="small"
                    variant="outlined"
                    @click="form.price = aiPriceSuggestion"
                  >
                    采用建议
                  </v-btn>
                </div>
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
              我同意 <a href="#" class="text-primary">服务条款</a> 和 <a href="#" class="text-primary">创作者协议</a>
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
          @click="submitListing"
        >
          提交审核
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts" setup>
import { ref, computed, watch } from 'vue'
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
const coverPreview = ref<string | null>(null)
const aiPriceSuggestion = ref<number | null>(null)

const form = ref({
  type: 'workflow' as 'workflow' | 'workspace',
  title: '',
  description: '',
  category: '',
  tags: '',
  coverFile: null as File[] | null,
  contentFile: null as File[] | null,
  isFree: false,
  price: 10,
  agreeTerms: false
})

// Computed
const dialog = computed({
  get: () => props.modelValue,
  set: (value) => emit('update:modelValue', value)
})

// Options
const categoryOptions = [
  { title: '摄影', value: 'photography' },
  { title: '设计', value: 'design' },
  { title: '插画', value: 'illustration' },
  { title: '文字设计', value: 'typography' },
  { title: '其他', value: 'other' }
]

// Validation Rules
const rules = {
  required: (value: any) => !!value || '此字段为必填项',
  minPrice: (value: number) => value >= 1 || '价格不能低于 1 Credit'
}

// Methods
function onCoverFileChange() {
  if (form.value.coverFile && form.value.coverFile[0]) {
    const file = form.value.coverFile[0]
    const reader = new FileReader()
    reader.onload = (e) => {
      coverPreview.value = e.target?.result as string
    }
    reader.readAsDataURL(file)
  } else {
    coverPreview.value = null
  }
}

async function generateAIPriceSuggestion() {
  if (form.value.isFree || !form.value.title || !form.value.description) return
  
  try {
    // Mock AI price suggestion
    await new Promise(resolve => setTimeout(resolve, 1000))
    const basePrice = form.value.type === 'workspace' ? 25 : 15
    const complexity = form.value.description.length > 100 ? 1.2 : 1.0
    aiPriceSuggestion.value = Math.round(basePrice * complexity)
  } catch (error) {
    console.error('Failed to get AI price suggestion:', error)
  }
}

async function submitListing() {
  if (!formValid.value) return

  submitting.value = true
  try {
    // Mock API call
    await new Promise(resolve => setTimeout(resolve, 2000))
    
    notificationStore.success('提交成功！您的创意正在审核中，通常在24小时内完成。')
    emit('created')
    dialog.value = false
    resetForm()
  } catch (error) {
    notificationStore.error('提交失败，请重试')
  } finally {
    submitting.value = false
  }
}

function resetForm() {
  form.value = {
    type: 'workflow',
    title: '',
    description: '',
    category: '',
    tags: '',
    coverFile: null,
    contentFile: null,
    isFree: false,
    price: 10,
    agreeTerms: false
  }
  coverPreview.value = null
  aiPriceSuggestion.value = null
}

// Watch for AI price suggestion trigger
let debounceTimer: NodeJS.Timeout | null = null
watch([() => form.value.title, () => form.value.description, () => form.value.type], () => {
  if (debounceTimer) clearTimeout(debounceTimer)
  debounceTimer = setTimeout(() => {
    generateAIPriceSuggestion()
  }, 1000)
})

watch(() => form.value.isFree, (isFree) => {
  if (isFree) {
    aiPriceSuggestion.value = null
  } else {
    generateAIPriceSuggestion()
  }
})
</script>
