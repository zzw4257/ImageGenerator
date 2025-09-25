import axios from '@/helpers/RequestHelper'
import type { ImageDto, PaginationMeta } from '@/types'

export const uploadImage = async (file: File): Promise<ImageDto> => {
  // OpenAPI: POST /api/Upload
  const formData = new FormData()
  formData.append('file', file)
  const { data } = await axios.post<ImageDto>('/Upload', formData, {
    headers: {
      'Content-Type': 'multipart/form-data'
    }
  })
  return data
}

export const listUploadedImages = async (page = 0, pageSize = 24): Promise<{ items: ImageDto[]; pagination: PaginationMeta }> => {
  const { data, headers } = await axios.get<ImageDto[]>('/Upload', { params: { pageNumber: page, pageSize } })
  const pagination: PaginationMeta = JSON.parse(headers['x-pagination'])
  return { items: data, pagination }
}