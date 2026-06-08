import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useApi, usePagination } from '../../hooks'
import { pagamentoService, vendaService } from '../../services/api'
import type { Venda } from '../../types/domain'
import type { PaginatedResponse } from '../../types/api-response'
import { LoadingSpinner } from '../../components/common/LoadingSpinner'
import { ErrorAlert } from '../../components/common/ErrorAlert'
import { Modal } from '../../components/common/Modal'
import { PagamentoForm, type PagamentoFormValues } from '../../components/forms/PagamentoForm'

export function VendasList() {
  const navigate = useNavigate()
  const { data, isLoading, error, execute } = useApi<PaginatedResponse<Venda>>()
  const vendaDetailApi = useApi<Venda>()
  const { currentPage, goToNextPage, goToPreviousPage, resetPagination } = usePagination()
  const [tipoFiltro, setTipoFiltro] = useState<'Dinheiro' | 'Fiado' | ''>('')
  const [statusFiltro, setStatusFiltro] = useState<string>('')
  const [isPagamentoModalOpen, setIsPagamentoModalOpen] = useState(false)

  useEffect(() => {
    void loadVendas(currentPage, tipoFiltro, statusFiltro)
  }, [currentPage, tipoFiltro, statusFiltro])

  const loadVendas = async (page: number, tipo: string, status: string) => {
    await execute(() =>
      vendaService.listar({
        pageNumber: page,
        pageSize: 10,
        tipoVenda: (tipo as 'Dinheiro' | 'Fiado' | '') || undefined,
        statusPagamento: (status as Venda['statusPagamento']) || undefined,
      })
    )
  }

  const openPagamentoModal = async (vendaId: string) => {
    const venda = await vendaDetailApi.execute(() => vendaService.obter(vendaId))

    if (venda) {
      setIsPagamentoModalOpen(true)
    }
  }

  const closePagamentoModal = () => {
    setIsPagamentoModalOpen(false)
    vendaDetailApi.setData(null)
    vendaDetailApi.setError(null)
  }

  const handleRegistrarPagamento = async (values: PagamentoFormValues) => {
    if (!vendaDetailApi.data) {
      throw new Error('Nenhuma venda selecionada para pagamento')
    }

    await pagamentoService.criar({
      vendaId: vendaDetailApi.data.id,
      clienteId: vendaDetailApi.data.clienteId,
      valorPago: values.valorPago,
      dataPagamento: values.dataPagamento,
      metodoPagamento: values.metodoPagamento,
      observacoes: values.observacoes,
    })

    closePagamentoModal()
    await loadVendas(currentPage, tipoFiltro, statusFiltro)
  }

  const getStatusColor = (status: Venda['statusPagamento']) => {
    switch (status) {
      case 'Pago':
        return 'text-green-600 bg-green-50'
      case 'Pendente':
        return 'text-red-600 bg-red-50'
      case 'PartialmentePago':
        return 'text-yellow-600 bg-yellow-50'
      default:
        return 'text-gray-600 bg-gray-50'
    }
  }

  return (
    <div className="space-y-6 p-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Vendas</h1>
          <p className="mt-1 text-gray-600">Histórico de todas as vendas registradas</p>
        </div>
        <button
          onClick={() => navigate('/vendas/nova')}
          className="btn-primary px-6 py-3"
        >
          + Nova Venda
        </button>
      </div>

      <div className="flex flex-wrap gap-2">
        <select
          value={tipoFiltro}
          onChange={(event) => {
            setTipoFiltro(event.target.value as 'Dinheiro' | 'Fiado' | '')
            resetPagination()
          }}
          className="input-field max-w-xs"
        >
          <option value="">Tipo: Todos</option>
          <option value="Dinheiro">À Vista</option>
          <option value="Fiado">Fiado</option>
        </select>

        <select
          value={statusFiltro}
          onChange={(event) => {
            setStatusFiltro(event.target.value)
            resetPagination()
          }}
          className="input-field max-w-xs"
        >
          <option value="">Status: Todos</option>
          <option value="Pendente">Pendente</option>
          <option value="PartialmentePago">Parcialmente Pago</option>
          <option value="Pago">Pago</option>
        </select>
      </div>

      {error && <ErrorAlert message={error} />}
      {vendaDetailApi.error && <ErrorAlert message={vendaDetailApi.error} />}
      {isLoading && <LoadingSpinner message="Carregando vendas..." />}

      {!isLoading && data && (
        <>
          {data.items.length === 0 ? (
            <div className="py-12 text-center">
              <p className="text-gray-600">Nenhuma venda encontrada</p>
            </div>
          ) : (
            <div className="overflow-x-auto">
              <table className="w-full">
                <thead className="border-b border-gray-200 bg-gray-50">
                  <tr>
                    <th className="px-6 py-3 text-left text-xs font-medium uppercase text-gray-700">Data</th>
                    <th className="px-6 py-3 text-left text-xs font-medium uppercase text-gray-700">Cliente</th>
                    <th className="px-6 py-3 text-left text-xs font-medium uppercase text-gray-700">Tipo</th>
                    <th className="px-6 py-3 text-right text-xs font-medium uppercase text-gray-700">Valor</th>
                    <th className="px-6 py-3 text-left text-xs font-medium uppercase text-gray-700">Status</th>
                    <th className="px-6 py-3 text-left text-xs font-medium uppercase text-gray-700">Risco</th>
                    <th className="px-6 py-3 text-right text-xs font-medium uppercase text-gray-700">Ações</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-gray-200">
                  {data.items.map((venda) => (
                    <tr key={venda.id} className="hover:bg-gray-50 transition">
                      <td className="px-6 py-3 text-sm text-gray-900">
                        {new Date(venda.dataVenda).toLocaleDateString('pt-BR')}
                      </td>
                      <td className="px-6 py-3 text-sm text-gray-900">
                        {venda.cliente?.nome || venda.clienteNome || 'N/A'}
                      </td>
                      <td className="px-6 py-3 text-sm">
                        <span className="inline-block rounded px-2 py-1 text-xs font-semibold bg-blue-50 text-blue-700">
                          {venda.tipoVenda}
                        </span>
                      </td>
                      <td className="px-6 py-3 text-right text-sm font-semibold text-gray-900">
                        R$ {venda.valorTotal.toFixed(2)}
                      </td>
                      <td className="px-6 py-3 text-sm">
                        <span
                          className={`inline-block rounded px-2 py-1 text-xs font-semibold ${getStatusColor(
                            venda.statusPagamento
                          )}`}
                        >
                          {venda.statusPagamento === 'PartialmentePago'
                            ? 'Parcial'
                            : venda.statusPagamento}
                        </span>
                      </td>
                      <td className="px-6 py-3 text-sm">
                        <span
                          className={`inline-block rounded px-2 py-1 text-xs font-semibold ${
                            venda.percentualRisco > 50
                              ? 'bg-red-50 text-red-700'
                              : venda.percentualRisco > 25
                                ? 'bg-yellow-50 text-yellow-700'
                                : 'bg-green-50 text-green-700'
                          }`}
                        >
                          {venda.percentualRisco.toFixed(0)}%
                        </span>
                      </td>
                      <td className="px-6 py-3 text-right text-sm">
                        <div className="flex justify-end gap-2">
                          <button
                            onClick={() => navigate(`/vendas/${venda.id}/editar`)}
                            className="btn-secondary px-3 py-1 text-xs"
                          >
                            Editar
                          </button>
                          {venda.tipoVenda === 'Fiado' &&
                            venda.statusPagamento !== 'Pago' &&
                            venda.statusPagamento !== 'Cancelado' && (
                              <button
                                onClick={() => openPagamentoModal(venda.id)}
                                className="btn-primary px-3 py-1 text-xs"
                              >
                                Pagamento
                              </button>
                            )}
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}

          {(currentPage > 1 || data.hasNextPage) && (
            <div className="flex items-center justify-center gap-2 pt-4">
              <button
                onClick={goToPreviousPage}
                disabled={currentPage === 1}
                className="btn-secondary px-3 py-1"
              >
                ← Anterior
              </button>
              <span className="text-sm text-gray-600">Página {data.pageNumber}</span>
              <button
                onClick={goToNextPage}
                disabled={!data.hasNextPage}
                className="btn-secondary px-3 py-1"
              >
                Próximo →
              </button>
            </div>
          )}
        </>
      )}

      <Modal
        isOpen={isPagamentoModalOpen}
        onClose={closePagamentoModal}
        title="Registrar pagamento"
        size="md"
      >
        {vendaDetailApi.isLoading && <LoadingSpinner size="sm" message="Carregando venda..." />}
        {!vendaDetailApi.isLoading && vendaDetailApi.data && (
          <PagamentoForm
            venda={vendaDetailApi.data}
            onSubmit={handleRegistrarPagamento}
            onCancel={closePagamentoModal}
          />
        )}
      </Modal>
    </div>
  )
}

export default VendasList
