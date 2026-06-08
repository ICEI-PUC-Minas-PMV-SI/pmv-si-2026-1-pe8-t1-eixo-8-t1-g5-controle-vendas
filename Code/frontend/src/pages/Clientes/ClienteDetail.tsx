import { useEffect } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import { useApi } from '../../hooks'
import { ErrorAlert } from '../../components/common/ErrorAlert'
import { LoadingSpinner } from '../../components/common/LoadingSpinner'
import { clienteService, vendaService } from '../../services/api'
import type { Cliente, Venda } from '../../types/domain'
import type { PaginatedResponse } from '../../types/api-response'

function getStatusClasses(status: Cliente['statusInadimplencia'] | undefined) {
  switch (status) {
    case 'Adimplente':
      return 'bg-green-50 text-green-700 border-green-200'
    case 'Atraso':
      return 'bg-yellow-50 text-yellow-700 border-yellow-200'
    case 'Inadimplente':
      return 'bg-red-50 text-red-700 border-red-200'
    default:
      return 'bg-gray-50 text-gray-700 border-gray-200'
  }
}

function getVendaStatusClasses(status: Venda['statusPagamento']) {
  switch (status) {
    case 'Pago':
      return 'bg-green-50 text-green-700'
    case 'Pendente':
      return 'bg-red-50 text-red-700'
    case 'PartialmentePago':
      return 'bg-yellow-50 text-yellow-700'
    default:
      return 'bg-gray-50 text-gray-700'
  }
}

export function ClienteDetail() {
  const navigate = useNavigate()
  const { clienteId } = useParams<{ clienteId: string }>()
  const clienteApi = useApi<Cliente>()
  const vendasApi = useApi<PaginatedResponse<Venda>>()

  useEffect(() => {
    if (!clienteId) {
      navigate('/clientes', { replace: true })
      return
    }

    void clienteApi.execute(() => clienteService.obter(clienteId))
    void vendasApi.execute(() =>
      vendaService.listar({
        clienteId,
        pageNumber: 1,
        pageSize: 50,
      })
    )
  }, [clienteId])

  if (clienteApi.isLoading && !clienteApi.data) {
    return <LoadingSpinner message="Carregando cliente..." />
  }

  return (
    <div className="space-y-6 p-6">
      <div className="flex flex-wrap items-start justify-between gap-3">
        <div>
          <Link to="/clientes" className="text-sm font-medium text-blue-600 hover:underline">
            ← Voltar para clientes
          </Link>
          <h1 className="mt-2 text-3xl font-bold text-gray-900">
            {clienteApi.data?.nome || 'Detalhes do cliente'}
          </h1>
          <p className="mt-1 text-gray-600">
            Histórico de compras, saldo em aberto e situação de crédito.
          </p>
        </div>
      </div>

      {clienteApi.error && <ErrorAlert message={clienteApi.error} />}
      {vendasApi.error && <ErrorAlert message={vendasApi.error} />}

      {clienteApi.data && (
        <>
          <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
            <div className="card">
              <p className="text-sm text-gray-600">Limite de crédito</p>
              <p className="mt-2 text-2xl font-bold text-gray-900">
                R$ {clienteApi.data.limiteCred.toFixed(2)}
              </p>
            </div>
            <div className="card">
              <p className="text-sm text-gray-600">Saldo devedor</p>
              <p className="mt-2 text-2xl font-bold text-red-600">
                R$ {(clienteApi.data.saldoDevedor || 0).toFixed(2)}
              </p>
            </div>
            <div className="card">
              <p className="text-sm text-gray-600">Status</p>
              <span
                className={`mt-2 inline-flex rounded-full border px-3 py-1 text-sm font-semibold ${getStatusClasses(
                  clienteApi.data.statusInadimplencia
                )}`}
              >
                {clienteApi.data.statusInadimplencia}
              </span>
            </div>
            <div className="card">
              <p className="text-sm text-gray-600">Média de atraso</p>
              <p className="mt-2 text-2xl font-bold text-gray-900">
                {clienteApi.data.diasMediaAtraso || 0} dias
              </p>
            </div>
          </div>

          <div className="grid gap-4 md:grid-cols-2">
            <div className="card">
              <h2 className="text-lg font-semibold text-gray-900">Contato</h2>
              <div className="mt-4 space-y-2 text-sm text-gray-700">
                <p>Email: {clienteApi.data.email || 'Não informado'}</p>
                <p>Telefone: {clienteApi.data.telefone || 'Não informado'}</p>
                <p>Endereço: {clienteApi.data.endereco || 'Não informado'}</p>
              </div>
            </div>
            <div className="card">
              <h2 className="text-lg font-semibold text-gray-900">Resumo</h2>
              <div className="mt-4 space-y-2 text-sm text-gray-700">
                <p>Cliente desde: {new Date(clienteApi.data.criadoEm).toLocaleDateString('pt-BR')}</p>
                <p>
                  Última atualização:{' '}
                  {new Date(clienteApi.data.atualizadoEm).toLocaleDateString('pt-BR')}
                </p>
                <p>
                  Crédito disponível:{' '}
                  R$ {(clienteApi.data.limiteCred - (clienteApi.data.saldoDevedor || 0)).toFixed(2)}
                </p>
              </div>
            </div>
          </div>
        </>
      )}

      <div className="card">
        <div>
          <h2 className="text-lg font-semibold text-gray-900">Histórico de vendas</h2>
          <p className="text-sm text-gray-600">Últimas movimentações desse cliente.</p>
        </div>

        {vendasApi.isLoading && <LoadingSpinner size="sm" message="Carregando vendas..." />}

        {!vendasApi.isLoading && vendasApi.data && vendasApi.data.items.length === 0 && (
          <div className="py-8 text-center text-sm text-gray-600">
            Nenhuma venda encontrada para este cliente.
          </div>
        )}

        {!vendasApi.isLoading && vendasApi.data && vendasApi.data.items.length > 0 && (
          <div className="mt-4 overflow-x-auto">
            <table className="w-full">
              <thead className="border-b border-gray-200 bg-gray-50">
                <tr>
                  <th className="px-4 py-3 text-left text-xs font-semibold uppercase text-gray-700">Data</th>
                  <th className="px-4 py-3 text-left text-xs font-semibold uppercase text-gray-700">Tipo</th>
                  <th className="px-4 py-3 text-left text-xs font-semibold uppercase text-gray-700">Status</th>
                  <th className="px-4 py-3 text-right text-xs font-semibold uppercase text-gray-700">Valor</th>
                  <th className="px-4 py-3 text-right text-xs font-semibold uppercase text-gray-700">Risco</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-200">
                {vendasApi.data.items.map((venda) => (
                  <tr key={venda.id}>
                    <td className="px-4 py-3 text-sm text-gray-900">
                      {new Date(venda.dataVenda).toLocaleDateString('pt-BR')}
                    </td>
                    <td className="px-4 py-3 text-sm text-gray-700">{venda.tipoVenda}</td>
                    <td className="px-4 py-3 text-sm">
                      <span
                        className={`inline-flex rounded-full px-2 py-1 text-xs font-semibold ${getVendaStatusClasses(
                          venda.statusPagamento
                        )}`}
                      >
                        {venda.statusPagamento === 'PartialmentePago'
                          ? 'Parcialmente pago'
                          : venda.statusPagamento}
                      </span>
                    </td>
                    <td className="px-4 py-3 text-right text-sm font-semibold text-gray-900">
                      R$ {venda.valorTotal.toFixed(2)}
                    </td>
                    <td className="px-4 py-3 text-right text-sm text-gray-700">
                      {venda.percentualRisco.toFixed(0)}%
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  )
}

export default ClienteDetail
