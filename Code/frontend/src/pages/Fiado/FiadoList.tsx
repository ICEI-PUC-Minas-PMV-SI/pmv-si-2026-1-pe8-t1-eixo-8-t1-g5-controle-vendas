import { useEffect, useState } from 'react'
import { useApi, usePagination } from '../../hooks'
import { Modal } from '../../components/common/Modal'
import { ErrorAlert } from '../../components/common/ErrorAlert'
import { LoadingSpinner } from '../../components/common/LoadingSpinner'
import { PagamentoForm, type PagamentoFormValues } from '../../components/forms/PagamentoForm'
import { pagamentoService, vendaService } from '../../services/api'
import type { Venda } from '../../types/domain'
import type { PaginatedResponse } from '../../types/api-response'

function getStatusClasses(status: Venda['statusPagamento']) {
  switch (status) {
    case 'Pendente':
      return 'bg-red-50 text-red-700'
    case 'PartialmentePago':
      return 'bg-yellow-50 text-yellow-700'
    default:
      return 'bg-green-50 text-green-700'
  }
}

export function FiadoList() {
  const { currentPage, goToNextPage, goToPreviousPage } = usePagination()
  const vendasApi = useApi<PaginatedResponse<Venda>>()
  const vendaDetailApi = useApi<Venda>()
  const [isModalOpen, setIsModalOpen] = useState(false)

  useEffect(() => {
    void vendasApi.execute(() => vendaService.listarEmAberto(currentPage, 10))
  }, [currentPage])

  const openPagamentoModal = async (vendaId: string) => {
    const detail = await vendaDetailApi.execute(() => vendaService.obter(vendaId))

    if (detail) {
      setIsModalOpen(true)
    }
  }

  const closePagamentoModal = () => {
    setIsModalOpen(false)
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
    await vendasApi.execute(() => vendaService.listarEmAberto(currentPage, 10))
  }

  return (
    <div className="space-y-6 p-6">
      <div>
        <h1 className="text-3xl font-bold text-gray-900">Fiado</h1>
        <p className="mt-1 text-gray-600">
          Acompanhe vendas em aberto e registre pagamentos parciais ou totais.
        </p>
      </div>

      {vendasApi.error && <ErrorAlert message={vendasApi.error} />}
      {vendaDetailApi.error && <ErrorAlert message={vendaDetailApi.error} />}
      {vendasApi.isLoading && <LoadingSpinner message="Carregando vendas em aberto..." />}

      {!vendasApi.isLoading && vendasApi.data && vendasApi.data.items.length === 0 && (
        <div className="card py-10 text-center text-gray-600">
          Nenhuma venda em aberto encontrada.
        </div>
      )}

      {!vendasApi.isLoading && vendasApi.data && vendasApi.data.items.length > 0 && (
        <>
          <div className="overflow-x-auto rounded-lg border border-gray-200 bg-white shadow-sm">
            <table className="w-full">
              <thead className="border-b border-gray-200 bg-gray-50">
                <tr>
                  <th className="px-4 py-3 text-left text-xs font-semibold uppercase text-gray-700">Cliente</th>
                  <th className="px-4 py-3 text-left text-xs font-semibold uppercase text-gray-700">Data</th>
                  <th className="px-4 py-3 text-right text-xs font-semibold uppercase text-gray-700">Valor</th>
                  <th className="px-4 py-3 text-right text-xs font-semibold uppercase text-gray-700">Saldo</th>
                  <th className="px-4 py-3 text-left text-xs font-semibold uppercase text-gray-700">Status</th>
                  <th className="px-4 py-3 text-right text-xs font-semibold uppercase text-gray-700">Ação</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-200">
                {vendasApi.data.items.map((venda) => (
                  <tr key={venda.id}>
                    <td className="px-4 py-3 text-sm font-medium text-gray-900">
                      {venda.cliente?.nome || venda.clienteNome || 'Cliente não identificado'}
                    </td>
                    <td className="px-4 py-3 text-sm text-gray-700">
                      {new Date(venda.dataVenda).toLocaleDateString('pt-BR')}
                    </td>
                    <td className="px-4 py-3 text-right text-sm text-gray-900">
                      R$ {venda.valorTotal.toFixed(2)}
                    </td>
                    <td className="px-4 py-3 text-right text-sm font-semibold text-red-600">
                      R$ {(venda.saldoDevedor ?? venda.valorTotal).toFixed(2)}
                    </td>
                    <td className="px-4 py-3 text-sm">
                      <span
                        className={`inline-flex rounded-full px-2 py-1 text-xs font-semibold ${getStatusClasses(
                          venda.statusPagamento
                        )}`}
                      >
                        {venda.statusPagamento === 'PartialmentePago'
                          ? 'Parcialmente pago'
                          : venda.statusPagamento}
                      </span>
                    </td>
                    <td className="px-4 py-3 text-right">
                      <button
                        onClick={() => openPagamentoModal(venda.id)}
                        className="btn-primary px-3 py-2 text-sm"
                      >
                        Registrar pagamento
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          {(currentPage > 1 || vendasApi.data.hasNextPage) && (
            <div className="flex items-center justify-center gap-2">
              <button
                onClick={goToPreviousPage}
                disabled={currentPage === 1}
                className="btn-secondary px-3 py-1"
              >
                ← Anterior
              </button>
              <span className="text-sm text-gray-600">Página {currentPage}</span>
              <button
                onClick={goToNextPage}
                disabled={!vendasApi.data.hasNextPage}
                className="btn-secondary px-3 py-1"
              >
                Próximo →
              </button>
            </div>
          )}
        </>
      )}

      <Modal isOpen={isModalOpen} onClose={closePagamentoModal} title="Registrar pagamento" size="md">
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

export default FiadoList
