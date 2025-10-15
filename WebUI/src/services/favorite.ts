import axios from '@/helpers/RequestHelper'
import type { ImageDto, PaginationMeta } from '@/types/api'

/**
 * Retrieves a paginated list of favorite images.
 * @param page - The page number to retrieve.
 * @param pageSize - The number of images per page.
 * @returns A promise that resolves to an object containing the list of favorite images and pagination metadata.
 */
export const listFavorites = async (page = 0, pageSize = 24): Promise<{ items: ImageDto[]; pagination: PaginationMeta }> => {
  const { data, headers } = await axios.get<ImageDto[]>('/Favorite', { params: { pageNumber: page, pageSize } })
  const pagination: PaginationMeta = JSON.parse(headers['x-pagination'])
  return { items: data, pagination }
}

/**
 * Adds an image to the user's favorites.
 * @param imageId - The ID of the image to add to favorites.
 * @returns A promise that resolves when the image is added.
 */
export const addFavorite = async (imageId: string): Promise<void> => {
  // OpenAPI: POST /api/Favorite/{imageId}
  await axios.post(`/Favorite/${imageId}`)
}

/**
 * Removes an image from the user's favorites.
 * @param imageId - The ID of the image to remove from favorites.
 * @returns A promise that resolves when the image is removed.
 */
export const removeFavorite = async (imageId: string): Promise<void> => {
  // OpenAPI: DELETE /api/Favorite/{imageId}
  await axios.delete(`/Favorite/${imageId}`)
}
