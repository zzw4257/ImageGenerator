<template>
  <v-container class="py-6">
    <v-row>
      <v-col cols="12">
        <h1 class="text-h4 font-weight-bold mb-2">AI 应用</h1>
        <p class="text-body-1 text-grey-darken-1">选择预设模板快速开始创作</p>
      </v-col>
    </v-row>

    <!-- Loading state -->
    <div v-if="loading" class="text-center py-12">
      <v-progress-circular color="primary" indeterminate size="64" />
      <p class="text-body-2 mt-4">加载预设模板中...</p>
    </div>

    <!-- Error state -->
    <div v-else-if="error" class="text-center py-12">
      <v-icon size="80" color="error" class="mb-4">mdi-alert-circle</v-icon>
      <h3 class="text-h6 mb-2">加载失败</h3>
      <p class="text-body-2 text-grey-darken-1 mb-4">{{ error }}</p>
      <v-btn color="primary" variant="outlined" @click="loadPresets">
        <v-icon start>mdi-refresh</v-icon>
        重试
      </v-btn>
    </div>

    <!-- Empty state -->
    <div v-else-if="presets.length === 0" class="text-center py-12">
      <v-icon size="80" color="grey-lighten-2" class="mb-4">mdi-image-outline</v-icon>
      <h3 class="text-h6 mb-2">暂无预设模板</h3>
      <p class="text-body-2 text-grey-darken-1">请联系管理员添加预设模板</p>
    </div>

    <!-- Presets grid -->
    <v-row v-else>
      <v-col
        v-for="preset in presets"
        :key="preset.id"
        cols="12"
        lg="3"
        md="4"
        sm="6"
      >
        <PresetCard :preset="preset" />
      </v-col>
    </v-row>

    <!-- Pagination -->
    <v-row v-if="presets.length > 0 && totalPages > 1" class="mt-6">
      <v-col cols="12" class="d-flex justify-center">
        <v-pagination
          v-model="currentPage"
          :length="totalPages"
          :total-visible="5"
          rounded="circle"
          @update:model-value="loadPresets"
        />
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts" setup>
  import { computed, onMounted, ref } from 'vue'
  import { usePresetStore } from '@/stores/presets'
  import PresetCard from '@/components/PresetCard.vue'
  import { useNotificationStore } from '@/stores/notification'

  const presetStore = usePresetStore()
  const notificationStore = useNotificationStore()

  const loading = ref(false)
  const error = ref<string | null>(null)
  const currentPage = ref(1)
  const totalPages = ref(1)

  const presets = computed(() => presetStore.items)

  async function loadPresets() {
    loading.value = true
    error.value = null
    try {
      await presetStore.fetchPresets(currentPage.value - 1, 12)
      totalPages.value = presetStore.pagination.TotalPages
    } catch (err: any) {
      error.value = err?.message || '加载预设模板失败'
      notificationStore.error('加载预设模板失败')
    } finally {
      loading.value = false
    }
  }

  onMounted(() => {
    loadPresets()
  })
</script>

<style scoped>
.preset-card {
  cursor: pointer;
  transition: all 0.3s ease;
}

.preset-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 25px rgba(0,0,0,0.15);
}

.h-100 {
  height: 100%;
}
</style>

<route lang="yaml">
meta:
  title: AI 图像生成
</route>
