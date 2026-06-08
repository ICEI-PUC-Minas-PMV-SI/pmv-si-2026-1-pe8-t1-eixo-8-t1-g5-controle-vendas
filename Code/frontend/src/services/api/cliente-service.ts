import axiosInstance from './axios-config'
import type { Cliente, Pagamento, Venda } from '../../types/domain'
import type { ApiResponse, PaginatedResponse } from '../../types/api-response'
import {
  buildPaginatedResponse,
  getStatusInadimplenciaCode,
  mapClienteDto,
  mapPagamentoDto,
  mapVendaDto,
} from '../../utils/domain-mappers'

interface CreateClientePayload {
  nome: string
  email?: string
  telefone?: string
  endereco?: string
  limiteCred: number
}

interface UpdateClientePayload {
  nome?: string
  email?: string
  telefone?: string
  endereco?: string
  limiteCred?: number
}

interface ListClientesParams {
  nome?: string
  status?: Cliente['statusInadimplencia']
  pageNumber?: number
  pageSize?: number
}

function removeUndefined<T extends Record<string, unknown>>(payload: T): Partial<T> {
  return Object.fromEntries(
    Object.entries(payload).filter(([, value]) => value !== undefined)
  ) as Partial<T>
}

function normalizeTelefone(telefone?: string): string | null {
  if (!telefone) return null
  const digits = telefone.replace(/\D/g, '')
  return digits.length > 0 ? digits : null
}

async function obterCliente(clienteId: string): Promise<Cliente> {
  const response = await axiosInstance.get<any, ApiResponse<unknown>>(`/clientes/${clienteId}`)
  return mapClienteDto(response.data)
}

async function listarClientes(params?: ListClientesParams): Promise<PaginatedResponse<Cliente>> {
  const pageNumber = params?.pageNumber || 1
  const pageSize = params?.pageSize || 10

  const response = await axiosInstance.get<any, ApiResponse<unknown[]>>('/clientes', {
    params: {
      pageNumber,
      pageSize,
      nome: params?.nome,
      status: getStatusInadimplenciaCode(params?.status),
    },
  })

  const items = Array.isArray(response.data) ? response.data.map(mapClienteDto) : []
  return buildPaginatedResponse(items, pageNumber, pageSize)
}

async function criarCliente(payload: CreateClientePayload): Promise<Cliente> {
  const response = await axiosInstance.post<any, ApiResponse<unknown>>('/clientes', {
    nome: payload.nome,
    email: payload.email || null,
    telefone: normalizeTelefone(payload.telefone),
    endereco: payload.endereco || null,
    limiteCredito: payload.limiteCred,
  })

  return mapClienteDto(response.data)
}

async function atualizarCliente(clienteId: string, payload: UpdateClientePayload): Promise<Cliente> {
  let clienteAtualizado: Cliente | null = null

  const updatePayload = removeUndefined({
    nome: payload.nome,
    email: payload.email,
    telefone: payload.telefone === undefined ? undefined : normalizeTelefone(payload.telefone),
    endereco: payload.endereco,
  })

  if (Object.keys(updatePayload).length > 0) {
    const response = await axiosInstance.put<any, ApiResponse<unknown>>(
      `/clientes/${clienteId}`,
      updatePayload
    )

    clienteAtualizado = mapClienteDto(response.data)
  }

  if (typeof payload.limiteCred === 'number') {
    const response = await axiosInstance.patch<any, ApiResponse<unknown>>(
      `/clientes/${clienteId}/limite-credito`,
      { novoLimite: payload.limiteCred }
    )

    clienteAtualizado = mapClienteDto(response.data)
  }

  return clienteAtualizado ?? obterCliente(clienteId)
}

async function atualizarLimiteCredito(clienteId: string, novoLimite: number): Promise<Cliente> {
  const response = await axiosInstance.patch<any, ApiResponse<unknown>>(
    `/clientes/${clienteId}/limite-credito`,
    { novoLimite }
  )

  return mapClienteDto(response.data)
}

async function deletarCliente(clienteId: string) {
  return axiosInstance.delete(`/clientes/${clienteId}`)
}

async function obterVendasCliente(
  clienteId: string,
  pageNumber = 1,
  pageSize = 10
): Promise<PaginatedResponse<Venda>> {
  const cliente = await obterCliente(clienteId)
  const response = await axiosInstance.get<any, ApiResponse<unknown[]>>('/vendas', {
    params: {
      clienteId,
      pageNumber,
      pageSize,
    },
  })

  const items = Array.isArray(response.data)
    ? response.data.map((venda) => mapVendaDto(venda, cliente))
    : []

  return buildPaginatedResponse(items, pageNumber, pageSize)
}

async function obterPagamentosCliente(
  clienteId: string,
  pageNumber = 1,
  pageSize = 10
): Promise<PaginatedResponse<Pagamento>> {
  const vendas = await obterVendasCliente(clienteId, 1, 100)
  const pagamentos = await Promise.all(
    vendas.items.map(async (venda) => {
      const response = await axiosInstance.get<any, ApiResponse<unknown[]>>(
        `/pagamentos/venda/${venda.id}`
      )

      return Array.isArray(response.data) ? response.data.map(mapPagamentoDto) : []
    })
  )

  return buildPaginatedResponse(pagamentos.flat(), pageNumber, pageSize)
}

const clienteService = {
  listar: listarClientes,
  obter: obterCliente,
  criar: criarCliente,
  atualizar: atualizarCliente,
  atualizarLimiteCredito,
  deletar: deletarCliente,
  obterVendas: obterVendasCliente,
  obterPagamentos: obterPagamentosCliente,
}

export default clienteService
