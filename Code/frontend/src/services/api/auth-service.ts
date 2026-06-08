import axiosInstance from './axios-config'
import type { LoginResponse } from '../../types/api-response'
import type { Usuario } from '../../types/domain'
import { mapLoginResponse } from '../../utils/domain-mappers'

interface RegisterResponse {
  data?: string
}

/**
 * Serviço de autenticação
 * Encapsula todas as chamadas de autenticação para a API
 */
const authService = {
  /**
   * Fazer login
   * POST /api/auth/login
   */
  async login(email: string, senha: string): Promise<LoginResponse> {
    try {
      console.log('[AuthService] Chamando POST /auth/login com email:', email)
      const response = await axiosInstance.post<any, any>('/auth/login', {
        email,
        senha,
      })
      
      const apiResponse = response as any
      console.log('[AuthService] ApiResponse recebida:', apiResponse)
      
      const session = mapLoginResponse(apiResponse)
      console.log('[AuthService] Session mapeada:', session)
      console.log('[AuthService] Session.token:', session.token ? 'Encontrado' : 'NÃO ENCONTRADO')
      console.log('[AuthService] Session.usuario:', session.usuario)

      // Armazenar token
      if (session.token) {
        console.log('[AuthService] Token encontrado, armazenando...')
        localStorage.setItem('auth_token', session.token)
        localStorage.setItem('auth_user', JSON.stringify(session.usuario))
        console.log('[AuthService] Token e usuário armazenados no localStorage')
        console.log('[AuthService] localStorage.getItem("auth_token"):', localStorage.getItem('auth_token'))
      } else {
        console.error('[AuthService] ❌ Token não encontrado na resposta!')
        console.error('[AuthService] Session completa:', JSON.stringify(session))
      }

      return session
    } catch (error) {
      console.error('[AuthService] Login falhou:', error)
      throw error
    }
  },

  /**
   * Fazer logout
   */
  async logout(): Promise<void> {
    try {
      // Tentar chamar endpoint de logout (se existir no backend)
      await axiosInstance.post('/auth/logout', {})
    } catch (error) {
      console.warn('Logout request failed (may be normal):', error)
    } finally {
      // Limpar token localmente
      localStorage.removeItem('auth_token')
      localStorage.removeItem('auth_user')
    }
  },

  /**
   * Verificar se o usuário está autenticado
   */
  isAuthenticated(): boolean {
    const token = localStorage.getItem('auth_token')
    return !!token
  },

  /**
   * Obter o token armazenado
   */
  getToken(): string | null {
    return localStorage.getItem('auth_token')
  },

  /**
   * Obter os dados do usuário armazenados
   */
  getUser(): Usuario | null {
    const userStr = localStorage.getItem('auth_user')
    return userStr ? (JSON.parse(userStr) as Usuario) : null
  },

  /**
   * Registrar novo usuário (se implementado no backend)
   */
  async register(_nomeCompleto: string, email: string, senha: string): Promise<string> {
    try {
      const response = await axiosInstance.post<any, RegisterResponse>('/auth/register', {
        email,
        senha,
      })
      return response.data || ''
    } catch (error) {
      console.error('Register failed:', error)
      throw error
    }
  },
}

export default authService
