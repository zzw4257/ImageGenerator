import axios from '@/helpers/RequestHelper'
import type { ConversationDto, GenerateImageDto, GenerationRecordDto, PaginationMeta} from '@/types/api'

export const createConversation = async () => {
  // OpenAPI: POST /api/Conversation/create
  const { data } = await axios.post<ConversationDto>('/Conversation/create')
  return data
}

export const getConversation = async (chatId: string): Promise<ConversationDto> => {
  // OpenAPI: GET /api/Conversation/{chatId}
  const { data } = await axios.get<ConversationDto | any>(`/Conversation/${chatId}`)
  return data
}

// Payload for generation can vary (prompt text, params, input images, etc.)
export const generateImage = async (chatId: string, payload: GenerateImageDto): Promise<GenerationRecordDto> => {
  // OpenAPI: POST /api/Conversation/generate/{chatId}
  const { data } = await axios.post<GenerationRecordDto>(`/Conversation/generate/${chatId}`, payload)
  return data
}

export const listConversations = async (page = 0, pageSize = 12): Promise<{ items: ConversationDto[]; pagination: PaginationMeta }> => {
  const { data, headers } = await axios.get<ConversationDto[]>(`/Conversation/conversations`, {
    params: { pageNumber: page, pageSize }
  })
  const pagination: PaginationMeta = JSON.parse(headers['x-pagination'])
  return { items: data, pagination }
}

export const quickGenerate = async (payload: GenerateImageDto) => {
  // OpenAPI: POST /api/Conversation/generate
  const { data } = await axios.post(`/Conversation/generate`, payload)
  return data
}

export const deleteConversation = async (chatId: string): Promise<void> => {
  // OpenAPI: DELETE /api/Conversation/{chatId}
  await axios.delete(`/Conversation/${chatId}`)
}
