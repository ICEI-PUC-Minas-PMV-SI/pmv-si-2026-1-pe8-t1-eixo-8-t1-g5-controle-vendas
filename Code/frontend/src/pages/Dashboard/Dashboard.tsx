import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAuth, useApi } from '../../hooks'
import { relatorioService } from '../../services/api'
import { KpiCard } from '../../components/cards/KpiCard'
import { ReceitaMensalChart, TopDevedoresChart } from '../../components/charts'
import { LoadingSpinner } from '../../components/common/LoadingSpinner'
import { ErrorAlert } from '../../components/common/ErrorAlert'
import type { DashboardKPIs } from '../../types/domain'
import type { ConcentracaoDto } from '../../services/api/relatorio-service'

interface ChartData {
  receita: Array<{
    mes: string
    receita: number
  }>
  devedores: Array<{
    nome: string
    valor: number
  }>
}

export function Dashboard() {
  const { user } = useAuth()
  const navigate = useNavigate()
  const dashboardApi = useApi<DashboardKPIs>()
  const concentracaoApi = useApi<ConcentracaoDto>()
  const [chartData, setChartData] = useState<ChartData>({
    receita: [],
    devedores: [],
  })

  // Carregar dados do dashboard
  useEffect(() => {
    const loadDashboard = async () => {
      try {
        // Carregar KPIs
        const kpis = await dashboardApi.execute(() =>
          relatorioService.getDashboard()
        )

        // Carregar concentração (para top devedores)
        const concentracao = await concentracaoApi.execute(() =>
          relatorioService.getConcentracao()
        )

        const receitaMensal = await relatorioService.getReceitaMensal(6)

        if (kpis && concentracao) {
          const devedores =
            concentracao?.topClientes?.map((c) => ({
              nome: c.cliente.nome,
              valor: c.totalDevendo,
            })) || []

          setChartData({
            receita: receitaMensal.map((item) => ({
              mes: item.nomeMes,
              receita: item.receita,
            })),
            devedores,
          })
        }
      } catch (err) {
        console.error('Erro ao carregar dashboard:', err)
      }
    }

    loadDashboard()
  }, [])

  const isLoading = dashboardApi.isLoading || concentracaoApi.isLoading
  const error = dashboardApi.error || concentracaoApi.error
  const kpis = dashboardApi.data

  return (
    <div className="p-6 space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">
            Bem-vinda, {user?.nomeCompleto}!
          </h1>
          <p className="text-gray-600 mt-1">
            Dashboard de vendas e inadimplência em tempo real
          </p>
        </div>
        <button
          onClick={() => navigate('/vendas/nova')}
          className="btn-primary px-6 py-3"
        >
          + Nova Venda
        </button>
      </div>

      {/* Alertas */}
      {error && <ErrorAlert message={error} />}
      {isLoading && <LoadingSpinner message="Carregando dashboard..." />}

      {/* KPI Cards */}
      {!isLoading && kpis && (
        <>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
            <KpiCard
              title="Receita Mês"
              value={kpis.receitaMes || 0}
              unit="R$"
              bgColor="blue"
              icon="💰"
            />
            <KpiCard
              title="A Receber"
              value={kpis.totalAReceber || 0}
              unit="R$"
              bgColor="orange"
              icon="📊"
            />
            <KpiCard
              title="Inadimplentes"
              value={kpis.clientesEmAtraso || 0}
              bgColor="red"
              icon="⚠️"
            />
            <KpiCard
              title="Ticket Médio"
              value={
                kpis.ticketMedio
                  ? (kpis.ticketMedio as number).toFixed(2)
                  : '0.00'
              }
              unit="R$"
              bgColor="green"
              icon="🎯"
            />
          </div>

          {/* Gráficos */}
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
            {/* Receita Mensal */}
            <div className="card">
              <h3 className="text-lg font-semibold text-gray-900 mb-4">
                Receita Últimos 6 Meses
              </h3>
              <ReceitaMensalChart data={chartData.receita} />
            </div>

            {/* Top Devedores */}
            <div className="card">
              <h3 className="text-lg font-semibold text-gray-900 mb-4">
                Top 5 Devedores
              </h3>
              <TopDevedoresChart data={chartData.devedores} />
            </div>
          </div>

          {/* Métricas Adicionais */}
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            <div className="card">
              <p className="text-sm font-medium text-gray-600">
                Taxa de Inadimplência
              </p>
              <p className="text-3xl font-bold text-red-600 mt-2">
                {kpis.taxaInadimplencia
                  ? (kpis.taxaInadimplencia as number).toFixed(1)
                  : '0'}
                %
              </p>
              <p className="text-xs text-gray-500 mt-2">
                Percentual de clientes inadimplentes
              </p>
            </div>

            <div className="card">
              <p className="text-sm font-medium text-gray-600">
                Período de Coleta
              </p>
              <p className="text-3xl font-bold text-blue-600 mt-2">
                {kpis.periodoColeta || 0}
              </p>
              <p className="text-xs text-gray-500 mt-2">dias em média</p>
            </div>

            <div className="card">
              <p className="text-sm font-medium text-gray-600">
                Índice de Concentração
              </p>
              <p className="text-3xl font-bold text-purple-600 mt-2">
                {kpis.indiceConcentracao
                  ? (kpis.indiceConcentracao as number).toFixed(1)
                  : '0'}
                %
              </p>
              <p className="text-xs text-gray-500 mt-2">
                Risco de concentração de crédito
              </p>
            </div>
          </div>

          {/* Quick Actions */}
          <div>
            <h2 className="text-xl font-bold text-gray-900 mb-4">
              Ações Rápidas
            </h2>
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
              <button
                onClick={() => navigate('/clientes')}
                className="card hover:shadow-md transition text-center py-6"
              >
                <div className="text-4xl mb-3">👥</div>
                <p className="font-semibold text-gray-900">Clientes</p>
                <p className="text-xs text-gray-600 mt-2">
                  Gerencie seus clientes
                </p>
              </button>

              <button
                onClick={() => navigate('/vendas/nova')}
                className="card hover:shadow-md transition text-center py-6"
              >
                <div className="text-4xl mb-3">➕</div>
                <p className="font-semibold text-gray-900">Nova Venda</p>
                <p className="text-xs text-gray-600 mt-2">Registrar venda</p>
              </button>

              <button
                onClick={() => navigate('/relatorios')}
                className="card hover:shadow-md transition text-center py-6"
              >
                <div className="text-4xl mb-3">📋</div>
                <p className="font-semibold text-gray-900">Relatórios</p>
                <p className="text-xs text-gray-600 mt-2">
                  Análises detalhadas
                </p>
              </button>
            </div>
          </div>
        </>
      )}
    </div>
  )
}

export default Dashboard
