import { useEffect, useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { useApi } from '../../hooks'
import { vendaService, clienteService } from '../../services/api'
import type { Cliente, Venda } from '../../types/domain'
import { ErrorAlert } from '../../components/common/ErrorAlert'
import { LoadingSpinner } from '../../components/common/LoadingSpinner'

export function NovaVenda() {
  const navigate = useNavigate()
  const { vendaId } = useParams<{ vendaId: string }>()
  const { isLoading: isSubmitting, execute: executeCreate } = useApi()
  const { isLoading: isLoadingClientes, execute: executeLoadClientes } = useApi<any>()
  const { isLoading: isLoadingVenda, execute: executeLoadVenda } = useApi<Venda>()

  const [formData, setFormData] = useState({
    clienteId: '',
    dataVenda: new Date().toISOString().split('T')[0],
    valorTotal: 0,
    tipoVenda: 'Fiado',
    descricao: '',
  })
  const [selectedCliente, setSelectedCliente] = useState<Cliente | null>(null)
  const [clientesList, setClientesList] = useState<Cliente[]>([])
  const [error, setError] = useState<string | null>(null)
  const [validationErrors, setValidationErrors] = useState<Record<string, string>>({})

  // Carregar clientes
  const handleLoadClientes = async () => {
    await executeLoadClientes(async () => {
      const result = await clienteService.listar({
        pageNumber: 1,
        pageSize: 100,
      })
      setClientesList(result.items)
      return result
    })
  }

  const loadVenda = async () => {
    if (!vendaId) return

    await executeLoadVenda(async () => {
      const venda = await vendaService.obter(vendaId)
      const cliente = await clienteService.obter(venda.clienteId)

      setFormData({
        clienteId: venda.clienteId,
        dataVenda: venda.dataVenda.split('T')[0],
        valorTotal: venda.valorTotal,
        tipoVenda: venda.tipoVenda,
        descricao: venda.descricao || '',
      })
      setSelectedCliente(cliente)

      return venda
    })
  }

  useEffect(() => {
    void handleLoadClientes()
  }, [])

  useEffect(() => {
    if (vendaId) {
      void loadVenda()
    }
  }, [vendaId])

  const handleClienteChange = async (e: React.ChangeEvent<HTMLSelectElement>) => {
    const clienteId = e.target.value
    setFormData((prev) => ({ ...prev, clienteId }))

    const cliente = clientesList.find((c) => c.id === clienteId)
    setSelectedCliente(cliente || null)

    if (clienteId) {
      try {
        const clienteDetalhado = await clienteService.obter(clienteId)
        setSelectedCliente(clienteDetalhado)
      } catch (err: any) {
        setError(err?.message || 'Erro ao carregar dados do cliente')
      }
    }

    // Limpar erro
    if (validationErrors.clienteId) {
      setValidationErrors((prev) => {
        const newErrors = { ...prev }
        delete newErrors.clienteId
        return newErrors
      })
    }
  }

  const validateForm = () => {
    const errors: Record<string, string> = {}

    if (!formData.clienteId) {
      errors.clienteId = 'Cliente é obrigatório'
    }

    if (formData.valorTotal <= 0) {
      errors.valorTotal = 'Valor deve ser maior que zero'
    }

    if (!formData.descricao.trim()) {
      errors.descricao = 'Descrição é obrigatória'
    }

    // Validar limite de crédito para vendas a fiado
    if (!vendaId && formData.tipoVenda === 'Fiado' && selectedCliente) {
      const saldoDevedor = selectedCliente.saldoDevedor || 0
      const novoSaldo = saldoDevedor + formData.valorTotal

      if (novoSaldo > selectedCliente.limiteCred) {
        errors.limit = `Limite de crédito excedido. Disponível: R$ ${(selectedCliente.limiteCred - saldoDevedor).toFixed(2)}`
      }
    }

    setValidationErrors(errors)
    return Object.keys(errors).length === 0
  }

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
    const { name, value } = e.target
    setFormData((prev) => ({
      ...prev,
      [name]: name === 'valorTotal' ? parseFloat(value) || 0 : value,
    }))
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setError(null)

    if (!validateForm()) {
      return
    }

    try {
      await executeCreate(async () => {
        if (vendaId) {
          await vendaService.atualizar(vendaId, {
            clienteId: formData.clienteId,
            dataVenda: formData.dataVenda,
            valorTotal: formData.valorTotal,
            tipoVenda: formData.tipoVenda as 'Dinheiro' | 'Fiado',
            descricao: formData.descricao,
          })
        } else {
          await vendaService.criar({
            clienteId: formData.clienteId,
            dataVenda: formData.dataVenda,
            valorTotal: formData.valorTotal,
            tipoVenda: formData.tipoVenda as 'Dinheiro' | 'Fiado',
            descricao: formData.descricao,
          })
        }
        navigate('/vendas')
        return null
      })
    } catch (err: any) {
      setError(err?.message || 'Erro ao registrar venda')
    }
  }

  if (vendaId && isLoadingVenda) {
    return <LoadingSpinner message="Carregando venda..." />
  }

  return (
    <div className="p-6 max-w-2xl">
      {/* Header */}
      <div className="mb-6">
        <h1 className="text-3xl font-bold text-gray-900">
          {vendaId ? 'Editar Venda' : 'Registrar Venda'}
        </h1>
        <p className="text-gray-600 mt-1">
          {vendaId ? 'Atualize os dados da venda' : 'Registre uma nova venda ou fiado'}
        </p>
      </div>

      {/* Alertas */}
      {error && <ErrorAlert message={error} onClose={() => setError(null)} />}
      {validationErrors.limit && <ErrorAlert message={validationErrors.limit} />}

      {/* Formulário */}
      <form onSubmit={handleSubmit} className="card space-y-4">
        {/* Tipo de Venda */}
        <div>
          <label htmlFor="tipoVenda" className="block text-sm font-medium text-gray-700 mb-1">
            Tipo de Venda *
          </label>
          <select
            id="tipoVenda"
            name="tipoVenda"
            value={formData.tipoVenda}
            onChange={handleChange}
            disabled={isSubmitting}
            className="input-field"
          >
            <option value="Dinheiro">À Vista</option>
            <option value="Fiado">Fiado</option>
          </select>
        </div>

        {/* Cliente */}
        <div>
          <label htmlFor="clienteId" className="block text-sm font-medium text-gray-700 mb-1">
            Cliente * {isLoadingClientes && <span className="text-xs text-gray-500 ml-2">Carregando...</span>}
          </label>
          <select
            id="clienteId"
            name="clienteId"
            value={formData.clienteId}
            onChange={handleClienteChange}
            disabled={isSubmitting || isLoadingClientes}
            className={`input-field ${validationErrors.clienteId ? 'border-red-500' : ''}`}
          >
            <option value="">-- Selecione um cliente --</option>
            {clientesList.map((cliente) => (
              <option key={cliente.id} value={cliente.id}>
                {cliente.nome}
              </option>
            ))}
          </select>
          {validationErrors.clienteId && (
            <p className="text-red-500 text-xs mt-1">{validationErrors.clienteId}</p>
          )}
        </div>

        {/* Mostrar informações do cliente selecionado */}
        {selectedCliente && (
          <div className="bg-blue-50 border border-blue-200 rounded-lg p-3">
            <p className="text-sm text-blue-900">
              <strong>Limite:</strong> R$ {selectedCliente.limiteCred.toFixed(2)} |
              <strong className="ml-2">Deve:</strong> R$ {(selectedCliente.saldoDevedor || 0).toFixed(2)} |
              <strong className="ml-2">Disponível:</strong> R$ {(selectedCliente.limiteCred - (selectedCliente.saldoDevedor || 0)).toFixed(2)}
            </p>
          </div>
        )}

        {/* Data */}
        <div>
          <label htmlFor="dataVenda" className="block text-sm font-medium text-gray-700 mb-1">
            Data *
          </label>
          <input
            type="date"
            id="dataVenda"
            name="dataVenda"
            value={formData.dataVenda}
            onChange={handleChange}
            disabled={isSubmitting}
            className="input-field"
          />
        </div>

        {/* Valor */}
        <div>
          <label htmlFor="valorTotal" className="block text-sm font-medium text-gray-700 mb-1">
            Valor (R$) *
          </label>
          <input
            type="number"
            id="valorTotal"
            name="valorTotal"
            value={formData.valorTotal}
            onChange={handleChange}
            disabled={isSubmitting}
            className={`input-field ${validationErrors.valorTotal ? 'border-red-500' : ''}`}
            placeholder="0.00"
            step="0.01"
            min="0"
          />
          {validationErrors.valorTotal && (
            <p className="text-red-500 text-xs mt-1">{validationErrors.valorTotal}</p>
          )}
        </div>

        {/* Descrição */}
        <div>
          <label htmlFor="descricao" className="block text-sm font-medium text-gray-700 mb-1">
            Descrição *
          </label>
          <textarea
            id="descricao"
            name="descricao"
            value={formData.descricao}
            onChange={handleChange}
            disabled={isSubmitting}
            className={`input-field ${validationErrors.descricao ? 'border-red-500' : ''}`}
            placeholder="Ex: Blusa vermelha, calça azul..."
            rows={3}
          />
          {validationErrors.descricao && (
            <p className="text-red-500 text-xs mt-1">{validationErrors.descricao}</p>
          )}
        </div>

        {/* Botões */}
        <div className="flex gap-2 pt-4">
          <button
            type="submit"
            disabled={isSubmitting}
            className="btn-primary flex-1"
          >
            {isSubmitting ? (vendaId ? 'Atualizando...' : 'Registrando...') : vendaId ? 'Atualizar Venda' : 'Registrar Venda'}
          </button>
          <button
            type="button"
            onClick={() => navigate('/vendas')}
            disabled={isSubmitting}
            className="btn-secondary flex-1"
          >
            Cancelar
          </button>
        </div>
      </form>
    </div>
  )
}

export default NovaVenda
