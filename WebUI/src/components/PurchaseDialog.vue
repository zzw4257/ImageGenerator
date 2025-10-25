<template>
  <v-dialog v-model="dialog" max-width="500" persistent>
    <v-card v-if="item" rounded="xl">
      <v-card-title class="pa-6 pb-0">
        <h2 class="text-h5 font-weight-bold">确认购买</h2>
      </v-card-title>

      <v-card-text class="pa-6">
        <!-- Item Preview -->
        <v-card variant="outlined" class="mb-4">
          <div class="d-flex">
            <v-img
              :src="item.coverUrl || '/images/default-preset.png'"
              width="120"
              height="80"
              cover
              class="rounded-l"
            />
            <div class="pa-3 flex-grow-1">
              <h3 class="text-h6 font-weight-medium mb-1">{{ item.title }}</h3>
              <p class="text-body-2 text-grey-darken-1 mb-2">{{ item.author.name }}</p>
              <v-chip
                :color="item.type === 'workflow' ? 'blue' : 'green'"
                size="small"
                variant="flat"
              >
                {{ item.type === 'workflow' ? '工作流' : '工作空间' }}
              </v-chip>
            </div>
          </div>
        </v-card>

        <!-- Price Breakdown -->
        <v-card variant="tonal" color="primary" class="mb-4">
          <v-card-text class="pa-4">
            <div class="d-flex justify-space-between align-center mb-2">
              <span>商品价格</span>
              <span class="font-weight-medium">{{ item.price }} Credits</span>
            </div>
            <div v-if="discount > 0" class="d-flex justify-space-between align-center mb-2">
              <span class="text-success">折扣优惠</span>
              <span class="text-success font-weight-medium">-{{ discount }} Credits</span>
            </div>
            <v-divider class="my-2" />
            <div class="d-flex justify-space-between align-center">
              <span class="text-h6 font-weight-bold">总计</span>
              <span class="text-h6 font-weight-bold text-primary">{{ finalPrice }} Credits</span>
            </div>
          </v-card-text>
        </v-card>

        <!-- Current Balance -->
        <v-alert
          :type="hasEnoughBalance ? 'info' : 'warning'"
          variant="tonal"
          class="mb-4"
        >
          <div class="d-flex justify-space-between align-center">
            <span>当前余额</span>
            <span class="font-weight-medium">{{ currentBalance }} Credits</span>
          </div>
          <div v-if="!hasEnoughBalance" class="mt-2">
            <span class="text-warning">余额不足，需要充值 {{ finalPrice - currentBalance }} Credits</span>
          </div>
        </v-alert>

        <!-- Purchase Options -->
        <div v-if="item.price === 0" class="text-center">
          <v-icon size="48" color="success" class="mb-2">mdi-gift</v-icon>
          <p class="text-body-1">这是一个免费资源！</p>
        </div>

        <!-- Terms -->
        <v-checkbox
          v-model="agreeTerms"
          color="primary"
          class="mt-2"
        >
          <template #label>
            我同意 <a href="#" class="text-primary">购买条款</a> 和 <a href="#" class="text-primary">使用协议</a>
          </template>
        </v-checkbox>
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
          v-if="!hasEnoughBalance && item.price > 0"
          color="warning"
          @click="goToRecharge"
        >
          去充值
        </v-btn>
        <v-btn
          v-else
          color="primary"
          :loading="purchasing"
          :disabled="!agreeTerms"
          @click="confirmPurchase"
        >
          {{ item.price === 0 ? '立即获取' : '确认购买' }}
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>

<script lang="ts" setup>
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useNotificationStore } from '@/stores/notification'

interface MarketItem {
  id: string
  title: string
  description: string
  type: 'workflow' | 'workspace'
  category: string
  price: number
  originalPrice?: number
  coverUrl: string
  author: {
    id: string
    name: string
    avatar?: string
    rating: number
  }
  stats: {
    downloads: number
    rating: number
    reviews: number
  }
  tags: string[]
  createdAt: string
  isFavorite: boolean
  isVerified: boolean
}

const router = useRouter()
const notificationStore = useNotificationStore()

// Props & Emits
const props = defineProps<{
  modelValue: boolean
  item: MarketItem | null
}>()

const emit = defineEmits<{
  'update:modelValue': [value: boolean]
  confirmed: [item: MarketItem]
}>()

// State
const purchasing = ref(false)
const agreeTerms = ref(false)
const currentBalance = ref(150) // Mock current balance
const discount = ref(0) // Mock discount

// Computed
const dialog = computed({
  get: () => props.modelValue,
  set: (value) => emit('update:modelValue', value)
})

const finalPrice = computed(() => {
  if (!props.item) return 0
  return Math.max(0, props.item.price - discount.value)
})

const hasEnoughBalance = computed(() => {
  return currentBalance.value >= finalPrice.value
})

// Methods
async function confirmPurchase() {
  if (!props.item || !agreeTerms.value) return

  purchasing.value = true
  try {
    // Mock purchase API call
    await new Promise(resolve => setTimeout(resolve, 1500))
    
    // Deduct credits
    currentBalance.value -= finalPrice.value
    
    emit('confirmed', props.item)
    dialog.value = false
    agreeTerms.value = false
  } catch (error) {
    notificationStore.error('购买失败，请重试')
  } finally {
    purchasing.value = false
  }
}

function goToRecharge() {
  dialog.value = false
  router.push('/recharge')
}
</script>
