import { useEffect, useState } from 'react'
import { useApi } from '../../hooks'
import { relatorioService } from '../../services/api'
import { LoadingSpinner } from '../../components/common/LoadingSpinner'
import { ErrorAlert } from '../../components/common/ErrorAlert'
import type {
  InadimplenciaDto,
  ConcentracaoDto,
  InteraloDto,
} from '../../services/api/relatorio-service'

enum ReportTab {
  Inadimplencia = 'inadimplencia',
  Concentracao = 'concentracao',
  Intervalo = 'intervalo',
}

export function RelatoriosPage() {
  const [activeTab, setActiveTab] = useState<ReportTab>(ReportTab.Inadimplencia)
  
  // APIs
  const inadimplenciaApi = useApi<InadimplenciaDto[]>()
  const concentracaoApi = useApi<ConcentracaoDto>()
  const intervaloApi = useApi<InteraloDto>()
  
  // Form states para filtros
  const [diasAtraso, setDiasAtraso] = useState(30)
  const [dataInicio, setDataInicio] = useState(
    new Date(new Date().setMonth(new Date().getMonth() - 1))
      .toISOString()
      .split('T')[0]
  )
  const [dataFim, setDataFim] = useState(
    new Date().toISOString().split('T')[0]
  )

  // Carregar dados ao mudar de aba
  useEffect(() => {
    loadData()
  }, [activeTab, diasAtraso, dataInicio, dataFim])

  const loadData = async () => {
    switch (activeTab) {
      case ReportTab.Inadimplencia:
        await inadimplenciaApi.execute(() =>
          relatorioService.getInadimplencia(diasAtraso)
        )
        break
      case ReportTab.Concentracao:
        await concentracaoApi.execute(() =>
          relatorioService.getConcentracao(dataInicio, dataFim)
        )
        break
      case ReportTab.Intervalo:
        await intervaloApi.execute(() =>
          relatorioService.getIntervalo(dataInicio, dataFim, diasAtraso)
        )
        break
    }
  }

  const tabs = [
    { id: ReportTab.Inadimplencia, label: 'Inadimplência', icon: '⚠️' },
    { id: ReportTab.Concentracao, label: 'Concentração', icon: '📊' },
    { id: ReportTab.Intervalo, label: 'Por Intervalo', icon: '📅' },
  ]

  const formatCurrency = (value: number) =>
    new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL',
    }).format(value)

  return (
    <div className="p-6 space-y-6">
      {/* Header */}
      <div>
        <h1 className="text-3xl font-bold text-gray-900">Relatórios</h1>
        <p className="text-gray-600 mt-1">
          Análises detalhadas de inadimplência, concentração e performance
        </p>
      </div>

      {/* Tabs */}
      <div className="border-b border-gray-200">
        <div className="flex gap-0 -mb-px">
          {tabs.map((tab) => (
            <button
              key={tab.id}
              onClick={() => setActiveTab(tab.id)}
              className={`px-6 py-4 border-b-2 font-medium transition ${
                activeTab === tab.id
                  ? 'border-blue-600 text-blue-600'
                  : 'border-transparent text-gray-600 hover:text-gray-900'
              }`}
            >
              <span className="mr-2">{tab.icon}</span>
              {tab.label}
            </button>
          ))}
        </div>
      </div>

      {/* Content */}
      <div className="bg-white rounded-lg shadow p-6 space-y-4">
        {activeTab === ReportTab.Inadimplencia && (
          <>
            <div className="space-y-2">
              <h2 className="text-xl font-semibold text-gray-900">
                Relatório de Inadimplência
              </h2>
              <p className="text-gray-600 text-sm">
                Clientes com atrasos superiores a {diasAtraso} dias
              </p>
            </div>

            <div className="flex items-end gap-4 p-4 bg-gray-50 rounded">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Dias de atraso mínimo
                </label>
                <input
                  type="number"
                  value={diasAtraso}
                  onChange={(e) => setDiasAtraso(parseInt(e.target.value))}
                  min="1"
                  className="input-field w-32"
                />
              </div>
              <button
                onClick={loadData}
                className="btn-primary"
              >
                Atualizar
              </button>
            </div>

            {inadimplenciaApi.isLoading && (
              <LoadingSpinner message="Carregando relatório..." />
            )}

            {inadimplenciaApi.error && (
              <ErrorAlert message={inadimplenciaApi.error} />
            )}

            {inadimplenciaApi.data && (
              <div className="overflow-x-auto mt-4">
                {inadimplenciaApi.data.length === 0 ? (
                  <div className="text-center py-8 text-gray-500">
                    Nenhum cliente em inadimplência neste período
                  </div>
                ) : (
                  <table className="w-full text-sm">
                    <thead className="bg-gray-100 border-b border-gray-300">
                      <tr>
                        <th className="px-4 py-2 text-left font-medium">Cliente</th>
                        <th className="px-4 py-2 text-right font-medium">Dias em Atraso</th>
                        <th className="px-4 py-2 text-right font-medium">Total Devido</th>
                      </tr>
                    </thead>
                    <tbody>
                      {inadimplenciaApi.data.map((item) => (
                        <tr key={item.cliente.id} className="border-b hover:bg-gray-50">
                          <td className="px-4 py-3">{item.cliente.nome}</td>
                          <td className="px-4 py-3 text-right">
                            <span className="inline-block bg-red-100 text-red-700 px-3 py-1 rounded font-semibold">
                              {item.diasAtraso}
                            </span>
                          </td>
                          <td className="px-4 py-3 text-right font-semibold text-red-600">
                            {formatCurrency(item.totalDevendo)}
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                )}
              </div>
            )}
          </>
        )}

        {activeTab === ReportTab.Concentracao && (
          <>
            <div className="space-y-2">
              <h2 className="text-xl font-semibold text-gray-900">
                Concentração de Crédito
              </h2>
              <p className="text-gray-600 text-sm">
                Análise de exposição de crédito por cliente
              </p>
            </div>

            <div className="flex items-end gap-4 p-4 bg-gray-50 rounded">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Data início
                </label>
                <input
                  type="date"
                  value={dataInicio}
                  onChange={(e) => setDataInicio(e.target.value)}
                  className="input-field"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Data fim
                </label>
                <input
                  type="date"
                  value={dataFim}
                  onChange={(e) => setDataFim(e.target.value)}
                  className="input-field"
                />
              </div>
              <button
                onClick={loadData}
                className="btn-primary"
              >
                Atualizar
              </button>
            </div>

            {concentracaoApi.isLoading && (
              <LoadingSpinner message="Carregando relatório..." />
            )}

            {concentracaoApi.error && (
              <ErrorAlert message={concentracaoApi.error} />
            )}

            {concentracaoApi.data && (
              <div className="space-y-6 mt-4">
                <div className="p-4 bg-blue-50 rounded border border-blue-200">
                  <p className="text-sm text-blue-700">Índice de Concentração</p>
                  <p className="text-2xl font-bold text-blue-900">
                    {concentracaoApi.data.indiceConcentracao.toFixed(2)}%
                  </p>
                </div>

                <div>
                  <h3 className="font-semibold text-gray-900 mb-3">
                    Top Clientes por Exposição
                  </h3>
                  <div className="overflow-x-auto">
                    <table className="w-full text-sm">
                      <thead className="bg-gray-100 border-b border-gray-300">
                        <tr>
                          <th className="px-4 py-2 text-left font-medium">Cliente</th>
                          <th className="px-4 py-2 text-right font-medium">Total Devendo</th>
                          <th className="px-4 py-2 text-right font-medium">% do Total</th>
                        </tr>
                      </thead>
                      <tbody>
                        {concentracaoApi.data.topClientes.map((item) => (
                          <tr key={item.cliente.id} className="border-b hover:bg-gray-50">
                            <td className="px-4 py-3">{item.cliente.nome}</td>
                            <td className="px-4 py-3 text-right">
                              {formatCurrency(item.totalDevendo)}
                            </td>
                            <td className="px-4 py-3 text-right">
                              <span className="inline-block bg-blue-100 text-blue-700 px-3 py-1 rounded font-semibold">
                                {(item.percentualTotal * 100).toFixed(1)}%
                              </span>
                            </td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  </div>
                </div>
              </div>
            )}
          </>
        )}

        {activeTab === ReportTab.Intervalo && (
          <>
            <div className="space-y-2">
              <h2 className="text-xl font-semibold text-gray-900">
                Relatório por Intervalo
              </h2>
              <p className="text-gray-600 text-sm">
                Análise de performance em um período específico
              </p>
            </div>

            <div className="flex items-end gap-4 p-4 bg-gray-50 rounded flex-wrap">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Data início
                </label>
                <input
                  type="date"
                  value={dataInicio}
                  onChange={(e) => setDataInicio(e.target.value)}
                  className="input-field"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Data fim
                </label>
                <input
                  type="date"
                  value={dataFim}
                  onChange={(e) => setDataFim(e.target.value)}
                  className="input-field"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Dias de atraso
                </label>
                <input
                  type="number"
                  value={diasAtraso}
                  onChange={(e) => setDiasAtraso(parseInt(e.target.value))}
                  min="1"
                  className="input-field w-32"
                />
              </div>
              <button
                onClick={loadData}
                className="btn-primary"
              >
                Atualizar
              </button>
            </div>

            {intervaloApi.isLoading && (
              <LoadingSpinner message="Carregando relatório..." />
            )}

            {intervaloApi.error && (
              <ErrorAlert message={intervaloApi.error} />
            )}

            {intervaloApi.data && (
              <div className="space-y-6 mt-4">
                <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                  <div className="p-4 bg-green-50 rounded border border-green-200">
                    <p className="text-sm text-green-700">Total Recebido</p>
                    <p className="text-2xl font-bold text-green-900">
                      {formatCurrency(intervaloApi.data.totalRecebimentos)}
                    </p>
                  </div>
                  <div className="p-4 bg-red-50 rounded border border-red-200">
                    <p className="text-sm text-red-700">Total Devido</p>
                    <p className="text-2xl font-bold text-red-900">
                      {formatCurrency(intervaloApi.data.totalDevido)}
                    </p>
                  </div>
                  <div className="p-4 bg-blue-50 rounded border border-blue-200">
                    <p className="text-sm text-blue-700">Clientes Ativos</p>
                    <p className="text-2xl font-bold text-blue-900">
                      {intervaloApi.data.clientesAtivos}
                    </p>
                  </div>
                </div>

                {intervaloApi.data.inadimplentes && intervaloApi.data.inadimplentes.length > 0 && (
                  <div>
                    <h3 className="font-semibold text-gray-900 mb-3">
                      Clientes em Inadimplência
                    </h3>
                    <div className="overflow-x-auto">
                      <table className="w-full text-sm">
                        <thead className="bg-gray-100 border-b border-gray-300">
                          <tr>
                            <th className="px-4 py-2 text-left font-medium">Cliente</th>
                            <th className="px-4 py-2 text-right font-medium">Dias em Atraso</th>
                            <th className="px-4 py-2 text-right font-medium">Total Devido</th>
                          </tr>
                        </thead>
                        <tbody>
                          {intervaloApi.data.inadimplentes.map((item) => (
                            <tr key={item.cliente.id} className="border-b hover:bg-gray-50">
                              <td className="px-4 py-3">{item.cliente.nome}</td>
                              <td className="px-4 py-3 text-right">{item.diasAtraso} dias</td>
                              <td className="px-4 py-3 text-right font-semibold text-red-600">
                                {formatCurrency(item.totalDevendo)}
                              </td>
                            </tr>
                          ))}
                        </tbody>
                      </table>
                    </div>
                  </div>
                )}
              </div>
            )}
          </>
        )}
      </div>
    </div>
  )
}

export default RelatoriosPage
