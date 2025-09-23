<template>
  <v-sheet class="input-area">
    <div class="pt-4 pb-4">
      <div class="d-flex justify-space-between align-center input-area-actions">
        <div>
          <v-btn class="upload-btn mr-2" icon="mdi-plus" size="small" variant="tonal" @click="uploadImage" />
          <v-btn class="upload-btn" icon="mdi-creation" size="small" variant="tonal" @click="createPrompt"/>
        </div>
        <v-btn class="upload-btn" variant="tonal" icon="mdi-send" size="small" :loading="isGenerating" @click="emit('generate')" />
      </div>
      <v-textarea v-model="modelValue" placeholder="请输入内容" variant="solo" flat rows="5" class="pa-0"
        bg-color="rgba(0,0,0,0)" hide-details hide-spin-buttons auto-grow />
    </div>
    <v-divider />
    <div class="pa-4">
      <h4 class="text-subtitle-1 font-weight-bold mb-3">Reference Images</h4>
      <div v-if="images.length === 0" class="text-center py-6">
        <v-icon size="48" color="grey-lighten-2" class="mb-2">mdi-image-plus</v-icon>
        <p class="text-caption text-grey-darken-1">No images uploaded yet</p>
      </div>
      <div v-else class="uploaded-images">
        <SmoothPicture v-for="(image, index) in images" :key="index" aspect-ratio="1" rounded="lg" elevation="1" class="uploaded-image" :url="`/${image.imagePath}`" height="150" width="150" cover>
          <template #default>
            <v-btn icon size="30px" class="ma-2 uploaded-image-remove" @click.stop="emit('remove-image', index)">
              <v-icon size="16">mdi-close</v-icon>
            </v-btn>
          </template>
        </SmoothPicture>
      </div>
    </div>
  </v-sheet>
</template>

<script lang="ts" setup>
import type { ImageDto } from '@/types/api';
import SmoothPicture from '../SmoothPicture.vue';
import { useNotificationStore } from '@/stores/notification';

defineProps<{ images: ImageDto[]; isGenerating: boolean }>()

const noticationStore = useNotificationStore();
const model = defineModel<string>({ default: '' })
const modelValue = model
const emit = defineEmits<{
  (e: 'trigger-upload', file: File): void
  (e: 'generate'): void
  (e: 'remove-image', index: number): void
}>();
let keydownHandler: ((e: KeyboardEvent) => void) | null = null

function uploadImage() {
  const input = document.createElement("input");
  input.type = "file";
  input.accept = "image/jpeg, image/png";
  input.onchange = (e) => {
    const target = e.target as HTMLInputElement;
    if (target.files && target.files[0]) {
      const file = target.files[0];
      emit('trigger-upload', file);
    }
  };
  input.click();
}

function createPrompt() {
  noticationStore.info('Feature coming soon!',{
    icon: 'mdi-alert-circle-outline'
  })
}

onMounted(() => {
  keydownHandler = (e: KeyboardEvent) => {
    if (e.shiftKey && e.key === 'Enter') {
      emit('generate')
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
.input-area { background-color: rgba(var(--v-theme-on-surface),0.08); padding: 10px; border-radius: 30px; position: relative; }
.input-area-actions { padding: 10px; padding-top: 0; }
.upload-btn:hover { transform: scale(1.05); }
.uploaded-images { display: flex; flex-wrap: wrap; gap: 10px;flex-direction: row;}
.uploaded-image { transition: transform 0.2s ease; position: relative;flex-shrink: 0;flex-grow: 0;}
.uploaded-image:hover { transform: translateY(-1px); }
.uploaded-image-remove { position: absolute; top: 0; right: 0; }
</style>
