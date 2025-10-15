import axios from '@/helpers/RequestHelper'
import type { ImageDto, PaginationMeta } from '@/types'

/**
 * Uploads an image file to the server.
 * @param file - The image file to upload.
 * @returns A promise that resolves to the uploaded image data.
 */
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

/**
 * Retrieves a paginated list of uploaded images.
 * @param page - The page number to retrieve.
 * @param pageSize - The number of images per page.
 * @returns A promise that resolves to an object containing the list of uploaded images and pagination metadata.
 */
export const listUploadedImages = async (page = 0, pageSize = 24): Promise<{ items: ImageDto[]; pagination: PaginationMeta }> => {
  const { data, headers } = await axios.get<ImageDto[]>('/Upload', { params: { pageNumber: page, pageSize } })
  const pagination: PaginationMeta = JSON.parse(headers['x-pagination'])
  return { items: data, pagination }
}