<template>
  <v-container fluid class="pa-8">
    <div class="d-flex justify-space-between align-center mb-6">
      <h1 class="text-h3 font-weight-bold text-primary">Recent Conversation</h1>
      <v-btn color="primary" size="large" rounded="xl" prepend-icon="mdi-plus" @click="createNewConversation">
        New Conversation
      </v-btn>
    </div>

    <v-row>
      <v-col v-for="conversation in conversations" :key="conversation.id" cols="12" sm="6" md="4" lg="3" xl="2">
        <ConversationCard :conversation="conversation" @click="openConversation(conversation.id)" :overlay-actions="[{
          key: 'del',
          text: 'Delete',
          icon: 'mdi-delete',
          color: 'primary',
          variant: 'outlined',
          size: 'small'
        }]" :show-overlay-actions="true" @overlay-action="(k) => handleOverlayAction(k, conversation.id)"/>
      </v-col>
    </v-row>

    <div class="d-flex justify-center mt-6">
      <v-pagination
        v-model="pageNumberUI"
        :length="pagination.TotalPages"
        rounded="circle"
        total-visible="7"
        @update:modelValue="onPageChange"
      />
    </div>

    <!-- Empty state -->
    <div v-if="!loading && conversations.length === 0" class="text-center mt-12">
      <v-icon size="120" color="grey-lighten-2" class="mb-4">
        mdi-image-multiple
      </v-icon>
      <h2 class="text-h5 text-grey-darken-1 mb-2">No conversations yet</h2>
      <p class="text-body-1 text-grey-darken-1 mb-4">Create your first image generation conversation</p>
      <v-btn color="primary" size="large" rounded="xl" prepend-icon="mdi-plus" @click="createNewConversation">
        Start Creating
      </v-btn>
    </div>

    <div v-if="loading" class="d-flex align-center justify-center py-12">
      <v-progress-circular indeterminate color="primary" />
    </div>
  </v-container>
</template>

<script lang="ts" setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'

/**
 * The main page of the application, displaying a list of recent conversations.
 */
import ConversationCard from '@/components/ConversationCard.vue'
import * as convoApi from '@/services/conversation'
import { useAppStore } from '@/stores/app'
import { useNotificationStore } from '@/stores/notification'
import type { ConversationUI } from '@/types/ui'

const appStore = useAppStore()
const notificationStore = useNotificationStore()
const router = useRouter()
const conversations = ref<ConversationUI[]>([])
const loading = ref(false)
const pagination = ref({ TotalCount: 0, PageSize: 9, PageNumber: 0, TotalPages: 0 })
// v-pagination is 1-based for UI convenience
const pageNumberUI = ref(1)
const pageSize = ref(9)

const loadConversations = async () => {
  loading.value = true
  try {
    const { items, pagination: meta } = await convoApi.listConversations(pageNumberUI.value - 1, pageSize.value)
    conversations.value = (items || []).map((x) => ({
      id: x.id || String(x.id ?? ''),
      thumbnail: x.generationRecords[0]?.outputImage?.imagePath || '',
      timestamp: new Date(x.updatedAt+"Z") || new Date(),
      recordCount: x.generationRecords.length ?? 0,
      lastMessage: x.generationRecords[0]?.prompt || 'Untitled',
    }))
    pagination.value = meta
  } finally {
    loading.value = false
  }
}

function onPageChange() {
  loadConversations()
}

onMounted(loadConversations)

const createNewConversation = async () => {
  const conv: any = await convoApi.createConversation()
  const id = conv?.id || conv?.chatId || Date.now().toString()
  router.push(`/conversation/${id}`)
}

const openConversation = (id: string) => {
  router.push(`/conversation/${id}`)
}

const handleOverlayAction = async (actionKey: string, conversationId: string) => {
  if (actionKey === 'del') {
    await appStore.showDeleteDialog({
        title: 'Delete Conversation',
        message: 'Are you sure you want to delete the conversation?',
        itemName: conversationId,
        confirmText: 'Delete',
        onConfirm: async () => {
            try {
                await convoApi.deleteConversation(conversationId)
                conversations.value = conversations.value.filter(c => c.id !== conversationId)
                notificationStore.success('Conversation deleted successfully')
            } catch (error) {
                notificationStore.error('Failed to delete conversation')
                throw error
            }
        }
    })
  }
}
</script>
