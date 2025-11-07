import { computed, ref } from 'vue'

export function useAuth () {
  const getRole = (): number => {
    try {
      if (typeof window !== 'undefined' && window.localStorage) {
        const role = localStorage.getItem('role')
        return role ? Number.parseInt(role) : 0
      }
    } catch (error) {
      console.error('Error accessing localStorage:', error)
    }
    return 0
  }

  const userRole = ref(getRole())

  // 响应式的用户角色
  const role = computed(() => userRole.value)

  return role
}
