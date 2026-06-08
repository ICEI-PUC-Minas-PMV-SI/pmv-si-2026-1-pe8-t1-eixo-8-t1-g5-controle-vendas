import type { Usuario } from './domain'

// Tipos padrão de resposta da API

export interface ApiResponse<T = any> {
  success: boolean
  data: T
  message: string
  errors?: string[]
}

export interface PaginatedResponse<T> {
  items: T[]
  totalItems: number
  pageNumber: number
  pageSize: number
  totalPages: number
  hasNextPage?: boolean
}

export interface LoginRequest {
  email: string
  senha: string
}

export interface LoginResponse {
  token: string
  expiresAt: string
  usuario: Usuario
}

export interface ErrorResponse {
  success: boolean
  message: string
  errors?: {
    [key: string]: string[]
  }
}
