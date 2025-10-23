import { defineStore } from 'pinia'
import axios from '@/helpers/RequestHelper'

function parsePagination(headers: any) {
  const h = headers?.['x-pagination'] ?? headers?.['X-Pagination']
  if (!h) return { TotalCount: 0, PageSize: 12, PageNumber: 0, TotalPages: 1 }
  try { return JSON.parse(h) } catch { return { TotalCount: 0, PageSize: 12, PageNumber: 0, TotalPages: 1 } }
}

export const useHistoryStore = defineStore('history', {
  state: () => ({
    generations: [] as any[],
    genPagination: { TotalPages: 1, PageNumber: 0, PageSize: 12, TotalCount: 0 } as any,
    genLoading: false,
    genError: null as string | null,
  }),
  actions: {
    // 尝试使用常见历史接口名；后端若不同可在这里调整 endpoint
    async fetchGenerations(page = 0, pageSize = 12) {
      this.genLoading = true
      this.genError = null
      try {
        // 优先尝试 /generate/history，若 404 再尝试 Conversation 路径
        let res
        try {
          res = await axios.get('/generate/history', { params: { pageNumber: page, pageSize } })
        } catch (e: any) {
          res = await axios.get('/Conversation/conversations', { params: { pageNumber: page, pageSize } })
        }
        this.generations = res.data
        this.genPagination = parsePagination(res.headers)
      } catch (e: any) {
        this.genError = e?.message ?? String(e)
        this.generations = []
      } finally {
        this.genLoading = false
      }
    }
  }
})