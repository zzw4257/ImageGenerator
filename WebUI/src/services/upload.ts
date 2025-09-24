import axios from '@/helpers/RequestHelper'
import type { ImageDto } from '@/types'

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

export const listUploadedImages = async (): Promise<ImageDto[]> => {
  // OpenAPI: GET /api/Upload
  const { data } = await axios.get<ImageDto[]>('/Upload')
  return data
}