<template>
  <v-card rounded="xl" elevation="2" class="preset-card" @click="openPreset">
    <v-img :src="preset.coverUrl || '/images/default-preset.png'" aspect-ratio="16/9" cover />
    <v-card-text class="pa-4">
      <div class="d-flex justify-space-between align-center">
        <div>
          <div class="text-h6 font-weight-medium">{{ preset.name }}</div>
          <div class="text-caption text-grey-darken-1">{{ preset.description }}</div>
        </div>
        <v-chip color="primary" variant="outlined" size="small">{{ preset.priceCredits ?? 0 }} credits</v-chip>
      </div>
    </v-card-text>
  </v-card>
</template>

<script lang="ts" setup>
import { useRouter } from 'vue-router'
import type { PresetDto } from '@/services/presets'

const props = withDefaults(defineProps<{ preset: PresetDto }>(), { 
  preset: () => ({
    id: '',
    name: '',
    prompt: '',
    provider: 'Stub',
    priceCredits: 0
  } as PresetDto)
})
const router = useRouter()

function openPreset() {
  // 解析 defaultParams JSON 字符串
  let params = {}
  if (props.preset.defaultParams) {
    try {
      params = JSON.parse(props.preset.defaultParams)
    } catch (e) {
      console.warn('Failed to parse defaultParams:', props.preset.defaultParams)
    }
  }

  // 使用 query 传递必要字段：prompt, provider, params(JSON)
  const q: Record<string, string> = {
    prompt: encodeURIComponent(props.preset.prompt || ''),
    provider: encodeURIComponent(props.preset.provider || 'Stub'),
    params: encodeURIComponent(JSON.stringify(params))
  }
  router.push({ path: '/generate', query: q })
}
</script>

<style scoped>
.preset-card { cursor: pointer; transition: transform 0.12s; }
.preset-card:hover { transform: translateY(-4px); }
</style>