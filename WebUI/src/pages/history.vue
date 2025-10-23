<template>
  <v-container class="pa-8">
    <v-tabs v-model="tab">
      <v-tab value="gen">Generations</v-tab>
      <v-tab value="tx">Transactions</v-tab>
    </v-tabs>

    <div v-if="tab === 'gen'">
      <!-- 简单复用会话列表作为生成历史 -->
      <v-row>
        <v-col v-for="c in conversations" :key="c.id" cols="12">
          <v-card class="pa-4 mb-3">
            <div class="d-flex justify-space-between">
              <div>{{ c.title || c.id }}</div>
              <div>{{ new Date(c.updatedAt+'Z').toLocaleString() }}</div>
            </div>
          </v-card>
        </v-col>
      </v-row>
      <v-pagination v-model="convPageUI" :length="convPagination.TotalPages || 1" @update:modelValue="loadConversations" />
    </div>

    <div v-else>
      <v-row>
        <v-col v-for="t in transactions" :key="t.id" cols="12">
          <v-list-item>
            <v-list-item-content>
              <v-list-item-title>{{ t.type }} {{ t.amount }}</v-list-item-title>
              <v-list-item-subtitle>{{ t.description }}</v-list-item-subtitle>
            </v-list-item-content>
            <v-list-item-action>{{ new Date(t.createdAt+'Z').toLocaleString() }}</v-list-item-action>
          </v-list-item>
          <v-divider />
        </v-col>
      </v-row>
      <v-pagination v-model="txPageUI" :length="txPagination.TotalPages || 1" @update:modelValue="loadTransactions" />
    </div>
  </v-container>
</template>

<script lang="ts" setup>
import { ref, onMounted } from 'vue'
import * as convoApi from '@/services/conversation'
import { listTransactions } from '@/services/wallet'

const tab = ref<'gen'|'tx'>('gen')

// generations
const conversations = ref<any[]>([])
const convPagination = ref({ TotalPages: 1 })
const convPageUI = ref(1)
async function loadConversations() {
  const { items, pagination } = await convoApi.listConversations(convPageUI.value - 1, 12)
  conversations.value = items
  convPagination.value = pagination
}

// transactions
const transactions = ref<any[]>([])
const txPagination = ref({ TotalPages: 1 })
const txPageUI = ref(1)
async function loadTransactions() {
  const { items, pagination } = await listTransactions(txPageUI.value - 1, 12)
  transactions.value = items
  txPagination.value = pagination
}

onMounted(() => { loadConversations(); loadTransactions() })
</script>

<route lang="yaml">
meta:
  title: History
</route>