import axios from '@/helpers/RequestHelper'
import type { AuthInfo } from '@/types'

export interface LoginFormDto {
  username: string
  password: string
}

export interface RegisterFormDto {
  username: string
  password: string
  invitationCode?: string
}

export const login = async (payload: LoginFormDto) => {
  // OpenAPI: POST /api/Authentication/login
  const { data } = await axios.post<AuthInfo>('/Authentication/login', payload)
  return data
}

export const register = async (payload: RegisterFormDto) => {
  // OpenAPI: POST /api/Authentication/register
  const { data } = await axios.post<AuthInfo>('/Authentication/register', payload)
  return data
}
