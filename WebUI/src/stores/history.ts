import { defineStore } from 'pinia'
import axios from '@/helpers/RequestHelper'
import type { GenerationRecordDto } from '@/types/api'

function parsePagination(headers: any) {
  const h = headers?.['x-pagination'] ?? headers?.['X-Pagination']
  if (!h) return { TotalCount: 0, PageSize: 12, PageNumber: 0, TotalPages: 1 }
  try { return JSON.parse(h) } catch { return { TotalCount: 0, PageSize: 12, PageNumber: 0, TotalPages: 1 } }
}

export interface GenerationHistoryItem extends GenerationRecordDto {
  conversationId: string
  thumbnail: string
}

export const useHistoryStore = defineStore('history', {
  state: () => ({
    generations: [] as GenerationHistoryItem[],
    genPagination: { TotalPages: 1, PageNumber: 0, PageSize: 12, TotalCount: 0 } as any,
    genLoading: false,
    genError: null as string | null,
  }),
  actions: {
    // 获取生成历史 - 通过Conversation API
    async fetchGenerations(page = 0, pageSize = 12) {
      this.genLoading = true
      this.genError = null
      try {
        const res = await axios.get('/Conversation/conversations', { 
          params: { pageNumber: page, pageSize } 
        })
        
        // 从conversations中提取所有generation records
        const allRecords: GenerationHistoryItem[] = []
        res.data.forEach((conversation: any) => {
          if (conversation.generationRecords && conversation.generationRecords.length > 0) {
            conversation.generationRecords.forEach((record: any) => {
              allRecords.push({
                ...record,
                conversationId: conversation.id,
                thumbnail: record.outputImage?.imagePath || '/images/placeholder.svg'
              })
            })
          }
        })
        
        // 按创建时间排序
        allRecords.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
        
        this.generations = allRecords
        this.genPagination = parsePagination(res.headers)
      } catch (e: any) {
        this.genError = e?.message ?? String(e)
        this.generations = []
        console.error('获取生成历史失败:', e)
      } finally {
        this.genLoading = false
      }
    },

    // 获取所有生成历史记录（用于筛选）
    async fetchAllGenerations() {
      this.genLoading = true
      this.genError = null
      try {
        // 获取所有对话记录，不分页
        const res = await axios.get('/Conversation/conversations', { 
          params: { pageNumber: 0, pageSize: 1000 } // 使用大页面大小获取所有数据
        })
        
        // 从conversations中提取所有generation records
        const allRecords: GenerationHistoryItem[] = []
        res.data.forEach((conversation: any) => {
          if (conversation.generationRecords && conversation.generationRecords.length > 0) {
            conversation.generationRecords.forEach((record: any) => {
              allRecords.push({
                ...record,
                conversationId: conversation.id,
                thumbnail: record.outputImage?.imagePath || '/images/placeholder.svg'
              })
            })
          }
        })
        
        // 按创建时间排序
        allRecords.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
        
        return allRecords
      } catch (e: any) {
        this.genError = e?.message ?? String(e)
        console.error('获取所有生成历史失败:', e)
        return []
      } finally {
        this.genLoading = false
      }
    }
  }
})