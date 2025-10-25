<template>
  <v-container fluid class="pa-8">
    <div class="d-flex justify-space-between align-center mb-6">
      <div>
        <h1 class="text-h4 font-weight-bold">工作流编辑器</h1>
        <p class="text-body-1 text-grey-darken-1 mt-2">拖拽式可视化 AIGC 工作流设计</p>
      </div>
      <div class="d-flex gap-2">
        <v-btn variant="outlined" prepend-icon="mdi-content-save">
          保存
        </v-btn>
        <v-btn color="primary" prepend-icon="mdi-play">
          运行
        </v-btn>
      </div>
    </div>

    <v-row class="h-100">
      <!-- Node Palette -->
      <v-col cols="12" md="3">
        <v-card elevation="2" rounded="xl" class="h-100">
          <v-card-title class="pa-4">
            <v-icon class="mr-2">mdi-puzzle</v-icon>
            节点库
          </v-card-title>
          <v-divider />
          <v-card-text class="pa-4">
            <v-expansion-panels variant="accordion">
              <v-expansion-panel
                v-for="category in nodeCategories"
                :key="category.title"
                :title="category.title"
              >
                <v-expansion-panel-text>
                  <div class="d-flex flex-column gap-2">
                    <v-card
                      v-for="node in category.nodes"
                      :key="node.type"
                      variant="outlined"
                      rounded="lg"
                      class="pa-3 cursor-pointer node-item"
                      @click="addNode(node)"
                    >
                      <div class="d-flex align-center">
                        <v-icon :color="node.color" class="mr-2">{{ node.icon }}</v-icon>
                        <div>
                          <div class="text-body-2 font-weight-medium">{{ node.name }}</div>
                          <div class="text-caption text-grey">{{ node.description }}</div>
                        </div>
                      </div>
                    </v-card>
                  </div>
                </v-expansion-panel-text>
              </v-expansion-panel>
            </v-expansion-panels>
          </v-card-text>
        </v-card>
      </v-col>

      <!-- Canvas Area -->
      <v-col cols="12" md="6">
        <v-card elevation="2" rounded="xl" class="h-100 position-relative">
          <v-card-title class="pa-4">
            <v-icon class="mr-2">mdi-graph</v-icon>
            工作流画布
          </v-card-title>
          <v-divider />
          <div class="canvas-area pa-4" style="height: 600px; background: #fafafa;">
            <!-- Placeholder Canvas -->
            <div class="d-flex align-center justify-center h-100">
              <div class="text-center">
                <v-icon size="80" color="grey-lighten-2" class="mb-4">mdi-graph-outline</v-icon>
                <h3 class="text-h6 mb-2 text-grey">工作流画布</h3>
                <p class="text-body-2 text-grey-darken-1">从左侧拖拽节点开始构建工作流</p>
              </div>
            </div>
          </div>
        </v-card>
      </v-col>

      <!-- Properties Panel -->
      <v-col cols="12" md="3">
        <v-card elevation="2" rounded="xl" class="h-100">
          <v-card-title class="pa-4">
            <v-icon class="mr-2">mdi-cog</v-icon>
            属性面板
          </v-card-title>
          <v-divider />
          <v-card-text class="pa-4">
            <div v-if="!selectedNode" class="text-center py-8">
              <v-icon size="48" color="grey-lighten-2" class="mb-3">mdi-cursor-default-click</v-icon>
              <p class="text-body-2 text-grey">选择节点查看属性</p>
            </div>
            
            <div v-else>
              <h4 class="text-h6 mb-3">{{ selectedNode.name }}</h4>
              <v-form>
                <v-text-field
                  v-model="selectedNode.label"
                  label="节点名称"
                  variant="outlined"
                  density="compact"
                  class="mb-3"
                />
                <v-textarea
                  v-model="selectedNode.prompt"
                  label="提示词"
                  variant="outlined"
                  density="compact"
                  rows="3"
                  class="mb-3"
                />
                <v-select
                  v-model="selectedNode.model"
                  :items="modelOptions"
                  label="AI 模型"
                  variant="outlined"
                  density="compact"
                  class="mb-3"
                />
              </v-form>
            </div>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <!-- Bottom Toolbar -->
    <v-card class="mt-6" variant="outlined" rounded="xl">
      <v-card-text class="pa-4">
        <div class="d-flex align-center justify-space-between">
          <div class="d-flex align-center gap-4">
            <v-btn size="small" variant="outlined" prepend-icon="mdi-undo">
              撤销
            </v-btn>
            <v-btn size="small" variant="outlined" prepend-icon="mdi-redo">
              重做
            </v-btn>
            <v-divider vertical />
            <v-btn size="small" variant="outlined" prepend-icon="mdi-magnify-plus">
              放大
            </v-btn>
            <v-btn size="small" variant="outlined" prepend-icon="mdi-magnify-minus">
              缩小
            </v-btn>
          </div>
          
          <div class="d-flex align-center gap-2">
            <v-chip size="small" variant="outlined">
              节点: {{ nodeCount }}
            </v-chip>
            <v-chip size="small" variant="outlined">
              连接: {{ connectionCount }}
            </v-chip>
            <v-chip size="small" :color="isValid ? 'success' : 'error'" variant="flat">
              {{ isValid ? '有效' : '无效' }}
            </v-chip>
          </div>
        </div>
      </v-card-text>
    </v-card>
  </v-container>
</template>

<script lang="ts" setup>
import { ref, computed } from 'vue'

interface WorkflowNode {
  type: string
  name: string
  description: string
  icon: string
  color: string
  label?: string
  prompt?: string
  model?: string
}

// State
const selectedNode = ref<WorkflowNode | null>(null)
const nodeCount = ref(0)
const connectionCount = ref(0)

// Node Categories
const nodeCategories = ref([
  {
    title: '输入节点',
    nodes: [
      {
        type: 'text-input',
        name: '文本输入',
        description: '输入提示词文本',
        icon: 'mdi-text-box',
        color: 'blue'
      },
      {
        type: 'image-input',
        name: '图片输入',
        description: '上传参考图片',
        icon: 'mdi-image',
        color: 'green'
      }
    ]
  },
  {
    title: 'AI 生成',
    nodes: [
      {
        type: 'text-to-image',
        name: '文生图',
        description: 'AI 文本生成图片',
        icon: 'mdi-creation',
        color: 'purple'
      },
      {
        type: 'image-to-image',
        name: '图生图',
        description: 'AI 图片转换',
        icon: 'mdi-image-edit',
        color: 'orange'
      }
    ]
  },
  {
    title: '处理节点',
    nodes: [
      {
        type: 'resize',
        name: '尺寸调整',
        description: '调整图片尺寸',
        icon: 'mdi-resize',
        color: 'teal'
      },
      {
        type: 'filter',
        name: '滤镜效果',
        description: '应用图片滤镜',
        icon: 'mdi-filter',
        color: 'pink'
      }
    ]
  },
  {
    title: '输出节点',
    nodes: [
      {
        type: 'output',
        name: '结果输出',
        description: '输出最终结果',
        icon: 'mdi-export',
        color: 'red'
      }
    ]
  }
])

const modelOptions = [
  'Qwen',
  'Flux',
  'OpenAI DALL-E',
  'Stable Diffusion'
]

// Computed
const isValid = computed(() => {
  return nodeCount.value > 0 && connectionCount.value >= 0
})

// Methods
function addNode(node: WorkflowNode) {
  nodeCount.value++
  selectedNode.value = { ...node, label: node.name }
}
</script>

<route lang="yaml">
meta:
  title: 工作流编辑器
</route>

<style scoped>
.canvas-area {
  border: 2px dashed #e0e0e0;
  border-radius: 12px;
}

.node-item:hover {
  background-color: rgba(var(--v-theme-primary), 0.05);
  border-color: rgb(var(--v-theme-primary));
}

.cursor-pointer {
  cursor: pointer;
}
</style>
