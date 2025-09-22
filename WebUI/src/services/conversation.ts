import axios from '@/helpers/RequestHelper'
import type { ConversationDto, GenerateImageDto, GenerationRecordDto, ImageDto } from '@/types/api'

export const createConversation = async () => {
  // OpenAPI: POST /api/Conversation/create
  const { data } = await axios.post<ConversationDto>('/Conversation/create')
  return data
}

export const getConversation = async (chatId: string): Promise<ConversationDto> => {
  // OpenAPI: GET /api/Conversation/{chatId}
  const { data } = await axios.get<ConversationDto | any>(`/Conversation/${encodeURIComponent(chatId)}`)
  return data
}

// Payload for generation can vary (prompt text, params, input images, etc.)
export const generateImage = async (chatId: string, payload: GenerateImageDto): Promise<GenerationRecordDto> => {
  // OpenAPI: POST /api/Conversation/generate/{chatId}
  const { data } = await axios.post<GenerationRecordDto>(`/Conversation/generate/${encodeURIComponent(chatId)}`, payload)
  return data
}

export const listConversations = async (): Promise<ConversationDto[]> => {
  // OpenAPI: GET /api/Conversation/conversations
  const { data } = await axios.get<ConversationDto[]>(`/Conversation/conversations`)
  return data
}

export const quickGenerate = async (payload: GenerateImageDto) => {
  // OpenAPI: POST /api/Conversation/generate
  const { data } = await axios.post(`/Conversation/generate`, payload)
  return data
}

export const uploadImage = async (file: File): Promise<ImageDto> => {
  const formData = new FormData()
  formData.append('file', file)
  const { data } = await axios.post<ImageDto>('/Conversation/upload', formData, {
    headers: {
      'Content-Type': 'multipart/form-data'
    }
  })
  return data
}
