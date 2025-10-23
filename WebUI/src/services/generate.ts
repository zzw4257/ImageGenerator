import axios from '@/helpers/RequestHelper'
import { ErrorHandler } from '@/utils/errorHandler'

export interface GenerateRequestDto {
  conversationId?: string
  presetId?: string
  prompt: string
  provider: string
  params?: string
  inputImageIds?: string[]
  quality?: 'standard' | 'hd'
  style?: 'vivid' | 'natural'
}

export interface GenerateResponseDto {
  taskId: string
  estCost: number
}

export interface GenerateTaskStatusDto {
  taskId: string
  status: number // 后端返回数字: 0=Pending, 1=Processing, 2=Completed, 3=Failed
  imageUrl?: string
  error?: string
  prompt?: string
  provider?: string
  createdAt: string
  completedAt?: string
}

/**
 * 提交图像生成任务
 */
export const generateImage = async (request: GenerateRequestDto): Promise<GenerateResponseDto> => {
  try {
    const { data } = await axios.post<GenerateResponseDto>('/generate', request)
    return data
  } catch (error) {
    ErrorHandler.handle(error, '图像生成')
    throw error
  }
}

/**
 * 获取生成任务状态
 */
export const getTaskStatus = async (taskId: string): Promise<GenerateTaskStatusDto> => {
  try {
    const { data } = await axios.get<GenerateTaskStatusDto>(`/generate/${taskId}`)
    return data
  } catch (error) {
    ErrorHandler.handle(error, '获取任务状态')
    throw error
  }
}

/**
 * 轮询任务状态直到完成
 */
export const pollTaskStatus = async (
  taskId: string,
  onUpdate?: (status: GenerateTaskStatusDto) => void,
  maxAttempts = 30,
  interval = 2000
): Promise<GenerateTaskStatusDto> => {
  let attempts = 0
  
  while (attempts < maxAttempts) {
    try {
      const status = await getTaskStatus(taskId)
      
      if (onUpdate) {
        onUpdate(status)
      }
      
      // 后端返回数字状态: 0=Pending, 1=Processing, 2=Completed, 3=Failed
      if (status.status === 2 || status.status === 3) {
        return status
      }
      
      await new Promise(resolve => setTimeout(resolve, interval))
      attempts++
    } catch (error) {
      // 轮询过程中的错误不显示通知，只记录日志
      console.error('Error polling task status:', error)
      throw error
    }
  }
  
  throw new Error('任务轮询超时')
}
