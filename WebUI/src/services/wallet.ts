import type { TransactionType } from '@/enums'
import axios from '@/helpers/RequestHelper'
import { ErrorHandler } from '@/utils/errorHandler'

export interface TransactionDto {
  id: string
  amount: number
  type: TransactionType
  balanceAfter: number
  description: string
  createdAt: string
  creatorId: string
}

export interface BalanceDto {
  balance: number
}

/**
 * 获取用户余额
 */
export async function getBalance (): Promise<BalanceDto> {
  try {
    const { data } = await axios.get<BalanceDto>('/wallet/balance')
    return data
  } catch (error) {
    ErrorHandler.handle(error, '获取余额')
    throw error
  }
}

/**
 * 获取交易流水列表
 */
export async function listTransactions (page = 0, pageSize = 12): Promise<{ items: TransactionDto[], pagination: any }> {
  try {
    const { data, headers } = await axios.get<TransactionDto[]>('/wallet/transactions', {
      params: { pageNumber: page, pageSize },
    })
    const pagination = headers['x-pagination']
      ? JSON.parse(headers['x-pagination'])
      : {
          TotalCount: data.length,
          PageSize: pageSize,
          PageNumber: page,
          TotalPages: 1,
        }
    return { items: data, pagination }
  } catch (error) {
    ErrorHandler.handle(error, '获取交易流水')
    throw error
  }
}

/**
 * 发放Credits
 */
export async function grantCredits (amount: number): Promise<TransactionDto> {
  try {
    const { data } = await axios.post<TransactionDto>(
      '/wallet/grant',
      JSON.stringify(amount),
      {
        headers: {
          'Content-Type': 'application/json',
        },
      },
    )
    return data
  } catch (error) {
    ErrorHandler.handle(error, '发放Credits')
    throw error
  }
}
