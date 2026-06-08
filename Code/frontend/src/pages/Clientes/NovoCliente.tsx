import { useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { useApi } from '../../hooks'
import { clienteService } from '../../services/api'
import type { Cliente } from '../../types/domain'
import { ErrorAlert } from '../../components/common/ErrorAlert'
import { LoadingSpinner } from '../../components/common/LoadingSpinner'
import { useEffect } from 'react'

export function NovoCliente() {
  const navigate = useNavigate()
  const { clienteId } = useParams()
  const { isLoading: isSubmitting, execute: executeCreate } = useApi()
  const { isLoading: isLoadingCliente, execute: executeLoadCliente } = useApi<Cliente>()

  const [formData, setFormData] = useState({
    nome: '',
    email: '',
    telefone: '',
    endereco: '',
    limiteCred: 0,
  })
  const [error, setError] = useState<string | null>(null)
  const [validationErrors, setValidationErrors] = useState<Record<string, string>>({})
  const [cliente, setCliente] = useState<Cliente | null>(null)

  // Carregar cliente se estiver editando
  useEffect(() => {
    if (clienteId) {
      loadCliente()
    }
  }, [clienteId])

  const loadCliente = async () => {
    if (!clienteId) return
    await executeLoadCliente(async () => {
      const result = await clienteService.obter(clienteId)
      setCliente(result)
      setFormData({
        nome: result.nome,
        email: result.email || '',
        telefone: result.telefone || '',
        endereco: result.endereco || '',
        limiteCred: result.limiteCred,
      })
      return result
    })
  }

  const validateForm = () => {
    const errors: Record<string, string> = {}

    if (!formData.nome.trim()) {
      errors.nome = 'Nome é obrigatório'
    }

    if (!formData.telefone.trim()) {
      errors.telefone = 'Telefone é obrigatório'
    }

    if (formData.email && !formData.email.includes('@')) {
      errors.email = 'Email inválido'
    }

    if (formData.limiteCred < 0) {
      errors.limiteCred = 'Limite de crédito não pode ser negativo'
    }

    setValidationErrors(errors)
    return Object.keys(errors).length === 0
  }

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    const { name, value } = e.target
    setFormData((prev) => ({
      ...prev,
      [name]: name === 'limiteCred' ? parseFloat(value) || 0 : value,
    }))

    if (validationErrors[name]) {
      setValidationErrors((prev) => {
        const newErrors = { ...prev }
        delete newErrors[name]
        return newErrors
      })
    }
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setError(null)

    if (!validateForm()) {
      return
    }

    try {
      await executeCreate(async () => {
        if (clienteId && cliente) {
          await clienteService.atualizar(clienteId, formData)
        } else {
          await clienteService.criar(formData)
        }
        navigate('/clientes')
        return null
      })
    } catch (err: any) {
      setError(err?.message || 'Erro ao salvar cliente')
    }
  }

  if (clienteId && isLoadingCliente) {
    return <LoadingSpinner message="Carregando cliente..." />
  }

  return (
    <div className="p-6 max-w-2xl">
      {/* Header */}
      <div className="mb-6">
        <h1 className="text-3xl font-bold text-gray-900">
          {clienteId ? 'Editar Cliente' : 'Novo Cliente'}
        </h1>
        <p className="text-gray-600 mt-1">
          {clienteId ? 'Atualize as informações do cliente' : 'Cadastre um novo cliente'}
        </p>
      </div>

      {/* Alertas */}
      {error && <ErrorAlert message={error} onClose={() => setError(null)} />}

      {/* Formulário */}
      <form onSubmit={handleSubmit} className="card space-y-4">
        {/* Nome */}
        <div>
          <label htmlFor="nome" className="block text-sm font-medium text-gray-700 mb-1">
            Nome *
          </label>
          <input
            type="text"
            id="nome"
            name="nome"
            value={formData.nome}
            onChange={handleChange}
            disabled={isSubmitting}
            className={`input-field ${validationErrors.nome ? 'border-red-500' : ''}`}
            placeholder="Nome completo"
          />
          {validationErrors.nome && (
            <p className="text-red-500 text-xs mt-1">{validationErrors.nome}</p>
          )}
        </div>

        {/* Email */}
        <div>
          <label htmlFor="email" className="block text-sm font-medium text-gray-700 mb-1">
            Email
          </label>
          <input
            type="email"
            id="email"
            name="email"
            value={formData.email}
            onChange={handleChange}
            disabled={isSubmitting}
            className={`input-field ${validationErrors.email ? 'border-red-500' : ''}`}
            placeholder="email@example.com"
          />
          {validationErrors.email && (
            <p className="text-red-500 text-xs mt-1">{validationErrors.email}</p>
          )}
        </div>

        {/* Telefone */}
        <div>
          <label htmlFor="telefone" className="block text-sm font-medium text-gray-700 mb-1">
            Telefone *
          </label>
          <input
            type="tel"
            id="telefone"
            name="telefone"
            value={formData.telefone}
            onChange={handleChange}
            disabled={isSubmitting}
            className={`input-field ${validationErrors.telefone ? 'border-red-500' : ''}`}
            placeholder="(31) 99999-9999"
          />
          {validationErrors.telefone && (
            <p className="text-red-500 text-xs mt-1">{validationErrors.telefone}</p>
          )}
        </div>

        {/* Endereço */}
        <div>
          <label htmlFor="endereco" className="block text-sm font-medium text-gray-700 mb-1">
            Endereço
          </label>
          <textarea
            id="endereco"
            name="endereco"
            value={formData.endereco}
            onChange={handleChange}
            disabled={isSubmitting}
            className="input-field"
            placeholder="Rua, número, bairro, cidade"
            rows={2}
          />
        </div>

        {/* Limite de Crédito */}
        <div>
          <label htmlFor="limiteCred" className="block text-sm font-medium text-gray-700 mb-1">
            Limite de Crédito (R$)
          </label>
          <input
            type="number"
            id="limiteCred"
            name="limiteCred"
            value={formData.limiteCred}
            onChange={handleChange}
            disabled={isSubmitting}
            className={`input-field ${validationErrors.limiteCred ? 'border-red-500' : ''}`}
            placeholder="0.00"
            step="0.01"
            min="0"
          />
          {validationErrors.limiteCred && (
            <p className="text-red-500 text-xs mt-1">{validationErrors.limiteCred}</p>
          )}
        </div>

        {/* Botões */}
        <div className="flex gap-2 pt-4">
          <button
            type="submit"
            disabled={isSubmitting}
            className="btn-primary flex-1"
          >
            {isSubmitting ? 'Salvando...' : clienteId ? 'Atualizar' : 'Criar'}
          </button>
          <button
            type="button"
            onClick={() => navigate('/clientes')}
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

export default NovoCliente
