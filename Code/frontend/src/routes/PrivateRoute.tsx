import React from 'react'
import { Navigate } from 'react-router-dom'
import { useAuth } from '../hooks'
import { LoadingSpinner } from '../components/common/LoadingSpinner'

interface PrivateRouteProps {
  children: React.ReactNode
}

/**
 * PrivateRoute
 * Componente que protege rotas que requerem autenticação
 * Se o usuário não estiver autenticado, redireciona para /login
 * Se estiver carregando, mostra spinner
 */
export function PrivateRoute({ children }: PrivateRouteProps) {
  const { isAuthenticated, isLoading } = useAuth()
  
  console.log('[PrivateRoute] isLoading:', isLoading, 'isAuthenticated:', isAuthenticated)

  // Aguardar carregamento da sessão
  if (isLoading) {
    console.log('[PrivateRoute] Carregando sessão...')
    return <LoadingSpinner message="Carregando..." />
  }

  if (!isAuthenticated) {
    console.log('[PrivateRoute] Usuário não autenticado, redirecionando para login')
    return <Navigate to="/login" replace />
  }

  console.log('[PrivateRoute] Renderizando children')
  return <>{children}</>
}
