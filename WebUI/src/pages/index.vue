<template>
  <v-container fluid class="pa-8">
    <div class="d-flex justify-space-between align-center mb-6">
      <h1 class="text-h3 font-weight-bold text-primary">Image Library</h1>
      <v-btn
        color="primary"
        size="large"
        rounded="xl"
        prepend-icon="mdi-plus"
        @click="createNewConversation"
      >
        New Conversation
      </v-btn>
    </div>

    <v-row>
      <v-col
        v-for="conversation in conversations"
        :key="conversation.id"
        cols="12"
        sm="6"
        md="4"
        lg="3"
        xl="2"
      >
        <ConversationCard
          :conversation="conversation"
          @click="openConversation(conversation.id)"
        />
      </v-col>
    </v-row>

    <!-- Empty state -->
    <div v-if="conversations.length === 0" class="text-center mt-12">
      <v-icon
        size="120"
        color="grey-lighten-2"
        class="mb-4"
      >
        mdi-image-multiple
      </v-icon>
      <h2 class="text-h5 text-grey-darken-1 mb-2">No conversations yet</h2>
      <p class="text-body-1 text-grey-darken-1 mb-4">Create your first image generation conversation</p>
      <v-btn
        color="primary"
        size="large"
        rounded="xl"
        prepend-icon="mdi-plus"
        @click="createNewConversation"
      >
        Start Creating
      </v-btn>
    </div>
  </v-container>
</template>

<script lang="ts" setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import ConversationCard from '@/components/ConversationCard.vue'

interface Conversation {
  id: string
  title: string
  thumbnail: string
  lastMessage: string
  timestamp: Date
  imageCount: number
}

const router = useRouter()
const conversations = ref<Conversation[]>([])

// Mock data for demonstration
const mockConversations: Conversation[] = [
  {
    id: '1',
    title: 'Sunset Landscapes',
    thumbnail: 'https://picsum.photos/400/300?random=1',
    lastMessage: 'A beautiful sunset over mountains',
    timestamp: new Date('2024-01-15'),
    imageCount: 5
  },
  {
    id: '2',
    title: 'Abstract Art',
    thumbnail: 'https://picsum.photos/400/300?random=2',
    lastMessage: 'Colorful abstract composition',
    timestamp: new Date('2024-01-14'),
    imageCount: 8
  },
  {
    id: '3',
    title: 'Character Design',
    thumbnail: 'https://picsum.photos/400/300?random=3',
    lastMessage: 'Fantasy character with armor',
    timestamp: new Date('2024-01-13'),
    imageCount: 12
  },
  {
    id: '4',
    title: 'Nature Photography',
    thumbnail: 'https://picsum.photos/400/300?random=4',
    lastMessage: 'Forest landscape with fog',
    timestamp: new Date('2024-01-12'),
    imageCount: 3
  }
]

onMounted(() => {
  // Simulate loading conversations
  setTimeout(() => {
    conversations.value = mockConversations
  }, 500)
})

const createNewConversation = () => {
  const newId = Date.now().toString()
  router.push(`/conversation/${newId}`)
}

const openConversation = (id: string) => {
  router.push(`/conversation/${id}`)
}
</script>

<style scoped>
.v-container {
  max-width: 1400px;
}
</style>
