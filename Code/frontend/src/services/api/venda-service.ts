import axiosInstance from './axios-config'
import type { Cliente, Venda } from '../../types/domain'
import type { ApiResponse, PaginatedResponse } from '../../types/api-response'
import {
  buildPaginatedResponse,
  getStatusPagamentoCode,
  getTipoVendaCode,
  mapClienteDto,
  mapVendaDto,
} from '../../utils/domain-mappers'

interface CreateVendaPayload {
  clienteId: string
  dataVenda: string
  valorTotal: number
  tipoVenda: 'Dinheiro' | 'Fiado'
  descricao: string
}

interface UpdateVendaPayload {
  clienteId?: string
  dataVenda?: string
  valorTotal?: number
  tipoVenda?: 'Dinheiro' | 'Fiado'
  descricao?: string
}

interface ListVendasParams {
  clienteId?: string
  tipoVenda?: 'Dinheiro' | 'Fiado'
  dataInicio?: string
  dataFim?: string
  statusPagamento?: Venda['statusPagamento']
  pageNumber?: number
  pageSize?: number
}

async function buscarClientes(clienteIds: string[]): Promise<Map<string, Cliente>> {
  const clienteMap = new Map<string, Cliente>()

  await Promise.all(
    [...new Set(clienteIds)]
      .filter(Boolean)
      .map(async (clienteId) => {
        try {
          const response = await axiosInstance.get<any, ApiResponse<unknown>>(
            `/clientes/${clienteId}`
          )

          clienteMap.set(clienteId, mapClienteDto(response.data))
        } catch (error) {
          console.warn(`Não foi possível carregar o cliente ${clienteId}`, error)
        }
      })
  )

  return clienteMap
}

async function enriquecerVendas(rawVendas: unknown[]): Promise<Venda[]> {
  const vendas = rawVendas.map((venda) => mapVendaDto(venda))
  const clientes = await buscarClientes(vendas.map((venda) => venda.clienteId))

  return vendas.map((venda) => ({
    ...venda,
    cliente: clientes.get(venda.clienteId),
    clienteNome: clientes.get(venda.clienteId)?.nome,
  }))
}

async function obterVenda(vendaId: string): Promise<Venda> {
  const response = await axiosInstance.get<any, ApiResponse<unknown>>(`/vendas/${vendaId}`)
  const venda = mapVendaDto(response.data)

  try {
    const clienteResponse = await axiosInstance.get<any, ApiResponse<unknown>>(
      `/clientes/${venda.clienteId}`
    )

    return mapVendaDto(response.data, mapClienteDto(clienteResponse.data))
  } catch {
    return venda
  }
}

async function listarVendas(params?: ListVendasParams): Promise<PaginatedResponse<Venda>> {
  const requestedPageNumber = params?.pageNumber || 1
  const requestedPageSize = params?.pageSize || 10

  const response = await axiosInstance.get<any, ApiResponse<unknown[]>>('/vendas', {
    params: {
      pageNumber: requestedPageNumber,
      pageSize: requestedPageSize,
      clienteId: params?.clienteId,
      tipoVenda: getTipoVendaCode(params?.tipoVenda),
      statusPagamento: getStatusPagamentoCode(params?.statusPagamento),
      dataInicio: params?.dataInicio,
      dataFim: params?.dataFim,
    },
  })

  const vendas = Array.isArray(response.data) ? await enriquecerVendas(response.data) : []

  return buildPaginatedResponse(vendas, requestedPageNumber, requestedPageSize)
}

async function criarVenda(payload: CreateVendaPayload): Promise<Venda> {
  const response = await axiosInstance.post<any, ApiResponse<unknown>>('/vendas', {
    clienteId: payload.clienteId,
    dataVenda: payload.dataVenda,
    valorTotal: payload.valorTotal,
    tipoVenda: getTipoVendaCode(payload.tipoVenda),
    descricao: payload.descricao || null,
  })

  return obterVenda(mapVendaDto(response.data).id)
}

async function atualizarVenda(vendaId: string, payload: UpdateVendaPayload): Promise<Venda> {
  const response = await axiosInstance.put<any, ApiResponse<unknown>>(`/vendas/${vendaId}`, {
    clienteId: payload.clienteId,
    dataVenda: payload.dataVenda,
    valorTotal: payload.valorTotal,
    tipoVenda: getTipoVendaCode(payload.tipoVenda),
    descricao: payload.descricao,
  })

  return obterVenda(mapVendaDto(response.data).id)
}

async function atualizarStatusVenda(
  vendaId: string,
  novoStatus: Venda['statusPagamento']
): Promise<Venda> {
  const response = await axiosInstance.patch<any, ApiResponse<unknown>>(
    `/vendas/${vendaId}/status`,
    { novoStatus: getStatusPagamentoCode(novoStatus) }
  )

  return mapVendaDto(response.data)
}

async function deletarVenda(vendaId: string) {
  return axiosInstance.delete(`/vendas/${vendaId}`)
}

async function obterPagamentosVenda(vendaId: string): Promise<Venda> {
  const response = await axiosInstance.get<any, ApiResponse<unknown>>(
    `/vendas/${vendaId}/historico-pagamentos`
  )

  const venda = mapVendaDto(response.data)

  try {
    const clienteResponse = await axiosInstance.get<any, ApiResponse<unknown>>(
      `/clientes/${venda.clienteId}`
    )

    return mapVendaDto(response.data, mapClienteDto(clienteResponse.data))
  } catch {
    return venda
  }
}

async function listarVendasEmAberto(
  pageNumber = 1,
  pageSize = 10
): Promise<PaginatedResponse<Venda>> {
  const baseResponse = await listarVendas({
    tipoVenda: 'Fiado',
    pageNumber: 1,
    pageSize: 100,
  })

  const emAberto = baseResponse.items.filter(
    (venda) => venda.statusPagamento === 'Pendente' || venda.statusPagamento === 'PartialmentePago'
  )
  const paged = buildPaginatedResponse(emAberto, pageNumber, pageSize)
  const detailedItems = await Promise.all(paged.items.map((venda) => obterVenda(venda.id)))

  return {
    ...paged,
    items: detailedItems,
  }
}

const vendaService = {
  listar: listarVendas,
  obter: obterVenda,
  criar: criarVenda,
  atualizar: atualizarVenda,
  atualizarStatus: atualizarStatusVenda,
  deletar: deletarVenda,
  obterPagamentos: obterPagamentosVenda,
  listarEmAberto: listarVendasEmAberto,
}

export default vendaService
