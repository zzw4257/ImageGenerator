<template>
  <v-sheet class="input-area">
    <div class="pt-4 pb-4">
      <div class="d-flex justify-space-between align-center input-area-actions">
        <div>
          <v-menu>
            <template v-slot:activator="{ props }">
              <v-btn class="upload-btn mr-2" icon="mdi-plus" size="small" variant="tonal" v-bind="props" />
            </template>
            <v-list rounded="lg" dense>
              <v-list-item @click="triggerUploadImage">Upload Image</v-list-item>
              <v-list-item @click="openLibraryDialog">Select From Library</v-list-item>
            </v-list>
          </v-menu>
          <v-btn class="upload-btn" icon="mdi-creation" size="small" variant="tonal" @click="openTemplateDialog" />
        </div>
        <v-btn 
          class="upload-btn" 
          variant="tonal" 
          icon="mdi-send" 
          size="small" 
          :loading="isGenerating"
          :disabled="!canGenerate"
          @click="emit('generate')" 
        />
      </div>
      <v-textarea v-model="modelValue" placeholder="请输入内容" variant="solo" flat rows="5" class="pa-0"
        bg-color="rgba(0,0,0,0)" hide-details hide-spin-buttons auto-grow />
      <!-- 费用提示 -->
      <div v-if="modelValue.trim()" class="px-4 pb-2">
        <v-alert
          :color="hasEnoughBalance ? 'primary-lighten-5' : 'error-lighten-5'"
          density="compact"
          variant="flat"
          rounded="lg"
        >
          <div class="d-flex align-center justify-space-between">
            <div class="d-flex align-center">
              <v-icon 
                size="16" 
                :color="hasEnoughBalance ? 'primary' : 'error'" 
                class="mr-2"
              >
                {{ hasEnoughBalance ? 'mdi-flash' : 'mdi-alert-circle' }}
              </v-icon>
              <span 
                :class="[
                  'text-caption font-weight-medium',
                  hasEnoughBalance ? 'text-primary' : 'text-error'
                ]"
              >
                本次操作预计消耗 {{ estimatedCredits }} Credits
              </span>
            </div>
            <span v-if="!hasEnoughBalance" class="text-caption text-error">
              余额不足
            </span>
            <span v-else class="text-caption text-grey-darken-1">
              余额: {{ walletBalance }} Credits
            </span>
          </div>
        </v-alert>
      </div>
    </div>
    <v-divider />
    <PromptTemplateDialog v-model="showTemplateDialog" @apply="onTemplateApplied" @close="showTemplateDialog = false" />
    <LibraryDialog v-model="showLibraryDialog" @select="(image) => emit('select-image', image)"
      @close="showLibraryDialog = false" />
    <div class="pa-4">
      <h4 class="text-subtitle-1 font-weight-bold mb-3">Reference Images</h4>
      <div v-if="images.length === 0" class="text-center py-6">
        <v-icon size="48" color="grey-lighten-2" class="mb-2">mdi-image-plus</v-icon>
        <p class="text-caption text-grey-darken-1">No images uploaded yet</p>
      </div>
      <div v-else class="uploaded-images">
        <SmoothPicture v-for="(image, index) in images" :key="index" :keep-aspect-ratio="false" rounded="lg"
          elevation="1" class="uploaded-image" :url="`/${image.imagePath}`" height="150" width="150">
          <template #default>
            <v-btn icon size="30px" class="ma-2 uploaded-image-remove" @click.stop="emit('remove-image', index)">
              <v-icon size="16">mdi-close</v-icon>
            </v-btn>
          </template>
          <template #placeholder>
            <div class="d-flex align-center justify-center fill-height">
              <v-progress-circular color="primary" indeterminate />
            </div>
          </template>
        </SmoothPicture>
      </div>
    </div>
  </v-sheet>
</template>

<script lang="ts" setup>
import { computed, onMounted, onUnmounted, ref } from 'vue'
import type { ImageDto } from '@/types/api';
import SmoothPicture from '../SmoothPicture.vue';
import { useNotificationStore } from '@/stores/notification';
import PromptTemplateDialog from '@/components/conversation/PromptTemplateDialog.vue'
import LibraryDialog from './LibraryDialog.vue';
import { getBalance } from '@/services/wallet';

const props = defineProps<{ images: ImageDto[]; isGenerating: boolean }>()

const noticationStore = useNotificationStore();
const model = defineModel<string>({ default: '' })
const modelValue = model
const emit = defineEmits<{
  (e: 'trigger-upload', image: File): void
  (e: 'select-image', image: ImageDto): void
  (e: 'generate'): void
  (e: 'remove-image', index: number): void
}>();

const walletBalance = ref(0)
const estimatedCredits = computed(() => {
  // 默认消耗 1 credit，如果有上传图片则可能是 image-to-image，消耗可能更多
  // 这里简化处理，统一显示 1 credit
  return 1
})

const hasEnoughBalance = computed(() => {
  return walletBalance.value >= estimatedCredits.value
})

const canGenerate = computed(() => {
  return modelValue.value.trim() && hasEnoughBalance.value && !props.isGenerating
})

async function loadWalletBalance() {
  try {
    const balance = await getBalance()
    walletBalance.value = balance.balance
  } catch (error) {
    console.error('加载余额失败:', error)
  }
}

let keydownHandler: ((e: KeyboardEvent) => void) | null = null

function triggerUploadImage() {
  const input = document.createElement("input");
  input.type = "file";
  input.accept = "image/jpeg, image/png, image/webp";
  input.onchange = async (e) => {
    const target = e.target as HTMLInputElement;
    if (target.files && target.files[0]) {
      const file = target.files[0];
      emit('trigger-upload', file)
    }
  };
  input.click();
}

const showTemplateDialog = ref(false)
const showLibraryDialog = ref(false)

function openLibraryDialog() {
  showLibraryDialog.value = true
}

function openTemplateDialog() {
  showTemplateDialog.value = true
}

function onTemplateApplied(payload: { finalPrompt: string }) {
  modelValue.value = payload.finalPrompt
  noticationStore.success('模板已应用', { icon: 'mdi-check-circle-outline' })
}

onMounted(async () => {
  await loadWalletBalance()
  keydownHandler = (e: KeyboardEvent) => {
    if (e.shiftKey && e.key === 'Enter') {
      if (canGenerate.value) {
        emit('generate')
      }
    }
  }
  document.addEventListener('keydown', keydownHandler)
})

onUnmounted(() => {
  if (keydownHandler) {
    document.removeEventListener('keydown', keydownHandler)
    keydownHandler = null
  }
})
</script>

<style scoped>
.input-area {
  background-color: rgba(var(--v-theme-on-surface), 0.08);
  padding: 10px;
  border-radius: 30px;
  position: relative;
}

.input-area-actions {
  padding: 10px;
  padding-top: 0;
}

.upload-btn:hover {
  transform: scale(1.05);
}

.uploaded-images {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
  flex-direction: row;
}

.uploaded-image {
  transition: transform 0.2s ease;
  position: relative;
  flex-shrink: 0;
  flex-grow: 0;
}

.uploaded-image:hover {
  transform: translateY(-1px);
}

.uploaded-image-remove {
  position: absolute;
  top: 0;
  right: 0;
}
</style>
