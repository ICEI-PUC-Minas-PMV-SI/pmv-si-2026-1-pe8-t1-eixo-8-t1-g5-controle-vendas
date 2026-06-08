import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom'
import { lazy, Suspense } from 'react'
import { Login } from '../pages/Login/Login'
import { Dashboard } from '../pages/Dashboard/Dashboard'
import { ClientesList } from '../pages/Clientes/ClientesList'
import { ClienteDetail } from '../pages/Clientes/ClienteDetail'
import { NovoCliente } from '../pages/Clientes/NovoCliente'
import { VendasList } from '../pages/Vendas/VendasList'
import { NovaVenda } from '../pages/Vendas/NovaVenda'
import { FiadoList } from '../pages/Fiado/FiadoList'
import { PrivateRoute } from './PrivateRoute'
import { PageLayout } from '../components/common/PageLayout'
import { LoadingSpinner } from '../components/common/LoadingSpinner'

// Lazy load RelatoriosPage for better code splitting
const RelatoriosPage = lazy(() => import('../pages/Relatorios/RelatoriosPage'))

/**
 * Router
 * Configuração de todas as rotas da aplicação
 */
export function Router() {
  return (
    <BrowserRouter>
      <Routes>
        {/* Rota de login (pública) */}
        <Route path="/login" element={<Login />} />

        {/* Rota raiz redireciona para dashboard */}
        <Route path="/" element={<Navigate to="/dashboard" replace />} />

        {/* Rotas privadas com layout */}
        <Route
          path="/dashboard"
          element={
            <PrivateRoute>
              <PageLayout>
                <Dashboard />
              </PageLayout>
            </PrivateRoute>
          }
        />

        <Route
          path="/clientes"
          element={
            <PrivateRoute>
              <PageLayout>
                <ClientesList />
              </PageLayout>
            </PrivateRoute>
          }
        />

        <Route
          path="/clientes/novo"
          element={
            <PrivateRoute>
              <PageLayout>
                <NovoCliente />
              </PageLayout>
            </PrivateRoute>
          }
        />

        <Route
          path="/clientes/:clienteId/editar"
          element={
            <PrivateRoute>
              <PageLayout>
                <NovoCliente />
              </PageLayout>
            </PrivateRoute>
          }
        />

        <Route
          path="/clientes/:clienteId"
          element={
            <PrivateRoute>
              <PageLayout>
                <ClienteDetail />
              </PageLayout>
            </PrivateRoute>
          }
        />

        <Route
          path="/vendas"
          element={
            <PrivateRoute>
              <PageLayout>
                <VendasList />
              </PageLayout>
            </PrivateRoute>
          }
        />

        <Route
          path="/vendas/nova"
          element={
            <PrivateRoute>
              <PageLayout>
                <NovaVenda />
              </PageLayout>
            </PrivateRoute>
          }
        />

        <Route
          path="/vendas/:vendaId/editar"
          element={
            <PrivateRoute>
              <PageLayout>
                <NovaVenda />
              </PageLayout>
            </PrivateRoute>
          }
        />

        <Route
          path="/fiado"
          element={
            <PrivateRoute>
              <PageLayout>
                <FiadoList />
              </PageLayout>
            </PrivateRoute>
          }
        />

        <Route
          path="/relatorios"
          element={
            <PrivateRoute>
              <PageLayout>
                <Suspense fallback={<LoadingSpinner message="Carregando relatórios..." />}>
                  <RelatoriosPage />
                </Suspense>
              </PageLayout>
            </PrivateRoute>
          }
        />

        {/* 404 */}
        <Route
          path="*"
          element={
            <div className="h-screen flex items-center justify-center bg-gray-50">
              <div className="text-center">
                <h1 className="text-4xl font-bold text-gray-900">404</h1>
                <p className="text-gray-600 mt-2">Página não encontrada</p>
                <a href="/dashboard" className="text-blue-600 hover:underline mt-4 inline-block">
                  Voltar ao Dashboard
                </a>
              </div>
            </div>
          }
        />
      </Routes>
    </BrowserRouter>
  )
}

export default Router
