<template>
  <v-container fluid class="pa-8">
    <!-- Hero Section -->
    <div class="text-center mb-12">
      <h1 class="text-h3 font-weight-bold mb-4">企业级 AIGC 解决方案</h1>
      <p class="text-h6 text-grey-darken-1 mb-6">为您的企业提供专业、安全、可扩展的 AI 创作服务</p>
      <div class="d-flex justify-center gap-4">
        <v-btn color="primary" size="large" rounded="xl" prepend-icon="mdi-phone">
          联系销售
        </v-btn>
        <v-btn variant="outlined" size="large" rounded="xl" prepend-icon="mdi-play-circle">
          预约演示
        </v-btn>
      </div>
    </div>

    <!-- Features Grid -->
    <v-row class="mb-12">
      <v-col v-for="feature in enterpriseFeatures" :key="feature.title" cols="12" md="6" lg="3">
        <v-card elevation="2" rounded="xl" class="h-100 text-center pa-6">
          <v-icon :color="feature.color" size="64" class="mb-4">{{ feature.icon }}</v-icon>
          <h3 class="text-h6 font-weight-bold mb-3">{{ feature.title }}</h3>
          <p class="text-body-2 text-grey-darken-1">{{ feature.description }}</p>
        </v-card>
      </v-col>
    </v-row>

    <!-- Solutions -->
    <div class="mb-12">
      <h2 class="text-h4 font-weight-bold text-center mb-8">解决方案</h2>
      <v-row>
        <v-col v-for="solution in solutions" :key="solution.title" cols="12" md="6">
          <v-card elevation="2" rounded="xl" class="h-100">
            <v-img
              :src="solution.image"
              height="200"
              cover
              class="rounded-t-xl"
            />
            <v-card-text class="pa-6">
              <h3 class="text-h5 font-weight-bold mb-3">{{ solution.title }}</h3>
              <p class="text-body-1 text-grey-darken-1 mb-4">{{ solution.description }}</p>
              <v-list density="compact">
                <v-list-item v-for="feature in solution.features" :key="feature" class="px-0">
                  <template #prepend>
                    <v-icon color="success" size="16">mdi-check-circle</v-icon>
                  </template>
                  <v-list-item-title class="text-body-2">{{ feature }}</v-list-item-title>
                </v-list-item>
              </v-list>
            </v-card-text>
            <v-card-actions class="pa-6 pt-0">
              <v-btn color="primary" variant="outlined" rounded="xl">
                了解更多
              </v-btn>
            </v-card-actions>
          </v-card>
        </v-col>
      </v-row>
    </div>

    <!-- Pricing Tiers -->
    <div class="mb-12">
      <h2 class="text-h4 font-weight-bold text-center mb-8">企业套餐</h2>
      <v-row>
        <v-col v-for="tier in pricingTiers" :key="tier.name" cols="12" md="4">
          <v-card 
            elevation="2" 
            rounded="xl" 
            class="h-100 text-center"
            :class="{ 'border-primary': tier.popular }"
            :style="tier.popular ? 'border: 2px solid rgb(var(--v-theme-primary))' : ''"
          >
            <v-chip
              v-if="tier.popular"
              color="primary"
              class="position-absolute popular-badge"
              variant="flat"
            >
              推荐
            </v-chip>
            
            <v-card-text class="pa-6">
              <h3 class="text-h5 font-weight-bold mb-2">{{ tier.name }}</h3>
              <p class="text-body-2 text-grey-darken-1 mb-4">{{ tier.description }}</p>
              
              <div class="mb-6">
                <div class="text-h3 font-weight-bold text-primary">{{ tier.price }}</div>
                <div class="text-body-2 text-grey">{{ tier.period }}</div>
              </div>

              <v-list class="mb-6" density="compact">
                <v-list-item v-for="feature in tier.features" :key="feature" class="px-0">
                  <template #prepend>
                    <v-icon color="success" size="16">mdi-check</v-icon>
                  </template>
                  <v-list-item-title class="text-body-2">{{ feature }}</v-list-item-title>
                </v-list-item>
              </v-list>

              <v-btn
                :color="tier.popular ? 'primary' : 'default'"
                :variant="tier.popular ? 'flat' : 'outlined'"
                size="large"
                rounded="xl"
                class="w-100"
              >
                {{ tier.buttonText }}
              </v-btn>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>
    </div>

    <!-- Case Studies -->
    <div class="mb-12">
      <h2 class="text-h4 font-weight-bold text-center mb-8">客户案例</h2>
      <v-row>
        <v-col v-for="case_study in caseStudies" :key="case_study.company" cols="12" md="6" lg="4">
          <v-card elevation="2" rounded="xl" class="h-100">
            <v-card-text class="pa-6 text-center">
              <v-avatar size="80" class="mb-4">
                <v-img :src="case_study.logo" />
              </v-avatar>
              <h4 class="text-h6 font-weight-bold mb-2">{{ case_study.company }}</h4>
              <p class="text-body-2 text-grey-darken-1 mb-4">{{ case_study.industry }}</p>
              
              <div class="mb-4">
                <div class="d-flex justify-space-around">
                  <div class="text-center">
                    <div class="text-h5 font-weight-bold text-primary">{{ case_study.metrics.efficiency }}</div>
                    <div class="text-caption text-grey">效率提升</div>
                  </div>
                  <div class="text-center">
                    <div class="text-h5 font-weight-bold text-success">{{ case_study.metrics.cost }}</div>
                    <div class="text-caption text-grey">成本节省</div>
                  </div>
                </div>
              </div>
              
              <p class="text-body-2 font-italic">"{{ case_study.testimonial }}"</p>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>
    </div>

    <!-- Contact Section -->
    <v-card elevation="2" rounded="xl" color="primary" class="text-white">
      <v-card-text class="pa-8 text-center">
        <h2 class="text-h4 font-weight-bold mb-4">准备开始了吗？</h2>
        <p class="text-h6 mb-6 opacity-90">联系我们的企业解决方案专家，获取定制化方案</p>
        
        <v-row class="mb-6" justify="center">
          <v-col cols="12" md="8">
            <v-row>
              <v-col cols="12" md="4" class="text-center">
                <v-icon size="32" class="mb-2">mdi-phone</v-icon>
                <div class="font-weight-medium">销售热线</div>
                <div>400-123-4567</div>
              </v-col>
              <v-col cols="12" md="4" class="text-center">
                <v-icon size="32" class="mb-2">mdi-email</v-icon>
                <div class="font-weight-medium">企业邮箱</div>
                <div>enterprise@aetherflow.ai</div>
              </v-col>
              <v-col cols="12" md="4" class="text-center">
                <v-icon size="32" class="mb-2">mdi-clock</v-icon>
                <div class="font-weight-medium">服务时间</div>
                <div>7x24 小时</div>
              </v-col>
            </v-row>
          </v-col>
        </v-row>
        
        <div class="d-flex justify-center gap-4">
          <v-btn color="white" size="large" rounded="xl" variant="flat">
            <v-icon start>mdi-calendar</v-icon>
            预约咨询
          </v-btn>
          <v-btn color="white" size="large" rounded="xl" variant="outlined">
            <v-icon start>mdi-download</v-icon>
            下载白皮书
          </v-btn>
        </div>
      </v-card-text>
    </v-card>
  </v-container>
</template>

<script lang="ts" setup>
import { ref } from 'vue'

// Enterprise Features
const enterpriseFeatures = ref([
  {
    title: '私有化部署',
    description: '在您的基础设施上部署，确保数据安全和合规性',
    icon: 'mdi-cloud-lock',
    color: 'primary'
  },
  {
    title: 'API 集成',
    description: '丰富的 API 接口，轻松集成到现有业务系统',
    icon: 'mdi-api',
    color: 'success'
  },
  {
    title: '定制化服务',
    description: '根据业务需求定制 AI 模型和工作流',
    icon: 'mdi-cog',
    color: 'warning'
  },
  {
    title: '7x24 支持',
    description: '专业技术团队提供全天候技术支持服务',
    icon: 'mdi-headset',
    color: 'info'
  }
])

// Solutions
const solutions = ref([
  {
    title: '电商营销解决方案',
    description: '为电商平台提供批量商品图片生成、营销素材制作等服务',
    image: '/images/solutions/ecommerce.jpg',
    features: [
      '批量商品图片生成',
      '营销海报自动制作',
      '多平台尺寸适配',
      '品牌风格统一'
    ]
  },
  {
    title: '内容创作平台',
    description: '为内容平台提供 AI 辅助创作工具，提升创作效率',
    image: '/images/solutions/content.jpg',
    features: [
      'AI 辅助写作',
      '智能配图生成',
      '视频缩略图制作',
      '多语言内容生成'
    ]
  },
  {
    title: '设计工作室',
    description: '为设计团队提供 AI 设计助手，加速创意实现',
    image: '/images/solutions/design.jpg',
    features: [
      '概念设计生成',
      '风格迁移工具',
      '设计变体生成',
      '团队协作平台'
    ]
  },
  {
    title: '教育培训',
    description: '为教育机构提供 AI 教学工具和课件制作服务',
    image: '/images/solutions/education.jpg',
    features: [
      '教学插图生成',
      '课件模板制作',
      '互动内容创建',
      '个性化学习材料'
    ]
  }
])

// Pricing Tiers
const pricingTiers = ref([
  {
    name: '标准版',
    description: '适合中小企业的基础需求',
    price: '¥9,999',
    period: '每月',
    popular: false,
    buttonText: '开始试用',
    features: [
      '50,000 次 API 调用',
      '标准技术支持',
      '基础数据分析',
      '5 个团队成员'
    ]
  },
  {
    name: '专业版',
    description: '适合成长型企业的高级需求',
    price: '¥29,999',
    period: '每月',
    popular: true,
    buttonText: '立即购买',
    features: [
      '200,000 次 API 调用',
      '优先技术支持',
      '高级数据分析',
      '20 个团队成员',
      '定制化模型训练',
      'SLA 保障'
    ]
  },
  {
    name: '企业版',
    description: '适合大型企业的定制需求',
    price: '定制报价',
    period: '联系销售',
    popular: false,
    buttonText: '联系销售',
    features: [
      '无限 API 调用',
      '专属客户经理',
      '完整数据分析',
      '无限团队成员',
      '私有化部署',
      '定制开发服务'
    ]
  }
])

// Case Studies
const caseStudies = ref([
  {
    company: '某知名电商平台',
    industry: '电子商务',
    logo: '/images/logos/ecommerce.png',
    metrics: {
      efficiency: '+300%',
      cost: '-60%'
    },
    testimonial: 'Aetherflow 帮助我们大幅提升了商品图片制作效率，显著降低了运营成本。'
  },
  {
    company: '某创意设计公司',
    industry: '创意设计',
    logo: '/images/logos/design.png',
    metrics: {
      efficiency: '+250%',
      cost: '-45%'
    },
    testimonial: 'AI 辅助设计让我们的创意团队能够专注于更有价值的创意工作。'
  },
  {
    company: '某在线教育平台',
    industry: '在线教育',
    logo: '/images/logos/education.png',
    metrics: {
      efficiency: '+400%',
      cost: '-70%'
    },
    testimonial: '课件制作效率的大幅提升让我们能够为学生提供更丰富的学习内容。'
  }
])
</script>

<route lang="yaml">
meta:
  title: 企业服务
</route>

<style scoped>
.popular-badge {
  top: -10px;
  right: 20px;
  z-index: 1;
}
</style>
