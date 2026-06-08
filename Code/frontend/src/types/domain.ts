// Tipos de domínio do negócio (alinhado e normalizado para o frontend)

export interface Usuario {
  id: string
  nomeCompleto: string
  email: string
  ativo?: boolean
  criadoEm?: string
  atualizadoEm?: string
}

export interface Cliente {
  id: string
  nome: string
  email?: string | null
  telefone?: string | null
  endereco?: string | null
  limiteCred: number
  statusInadimplencia: 'Adimplente' | 'Atraso' | 'Inadimplente'
  statusInadimplenciaCodigo?: number
  saldoDevedor?: number
  diasMediaAtraso?: number
  criadoEm: string
  atualizadoEm: string
  deletadoEm?: string
}

export interface Pagamento {
  id: string
  vendaId: string
  clienteId: string
  dataPagamento: string
  valorPago: number
  metodoPagamento: 'Dinheiro' | 'Cheque' | 'Transferencia' | 'Outro'
  metodoPagamentoCodigo?: number
  comprovanteArquivo?: string | null
  observacoes?: string | null
  criadoEm: string
  deletadoEm?: string
}

export interface Venda {
  id: string
  clienteId: string
  cliente?: Cliente
  clienteNome?: string
  dataVenda: string
  valorTotal: number
  tipoVenda: 'Dinheiro' | 'Fiado'
  tipoVendaCodigo?: number
  descricao?: string | null
  statusPagamento: 'Pendente' | 'PartialmentePago' | 'Pago' | 'Cancelado'
  statusPagamentoCodigo?: number
  saldoDevedor?: number
  percentualRisco: number
  pagamentos?: Pagamento[]
  criadoEm: string
  atualizadoEm?: string
  deletadoEm?: string
}

export interface RelatoriosDiarios {
  id: string
  dataRelatorio: string
  totalRecebimentos: number
  totalAReceber: number
  totalInadimplentes: number
  taxaPadraoPercentual: number
  indiceConcentracaoPercentual: number
  criadoEm: string
}

// KPIs do Dashboard
export interface DashboardKPIs {
  totalAReceber: number
  clientesEmAtraso: number
  receitaMes: number
  ticketMedio: number
  taxaInadimplencia: number
  periodoColeta: number
  indiceConcentracao: number
  taxaPadraoPercentual?: number
  dataReferencia?: string
}

// Status de inadimplência
export enum StatusInadimplencia {
  Adimplente = 'Adimplente',
  Atraso = 'Atraso',
  Inadimplente = 'Inadimplente',
}

// Tipos de venda
export enum TipoVenda {
  Dinheiro = 'Dinheiro',
  Fiado = 'Fiado',
}

// Status de pagamento
export enum StatusPagamento {
  Pendente = 'Pendente',
  PartialmentePago = 'PartialmentePago',
  Pago = 'Pago',
  Cancelado = 'Cancelado',
}
