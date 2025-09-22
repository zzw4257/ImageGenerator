import type { ImageDto } from '@/types/api'

export interface TimelineItem {
  id: string
  type: 'prompt' | 'image'
  prompt: string
  timestamp: Date
  image?: ImageDto
}
