import axios from '@/helpers/RequestHelper'

export interface TransactionDto {
  id: string
  amount: number
  type: string
  createdAt: string
  description?: string
}

export const listTransactions = async (page = 0, pageSize = 12): Promise<{ items: TransactionDto[]; pagination: any }> => {
  const { data, headers } = await axios.get<TransactionDto[]>('/wallet/transactions', { params: { pageNumber: page, pageSize } })
  const pagination = headers['x-pagination'] ? JSON.parse(headers['x-pagination']) : { TotalCount: data.length, PageSize: pageSize, PageNumber: page, TotalPages: 1 }
  return { items: data, pagination }
}