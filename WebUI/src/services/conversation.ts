import axios from '@/helpers/RequestHelper'
import type { ConversationDto, GenerateImageDto, GenerationRecordDto, PaginationMeta} from '@/types/api'

/**
 * Creates a new conversation.
 * @returns A promise that resolves to the created conversation.
 */
export const createConversation = async () => {
  // OpenAPI: POST /api/Conversation/create
  const { data } = await axios.post<ConversationDto>('/Conversation/create')
  return data
}

/**
 * Retrieves a specific conversation by its ID.
 * @param chatId - The ID of the conversation to retrieve.
 * @returns A promise that resolves to the conversation data.
 */
export const getConversation = async (chatId: string): Promise<ConversationDto> => {
  // OpenAPI: GET /api/Conversation/{chatId}
  const { data } = await axios.get<ConversationDto | any>(`/Conversation/${chatId}`)
  return data
}

/**
 * Generates an image within a specific conversation.
 * @param chatId - The ID of the conversation.
 * @param payload - The data for generating the image.
 * @returns A promise that resolves to the generation record.
 */
export const generateImage = async (chatId: string, payload: GenerateImageDto): Promise<GenerationRecordDto> => {
  // 使用正确的生成API端点
  const { data } = await axios.post<GenerationRecordDto>('/api/generate', {
    conversationId: chatId,
    prompt: payload.prompt,
    provider: payload.clientType || 'Stub',
    params: JSON.stringify(payload),
    quality: 'standard',
    style: 'vivid'
  })
  return data
}

/**
 * Retrieves a paginated list of conversations.
 * @param page - The page number to retrieve.
 * @param pageSize - The number of conversations per page.
 * @returns A promise that resolves to an object containing the list of conversations and pagination metadata.
 */
export const listConversations = async (page = 0, pageSize = 12): Promise<{ items: ConversationDto[]; pagination: PaginationMeta }> => {
  const { data, headers } = await axios.get<ConversationDto[]>(`/Conversation/conversations`, {
    params: { pageNumber: page, pageSize }
  })
  const pagination: PaginationMeta = JSON.parse(headers['x-pagination'])
  return { items: data, pagination }
}

/**
 * A quick-generate function that likely creates a conversation and generates an image in one call.
 * @param payload - The data for generating the image.
 * @returns A promise that resolves to the result of the quick generation.
 */
export const quickGenerate = async (payload: GenerateImageDto) => {
  // OpenAPI: POST /api/Conversation/generate
  const { data } = await axios.post(`/Conversation/generate`, payload)
  return data
}

/**
 * Deletes a specific conversation by its ID.
 * @param chatId - The ID of the conversation to delete.
 * @returns A promise that resolves when the conversation is deleted.
 */
export const deleteConversation = async (chatId: string): Promise<void> => {
  // OpenAPI: DELETE /api/Conversation/{chatId}
  await axios.delete(`/Conversation/${chatId}`)
}
