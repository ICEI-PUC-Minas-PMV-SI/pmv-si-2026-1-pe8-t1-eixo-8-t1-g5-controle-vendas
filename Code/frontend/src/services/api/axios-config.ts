import axios, { AxiosError, type AxiosResponse } from 'axios'
import { ApiError, AuthError, ValidationError, BusinessError, NetworkError } from '../../types/errors'
import type { ApiResponse } from '../../types/api-response'

// URL base da API
const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api'
const API_TIMEOUT = Number(import.meta.env.VITE_API_TIMEOUT || 30000)

// Debug
if (import.meta.env.DEV) {
  console.log('[Axios Config] API_BASE_URL:', API_BASE_URL)
}

// Criar instância do axios
const axiosInstance = axios.create({
  baseURL: API_BASE_URL,
  timeout: API_TIMEOUT,
  headers: {
    'Content-Type': 'application/json',
  },
})

/**
 * Interceptor de requisição
 * Injeta o token JWT no header Authorization
 */
axiosInstance.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('auth_token')

    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }

    // Log em desenvolvimento
    if (import.meta.env.DEV) {
      console.log(`[API] ${config.method?.toUpperCase()} ${config.url}`)
    }

    return config
  },
  (error) => {
    console.error('[API Request Error]', error)
    return Promise.reject(error)
  }
)

/**
 * Interceptor de resposta
 * Trata erros e padroniza respostas
 */
axiosInstance.interceptors.response.use(
  (response: AxiosResponse<ApiResponse>) => {
    // Log em desenvolvimento
    if (import.meta.env.DEV) {
      console.log(`[API Response] Status ${response.status}`)
      console.log('[API Response] response.data:', response.data)
      console.log('[API Response] response.data.data:', (response.data as any)?.data)
    }

    // Retorna apenas os dados (desembrulha do AxiosResponse)
    return response.data as any
  },
  (error: AxiosError<any>) => {
    const status = error.response?.status || 0
    const data = error.response?.data
    const message = data?.message || error.message || 'Erro desconhecido'

    // Log de erro
    console.error(`[API Error] ${status}: ${message}`, data)

    // Tratar erro 401 (não autenticado)
    if (status === 401) {
      // Limpar token do localStorage
      localStorage.removeItem('auth_token')
      localStorage.removeItem('auth_user')

      // Redirecionar para login
      window.location.href = '/login'

      return Promise.reject(
        new AuthError('Sessão expirada. Faça login novamente.')
      )
    }

    // Tratar erro 400 (validação)
    if (status === 400) {
      return Promise.reject(
        new ValidationError(
          message,
          data?.errors
        )
      )
    }

    // Tratar erro 409 (conflito - limite de crédito excedido, etc)
    if (status === 409) {
      return Promise.reject(
        new BusinessError(message, data?.details)
      )
    }

    // Tratar erro 404 (não encontrado)
    if (status === 404) {
      return Promise.reject(
        new ApiError(message, status)
      )
    }

    // Tratar erro de conexão (timeout, offline, etc)
    if (!error.response) {
      return Promise.reject(
        new NetworkError(
          'Erro de conexão. Verifique sua internet e tente novamente.'
        )
      )
    }

    // Erro genérico
    return Promise.reject(
      new ApiError(message || 'Erro ao processar a requisição', status, data)
    )
  }
)

export default axiosInstance
