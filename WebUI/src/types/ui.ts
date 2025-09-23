import type { ImageDto } from '@/types/api'

export interface TimelineItem {
  id: string
  type: 'prompt' | 'image'
  prompt: string
  timestamp: Date
  image: ImageDto[]
}

export interface ConversationUI {
  id: string
  thumbnail: string
  lastMessage: string
  timestamp: Date
  recordCount: number
}
