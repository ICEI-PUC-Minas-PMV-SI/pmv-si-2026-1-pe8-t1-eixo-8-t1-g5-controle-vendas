import axiosInstance from './axios-config'
import type {
  DashboardKPIs,
  Cliente,
  Venda,
} from '../../types/domain'
import type { ApiResponse } from '../../types/api-response'
import { mapDashboardKpis } from '../../utils/domain-mappers'

/**
 * Tipos específicos para responses de relatórios
 */
export interface ReceiveisDto {
  totalAReceber: number
  clientesEmAtraso: number
  vendas: Venda[]
}

export interface TaxaPadraoDto {
  taxaPadrao: number
  dataReferencia: string
  periodoAnalisado: string
}

export interface PeriodoColetaDto {
  periodoMedioEmDias: number
  periodoMinimo: number
  periodoMaximo: number
}

export interface ConcentracaoDto {
  indiceConcentracao: number
  topClientes: Array<{
    cliente: { id: string; nome: string } | Cliente
    totalDevendo: number
    percentualTotal: number
  }>
}

export interface InadimplenciaDto {
  cliente: { id: string; nome: string } | Cliente
  diasAtraso: number
  totalDevendo: number
  ultimoContato?: string
}

export interface InteraloDto {
  periodoInicio: string
  periodoFim: string
  totalRecebimentos: number
  totalDevido: number
  clientesAtivos: number
  inadimplentes: InadimplenciaDto[]
}

export interface ReceitaMensalDto {
  ano: number
  mes: number
  nomeMes: string
  receita: number
}

/**
 * Serviço de Relatórios
 * Wrapper para todos os endpoints /api/relatorios/*
 */
const relatorioService = {
  /**
   * GET /api/relatorios/dashboard
   * Retorna KPIs principais do dashboard
   */
  async getDashboard(
    dataInicio?: string,
    dataFim?: string
  ): Promise<DashboardKPIs> {
    const response = await axiosInstance.get<any, ApiResponse<any>>(
      '/relatorios/dashboard',
      {
        params: {
          ...(dataInicio && { dataInicio }),
          ...(dataFim && { dataFim }),
        },
      }
    )
    return mapDashboardKpis(response.data)
  },

  async getReceitaMensal(meses = 6): Promise<ReceitaMensalDto[]> {
    const response = await axiosInstance.get<any, ApiResponse<any[]>>(
      '/relatorios/receita-mensal',
      {
        params: { meses },
      }
    )
    const data = response.data

    return Array.isArray(data)
      ? data.map((item: any) => ({
          ano: Number(item.ano) || 0,
          mes: Number(item.mes) || 0,
          nomeMes: item.nomeMes || '',
          receita: Number(item.receita) || 0,
        }))
      : []
  },

  /**
   * GET /api/relatorios/recebiveis
   * Retorna total a receber e clientes em atraso
   */
  async getRecebiveis(
    dataInicio?: string,
    dataFim?: string
  ): Promise<ReceiveisDto> {
    const response = await axiosInstance.get<any, ApiResponse<ReceiveisDto>>(
      '/relatorios/recebiveis',
      {
        params: {
          ...(dataInicio && { dataInicio }),
          ...(dataFim && { dataFim }),
        },
      }
    )
    return response.data
  },

  /**
   * GET /api/relatorios/taxa-padrao
   * Retorna taxa padrão de conversão
   */
  async getTaxaPadrao(
    dataInicio?: string,
    dataFim?: string
  ): Promise<TaxaPadraoDto> {
    const response = await axiosInstance.get<any, ApiResponse<TaxaPadraoDto>>(
      '/relatorios/taxa-padrao',
      {
        params: {
          ...(dataInicio && { dataInicio }),
          ...(dataFim && { dataFim }),
        },
      }
    )
    return response.data
  },

  /**
   * GET /api/relatorios/periodo-coleta
   * Retorna período médio de coleta
   */
  async getPeriodoColeta(): Promise<PeriodoColetaDto> {
    const response = await axiosInstance.get<any, ApiResponse<PeriodoColetaDto>>(
      '/relatorios/periodo-coleta'
    )
    return response.data
  },

  /**
   * GET /api/relatorios/concentracao
   * Retorna índice de concentração de crédito
   */
  async getConcentracao(
    dataInicio?: string,
    dataFim?: string
  ): Promise<ConcentracaoDto> {
    const response = await axiosInstance.get<any, ApiResponse<any>>(
      '/relatorios/concentracao',
      {
        params: {
          ...(dataInicio && { dataInicio }),
          ...(dataFim && { dataFim }),
        },
      }
    )
    const data = response.data
    
    // Mapear os dados retornados pelo backend
    return {
      indiceConcentracao: data.indiceConcentracao || 0,
      topClientes: (data.topClientes || []).map((item: any) => ({
        cliente: item.cliente || { id: item.clienteId, nome: item.nome },
        totalDevendo: item.totalDevendo || 0,
        percentualTotal: item.percentualTotal || 0,
      })),
    }
  },

  /**
   * GET /api/relatorios/inadimplencia
   * Retorna lista de clientes inadimplentes
   */
  async getInadimplencia(diasAtraso?: number): Promise<InadimplenciaDto[]> {
    const response = await axiosInstance.get<any, ApiResponse<any[]>>(
      '/relatorios/inadimplencia',
      {
        params: {
          ...(diasAtraso && { diasAtraso }),
        },
      }
    )
    const data = response.data
    
    // Mapear os dados retornados pelo backend
    if (Array.isArray(data)) {
      return data.map((item: any) => ({
        cliente: item.cliente || { id: item.clienteId, nome: item.nome },
        diasAtraso: item.diasAtraso || diasAtraso || 30,
        totalDevendo: item.totalDevendo || 0,
      }))
    }
    return []
  },

  /**
   * GET /api/relatorios/intervalo
   * Retorna relatório completo em intervalo de datas
   */
  async getIntervalo(
    dataInicio: string,
    dataFim: string,
    diasAtraso?: number
  ): Promise<InteraloDto> {
    const response = await axiosInstance.get<any, ApiResponse<any>>(
      '/relatorios/intervalo',
      {
        params: {
          dataInicio,
          dataFim,
          ...(diasAtraso && { diasAtraso }),
        },
      }
    )
    const data = response.data
    
    // Mapear os dados retornados pelo backend
    return {
      periodoInicio: data.periodoInicio || dataInicio,
      periodoFim: data.periodoFim || dataFim,
      totalRecebimentos: data.totalRecebimentos || 0,
      totalDevido: data.totalDevido || 0,
      clientesAtivos: data.clientesAtivos || 0,
      inadimplentes: (data.inadimplentes || []).map((item: any) => ({
        cliente: item.cliente || { id: item.clienteId, nome: item.nome },
        diasAtraso: item.diasAtraso || diasAtraso || 30,
        totalDevendo: item.totalDevendo || 0,
      })),
    }
  },
}

export default relatorioService
