import type { Cliente, Pagamento, Usuario, Venda } from '../types/domain'
import type { LoginResponse, PaginatedResponse } from '../types/api-response'

const statusInadimplenciaLabels = {
  1: 'Adimplente',
  2: 'Atraso',
  3: 'Inadimplente',
} as const

const tipoVendaLabels = {
  1: 'Dinheiro',
  2: 'Fiado',
} as const

const statusPagamentoLabels = {
  1: 'Pendente',
  2: 'PartialmentePago',
  3: 'Pago',
  4: 'Cancelado',
} as const

const metodoPagamentoLabels = {
  1: 'Dinheiro',
  2: 'Cheque',
  3: 'Transferencia',
  4: 'Outro',
} as const

function asRecord(value: unknown): Record<string, unknown> {
  return typeof value === 'object' && value !== null
    ? (value as Record<string, unknown>)
    : {}
}

function asString(value: unknown, fallback = ''): string {
  if (typeof value === 'string') return value
  if (typeof value === 'number') return String(value)
  return fallback
}

function asNullableString(value: unknown): string | null {
  const parsed = asString(value).trim()
  return parsed.length > 0 ? parsed : null
}

function asNumber(value: unknown, fallback = 0): number {
  if (typeof value === 'number' && !Number.isNaN(value)) return value
  if (typeof value === 'string') {
    const parsed = Number(value)
    return Number.isFinite(parsed) ? parsed : fallback
  }
  return fallback
}

function asArray(value: unknown): unknown[] {
  return Array.isArray(value) ? value : []
}

function isValidLabel<T extends string>(
  value: unknown,
  allowed: readonly T[],
  fallback: T
): T {
  return typeof value === 'string' && allowed.includes(value as T)
    ? (value as T)
    : fallback
}

export function getStatusInadimplenciaLabel(value: unknown): Cliente['statusInadimplencia'] {
  if (typeof value === 'string') {
    return isValidLabel(value, ['Adimplente', 'Atraso', 'Inadimplente'], 'Adimplente')
  }

  return statusInadimplenciaLabels[asNumber(value, 1) as keyof typeof statusInadimplenciaLabels] ?? 'Adimplente'
}

export function getStatusInadimplenciaCode(value: unknown): number | undefined {
  if (typeof value === 'number') return value
  switch (value) {
    case 'Adimplente':
      return 1
    case 'Atraso':
      return 2
    case 'Inadimplente':
      return 3
    default:
      return undefined
  }
}

export function getTipoVendaLabel(value: unknown): Venda['tipoVenda'] {
  if (typeof value === 'string') {
    return isValidLabel(value, ['Dinheiro', 'Fiado'], 'Dinheiro')
  }

  return tipoVendaLabels[asNumber(value, 1) as keyof typeof tipoVendaLabels] ?? 'Dinheiro'
}

export function getTipoVendaCode(value: unknown): number | undefined {
  if (typeof value === 'number') return value
  switch (value) {
    case 'Dinheiro':
      return 1
    case 'Fiado':
      return 2
    default:
      return undefined
  }
}

export function getStatusPagamentoLabel(value: unknown): Venda['statusPagamento'] {
  if (typeof value === 'string') {
    return isValidLabel(
      value,
      ['Pendente', 'PartialmentePago', 'Pago', 'Cancelado'],
      'Pendente'
    )
  }

  return statusPagamentoLabels[asNumber(value, 1) as keyof typeof statusPagamentoLabels] ?? 'Pendente'
}

export function getStatusPagamentoCode(value: unknown): number | undefined {
  if (typeof value === 'number') return value
  switch (value) {
    case 'Pendente':
      return 1
    case 'PartialmentePago':
      return 2
    case 'Pago':
      return 3
    case 'Cancelado':
      return 4
    default:
      return undefined
  }
}

export function getMetodoPagamentoLabel(value: unknown): Pagamento['metodoPagamento'] {
  if (typeof value === 'string') {
    return isValidLabel(
      value,
      ['Dinheiro', 'Cheque', 'Transferencia', 'Outro'],
      'Dinheiro'
    )
  }

  return metodoPagamentoLabels[asNumber(value, 1) as keyof typeof metodoPagamentoLabels] ?? 'Dinheiro'
}

export function getMetodoPagamentoCode(value: unknown): number | undefined {
  if (typeof value === 'number') return value
  switch (value) {
    case 'Dinheiro':
      return 1
    case 'Cheque':
      return 2
    case 'Transferencia':
      return 3
    case 'Outro':
      return 4
    default:
      return undefined
  }
}

export function mapClienteDto(input: unknown): Cliente {
  const dto = asRecord(input)

  return {
    id: asString(dto.id),
    nome: asString(dto.nome),
    email: asNullableString(dto.email),
    telefone: asNullableString(dto.telefone),
    endereco: asNullableString(dto.endereco),
    limiteCred: asNumber(dto.limiteCred ?? dto.limiteCredito),
    statusInadimplencia: getStatusInadimplenciaLabel(dto.statusInadimplencia),
    statusInadimplenciaCodigo: getStatusInadimplenciaCode(dto.statusInadimplencia),
    saldoDevedor: dto.saldoDevedor != null ? asNumber(dto.saldoDevedor) : undefined,
    diasMediaAtraso: dto.diasMediaAtraso != null ? asNumber(dto.diasMediaAtraso) : undefined,
    criadoEm: asString(dto.criadoEm),
    atualizadoEm: asString(dto.atualizadoEm),
    deletadoEm: asNullableString(dto.deletadoEm) ?? undefined,
  }
}

export function mapPagamentoDto(input: unknown): Pagamento {
  const dto = asRecord(input)

  return {
    id: asString(dto.id),
    vendaId: asString(dto.vendaId),
    clienteId: asString(dto.clienteId),
    dataPagamento: asString(dto.dataPagamento),
    valorPago: asNumber(dto.valorPago),
    metodoPagamento: getMetodoPagamentoLabel(dto.metodoPagamento),
    metodoPagamentoCodigo: getMetodoPagamentoCode(dto.metodoPagamento),
    comprovanteArquivo: asNullableString(dto.comprovanteArquivo),
    observacoes: asNullableString(dto.observacoes),
    criadoEm: asString(dto.criadoEm),
    deletadoEm: asNullableString(dto.deletadoEm) ?? undefined,
  }
}

export function mapVendaDto(input: unknown, cliente?: Cliente): Venda {
  const dto = asRecord(input)
  const pagamentos = asArray(dto.pagamentos).map(mapPagamentoDto)

  return {
    id: asString(dto.id),
    clienteId: asString(dto.clienteId),
    cliente: cliente ?? (dto.cliente ? mapClienteDto(dto.cliente) : undefined),
    clienteNome: cliente?.nome ?? asString(dto.clienteNome),
    dataVenda: asString(dto.dataVenda),
    valorTotal: asNumber(dto.valorTotal),
    tipoVenda: getTipoVendaLabel(dto.tipoVenda),
    tipoVendaCodigo: getTipoVendaCode(dto.tipoVenda),
    descricao: asNullableString(dto.descricao),
    statusPagamento: getStatusPagamentoLabel(dto.statusPagamento),
    statusPagamentoCodigo: getStatusPagamentoCode(dto.statusPagamento),
    saldoDevedor: dto.saldoDevedor != null ? asNumber(dto.saldoDevedor) : undefined,
    percentualRisco: asNumber(dto.percentualRisco),
    pagamentos: pagamentos.length ? pagamentos : undefined,
    criadoEm: asString(dto.criadoEm),
    atualizadoEm: asString(dto.atualizadoEm),
    deletadoEm: asNullableString(dto.deletadoEm) ?? undefined,
  }
}

export function mapLoginResponse(input: unknown): LoginResponse {
  const root = asRecord(input)
  
  // A API pode retornar os dados embrulhados em 'data' (ApiResponse) ou diretamente
  // Se houver uma propriedade 'data' que é um objeto e tem 'token' ou 'usuarioId', usamos ela
  const dataObj = asRecord(root.data)
  const hasDataPayload = root.data && (dataObj.token || dataObj.usuarioId || dataObj.email)
  const dto = hasDataPayload ? dataObj : root

  const email = asString(dto.email)
  const nomeCompleto = asString(dto.nomeCompleto) || (email.includes('@') ? email.split('@')[0] : email)

  const usuario: Usuario = {
    id: asString(dto.usuarioId || dto.id),
    nomeCompleto,
    email,
    ativo: true,
  }

  return {
    token: asString(dto.token),
    expiresAt: asString(dto.expiresAt),
    usuario,
  }
}

export function buildPaginatedResponse<T>(
  items: T[],
  pageNumber = 1,
  pageSize = 10
): PaginatedResponse<T> {
  if (items.length > pageSize) {
    const safePage = Math.max(1, pageNumber)
    const startIndex = (safePage - 1) * pageSize
    const pagedItems = items.slice(startIndex, startIndex + pageSize)

    return {
      items: pagedItems,
      totalItems: items.length,
      pageNumber: safePage,
      pageSize,
      totalPages: Math.max(1, Math.ceil(items.length / pageSize)),
      hasNextPage: startIndex + pageSize < items.length,
    }
  }

  const hasNextPage = items.length === pageSize

  return {
    items,
    totalItems: (Math.max(1, pageNumber) - 1) * pageSize + items.length + (hasNextPage ? 1 : 0),
    pageNumber: Math.max(1, pageNumber),
    pageSize,
    totalPages: hasNextPage ? Math.max(2, pageNumber + 1) : Math.max(1, pageNumber),
    hasNextPage,
  }
}

export function mapDashboardKpis(input: unknown) {
  const dto = asRecord(input)
  const totalReceber = asNumber(dto.totalReceber ?? dto.totalAReceber)
  const receitaMes = asNumber(dto.receitaMes)
  const ticketMedio = asNumber(dto.ticketMedio)
  const taxaInadimplencia = asNumber(dto.taxaInadimplencia)
  const taxaPadraoPercentual = asNumber(dto.taxaPadraoPercentual)
  const periodoMedioColeta = asNumber(dto.periodoMedioColeta ?? dto.periodoColeta)
  const indiceConcentracaoPercentual = asNumber(
    dto.indiceConcentracaoPercentual ?? dto.indiceConcentracao
  )
  const totalInadimplentes = asNumber(dto.totalInadimplentes ?? dto.clientesEmAtraso)

  return {
    totalAReceber: totalReceber,
    clientesEmAtraso: totalInadimplentes,
    receitaMes,
    ticketMedio,
    taxaInadimplencia,
    periodoColeta: periodoMedioColeta,
    indiceConcentracao: indiceConcentracaoPercentual,
    taxaPadraoPercentual,
    dataReferencia: asString(dto.dataReferencia),
  }
}
