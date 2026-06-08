import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { useApi } from '../../hooks'
import { clienteService } from '../../services/api'
import type { Cliente } from '../../types/domain'
import type { PaginatedResponse } from '../../types/api-response'
import { LoadingSpinner } from '../../components/common/LoadingSpinner'
import { ErrorAlert } from '../../components/common/ErrorAlert'

export function ClientesList() {
  const navigate = useNavigate()
  const { data, isLoading, error, execute } = useApi<PaginatedResponse<Cliente>>()
  const [searchTerm, setSearchTerm] = useState('')
  const [currentPage, setCurrentPage] = useState(1)
  const [deleteError, setDeleteError] = useState<string | null>(null)

  // Carregar clientes quando página ou filtro mudar
  useEffect(() => {
    loadClientes(currentPage, searchTerm)
  }, [currentPage, searchTerm])

  const loadClientes = async (page: number, search: string) => {
    await execute(() =>
      clienteService.listar({
        pageNumber: page,
        pageSize: 10,
        nome: search || undefined,
      })
    )
  }

  const handleCreate = () => {
    navigate('/clientes/novo')
  }

  const handleEdit = (cliente: Cliente) => {
    navigate(`/clientes/${cliente.id}/editar`)
  }

  const handleDelete = async (clienteId: string) => {
    if (!window.confirm('Tem certeza que deseja deletar este cliente?')) {
      return
    }

    try {
      await clienteService.deletar(clienteId)
      await loadClientes(currentPage, searchTerm)
    } catch (err: any) {
      setDeleteError(err?.message || 'Erro ao deletar cliente')
    }
  }

  return (
    <div className="p-6 space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Clientes</h1>
          <p className="text-gray-600 mt-1">Gerencie seus clientes e limites de crédito</p>
        </div>
        <button
          onClick={handleCreate}
          className="btn-primary px-6 py-3"
        >
          + Novo Cliente
        </button>
      </div>

      {/* Filtro */}
      <div className="flex gap-2">
        <input
          type="text"
          placeholder="Buscar por nome..."
          value={searchTerm}
          onChange={(e) => {
            setSearchTerm(e.target.value)
            setCurrentPage(1)
          }}
          className="input-field flex-1"
        />
      </div>

      {/* Alertas */}
      {error && <ErrorAlert message={error} />}
      {deleteError && <ErrorAlert message={deleteError} onClose={() => setDeleteError(null)} />}

      {/* Loading */}
      {isLoading && <LoadingSpinner message="Carregando clientes..." />}

      {/* Lista */}
      {!isLoading && data && (
        <>
          {data.items.length === 0 ? (
            <div className="text-center py-12">
              <p className="text-gray-600">Nenhum cliente encontrado</p>
            </div>
          ) : (
            <div className="grid gap-4">
              {data.items.map((cliente) => (
                <div
                  key={cliente.id}
                  className="card flex items-center justify-between hover:shadow-md transition"
                >
                  <div className="flex-1">
                    <h3 className="font-semibold text-gray-900">{cliente.nome}</h3>
                    <p className="text-sm text-gray-600 mt-1">
                      📱 {cliente.telefone} | 💳 R$ {cliente.limiteCred.toFixed(2)}
                    </p>
                    <p className="text-xs text-gray-500 mt-1">
                      Status: <span className={`font-semibold ${
                        cliente.statusInadimplencia === 'Adimplente' ? 'text-green-600' :
                        cliente.statusInadimplencia === 'Atraso' ? 'text-yellow-600' :
                        'text-red-600'
                      }`}>
                        {cliente.statusInadimplencia}
                      </span>
                    </p>
                    {cliente.saldoDevedor && cliente.saldoDevedor > 0 && (
                      <p className="text-sm text-red-600 mt-1 font-semibold">
                        Deve: R$ {cliente.saldoDevedor.toFixed(2)}
                      </p>
                    )}
                  </div>
                  <div className="flex gap-2">
                    <button
                      onClick={() => handleEdit(cliente)}
                      className="btn-secondary btn-sm px-3 py-2"
                    >
                      ✏️ Editar
                    </button>
                    <button
                      onClick={() => handleDelete(cliente.id)}
                      className="btn-danger btn-sm px-3 py-2"
                    >
                      🗑️ Deletar
                    </button>
                  </div>
                </div>
              ))}
            </div>
          )}

          {/* Paginação */}
          {data.totalPages > 1 && (
            <div className="flex items-center justify-center gap-2 pt-4">
              <button
                onClick={() => setCurrentPage(Math.max(1, currentPage - 1))}
                disabled={currentPage === 1}
                className="btn-secondary px-3 py-1 disabled:opacity-50"
              >
                ← Anterior
              </button>
              <span className="text-gray-600 text-sm">
                Página {data.pageNumber} de {data.totalPages}
              </span>
              <button
                onClick={() => setCurrentPage(Math.min(data.totalPages, currentPage + 1))}
                disabled={currentPage === data.totalPages}
                className="btn-secondary px-3 py-1 disabled:opacity-50"
              >
                Próximo →
              </button>
            </div>
          )}
        </>
      )}

      {/* Modal de Formulário */}
    </div>
  )
}

export default ClientesList
