# Script de Seed de Dados - Sales API

## Descrição
Script PostgreSQL para popular o banco de dados com dados de teste realistas.

**Dados gerados:**
- ✅ 50 Clientes com informações variadas
- ✅ 150 Vendas (3 por cliente) com datas aleatórias nos últimos 60 dias
- ✅ 100 Pagamentos associados a vendas do tipo "Fiado"
- ✅ 30 Relatórios Diários (um para cada um dos últimos 30 dias)

**Total: ~200 inserts com dados realistas e relacionados**

## Como Executar

### Pré-requisitos
- PostgreSQL 13+ instalado
- Banco de dados "ma_retail" já criado
- Migrations aplicadas (`dotnet ef database update`)

### Opção 1: Via CLI psql
```bash
psql -U seu_usuario -d ma_retail -f backend/seed_data.sql
```

### Opção 2: Via Azure Data Studio
1. Conecte-se ao banco de dados
2. Abra `seed_data.sql` no editor
3. Execute o script (F5 ou Execute)

### Opção 3: Via C# (Entity Framework)
Se preferir executar como código:
```bash
cd backend/MariaAparecida.Retail.Api
dotnet run
# Depois fazer POST a um endpoint que execute o seed
```

## Estrutura dos Dados

### Clientes (50)
- Nomes diversos com localizações em Belo Horizonte, Contagem e Osasco
- Limites de crédito variando de R$ 2.000 a R$ 5.700
- Status de inadimplência distribuídos: Adimplente (1), Atraso (2), Inadimplente (3)
- Emails únicos e telefones formatados

### Vendas (150)
- 3 vendas por cliente
- DataVenda: aleatória nos últimos 60 dias
- ValorTotal: R$ 100 a R$ 10.000
- TipoVenda: 50% Dinheiro (1), 50% Fiado (2)
- StatusPagamento: distribuído entre Pendente, PartialmentePago, Pago, Cancelado
- PercentualRisco: 0% a 100%

### Pagamentos (100)
- Apenas para vendas do tipo "Fiado" (TipoVenda = 2)
- ValorPago: 50% a 100% do valor da venda
- MetodoPagamento: Dinheiro (1), Cheque (2), Transferência (3), Outro (4)
- DataPagamento: até 15 dias após a venda

### Relatórios Diários (30)
- Um por dia (últimos 30 dias)
- TotalRecebimentos: R$ 5.000 a R$ 20.000
- TotalAReceber: R$ 3.000 a R$ 13.000
- TotalInadimplentes: 2 a 10 clientes
- Percentuais variados para análise

## Campos Utilizados

### Cliente
```sql
Id (UUID)
Nome (string, obrigatório)
Email (string, opcional)
Telefone (string, opcional)
Endereco (string, opcional)
LimiteCredito (decimal, padrão: 0)
StatusInadimplencia (int enum: 1=Adimplente, 2=Atraso, 3=Inadimplente)
CriadoEm (timestamp, padrão: NOW())
AtualizadoEm (timestamp, padrão: NOW())
DeletadoEm (timestamp, soft delete, NULL)
```

### Venda
```sql
Id (UUID)
ClienteId (UUID, FK)
DataVenda (date)
ValorTotal (decimal)
TipoVenda (int enum: 1=Dinheiro, 2=Fiado)
Descricao (string, opcional)
StatusPagamento (int enum: 1=Pendente, 2=PartialmentePago, 3=Pago, 4=Cancelado)
PercentualRisco (decimal 0-100)
CriadoEm (timestamp)
AtualizadoEm (timestamp)
DeletadoEm (timestamp, soft delete, NULL)
```

### Pagamento
```sql
Id (UUID)
VendaId (UUID, FK)
ClienteId (UUID, FK)
DataPagamento (date)
ValorPago (decimal)
MetodoPagamento (int enum: 1=Dinheiro, 2=Cheque, 3=Transferência, 4=Outro)
ComprovanteArquivo (string, opcional)
Observacoes (string, opcional)
CriadoEm (timestamp)
DeletadoEm (timestamp, soft delete, NULL)
```

### Relatório Diário
```sql
Id (UUID)
DataRelatorio (date)
TotalRecebimentos (decimal)
TotalAReceber (decimal)
TotalInadimplentes (int)
TaxaPadraoPercentual (decimal)
IndiceConcentracaoPercentual (decimal)
CriadoEm (timestamp)
```

## Consultas Úteis Após Seed

### Contar dados inseridos
```sql
SELECT 
    (SELECT COUNT(*) FROM "Clientes") as clientes,
    (SELECT COUNT(*) FROM "Vendas") as vendas,
    (SELECT COUNT(*) FROM "Pagamentos") as pagamentos,
    (SELECT COUNT(*) FROM "RelatoriosDiarios") as relatorios;
```

### Ver vendas por cliente (top 10)
```sql
SELECT c."Nome", COUNT(v."Id") as total_vendas, SUM(v."ValorTotal") as total_valor
FROM "Vendas" v
JOIN "Clientes" c ON v."ClienteId" = c."Id"
GROUP BY c."Id", c."Nome"
ORDER BY total_valor DESC
LIMIT 10;
```

### Clientes com atraso ou inadimplentes
```sql
SELECT "Nome", "StatusInadimplencia", "LimiteCredito", 
       COUNT(DISTINCT v."Id") as vendas_fiado
FROM "Clientes" c
LEFT JOIN "Vendas" v ON c."Id" = v."ClienteId" AND v."TipoVenda" = 2
WHERE c."StatusInadimplencia" IN (2, 3)
GROUP BY c."Id", c."Nome", c."StatusInadimplencia", c."LimiteCredito"
ORDER BY c."StatusInadimplencia" DESC;
```

## Limpar Dados (Reset)

Se precisar limpar e recomeçar:
```sql
-- Desabilitar verificação de FK temporariamente
DELETE FROM "Pagamentos";
DELETE FROM "Vendas";
DELETE FROM "RelatoriosDiarios";
DELETE FROM "Clientes";

-- Verificação voltará automaticamente
```

## Notas Importantes

1. **Soft Delete**: Todos os dados têm `DeletadoEm = NULL`. O sistema implementa soft delete, então dados não são removidos fisicamente.

2. **UUIDs**: São gerados aleatoriamente com `gen_random_uuid()`. Cada execução criará UUIDs diferentes.

3. **Timestamps UTC**: Todos os timestamps são em UTC conforme configurado no DbContext (`NOW() at time zone 'utc'`).

4. **Integridade Referencial**: As vendas e pagamentos estão corretamente relacionados aos clientes.

5. **Realismo de Dados**: Os dados são gerados para refletir um cenário realista de varejo, com:
   - Distribuição realista de tipos de venda
   - Datas históricas para testes de relatórios
   - Valores variados para teste de cálculos
   - Status de pagamento diversificados

## Troubleshooting

### Erro: "relation does not exist"
- **Causa**: Tabelas não existem (migrations não foram aplicadas)
- **Solução**: Execute `dotnet ef database update` primeiro

### Erro: "duplicate key value"
- **Causa**: Dados já existem no banco
- **Solução**: Limpe o banco com o script de reset acima

### Erro de FK constraint
- **Causa**: Cliente não existe quando inserindo venda
- **Solução**: Clientes devem ser inseridos antes de vendas (ordem do script)

## Performance

O script é otimizado para:
- **CTEs (Common Table Expressions)**: Evita múltiplas execuções de queries
- **Batch inserts**: Agrupados por tipo de entidade
- **Índices**: Criados automaticamente pelas migrations

Tempo esperado de execução: **< 1 segundo** em um PostgreSQL local.
