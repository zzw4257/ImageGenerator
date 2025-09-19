export const setAllItem = (items: Record<string, string>) => {
  for (const [key, value] of Object.entries(items)) {
    localStorage.setItem(key, value)
    }
}

export const getAllItem = (keys: string[]): Record<string, string> => {
  const result: Record<string, string> = {}
  for (const key of keys) {
    const value = localStorage.getItem(key)
    if (value !== null) {
      result[key] = value
    }
  }
  return result
}