import axios from '@/helpers/RequestHelper'
import type { ImageDto, PaginationMeta } from '@/types/api'

export const listFavorites = async (page = 0, pageSize = 24): Promise<{ items: ImageDto[]; pagination: PaginationMeta }> => {
  const { data, headers } = await axios.get<ImageDto[]>('/Favorite', { params: { pageNumber: page, pageSize } })
  const pagination: PaginationMeta = JSON.parse(headers['x-pagination'])
  return { items: data, pagination }
}

export const addFavorite = async (imageId: string): Promise<void> => {
  // OpenAPI: POST /api/Favorite/{imageId}
  await axios.post(`/Favorite/${imageId}`)
}

export const removeFavorite = async (imageId: string): Promise<void> => {
  // OpenAPI: DELETE /api/Favorite/{imageId}
  await axios.delete(`/Favorite/${imageId}`)
}
