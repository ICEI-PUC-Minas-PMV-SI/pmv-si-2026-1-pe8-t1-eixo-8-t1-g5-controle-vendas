import { useState, useEffect } from 'react'
import type { Cliente } from '../../types/domain'
import { ErrorAlert } from '../common/ErrorAlert'

interface ClienteFormProps {
  cliente?: Cliente
  onSubmit: (data: any) => Promise<void>
  onCancel: () => void
  isLoading?: boolean
}

export function ClienteForm({
  cliente,
  onSubmit,
  onCancel,
  isLoading = false,
}: ClienteFormProps) {
  const [formData, setFormData] = useState({
    nome: '',
    email: '',
    telefone: '',
    endereco: '',
    limiteCred: 0,
  })
  const [error, setError] = useState<string | null>(null)
  const [validationErrors, setValidationErrors] = useState<Record<string, string>>({})

  useEffect(() => {
    if (cliente) {
      setFormData({
        nome: cliente.nome,
        email: cliente.email || '',
        telefone: cliente.telefone || '',
        endereco: cliente.endereco || '',
        limiteCred: cliente.limiteCred,
      })
    }
  }, [cliente])

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
    // Limpar erro de validação do campo
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
      await onSubmit(formData)
    } catch (err: any) {
      setError(err?.message || 'Erro ao salvar cliente')
    }
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      {error && <ErrorAlert message={error} onClose={() => setError(null)} />}

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
          disabled={isLoading}
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
          disabled={isLoading}
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
          disabled={isLoading}
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
          disabled={isLoading}
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
          disabled={isLoading}
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
          disabled={isLoading}
          className="btn-primary flex-1"
        >
          {isLoading ? 'Salvando...' : cliente ? 'Atualizar' : 'Criar'}
        </button>
        <button
          type="button"
          onClick={onCancel}
          disabled={isLoading}
          className="btn-secondary flex-1"
        >
          Cancelar
        </button>
      </div>
    </form>
  )
}

export default ClienteForm
