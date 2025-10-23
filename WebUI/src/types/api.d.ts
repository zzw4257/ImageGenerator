import type { GenerationStatus, GenerationType } from '../enums'

export interface ConversationDto {
  id: string
  title: string
  createdAt: string
  updatedAt: string
  generationRecords: GenerationRecordDto[]
}

export interface PaginationMeta {
  TotalCount: number
  PageSize: number
  PageNumber: number
  TotalPages: number
}

export interface GenerationRecordDto {
  id: string
  generationType: GenerationType
  prompt: string
  generationParams: string
  status: GenerationStatus
  createdAt: string
  completedAt?: string | null
  inputImages: ImageDto[]
  outputImage?: ImageDto | null
}

export interface ImageDto {
  id: string
  size: number
  imagePath: string
  isFavorite: boolean
  createdAt: string
}

export interface InvitationDto {
  id: string
  code: string
  createdAt: string
  remainingUses: number
  issuerId: string
}

export interface GenerateImageDto {
  prompt: string
  generationType: GenerationType
  quality?: 'standard' | 'hd'
  style?: 'vivid' | 'natural'
  inputImageIds?: string[] // GUIDs represented as strings
}

export interface ProfileDto {
  id: string
  username: string
  createdAt: string // ISO date
  subscriptionExpiration: string // ISO date
  credits: number
  lastCreditClaimedAt?: string | null // ISO date or null
}

export interface TransactionDto {
  id: string
  type: 'income' | 'expense'
  amount: number
  description: string
  createdAt: Date
}

export interface GenerationTaskDto {
  id: string
  prompt: string
  thumbnail: string
  status: 'completed' | 'processing' | 'failed'
  statusText: string
  credits: number
  createdAt: Date
}
