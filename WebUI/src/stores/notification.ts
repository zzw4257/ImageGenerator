import { defineStore } from 'pinia'

export type NotificationType = 'success' | 'error' | 'warning' | 'info'

export interface NotificationMessage {
  id: string
  message: string
  type: NotificationType
  icon?: string
  timeout?: number
  persistent?: boolean
  action?: {
    text: string
    handler: () => void
  }
}

export const useNotificationStore = defineStore('notification', () => {
  const messages = ref<NotificationMessage[]>([])
  const currentMessage = ref<NotificationMessage | null>(null)
  const isShowing = ref(false)

  const defaultIcons: Record<NotificationType, string> = {
    success: 'mdi-check-circle',
    error: 'mdi-alert-circle',
    warning: 'mdi-alert',
    info: 'mdi-information'
  }

  const defaultTimeouts: Record<NotificationType, number> = {
    success: 4000,
    info: 5000,
    warning: 6000,
    error: 8000
  }

  const generateId = () => `notification_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`

  const addMessage = (options: {
    message: string
    type: NotificationType
    icon?: string
    timeout?: number
    persistent?: boolean
    action?: {
      text: string
      handler: () => void
    }
  }) => {
    const notification: NotificationMessage = {
      id: generateId(),
      message: options.message,
      type: options.type,
      icon: options.icon || defaultIcons[options.type],
      timeout: options.persistent ? 0 : (options.timeout || defaultTimeouts[options.type]),
      persistent: options.persistent || false,
      action: options.action
    }

    messages.value.push(notification)
    processQueue()
  }

  const removeMessage = (id: string) => {
    const index = messages.value.findIndex(msg => msg.id === id)
    if (index > -1) {
      messages.value.splice(index, 1)
    }
    
    if (currentMessage.value?.id === id) {
      currentMessage.value = null
      isShowing.value = false
      // Process next message after a short delay
      setTimeout(processQueue, 300)
    }
  }

  const processQueue = () => {
    if (isShowing.value || messages.value.length === 0) return

    const nextMessage = messages.value[0]
    if (!nextMessage) return
    
    currentMessage.value = nextMessage
    isShowing.value = true

    // Auto-remove non-persistent messages
    if (!nextMessage.persistent && nextMessage.timeout && nextMessage.timeout > 0) {
      setTimeout(() => {
        removeMessage(nextMessage.id)
      }, nextMessage.timeout)
    }
  }

  const clearAll = () => {
    messages.value = []
    currentMessage.value = null
    isShowing.value = false
  }

  // Convenience methods
  const success = (message: string, options?: Partial<Omit<NotificationMessage, 'id' | 'message' | 'type'>>) => {
    addMessage({ message, type: 'success', ...options })
  }

  const error = (message: string, options?: Partial<Omit<NotificationMessage, 'id' | 'message' | 'type'>>) => {
    addMessage({ message, type: 'error', ...options })
  }

  const warning = (message: string, options?: Partial<Omit<NotificationMessage, 'id' | 'message' | 'type'>>) => {
    addMessage({ message, type: 'warning', ...options })
  }

  const info = (message: string, options?: Partial<Omit<NotificationMessage, 'id' | 'message' | 'type'>>) => {
    addMessage({ message, type: 'info', ...options })
  }

  const dismiss = () => {
    if (currentMessage.value) {
      removeMessage(currentMessage.value.id)
    }
  }

  return {
    // State
    messages: readonly(messages),
    currentMessage: readonly(currentMessage),
    isShowing: readonly(isShowing),
    
    // Actions
    addMessage,
    removeMessage,
    clearAll,
    dismiss,
    
    // Convenience methods
    success,
    error,
    warning,
    info
  }
})