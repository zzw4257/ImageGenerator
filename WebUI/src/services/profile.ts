import axios from '@/helpers/RequestHelper'
import type { ProfileDto } from '@/types/api'

export const getProfile = async (): Promise<ProfileDto> => {
  // OpenAPI: GET /api/Profile
  const { data } = await axios.get<ProfileDto>('/Profile')
  return data
}

export const claimCredits = async (): Promise<ProfileDto> => {
  // OpenAPI: POST /api/Profile/credits/claim
  const { data } = await axios.post<ProfileDto>('/Profile/credits/claim')
  return data
}
