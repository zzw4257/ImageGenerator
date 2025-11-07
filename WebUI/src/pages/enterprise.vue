<template>
  <v-container class="py-8 enterprise-page">
    <v-row class="align-center mb-10" dense>
      <v-col cols="12" md="7">
        <div class="mb-4">
          <h1 class="text-h3 font-weight-bold mb-3">为企业级创意交付打造生成式工作流</h1>
          <p class="text-body-1 text-grey-darken-1">
            Aetherflow Enterprise 将生成式 AI 与创意项目流程深度整合，帮助品牌在多渠道场景下实现高质量、可控的视觉生产。
          </p>
        </div>
        <div class="d-flex flex-wrap gap-3">
          <v-btn
            color="primary"
            :loading="contactLoading"
            prepend-icon="mdi-account-tie"
            rounded="lg"
            size="large"
            variant="flat"
            @click="openContact"
          >
            联系销售
          </v-btn>

          <v-btn
            color="primary"
            prepend-icon="mdi-calendar-clock"
            rounded="lg"
            size="large"
            variant="outlined"
            @click="openDemo"
          >
            预约演示
          </v-btn>
        </div>
      </v-col>
      <v-col cols="12" md="5">
        <v-sheet class="pa-6 enterprise-hero-card" color="surface" elevation="2" rounded="xl">
          <div class="d-flex align-center gap-3 mb-4">
            <v-avatar color="primary" size="48">
              <v-icon color="white">mdi-timeline-clock</v-icon>
            </v-avatar>
            <div>
              <div class="text-subtitle-1 font-weight-semibold">交付速度提升</div>
              <div class="text-caption text-grey-darken-2">
                自研自动化流程使项目平均交付时间缩短 42%
              </div>
            </div>
          </div>
          <v-divider class="mb-4" />
          <div class="d-flex justify-space-between text-center">
            <div>
              <div class="text-h4 font-weight-bold mb-1">92%</div>
              <div class="text-caption text-grey-darken-1">客户满意度</div>
            </div>
            <div>
              <div class="text-h4 font-weight-bold mb-1">38+</div>
              <div class="text-caption text-grey-darken-1">行业场景</div>
            </div>
            <div>
              <div class="text-h4 font-weight-bold mb-1">7x</div>
              <div class="text-caption text-grey-darken-1">素材复用率</div>
            </div>
          </div>
        </v-sheet>
      </v-col>
    </v-row>

    <v-row class="mb-10" dense>
      <v-col
        v-for="capability in capabilities"
        :key="capability.title"
        cols="12"
        lg="4"
        md="6"
      >
        <v-hover v-slot="{ isHovering, props }">
          <v-card
            v-bind="props"
            class="h-100 capability-card"
            :elevation="isHovering ? 8 : 2"
            rounded="xl"
          >
            <v-card-text class="pa-6">
              <v-avatar class="mb-4" :color="capability.color" size="48">
                <v-icon color="white">{{ capability.icon }}</v-icon>
              </v-avatar>
              <div class="text-subtitle-1 font-weight-semibold mb-2">
                {{ capability.title }}
              </div>
              <p class="text-body-2 text-grey-darken-1 mb-4">
                {{ capability.description }}
              </p>
              <div class="text-caption text-grey-darken-1">
                {{ capability.note }}
              </div>
            </v-card-text>
          </v-card>
        </v-hover>
      </v-col>
    </v-row>

    <v-row class="mb-10" dense>
      <v-col cols="12" md="7">
        <v-sheet class="pa-6 h-100" color="surface" elevation="2" rounded="xl">
          <h2 class="text-h5 font-weight-medium mb-4">端到端企业交付流程</h2>
          <v-timeline align="start" side="end">
            <v-timeline-item
              v-for="stage in workflowStages"
              :key="stage.title"
              dot-color="primary"
              size="small"
            >
              <template #opposite>
                <div class="text-caption text-grey-darken-1">
                  {{ stage.duration }}
                </div>
              </template>

              <v-card class="pa-4" rounded="xl">
                <div class="d-flex align-center gap-2 mb-2">
                  <v-avatar :color="stage.color" size="32">
                    <v-icon color="white">{{ stage.icon }}</v-icon>
                  </v-avatar>
                  <div class="text-subtitle-1 font-weight-semibold">
                    {{ stage.title }}
                  </div>
                </div>
                <p class="text-body-2 text-grey-darken-1 mb-2">
                  {{ stage.description }}
                </p>
                <div class="d-flex flex-wrap gap-2">
                  <v-chip
                    v-for="tag in stage.highlights"
                    :key="tag"
                    color="primary-lighten-4"
                    size="x-small"
                    variant="flat"
                  >
                    #{{ tag }}
                  </v-chip>
                </div>
              </v-card>
            </v-timeline-item>
          </v-timeline>
        </v-sheet>
      </v-col>

      <v-col cols="12" md="5">
        <v-sheet class="pa-6 h-100" color="surface" elevation="2" rounded="xl">
          <h2 class="text-h6 font-weight-medium mb-4">适配业务场景</h2>
          <v-expansion-panels variant="accordion">
            <v-expansion-panel
              v-for="scenario in businessScenarios"
              :key="scenario.title"
            >
              <v-expansion-panel-title>
                <div class="d-flex align-center gap-3">
                  <v-avatar :color="scenario.color" size="32">
                    <v-icon color="white">{{ scenario.icon }}</v-icon>
                  </v-avatar>
                  <div>
                    <div class="text-body-1 font-weight-medium">{{ scenario.title }}</div>
                    <div class="text-caption text-grey-darken-1">{{ scenario.subtitle }}</div>
                  </div>
                </div>
              </v-expansion-panel-title>
              <v-expansion-panel-text>
                <ul class="text-body-2 text-grey-darken-1 pl-4">
                  <li v-for="point in scenario.points" :key="point" class="mb-2">
                    {{ point }}
                  </li>
                </ul>
              </v-expansion-panel-text>
            </v-expansion-panel>
          </v-expansion-panels>
        </v-sheet>
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12">
        <v-sheet class="pa-7" color="primary" elevation="2" rounded="xl">
          <div class="d-flex flex-column flex-md-row align-md-center justify-space-between gap-4">
            <div>
              <div class="text-h5 font-weight-bold text-white mb-2">
                准备好升级企业创意团队的生产力了吗？
              </div>
              <div class="text-body-2 text-white text-opacity-80">
                提交需求后，顾问将在 24 小时内与您联系，提供个性化方案与演示。
              </div>
            </div>

            <div class="d-flex flex-column flex-sm-row gap-3">
              <v-btn
                color="white"
                :loading="contactLoading"
                prepend-icon="mdi-account-tie"
                rounded="lg"
                variant="flat"
                @click="openContact"
              >
                联系销售
              </v-btn>
              <v-btn
                class="text-primary"
                color="white"
                prepend-icon="mdi-calendar-clock"
                rounded="lg"
                variant="outlined"
                @click="openDemo"
              >
                预约演示
              </v-btn>
            </div>
          </div>
        </v-sheet>
      </v-col>
    </v-row>

    <v-dialog v-model="contactDialog" max-width="420">
      <v-card rounded="xl">
        <v-card-title class="text-h6 font-weight-semibold">
          已收到您的联系意向
        </v-card-title>
        <v-card-text class="text-body-2 text-grey-darken-1">
          我们的企业顾问会在 1 个工作日内联系您，了解业务需求并提供初步方案。若需加急，请在备注中补充更多信息。
        </v-card-text>
        <v-card-actions class="justify-end">
          <v-btn color="primary" rounded="lg" variant="flat" @click="closeContact">
            确定
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-dialog v-model="demoDialog" max-width="520">
      <v-card rounded="xl">
        <v-card-title class="text-h6 font-weight-semibold">
          预约企业演示
        </v-card-title>
        <v-card-text>
          <v-form ref="demoFormRef" @submit.prevent="submitDemo">
            <v-text-field
              v-model="demoForm.company"
              class="mb-3"
              label="公司/品牌名称"
              placeholder="请输入您的公司名称"
              required
              variant="outlined"
            />
            <v-text-field
              v-model="demoForm.contact"
              class="mb-3"
              label="联系人"
              placeholder="请填写联系人姓名"
              required
              variant="outlined"
            />
            <v-text-field
              v-model="demoForm.email"
              class="mb-3"
              label="邮箱"
              placeholder="用于接收演示日程"
              required
              type="email"
              variant="outlined"
            />
            <v-select
              v-model="demoForm.intent"
              class="mb-3"
              :items="intentOptions"
              label="重点关注的业务场景"
              variant="outlined"
            />
            <v-textarea
              v-model="demoForm.notes"
              class="mb-1"
              label="补充需求"
              placeholder="例如希望了解的内容、当前使用的工具或项目时间表"
              rows="3"
              variant="outlined"
            />
          </v-form>
          <v-alert
            v-if="demoSuccess"
            class="mt-4"
            type="success"
            variant="tonal"
          >
            已收到您的预约，我们会在 24 小时内与您确认演示安排。
          </v-alert>
        </v-card-text>
        <v-card-actions class="justify-end">
          <v-btn rounded="lg" variant="text" @click="closeDemo">
            取消
          </v-btn>
          <v-btn
            color="primary"
            :loading="demoSubmitting"
            rounded="lg"
            variant="flat"
            @click="submitDemo"
          >
            提交预约
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-container>
</template>

<script lang="ts" setup>
  import { reactive, ref } from 'vue'

  interface Capability {
    title: string
    description: string
    note: string
    icon: string
    color: string
  }

  interface WorkflowStage {
    title: string
    description: string
    duration: string
    icon: string
    highlights: string[]
    color: string
  }

  interface BusinessScenario {
    title: string
    subtitle: string
    icon: string
    color: string
    points: string[]
  }

  interface DemoForm {
    company: string
    contact: string
    email: string
    intent: string
    notes: string
  }

  const contactDialog = ref(false)
  const demoDialog = ref(false)
  const contactLoading = ref(false)
  const demoSubmitting = ref(false)
  const demoSuccess = ref(false)
  const demoFormRef = ref()
  const demoForm = reactive<DemoForm>({
    company: '',
    contact: '',
    email: '',
    intent: '品牌营销与活动',
    notes: '',
  })

  const intentOptions = [
    '品牌营销与活动',
    '企业内训与知识库',
    '线下展览与空间体验',
    '产品可视化与电商上新',
  ]

  const capabilities: Capability[] = [
    {
      title: '品牌一致性守护',
      description: '通过品牌语料导入与多模态风格对齐，确保跨渠道资产保持统一语气与视觉识别。',
      note: '提供品牌审核、自动化水印与合规检测。',
      icon: 'mdi-shield-check',
      color: 'primary',
    },
    {
      title: '协同化创作看板',
      description: '将策划、设计、审稿等流程整合到统一工作台，实时跟踪版本与反馈。',
      note: '支持 Jira、Feishu、Slack 等协作工具集成。',
      icon: 'mdi-view-dashboard-edit',
      color: 'deep-purple-accent-2',
    },
    {
      title: '多地区合规部署',
      description: '支持本地化部署与私有化部署选项，满足金融、医疗等行业的安全合规要求。',
      note: '通过 ISO/IEC 27001 与等保三级审计。',
      icon: 'mdi-shield-lock',
      color: 'teal',
    },
    {
      title: '资产智能复用',
      description: '自动标注素材标签与使用场景，从资产库一键生成面向不同渠道的适配方案。',
      note: '提供多分辨率导出与自动排版建议。',
      icon: 'mdi-sync-circle',
      color: 'orange-accent-2',
    },
    {
      title: '定制模型训练',
      description: '结合企业数据构建专属模型，针对品牌调性与产品特性进行高精度训练。',
      note: '包含数据清洗、提示词工程与持续迭代服务。',
      icon: 'mdi-brain',
      color: 'indigo',
    },
    {
      title: '绩效与洞察分析',
      description: '实时回收渠道表现数据，帮助团队洞察素材表现并自动推荐下一轮内容策略。',
      note: '支持 ROI、转化率与互动指标追踪。',
      icon: 'mdi-chart-pie',
      color: 'cyan',
    },
  ]

  const workflowStages: WorkflowStage[] = [
    {
      title: '需求洞察与策略同步',
      description: '顾问团队与业务方共同梳理内容需求、渠道组合与品牌指引，输出联合执行清单。',
      duration: '第 1 天',
      icon: 'mdi-headset',
      highlights: ['需求梳理', '品牌指引导入'],
      color: 'primary',
    },
    {
      title: '内容生成与多人协作',
      description: '多模态生成与模板化组件同步输出，团队成员可在同一工作流中快速迭代。',
      duration: '第 2-3 天',
      icon: 'mdi-account-group',
      highlights: ['实时协作', '提示词版本管理'],
      color: 'deep-purple-accent-2',
    },
    {
      title: '审核合规与资产管理',
      description: '自动化审核流程保障合规性，最终素材将进入企业资产库并自动打上标签。',
      duration: '第 3 天',
      icon: 'mdi-shield-check',
      highlights: ['合规模板', '资产归档'],
      color: 'teal',
    },
    {
      title: '多渠道分发与复盘',
      description: '一键产出适配不同渠道规格的素材，后台追踪表现指标，为下一轮内容提供数据支撑。',
      duration: '第 4 天',
      icon: 'mdi-send-check',
      highlights: ['跨渠道发布', '绩效洞察'],
      color: 'cyan',
    },
  ]

  const businessScenarios: BusinessScenario[] = [
    {
      title: '品牌营销与活动运营',
      subtitle: 'Campaign 视觉、社媒素材、活动主视觉',
      icon: 'mdi-bullhorn',
      color: 'pink-accent-2',
      points: [
        '面向社交平台生成多尺寸动态素材',
        '实时监测互动反馈并自动调整模板',
        '支持线下活动周边与物料打版',
      ],
    },
    {
      title: '零售与电商',
      subtitle: '新品发布、SKU 大规模拍摄替代',
      icon: 'mdi-cart-check',
      color: 'amber',
      points: [
        '批量生成商品场景图与主题海报',
        '自动适配各大平台详情页与广告位尺寸',
        '根据销量数据推荐下一批主推视觉',
      ],
    },
    {
      title: '企业培训与知识库',
      subtitle: '内训教材、知识库可视化',
      icon: 'mdi-school',
      color: 'light-blue-accent-3',
      points: [
        '生成课程配图与教学场景模拟',
        '将知识库内容转化为动态演示素材',
        '支持多语言输出与字幕生成',
      ],
    },
    {
      title: '工业与制造',
      subtitle: '产品展示、方案标书、可视化汇报',
      icon: 'mdi-factory',
      color: 'deep-orange',
      points: [
        '实现复杂设备的 3D 可视化介绍',
        '自动化生成招投标材料中的视觉页',
        '提供多角度场景化解决方案演示',
      ],
    },
  ]

  const wait = (duration: number) => new Promise(resolve => setTimeout(resolve, duration))

  async function openContact () {
    contactLoading.value = true
    await wait(260)
    contactLoading.value = false
    contactDialog.value = true
  }

  function closeContact () {
    contactDialog.value = false
  }

  function openDemo () {
    demoDialog.value = true
    demoSuccess.value = false
  }

  function closeDemo () {
    demoDialog.value = false
    demoSubmitting.value = false
  }

  async function submitDemo () {
    if (!demoForm.company || !demoForm.contact || !demoForm.email) {
      demoSuccess.value = false
      return
    }
    demoSubmitting.value = true
    await wait(420)
    demoSubmitting.value = false
    demoSuccess.value = true
  }
</script>

<style scoped>
.enterprise-page .enterprise-hero-card {
  border: 1px solid rgba(var(--v-theme-primary), 0.12);
  background:
    linear-gradient(135deg, rgba(var(--v-theme-primary), 0.08), transparent),
    rgb(var(--v-theme-surface));
}

.capability-card {
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}

.capability-card:hover {
  transform: translateY(-4px);
}

.gap-3 {
  gap: 12px;
}

.gap-4 {
  gap: 16px;
}
</style>

<route lang="yaml">
meta:
  layout: default
  title: 企业方案
</route>
