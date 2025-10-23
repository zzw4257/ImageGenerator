import axios from '@/helpers/RequestHelper'

export interface PresetDto {
  id: string
  title: string
  description?: string
  cover?: string
  cost?: number
  prompt: string
  provider?: string
  params?: Record<string, any>
}

/**
 * 列出预设，返回 items + pagination（遵循 X-Pagination header）
 */
export const listPresets = async (page = 0, pageSize = 12): Promise<{ items: PresetDto[]; pagination: any }> => {
  const { data, headers } = await axios.get<PresetDto[]>('/presets', { params: { pageNumber: page, pageSize } })
  const pagination = headers['x-pagination'] ? JSON.parse(headers['x-pagination']) : { TotalCount: data.length, PageSize: pageSize, PageNumber: page, TotalPages: 1 }
  return { items: data, pagination }
}

/**
 * （可选）按 id 获取单个预设
 */
export const getPreset = async (id: string): Promise<PresetDto> => {
  const { data } = await axios.get<PresetDto>(`/presets/${encodeURIComponent(id)}`)
  return data
}