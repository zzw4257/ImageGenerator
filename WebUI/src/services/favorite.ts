import axios from '@/helpers/RequestHelper'
import type { ImageDto } from '@/types/api'

export const listFavorites = async (): Promise<ImageDto[]> => {
  // OpenAPI: GET /api/Favorite
  const { data } = await axios.get<ImageDto[]>('/Favorite')
  return data
}

export const addFavorite = async (imageId: string): Promise<void> => {
  // OpenAPI: POST /api/Favorite/{imageId}
  await axios.post(`/Favorite/${imageId}`)
}

export const removeFavorite = async (imageId: string): Promise<void> => {
  // OpenAPI: DELETE /api/Favorite/{imageId}
  await axios.delete(`/Favorite/${imageId}`)
}
