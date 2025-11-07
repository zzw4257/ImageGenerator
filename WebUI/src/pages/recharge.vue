<template>
  <v-container class="py-6">
    <v-row>
      <v-col cols="12">
        <h1 class="text-h4 font-weight-bold mb-2">充值 Credits</h1>
        <p class="text-body-1 text-grey-darken-1">为您的账户充值 Credits 以继续使用图像生成服务</p>
      </v-col>
    </v-row>

    <!-- 当前余额 -->
    <v-row class="mb-6">
      <v-col cols="12" md="6">
        <v-card class="pa-6 text-white" color="primary" rounded="xl" variant="flat">
          <div class="d-flex align-center justify-space-between">
            <div>
              <h2 class="text-h5 font-weight-bold mb-2">当前余额</h2>
              <div class="d-flex align-end">
                <span class="text-h3 font-weight-bold mr-2">{{ walletBalance }}</span>
                <span class="text-h6 font-weight-medium mb-2">Credits</span>
              </div>
            </div>
            <v-icon color="white" size="64">mdi-wallet</v-icon>
          </div>
        </v-card>
      </v-col>
    </v-row>

    <!-- 充值表单 -->
    <v-row>
      <v-col cols="12" md="8">
        <v-card elevation="2" rounded="xl">
          <v-card-title class="d-flex align-center">
            <v-icon class="mr-2">mdi-credit-card-plus</v-icon>
            选择充值金额
          </v-card-title>
          <v-divider />
          <v-card-text class="pa-6">
            <!-- 预设金额 -->
            <div class="mb-6">
              <h3 class="text-h6 font-weight-medium mb-4">推荐套餐</h3>
              <v-row>
                <v-col
                  v-for="preset in presetAmounts"
                  :key="preset.amount"
                  cols="6"
                  md="4"
                >
                  <v-card
                    :class="['preset-card', { 'selected': selectedAmount === preset.amount }]"
                    :color="selectedAmount === preset.amount ? 'primary' : 'default'"
                    elevation="0"
                    hover
                    rounded="xl"
                    variant="outlined"
                    @click="selectAmount(preset.amount)"
                  >
                    <v-card-text class="text-center pa-6">
                      <v-icon
                        :color="selectedAmount === preset.amount ? 'white' : 'primary'"
                        size="32"
                        class="mb-2"
                      >
                        {{ preset.icon }}
                      </v-icon>
                      <div
                        :class="[
                          'text-h5 font-weight-bold mb-1',
                          selectedAmount === preset.amount ? 'text-white' : 'text-primary'
                        ]"
                      >
                        {{ preset.amount }} Credits
                      </div>
                      <div
                        :class="[
                          'text-body-2',
                          selectedAmount === preset.amount ? 'text-white' : 'text-grey-darken-1'
                        ]"
                      >
                        {{ preset.description }}
                      </div>
                    </v-card-text>
                  </v-card>
                </v-col>
              </v-row>
            </div>

            <!-- 自定义金额 -->
            <div class="mb-6">
              <h3 class="text-h6 font-weight-medium mb-4">自定义金额</h3>
              <v-text-field
                v-model.number="customAmount"
                :disabled="selectedAmount !== null"
                label="输入充值金额"
                placeholder="请输入充值金额"
                prepend-inner-icon="mdi-currency-usd"
                suffix="Credits"
                type="number"
                variant="outlined"
                @input="handleCustomAmountInput"
              >
                <template #append>
                  <v-btn
                    :disabled="selectedAmount !== null || !customAmount || customAmount <= 0"
                    variant="text"
                    @click="selectAmount(customAmount)"
                  >
                    选择
                  </v-btn>
                </template>
              </v-text-field>
              <v-alert
                v-if="customAmount && (customAmount < minAmount || customAmount > maxAmount)"
                color="warning"
                density="compact"
                variant="tonal"
              >
                <template #text>
                  充值金额需要在 {{ minAmount }} - {{ maxAmount }} Credits 之间
                </template>
              </v-alert>
            </div>

            <!-- 充值说明 -->
            <v-alert
              color="info"
              density="compact"
              variant="tonal"
            >
              <template #prepend>
                <v-icon>mdi-information</v-icon>
              </template>
              <template #text>
                <div class="text-body-2">
                  <div class="mb-2"><strong>充值说明：</strong></div>
                  <ul class="mb-0 pl-4">
                    <li>充值成功后，Credits 将立即到账</li>
                    <li>所有充值记录可在"账户与历史"页面查看</li>
                    <li>Credits 可用于生成图像，不同模型消耗不同</li>
                  </ul>
                </div>
              </template>
            </v-alert>
          </v-card-text>

          <v-card-actions class="pa-6 pt-0">
            <v-spacer />
            <v-btn
              color="grey"
              variant="text"
              @click="$router.back()"
            >
              取消
            </v-btn>
            <v-btn
              :disabled="!selectedAmount || recharging"
              :loading="recharging"
              color="primary"
              size="large"
              variant="flat"
              @click="recharge"
            >
              <v-icon start>mdi-check</v-icon>
              确认充值 {{ selectedAmount }} Credits
            </v-btn>
          </v-card-actions>
        </v-card>
      </v-col>

      <!-- 充值说明卡片 -->
      <v-col cols="12" md="4">
        <v-card elevation="2" rounded="xl">
          <v-card-title class="d-flex align-center">
            <v-icon class="mr-2">mdi-help-circle</v-icon>
            使用说明
          </v-card-title>
          <v-divider />
          <v-card-text class="pa-4">
            <v-list>
              <v-list-item class="px-0">
                <template #prepend>
                  <v-icon color="primary">mdi-flash</v-icon>
                </template>
                <v-list-item-title class="text-body-2">
                  <strong>什么是 Credits？</strong>
                </v-list-item-title>
                <v-list-item-subtitle class="text-body-2 mt-1">
                  Credits 是平台的虚拟货币，用于生成图像
                </v-list-item-subtitle>
              </v-list-item>

              <v-list-item class="px-0">
                <template #prepend>
                  <v-icon color="primary">mdi-image</v-icon>
                </template>
                <v-list-item-title class="text-body-2">
                  <strong>Credits 消耗</strong>
                </v-list-item-title>
                <v-list-item-subtitle class="text-body-2 mt-1">
                  Stub: 1 Credit<br>
                  Qwen: 2 Credits<br>
                  Flux: 3 Credits
                </v-list-item-subtitle>
              </v-list-item>

              <v-list-item class="px-0">
                <template #prepend>
                  <v-icon color="primary">mdi-refresh</v-icon>
                </template>
                <v-list-item-title class="text-body-2">
                  <strong>充值到账</strong>
                </v-list-item-title>
                <v-list-item-subtitle class="text-body-2 mt-1">
                  充值成功后立即到账，无需等待
                </v-list-item-subtitle>
              </v-list-item>
            </v-list>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts" setup>
  import { ref, onMounted } from 'vue'
  import { useRouter } from 'vue-router'
  import { getBalance, grantCredits } from '@/services/wallet'
  import { useNotificationStore } from '@/stores/notification'

  const router = useRouter()
  const notificationStore = useNotificationStore()

  const walletBalance = ref(0)
  const selectedAmount = ref<number | null>(null)
  const customAmount = ref<number | null>(null)
  const recharging = ref(false)

  const minAmount = 1
  const maxAmount = 1000

  const presetAmounts = [
    {
      amount: 10,
      description: '入门套餐',
      icon: 'mdi-rocket-launch'
    },
    {
      amount: 50,
      description: '标准套餐',
      icon: 'mdi-star'
    },
    {
      amount: 100,
      description: '专业套餐',
      icon: 'mdi-crown'
    },
    {
      amount: 200,
      description: '高级套餐',
      icon: 'mdi-diamond-stone'
    },
    {
      amount: 500,
      description: '企业套餐',
      icon: 'mdi-office-building'
    },
    {
      amount: 1000,
      description: '超值套餐',
      icon: 'mdi-trophy'
    }
  ]

  function selectAmount(amount: number) {
    selectedAmount.value = amount
    customAmount.value = null
  }

  function handleCustomAmountInput() {
    if (customAmount.value) {
      selectedAmount.value = null
    }
  }

  async function recharge() {
    if (!selectedAmount.value) return

    recharging.value = true
    try {
      await grantCredits(selectedAmount.value)
      
      // 刷新余额
      await loadBalance()
      
      notificationStore.success(`成功充值 ${selectedAmount.value} Credits！`)
      
      // 延迟跳转，让用户看到成功提示
      setTimeout(() => {
        router.push('/account')
      }, 1500)
    } catch (error: any) {
      console.error('充值失败:', error)
      notificationStore.error(error?.message || '充值失败，请稍后重试')
    } finally {
      recharging.value = false
    }
  }

  async function loadBalance() {
    try {
      const balance = await getBalance()
      walletBalance.value = balance.balance
    } catch (error) {
      console.error('加载余额失败:', error)
    }
  }

  onMounted(async () => {
    await loadBalance()
  })
</script>

<style scoped>
.preset-card {
  cursor: pointer;
  transition: all 0.3s ease;
}

.preset-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.preset-card.selected {
  border-width: 2px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
}
</style>

<route lang="yaml">
meta:
  title: 充值 Credits
</route>

