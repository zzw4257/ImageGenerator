import { useNotificationStore } from '@/stores/notification'

/**
 * 全局错误处理工具
 */
export class ErrorHandler {
  private static notificationStore = useNotificationStore()

  /**
   * 处理API错误
   */
  static handleApiError(error: any, defaultMessage = '操作失败') {
    console.error('API Error:', error)
    
    let message = defaultMessage
    
    if (error?.response?.data?.message) {
      message = error.response.data.message
    } else if (error?.message) {
      message = error.message
    } else if (typeof error === 'string') {
      message = error
    }

    // 根据HTTP状态码提供更友好的错误信息
    if (error?.response?.status) {
      switch (error.response.status) {
        case 401:
          message = '登录已过期，请重新登录'
          break
        case 403:
          message = '没有权限执行此操作'
          break
        case 404:
          message = '请求的资源不存在'
          break
        case 429:
          message = '请求过于频繁，请稍后再试'
          break
        case 500:
          message = '服务器内部错误'
          break
        case 502:
        case 503:
        case 504:
          message = '服务暂时不可用，请稍后再试'
          break
      }
    }

    this.notificationStore.error(message)
    return message
  }

  /**
   * 处理网络错误
   */
  static handleNetworkError(error: any) {
    console.error('Network Error:', error)
    
    let message = '网络连接失败'
    
    if (error?.code === 'NETWORK_ERROR' || error?.message?.includes('Network Error')) {
      message = '网络连接失败，请检查网络设置'
    } else if (error?.code === 'TIMEOUT') {
      message = '请求超时，请稍后再试'
    }

    this.notificationStore.error(message)
    return message
  }

  /**
   * 处理验证错误
   */
  static handleValidationError(error: any) {
    console.error('Validation Error:', error)
    
    let message = '输入数据有误'
    
    if (error?.response?.data?.errors) {
      const errors = error.response.data.errors
      const firstError = Object.values(errors)[0]
      if (Array.isArray(firstError) && firstError.length > 0) {
        message = firstError[0]
      }
    }

    this.notificationStore.error(message)
    return message
  }

  /**
   * 处理业务逻辑错误
   */
  static handleBusinessError(error: any, context = '') {
    console.error('Business Error:', error)
    
    let message = context ? `${context}失败` : '操作失败'
    
    if (error?.message) {
      message = error.message
    }

    this.notificationStore.error(message)
    return message
  }

  /**
   * 通用错误处理
   */
  static handle(error: any, context = '') {
    if (error?.response) {
      // API错误
      return this.handleApiError(error, context ? `${context}失败` : '操作失败')
    } else if (error?.code === 'NETWORK_ERROR' || error?.message?.includes('Network Error')) {
      // 网络错误
      return this.handleNetworkError(error)
    } else if (error?.message) {
      // 其他错误
      return this.handleBusinessError(error, context)
    } else {
      // 未知错误
      return this.handleBusinessError(new Error('未知错误'), context)
    }
  }
}

/**
 * 错误处理装饰器
 */
export function withErrorHandling<T extends (...args: any[]) => Promise<any>>(
  fn: T,
  context = ''
): T {
  return (async (...args: Parameters<T>) => {
    try {
      return await fn(...args)
    } catch (error) {
      ErrorHandler.handle(error, context)
      throw error
    }
  }) as T
}

/**
 * 静默错误处理（不显示通知）
 */
export function withSilentErrorHandling<T extends (...args: any[]) => Promise<any>>(
  fn: T
): T {
  return (async (...args: Parameters<T>) => {
    try {
      return await fn(...args)
    } catch (error) {
      console.error('Silent Error:', error)
      throw error
    }
  }) as T
}
