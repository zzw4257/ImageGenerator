import { defineStore } from 'pinia'

/**
 * The type of notification.
 */
export type NotificationType = 'success' | 'error' | 'warning' | 'info'

/**
 * Represents a notification message.
 */
export interface NotificationMessage {
  /** The unique ID of the message. */
  id: string
  /** The content of the message. */
  message: string
  /** The type of the message. */
  type: NotificationType
  /** The icon to display. */
  icon?: string
  /** The duration in milliseconds to display the message. */
  timeout?: number
  /** Whether the message should persist until dismissed. */
  persistent?: boolean
  /** An optional action to display with the message. */
  action?: {
    text: string
    handler: () => void
  }
}

/**
 * A store for managing a queue of snackbar notifications.
 */
export const useNotificationStore = defineStore('notification', () => {
  /** The queue of notification messages. */
  const messages = ref<NotificationMessage[]>([])
  /** The currently displayed message. */
  const currentMessage = ref<NotificationMessage | null>(null)
  /** Whether a notification is currently being shown. */
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

  /**
   * Adds a new notification message to the queue.
   * @param options - The options for the notification message.
   */
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

  /**
   * Removes a notification message from the queue.
   * @param id - The ID of the message to remove.
   */
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

  /**
   * Processes the notification queue and displays the next message.
   */
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

  /**
   * Clears all notification messages.
   */
  const clearAll = () => {
    messages.value = []
    currentMessage.value = null
    isShowing.value = false
  }

  /**
   * Shows a success notification.
   * @param message - The message to display.
   * @param options - Additional options for the notification.
   */
  const success = (message: string, options?: Partial<Omit<NotificationMessage, 'id' | 'message' | 'type'>>) => {
    addMessage({ message, type: 'success', ...options })
  }

  /**
   * Shows an error notification.
   * @param message - The message to display.
   * @param options - Additional options for the notification.
   */
  const error = (message: string, options?: Partial<Omit<NotificationMessage, 'id' | 'message' | 'type'>>) => {
    addMessage({ message, type: 'error', ...options })
  }

  /**
   * Shows a warning notification.
   * @param message - The message to display.
   * @param options - Additional options for the notification.
   */
  const warning = (message: string, options?: Partial<Omit<NotificationMessage, 'id' | 'message' | 'type'>>) => {
    addMessage({ message, type: 'warning', ...options })
  }

  /**
   * Shows an info notification.
   * @param message - The message to display.
   * @param options - Additional options for the notification.
   */
  const info = (message: string, options?: Partial<Omit<NotificationMessage, 'id' | 'message' | 'type'>>) => {
    addMessage({ message, type: 'info', ...options })
  }

  /**
   * Dismisses the currently displayed notification.
   */
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