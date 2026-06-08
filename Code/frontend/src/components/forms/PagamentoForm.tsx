import { type FormEvent, useMemo, useState } from 'react'
import { useForm } from '../../hooks'
import type { Pagamento, Venda } from '../../types/domain'
import { ErrorAlert } from '../common/ErrorAlert'

export interface PagamentoFormValues {
  valorPago: number
  dataPagamento: string
  metodoPagamento: Pagamento['metodoPagamento']
  observacoes: string
}

interface PagamentoFormProps {
  venda: Venda
  onSubmit: (values: PagamentoFormValues) => Promise<void>
  onCancel: () => void
}

function getDefaultDate(): string {
  return new Date().toISOString().split('T')[0]
}

export function PagamentoForm({ venda, onSubmit, onCancel }: PagamentoFormProps) {
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [submitError, setSubmitError] = useState<string | null>(null)

  const saldoDevedor = useMemo(() => {
    if (typeof venda.saldoDevedor === 'number') {
      return venda.saldoDevedor
    }

    const totalPago = venda.pagamentos?.reduce((sum, pagamento) => sum + pagamento.valorPago, 0) ?? 0
    return Math.max(venda.valorTotal - totalPago, 0)
  }, [venda.pagamentos, venda.saldoDevedor, venda.valorTotal])

  const { values, errors, setFieldValue, validateForm } = useForm<PagamentoFormValues>(
    {
      valorPago: saldoDevedor,
      dataPagamento: getDefaultDate(),
      metodoPagamento: 'Dinheiro',
      observacoes: '',
    },
    (currentValues) => {
      const nextErrors: Partial<Record<keyof PagamentoFormValues, string>> = {}

      if (currentValues.valorPago <= 0) {
        nextErrors.valorPago = 'Informe um valor maior que zero'
      } else if (currentValues.valorPago > saldoDevedor) {
        nextErrors.valorPago = `O valor não pode ultrapassar R$ ${saldoDevedor.toFixed(2)}`
      }

      if (!currentValues.dataPagamento) {
        nextErrors.dataPagamento = 'Informe a data do pagamento'
      }

      return nextErrors
    }
  )

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    setSubmitError(null)

    if (!validateForm()) {
      return
    }

    try {
      setIsSubmitting(true)
      await onSubmit(values)
    } catch (error) {
      setSubmitError(error instanceof Error ? error.message : 'Erro ao registrar pagamento')
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <div className="rounded-lg border border-blue-200 bg-blue-50 p-4 text-sm text-blue-900">
        <p className="font-semibold">{venda.cliente?.nome || venda.clienteNome || 'Cliente'}</p>
        <p className="mt-1">
          Saldo em aberto: <strong>R$ {saldoDevedor.toFixed(2)}</strong>
        </p>
      </div>

      {submitError && <ErrorAlert message={submitError} onClose={() => setSubmitError(null)} />}

      <div>
        <label htmlFor="valorPago" className="mb-1 block text-sm font-medium text-gray-700">
          Valor pago
        </label>
        <input
          id="valorPago"
          type="number"
          min="0"
          step="0.01"
          value={values.valorPago}
          onChange={(event) => setFieldValue('valorPago', Number(event.target.value))}
          className={`input-field ${errors.valorPago ? 'border-red-500' : ''}`}
          disabled={isSubmitting}
        />
        {errors.valorPago && <p className="mt-1 text-xs text-red-500">{errors.valorPago}</p>}
      </div>

      <div>
        <label htmlFor="dataPagamento" className="mb-1 block text-sm font-medium text-gray-700">
          Data do pagamento
        </label>
        <input
          id="dataPagamento"
          type="date"
          value={values.dataPagamento}
          onChange={(event) => setFieldValue('dataPagamento', event.target.value)}
          className={`input-field ${errors.dataPagamento ? 'border-red-500' : ''}`}
          disabled={isSubmitting}
        />
        {errors.dataPagamento && (
          <p className="mt-1 text-xs text-red-500">{errors.dataPagamento}</p>
        )}
      </div>

      <div>
        <label htmlFor="metodoPagamento" className="mb-1 block text-sm font-medium text-gray-700">
          Método
        </label>
        <select
          id="metodoPagamento"
          value={values.metodoPagamento}
          onChange={(event) =>
            setFieldValue('metodoPagamento', event.target.value as Pagamento['metodoPagamento'])
          }
          className="input-field"
          disabled={isSubmitting}
        >
          <option value="Dinheiro">Dinheiro</option>
          <option value="Cheque">Cheque</option>
          <option value="Transferencia">Transferência</option>
          <option value="Outro">Outro</option>
        </select>
      </div>

      <div>
        <label htmlFor="observacoes" className="mb-1 block text-sm font-medium text-gray-700">
          Observações
        </label>
        <textarea
          id="observacoes"
          rows={3}
          value={values.observacoes}
          onChange={(event) => setFieldValue('observacoes', event.target.value)}
          className="input-field"
          disabled={isSubmitting}
        />
      </div>

      <div className="flex gap-3 pt-2">
        <button type="submit" disabled={isSubmitting} className="btn-primary flex-1">
          {isSubmitting ? 'Registrando...' : 'Registrar pagamento'}
        </button>
        <button type="button" onClick={onCancel} disabled={isSubmitting} className="btn-secondary flex-1">
          Cancelar
        </button>
      </div>
    </form>
  )
}

export default PagamentoForm
