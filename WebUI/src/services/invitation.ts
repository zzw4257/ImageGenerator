import axios from '@/helpers/RequestHelper'
import type { InvitationDto } from '@/types/api'

export const listInvitationCodes = async (): Promise<InvitationDto[]> => {
  // OpenAPI: GET /api/Invitation/codes
  const { data } = await axios.get<InvitationDto[]>('/Invitation/codes')
  return data
}

export const createInvitationCode = async (): Promise<InvitationDto> => {
  // OpenAPI: POST /api/Invitation/create
  const { data } = await axios.post<InvitationDto>('/Invitation/create')
  return data
}

export const deleteInvitationCode = async (invitationId: string): Promise<void> => {
  // OpenAPI: DELETE /api/Invitation/{invitationId}
  await axios.delete(`/Invitation/${encodeURIComponent(invitationId)}`)
}