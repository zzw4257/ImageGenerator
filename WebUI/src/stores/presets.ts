import { defineStore } from 'pinia'
import axios from '@/helpers/RequestHelper'

export interface PresetDto {
  id: string
  name: string
  description?: string
  coverUrl?: string
  priceCredits?: number
  prompt: string
  provider?: string
  defaultParams?: string // JSON string from backend
  tags?: string[]
}

function parsePagination(headers: any) {
  const h = headers?.['x-pagination'] ?? headers?.['X-Pagination']
  if (!h) return { TotalCount: 0, PageSize: 12, PageNumber: 0, TotalPages: 1 }
  try { return JSON.parse(h) } catch { return { TotalCount: 0, PageSize: 12, PageNumber: 0, TotalPages: 1 } }
}

export const usePresetStore = defineStore('preset', {
  state: () => ({
    items: [] as PresetDto[],
    pagination: { TotalCount: 0, PageSize: 12, PageNumber: 0, TotalPages: 1 } as any,
    loading: false,
    error: '' as string | null,
    selectedPreset: (null as PresetDto | null)
  }),
  actions: {
    async fetchPresets(page = 0, pageSize = 12) {
      this.loading = true
      this.error = null
      try {
        const res = await axios.get<PresetDto[]>('/presets', { params: { pageNumber: page, pageSize } })
        this.items = res.data
        this.pagination = parsePagination(res.headers)
      } catch (e: any) {
        this.error = e?.message ?? String(e)
        this.items = []
      } finally {
        this.loading = false
      }
    },
    selectPreset(p?: PresetDto | null) {
      this.selectedPreset = p ?? null
      if (p) sessionStorage.setItem('selectedPreset', JSON.stringify(p))
      else sessionStorage.removeItem('selectedPreset')
    },
    restoreSelectedFromSession() {
      const raw = sessionStorage.getItem('selectedPreset')
      if (!raw) return
      try { this.selectedPreset = JSON.parse(raw) } catch { this.selectedPreset = null }
    },
    async getById(id: string) {
      try {
        const { data } = await axios.get<PresetDto>(`/presets/${encodeURIComponent(id)}`)
        return data
      } catch (e) {
        return null
      }
    }
  }
})