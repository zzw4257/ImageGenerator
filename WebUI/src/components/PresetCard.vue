<template>
  <v-card rounded="xl" elevation="2" class="preset-card" @click="openPreset">
    <v-img :src="preset.cover || '/images/default-preset.png'" aspect-ratio="16/9" cover />
    <v-card-text class="pa-4">
      <div class="d-flex justify-space-between align-center">
        <div>
          <div class="text-h6 font-weight-medium">{{ preset.title }}</div>
          <div class="text-caption text-grey-darken-1">{{ preset.description }}</div>
        </div>
        <v-chip color="primary" variant="outlined" size="small">{{ preset.cost ?? 0 }} credits</v-chip>
      </div>
    </v-card-text>
  </v-card>
</template>

<script lang="ts" setup>
import { useRouter } from 'vue-router'

const props = withDefaults(defineProps<{ preset: any }>(), { preset: {} })
const router = useRouter()

function openPreset() {
  // 使用 query 传递必要字段：prompt, provider, params(JSON)
  const q: Record<string, string> = {
    prompt: encodeURIComponent(props.preset.prompt || ''),
    provider: encodeURIComponent(props.preset.provider || 'gemini'),
    params: encodeURIComponent(JSON.stringify(props.preset.params || {}))
  }
  router.push({ path: '/generate', query: q })
}
</script>

<style scoped>
.preset-card { cursor: pointer; transition: transform 0.12s; }
.preset-card:hover { transform: translateY(-4px); }
</style>