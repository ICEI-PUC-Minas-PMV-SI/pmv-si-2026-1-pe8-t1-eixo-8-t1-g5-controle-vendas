import axiosInstance from './axios-config'
import type { Pagamento } from '../../types/domain'
import type { ApiResponse, PaginatedResponse } from '../../types/api-response'
import {
  buildPaginatedResponse,
  getMetodoPagamentoCode,
  mapPagamentoDto,
} from '../../utils/domain-mappers'

interface CreatePagamentoPayload {
  vendaId: string
  clienteId: string
  valorPago: number
  dataPagamento: string
  metodoPagamento: Pagamento['metodoPagamento']
  observacoes?: string
}

interface UpdatePagamentoPayload {
  valorPago?: number
  metodoPagamento?: Pagamento['metodoPagamento']
  observacoes?: string
  comprovanteArquivo?: string
}

interface ListPagamentosParams {
  dataInicio?: string
  dataFim?: string
  metodoPagamento?: Pagamento['metodoPagamento']
  pageNumber?: number
  pageSize?: number
}

async function obterPagamento(pagamentoId: string): Promise<Pagamento> {
  const response = await axiosInstance.get<any, ApiResponse<unknown>>(`/pagamentos/${pagamentoId}`)
  return mapPagamentoDto(response.data)
}

async function listarPagamentos(
  params?: ListPagamentosParams
): Promise<PaginatedResponse<Pagamento>> {
  const pageNumber = params?.pageNumber || 1
  const pageSize = params?.pageSize || 10

  const response = await axiosInstance.get<any, ApiResponse<unknown[]>>('/pagamentos', {
    params: {
      pageNumber,
      pageSize,
      dataInicio: params?.dataInicio,
      dataFim: params?.dataFim,
      metodo: getMetodoPagamentoCode(params?.metodoPagamento),
    },
  })

  const items = Array.isArray(response.data) ? response.data.map(mapPagamentoDto) : []
  return buildPaginatedResponse(items, pageNumber, pageSize)
}

async function criarPagamento(payload: CreatePagamentoPayload): Promise<Pagamento> {
  const response = await axiosInstance.post<any, ApiResponse<unknown>>('/pagamentos', {
    vendaId: payload.vendaId,
    clienteId: payload.clienteId,
    valorPago: payload.valorPago,
    dataPagamento: payload.dataPagamento,
    metodoPagamento: getMetodoPagamentoCode(payload.metodoPagamento),
    observacoes: payload.observacoes || null,
    comprovanteArquivo: null,
  })

  return mapPagamentoDto(response.data)
}

async function atualizarPagamento(
  pagamentoId: string,
  payload: UpdatePagamentoPayload
): Promise<Pagamento> {
  const atual = await obterPagamento(pagamentoId)

  const response = await axiosInstance.put<any, ApiResponse<unknown>>(
    `/pagamentos/${pagamentoId}`,
    {
      valorPago: payload.valorPago ?? atual.valorPago,
      metodoPagamento: getMetodoPagamentoCode(payload.metodoPagamento ?? atual.metodoPagamento),
      comprovanteArquivo: payload.comprovanteArquivo ?? atual.comprovanteArquivo ?? null,
      observacoes: payload.observacoes ?? atual.observacoes ?? null,
    }
  )

  return mapPagamentoDto(response.data)
}

async function deletarPagamento(pagamentoId: string) {
  return axiosInstance.delete(`/pagamentos/${pagamentoId}`)
}

async function listarPagamentosPorVenda(vendaId: string): Promise<Pagamento[]> {
  const response = await axiosInstance.get<any, ApiResponse<unknown[]>>(
    `/pagamentos/venda/${vendaId}`
  )

  return Array.isArray(response.data) ? response.data.map(mapPagamentoDto) : []
}

const pagamentoService = {
  listar: listarPagamentos,
  obter: obterPagamento,
  criar: criarPagamento,
  atualizar: atualizarPagamento,
  deletar: deletarPagamento,
  listarPorVenda: listarPagamentosPorVenda,
}

export default pagamentoService
