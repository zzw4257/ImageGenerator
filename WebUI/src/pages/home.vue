<template>
  <v-container class="py-6">
    <v-row>
      <v-col cols="12">
        <h1 class="text-h4 font-weight-bold mb-2">AI 应用</h1>
        <p class="text-body-1 text-grey-darken-1">选择预设模板快速开始创作</p>
      </v-col>
    </v-row>

    <v-row>
      <v-col
        v-for="preset in presets"
        :key="preset.id"
        cols="12"
        lg="3"
        md="4"
        sm="6"
      >
        <v-card
          class="h-100 preset-card"
          elevation="2"
          rounded="xl"
          @click="usePreset(preset)"
        >
          <v-img
            :alt="preset.title"
            class="rounded-t-xl"
            cover
            height="200"
            :src="preset.cover"
          />

          <v-card-text class="pa-4">
            <div class="d-flex align-center justify-space-between mb-2">
              <h3 class="text-h6 font-weight-medium">{{ preset.title }}</h3>
              <v-chip
                :color="getModelColor(preset.model)"
                size="small"
                variant="flat"
              >
                {{ preset.model }}
              </v-chip>
            </div>

            <p class="text-body-2 text-grey-darken-2 mb-3">
              {{ preset.description }}
            </p>

            <div class="d-flex align-center justify-space-between">
              <div class="d-flex align-center">
                <v-icon
                  class="mr-1"
                  color="primary"
                  size="16"
                >mdi-flash</v-icon>
                <span class="text-caption font-weight-medium">
                  {{ preset.credits }} Credits
                </span>
              </div>
              <v-icon
                color="primary"
                size="20"
              >mdi-arrow-right</v-icon>
            </div>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts" setup>
  import { ref } from 'vue'
  import { useRouter } from 'vue-router'

  interface Preset {
    id: string
    title: string
    description: string
    cover: string
    model: string
    credits: number
    prompt: string
    params: {
      resolution: string
      style?: string
    }
  }

  const router = useRouter()

  const presets = ref<Preset[]>([
    {
      id: '1',
      title: '梦幻风景',
      description: '生成梦幻般的自然风景图像',
      cover: '/images/presets/landscape.jpg',
      model: 'Flux',
      credits: 2,
      prompt: 'A dreamy landscape with mountains, lakes, and mist, fantasy style, highly detailed',
      params: {
        resolution: '1024x1024',
        style: 'fantasy',
      },
    },
    {
      id: '2',
      title: '肖像艺术',
      description: '艺术风格的人物肖像生成',
      cover: '/images/presets/portrait.jpg',
      model: 'Qwen',
      credits: 3,
      prompt: 'Professional portrait photography, cinematic lighting, detailed facial features',
      params: {
        resolution: '768x1024',
        style: 'cinematic',
      },
    },
    {
      id: '3',
      title: '抽象艺术',
      description: '创造独特的抽象艺术作品',
      cover: '/images/presets/abstract.jpg',
      model: 'Stub',
      credits: 1,
      prompt: 'Abstract geometric patterns, vibrant colors, modern art style',
      params: {
        resolution: '1024x1024',
        style: 'abstract',
      },
    },
    {
      id: '4',
      title: '产品设计',
      description: '产品概念设计和展示',
      cover: '/images/presets/product.jpg',
      model: 'Flux',
      credits: 2,
      prompt: 'Product design visualization, clean background, professional lighting',
      params: {
        resolution: '1024x1024',
        style: 'professional',
      },
    },
  ])

  function getModelColor (model: string) {
    const colors = {
      Flux: 'purple-lighten-4',
      Qwen: 'blue-lighten-4',
      Stub: 'green-lighten-4',
    }
    return colors[model as keyof typeof colors] || 'grey-lighten-3'
  }

  function usePreset (preset: Preset) {
    router.push({
      path: '/generate',
      query: {
        preset: preset.id,
        prompt: preset.prompt,
        model: preset.model,
        resolution: preset.params.resolution,
        style: preset.params.style,
      },
    })
  }
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
