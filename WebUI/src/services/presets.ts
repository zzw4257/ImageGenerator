import axios from '@/helpers/RequestHelper'
import { ErrorHandler } from '@/utils/errorHandler'

export interface PresetDto {
  id: string
  name: string
  description?: string
  coverUrl?: string
  priceCredits?: number
  prompt: string
  provider?: string
  defaultParams?: string // JSON string from backend
  tags?: string[]
}

/**
 * 列出预设，返回 items + pagination（遵循 X-Pagination header）
 */
export const listPresets = async (page = 0, pageSize = 12): Promise<{ items: PresetDto[]; pagination: any }> => {
  try {
    const { data, headers } = await axios.get<PresetDto[]>('/Presets', { params: { pageNumber: page, pageSize } })
    const pagination = headers['x-pagination'] ? JSON.parse(headers['x-pagination']) : { TotalCount: data.length, PageSize: pageSize, PageNumber: page, TotalPages: 1 }
    return { items: data, pagination }
  } catch (error) {
    ErrorHandler.handle(error, '获取预设列表')
    throw error
  }
}

/**
 * （可选）按 id 获取单个预设
 */
export const getPreset = async (id: string): Promise<PresetDto> => {
  try {
    const { data } = await axios.get<PresetDto>(`/Presets/${encodeURIComponent(id)}`)
    return data
  } catch (error) {
    ErrorHandler.handle(error, '获取预设详情')
    throw error
  }
}