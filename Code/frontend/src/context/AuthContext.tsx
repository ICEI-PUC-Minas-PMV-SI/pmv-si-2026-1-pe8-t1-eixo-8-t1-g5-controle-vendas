import React, { createContext, useReducer, useCallback, useEffect, type ReactNode } from 'react'
import authService from '../services/api/auth-service'
import type { Usuario } from '../types/domain'

interface AuthState {
  user: Usuario | null
  token: string | null
  isLoading: boolean
  error: string | null
  isAuthenticated: boolean
}

interface AuthAction {
  type: 'LOGIN_REQUEST' | 'LOGIN_SUCCESS' | 'LOGIN_ERROR' | 'LOGOUT' | 'RESTORE_SESSION' | 'INIT_COMPLETE'
  payload?: any
}

export interface AuthContextType extends AuthState {
  login: (email: string, senha: string) => Promise<void>
  logout: () => Promise<void>
  clearError: () => void
}

const initialState: AuthState = {
  user: null,
  token: null,
  isLoading: true,  // ← MUDANÇA: Começa carregando para verificar sessão
  error: null,
  isAuthenticated: false,
}

const authReducer = (state: AuthState, action: AuthAction): AuthState => {
  switch (action.type) {
    case 'LOGIN_REQUEST':
      return {
        ...state,
        isLoading: true,
        error: null,
      }

    case 'LOGIN_SUCCESS':
      return {
        ...state,
        user: action.payload.usuario,
        token: action.payload.token,
        isLoading: false,
        error: null,
        isAuthenticated: true,
      }

    case 'LOGIN_ERROR':
      return {
        ...state,
        isLoading: false,
        error: action.payload,
        isAuthenticated: false,
      }

    case 'LOGOUT':
      return {
        ...state,
        user: null,
        token: null,
        isLoading: false,
        error: null,
        isAuthenticated: false,
      }

    case 'RESTORE_SESSION':
      return {
        ...state,
        user: action.payload.user,
        token: action.payload.token,
        isAuthenticated: true,
        isLoading: false,
      }

    case 'INIT_COMPLETE':
      return {
        ...state,
        isLoading: false,
      }

    default:
      return state
  }
}

export const AuthContext = createContext<AuthContextType | undefined>(undefined)

export interface AuthProviderProps {
  children: ReactNode
}

export function AuthProvider({ children }: AuthProviderProps) {
  const [state, dispatch] = useReducer(authReducer, initialState)

  // Restaurar sessão ao montar o componente
  useEffect(() => {
    const token = authService.getToken()
    const user = authService.getUser()

    console.log('[AuthProvider] Verificando sessão armazenada...')
    console.log('[AuthProvider] Token:', token ? 'Encontrado' : 'Não encontrado')
    
    if (token && user) {
      console.log('[AuthProvider] Restaurando sessão...')
      dispatch({
        type: 'RESTORE_SESSION',
        payload: {
          token,
          user,
        },
      })
    } else {
      console.log('[AuthProvider] Sem sessão armazenada, finalizando carregamento')
      // Finalizar carregamento mesmo sem token
      dispatch({
        type: 'INIT_COMPLETE',
      })
    }
  }, [])

  const login = useCallback(async (email: string, senha: string) => {
    dispatch({ type: 'LOGIN_REQUEST' })

    try {
      console.log('[AuthContext] Iniciando login...')
      const response = await authService.login(email, senha)
      console.log('[AuthContext] Login sucesso, resposta:', response)

      dispatch({
        type: 'LOGIN_SUCCESS',
        payload: response,
      })
      console.log('[AuthContext] State atualizado com LOGIN_SUCCESS')
    } catch (error: any) {
      console.error('[AuthContext] Erro no login:', error)
      const errorMessage =
        error?.message || 'Erro ao fazer login. Verifique as credenciais.'

      dispatch({
        type: 'LOGIN_ERROR',
        payload: errorMessage,
      })

      throw error
    }
  }, [])

  const logout = useCallback(async () => {
    try {
      await authService.logout()
    } catch (error) {
      console.error('Logout error:', error)
    } finally {
      dispatch({ type: 'LOGOUT' })
    }
  }, [])

  const clearError = useCallback(() => {
    dispatch({
      type: 'LOGIN_ERROR',
      payload: null,
    })
  }, [])

  const value: AuthContextType = {
    ...state,
    login,
    logout,
    clearError,
  }

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export function useAuth(): AuthContextType {
  const context = React.useContext(AuthContext)

  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider')
  }

  return context
}
