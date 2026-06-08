-- Script para popular o banco de dados com dados de teste (PostgreSQL)
-- Contém: 50 Clientes, 150 Vendas, 100 Pagamentos, 30 Relatórios Diários

-- ============================================
-- 1. INSERT CLIENTES (50)
-- ============================================

INSERT INTO "Clientes" ("Id", "Nome", "Email", "Telefone", "Endereco", "LimiteCredito", "StatusInadimplencia", "CriadoEm", "AtualizadoEm", "DeletadoEm") VALUES
(gen_random_uuid(), 'João Silva', 'joao.silva@email.com', '(31) 99999-0001', 'Rua A, 100 - Belo Horizonte', 5000.00, 1, NOW(), NOW(), NULL),
(gen_random_uuid(), 'Maria Santos', 'maria.santos@email.com', '(31) 99999-0002', 'Rua B, 200 - Belo Horizonte', 3500.00, 1, NOW(), NOW(), NULL),
(gen_random_uuid(), 'Carlos Oliveira', 'carlos.oliveira@email.com', '(31) 99999-0003', 'Rua C, 300 - Belo Horizonte', 4000.00, 1, NOW(), NOW(), NULL),
(gen_random_uuid(), 'Ana Costa', 'ana.costa@email.com', '(31) 99999-0004', 'Rua D, 400 - Belo Horizonte', 2500.00, 2, NOW(), NOW(), NULL),
(gen_random_uuid(), 'Pedro Ferreira', 'pedro.ferreira@email.com', '(31) 99999-0005', 'Rua E, 500 - Belo Horizonte', 6000.00, 1, NOW(), NOW(), NULL),
(gen_random_uuid(), 'Juliana Lima', 'juliana.lima@email.com', '(31) 99999-0006', 'Rua F, 600 - Belo Horizonte', 3000.00, 1, NOW(), NOW(), NULL),
(gen_random_uuid(), 'Ricardo Gomes', 'ricardo.gomes@email.com', '(31) 99999-0007', 'Rua G, 700 - Belo Horizonte', 5500.00, 1, NOW(), NOW(), NULL),
(gen_random_uuid(), 'Fernanda Dias', 'fernanda.dias@email.com', '(31) 99999-0008', 'Rua H, 800 - Belo Horizonte', 2000.00, 2, NOW(), NOW(), NULL),
(gen_random_uuid(), 'Bruno Martins', 'bruno.martins@email.com', '(31) 99999-0009', 'Rua I, 900 - Belo Horizonte', 4500.00, 1, NOW(), NOW(), NULL),
(gen_random_uuid(), 'Camila Rocha', 'camila.rocha@email.com', '(31) 99999-0010', 'Rua J, 1000 - Belo Horizonte', 3500.00, 1, NOW(), NOW(), NULL),
(gen_random_uuid(), 'Diego Alves', 'diego.alves@email.com', '(31) 99999-0011', 'Rua K, 1100 - Belo Horizonte', 5000.00, 1, NOW(), NOW(), NULL),
(gen_random_uuid(), 'Gabriela Souza', 'gabriela.souza@email.com', '(31) 99999-0012', 'Rua L, 1200 - Belo Horizonte', 3200.00, 1, NOW(), NOW(), NULL),
(gen_random_uuid(), 'Felipe Nunes', 'felipe.nunes@email.com', '(31) 99999-0013', 'Rua M, 1300 - Belo Horizonte', 4800.00, 1, NOW(), NOW(), NULL),
(gen_random_uuid(), 'Isabela Pereira', 'isabela.pereira@email.com', '(31) 99999-0014', 'Rua N, 1400 - Belo Horizonte', 2800.00, 1, NOW(), NOW(), NULL),
(gen_random_uuid(), 'Lucas Barbosa', 'lucas.barbosa@email.com', '(31) 99999-0015', 'Rua O, 1500 - Belo Horizonte', 5200.00, 3, NOW(), NOW(), NULL),
(gen_random_uuid(), 'Mariana Castro', 'mariana.castro@email.com', '(31) 99999-0016', 'Rua P, 1600 - Belo Horizonte', 3800.00, 1, NOW(), NOW(), NULL),
(gen_random_uuid(), 'Rodrigo Mendes', 'rodrigo.mendes@email.com', '(31) 99999-0017', 'Rua Q, 1700 - Belo Horizonte', 4200.00, 1, NOW(), NOW(), NULL),
(gen_random_uuid(), 'Patrícia Ribeiro', 'patricia.ribeiro@email.com', '(31) 99999-0018', 'Rua R, 1800 - Belo Horizonte', 2900.00, 2, NOW(), NOW(), NULL),
(gen_random_uuid(), 'Gustavo Teixeira', 'gustavo.teixeira@email.com', '(31) 99999-0019', 'Rua S, 1900 - Belo Horizonte', 5400.00, 1, NOW(), NOW(), NULL),
(gen_random_uuid(), 'Beatriz Morais', 'beatriz.morais@email.com', '(31) 99999-0020', 'Rua T, 2000 - Belo Horizonte', 3100.00, 1, NOW(), NOW(), NULL),
(gen_random_uuid(), 'André Campos', 'andre.campos@email.com', '(31) 99999-0021', 'Avenida A, 100 - Contagem', 4700.00, 1, NOW(), NOW(), NULL),
(gen_random_uuid(), 'Larissa Soares', 'larissa.soares@email.com', '(31) 99999-0022', 'Avenida B, 200 - Contagem', 3400.00, 1, NOW(), NOW(), NULL),
(gen_random_uuid(), 'Thiago Viana', 'thiago.viana@email.com', '(31) 99999-0023', 'Avenida C, 300 - Contagem', 4900.00, 1, NOW(), NOW(), NULL),
(gen_random_uuid(), 'Vanessa Leite', 'vanessa.leite@email.com', '(31) 99999-0024', 'Avenida D, 400 - Contagem', 2700.00, 1, NOW(), NOW(), NULL),
(gen_random_uuid(), 'Mateus Siqueira', 'mateus.siqueira@email.com', '(31) 99999-0025', 'Avenida E, 500 - Contagem', 5600.00, 1, NOW(), NOW(), NULL),
(gen_random_uuid(), 'Adriana Moura', 'adriana.moura@email.com', '(31) 99999-0026', 'Avenida F, 600 - Contagem', 3600.00, 2, NOW(), NOW(), NULL),
(gen_random_uuid(), 'Vitor Azevedo', 'vitor.azevedo@email.com', '(31) 99999-0027', 'Avenida G, 700 - Contagem', 4100.00, 1, NOW(), NOW(), NULL),
(gen_random_uuid(), 'Simone Ávila', 'simone.avila@email.com', '(31) 99999-0028', 'Avenida H, 800 - Contagem', 3300.00, 1, NOW(), NOW(), NULL),
(gen_random_uuid(), 'Alexandre Veiga', 'alexandre.veiga@email.com', '(31) 99999-0029', 'Avenida I, 900 - Contagem', 5100.00, 1, NOW(), NOW(), NULL),
(gen_random_uuid(), 'Priscila Tavares', 'priscila.tavares@email.com', '(31) 99999-0030', 'Avenida J, 1000 - Contagem', 2600.00, 1, NOW(), NOW(), NULL),
(gen_random_uuid(), 'Henrique Vieira', 'henrique.vieira@email.com', '(31) 99999-0031', 'Avenida K, 1100 - Contagem', 4600.00, 1, NOW(), NOW(), NULL),
(gen_random_uuid(), 'Carolina Bento', 'carolina.bento@email.com', '(31) 99999-0032', 'Avenida L, 1200 - Contagem', 3700.00, 1, NOW(), NOW(), NULL),
(gen_random_uuid(), 'Emerson Duarte', 'emerson.duarte@email.com', '(31) 99999-0033', 'Avenida M, 1300 - Contagem', 5300.00, 1, NOW(), NOW(), NULL),
(gen_random_uuid(), 'Fernanda Brás', 'fernanda.bras@email.com', '(31) 99999-0034', 'Avenida N, 1400 - Contagem', 2400.00, 3, NOW(), NOW(), NULL),
(gen_random_uuid(), 'Guilherme Reis', 'guilherme.reis@email.com', '(31) 99999-0035', 'Avenida O, 1500 - Contagem', 4400.00, 1, NOW(), NOW(), NULL),
(gen_random_uuid(), 'Marcela Gama', 'marcela.gama@email.com', '(31) 99999-0036', 'Avenida P, 1600 - Contagem', 3900.00, 1, NOW(), NOW(), NULL),
(gen_random_uuid(), 'Rafael Pires', 'rafael.pires@email.com', '(31) 99999-0037', 'Avenida Q, 1700 - Contagem', 5000.00, 2, NOW(), NOW(), NULL),
(gen_random_uuid(), 'Soraya Machado', 'soraya.machado@email.com', '(31) 99999-0038', 'Avenida R, 1800 - Contagem', 3050.00, 1, NOW(), NOW(), NULL),
(gen_random_uuid(), 'Tiago Robles', 'tiago.robles@email.com', '(31) 99999-0039', 'Avenida S, 1900 - Contagem', 4300.00, 1, NOW(), NOW(), NULL),
(gen_random_uuid(), 'Veridiana Melo', 'veridiana.melo@email.com', '(31) 99999-0040', 'Avenida T, 2000 - Contagem', 3650.00, 1, NOW(), NOW(), NULL),
(gen_random_uuid(), 'Wagner Ponte', 'wagner.ponte@email.com', '(31) 99999-0041', 'Rua Alfa, 100 - Osasco', 5700.00, 1, NOW(), NOW(), NULL),
(gen_random_uuid(), 'Ximena Ponte', 'ximena.ponte@email.com', '(31) 99999-0042', 'Rua Beta, 200 - Osasco', 2300.00, 1, NOW(), NOW(), NULL),
(gen_random_uuid(), 'Yasmin Pouso', 'yasmin.pouso@email.com', '(31) 99999-0043', 'Rua Gama, 300 - Osasco', 4050.00, 1, NOW(), NOW(), NULL),
(gen_random_uuid(), 'Zélia Ramires', 'zelia.ramires@email.com', '(31) 99999-0044', 'Rua Delta, 400 - Osasco', 3750.00, 1, NOW(), NOW(), NULL),
(gen_random_uuid(), 'Alain Ribas', 'alain.ribas@email.com', '(31) 99999-0045', 'Rua Epsilon, 500 - Osasco', 5450.00, 2, NOW(), NOW(), NULL),
(gen_random_uuid(), 'Bruna Riba', 'bruna.riba@email.com', '(31) 99999-0046', 'Rua Zeta, 600 - Osasco', 2950.00, 1, NOW(), NOW(), NULL),
(gen_random_uuid(), 'Cláudio Romero', 'claudio.romero@email.com', '(31) 99999-0047', 'Rua Eta, 700 - Osasco', 4650.00, 1, NOW(), NOW(), NULL),
(gen_random_uuid(), 'Denise Rotta', 'denise.rotta@email.com', '(31) 99999-0048', 'Rua Theta, 800 - Osasco', 3450.00, 1, NOW(), NOW(), NULL),
(gen_random_uuid(), 'Edson Rubim', 'edson.rubim@email.com', '(31) 99999-0049', 'Rua Iota, 900 - Osasco', 5250.00, 3, NOW(), NOW(), NULL),
(gen_random_uuid(), 'Emília Rupolo', 'emilia.rupolo@email.com', '(31) 99999-0050', 'Rua Kappa, 1000 - Osasco', 2750.00, 1, NOW(), NOW(), NULL);

-- ============================================
-- 2. INSERT VENDAS (150)
-- Gerando 3 vendas por cliente (50 x 3 = 150)
-- ============================================

WITH cliente_ids AS (
    SELECT id FROM "Clientes" ORDER BY id LIMIT 50
),
vendas_data AS (
    SELECT 
        gen_random_uuid() as id,
        c.id as cliente_id,
        (NOW() - INTERVAL '1 day' * floor(random() * 60)::integer) as data_venda,
        round((100 + random() * 9900)::numeric, 2) as valor_total,
        (floor(random() * 2) + 1)::integer as tipo_venda,
        'Venda de mercadorias diversas' as descricao,
        (floor(random() * 4) + 1)::integer as status_pagamento,
        round((random() * 100)::numeric, 2) as percentual_risco,
        (NOW() - INTERVAL '1 day' * floor(random() * 60)::integer) as criado_em,
        (NOW() - INTERVAL '1 day' * floor(random() * 60)::integer) as atualizado_em
    FROM cliente_ids c
    CROSS JOIN generate_series(1, 3)
)
INSERT INTO "Vendas" ("Id", "ClienteId", "DataVenda", "ValorTotal", "TipoVenda", "Descricao", "StatusPagamento", "PercentualRisco", "CriadoEm", "AtualizadoEm", "DeletadoEm")
SELECT id, cliente_id, data_venda, valor_total, tipo_venda, descricao, status_pagamento, percentual_risco, criado_em, atualizado_em, NULL
FROM vendas_data;

-- ============================================
-- 3. INSERT PAGAMENTOS (100)
-- Associando pagamentos a vendas Fiado (TipoVenda = 2)
-- ============================================

WITH vendas_fiado AS (
    SELECT id, "ClienteId", "ValorTotal", "DataVenda"
    FROM "Vendas" 
    WHERE "TipoVenda" = 2 
    ORDER BY RANDOM()
    LIMIT 100
)
INSERT INTO "Pagamentos" ("Id", "VendaId", "ClienteId", "DataPagamento", "ValorPago", "MetodoPagamento", "ComprovanteArquivo", "Observacoes", "CriadoEm", "DeletadoEm")
SELECT 
    gen_random_uuid(),
    v.id,
    v."ClienteId",
    v."DataVenda" + INTERVAL '1 day' * floor(random() * 15)::integer,
    round((v."ValorTotal" * (0.5 + random() * 0.5))::numeric, 2),
    (floor(random() * 4) + 1)::integer,
    CONCAT('comprovante_', substring(gen_random_uuid()::text, 1, 8), '.pdf'),
    'Pagamento registrado',
    NOW() - INTERVAL '1 day' * floor(random() * 15)::integer,
    NULL
FROM vendas_fiado v;

-- ============================================
-- 4. INSERT RELATÓRIOS DIÁRIOS (30)
-- Um relatório para cada um dos últimos 30 dias
-- ============================================

WITH datas AS (
    SELECT generate_series(0, 29) as dias_atras
),
relatorios_base AS (
    SELECT 
        gen_random_uuid() as id,
        (NOW() - INTERVAL '1 day' * d.dias_atras)::date as data_relatorio,
        round((5000 + random() * 15000)::numeric, 2) as total_recebimentos,
        round((3000 + random() * 10000)::numeric, 2) as total_a_receber,
        (floor(random() * 8) + 2)::integer as total_inadimplentes,
        round((2.5 + random() * 7.5)::numeric, 2) as taxa_padrao_percentual,
        round((15 + random() * 35)::numeric, 2) as indice_concentracao_percentual,
        NOW() - INTERVAL '1 day' * d.dias_atras as criado_em
    FROM datas d
)
INSERT INTO "RelatoriosDiarios" ("Id", "DataRelatorio", "TotalRecebimentos", "TotalAReceber", "TotalInadimplentes", "TaxaPadraoPercentual", "IndiceConcentracaoPercentual", "CriadoEm")
SELECT id, data_relatorio, total_recebimentos, total_a_receber, total_inadimplentes, taxa_padrao_percentual, indice_concentracao_percentual, criado_em
FROM relatorios_base;

