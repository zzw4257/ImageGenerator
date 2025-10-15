import axios from '@/helpers/RequestHelper'
import type { AuthInfo } from '@/types'

/**
 * Represents the data transfer object for a login form.
 */
export interface LoginFormDto {
  /**
   * The user's username.
   */
  username: string
  /**
   * The user's password.
   */
  password: string
}

/**
 * Represents the data transfer object for a registration form.
 */
export interface RegisterFormDto {
  /**
   * The user's username.
   */
  username: string
  /**
   * The user's password.
   */
  password: string
  /**
   * The invitation code required for registration.
   */
  invitationCode?: string
}

/**
 * Sends a login request to the API.
 * @param payload - The login form data.
 * @returns A promise that resolves to the authentication information.
 */
export const login = async (payload: LoginFormDto) => {
  // OpenAPI: POST /api/Authentication/login
  const { data } = await axios.post<AuthInfo>('/Authentication/login', payload)
  return data
}

/**
 * Sends a registration request to the API.
 * @param payload - The registration form data.
 * @returns A promise that resolves to the authentication information.
 */
export const register = async (payload: RegisterFormDto) => {
  // OpenAPI: POST /api/Authentication/register
  const { data } = await axios.post<AuthInfo>('/Authentication/register', payload)
  return data
}
