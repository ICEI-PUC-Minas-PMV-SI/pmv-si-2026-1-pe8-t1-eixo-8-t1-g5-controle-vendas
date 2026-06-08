# Sistema de Informação para Gestão e Controle de Vendas com Inteligência Competitiva

**Projeto de Estudo de Caso e Implementação de Sistema de Informação para o Comércio de Roupas da Sra. Maria Aparecida Santos de Andrade**

`CURSO: Bacharelado em Sistemas de Informação`

`INSTITUIÇÃO: Pontifícia Universidade Católica de Minas Gerais (PUC Minas)`

`DISCIPLINA: Projeto - Trabalho de Conclusão de Curso (8º Período - Eixo 8)`

`ANO: 2026`

---

## 📋 Descrição do Projeto

Este projeto implementa um **sistema de informação web integrado** para modernizar e digitalizar as operações de um micro empreendimento de comércio de vestuário feminino em Belo Horizonte, MG. 

A solução foi desenvolvida com foco em três pilares fundamentais:

1. **Gestão do Crédito Informal ("Fiado")**: Digitalização e estruturação do controle de vendas parceladas, permitindo rastreabilidade completa de inadimplência e fluxo de caixa.
2. **Inteligência Competitiva**: Geração de KPIs e indicadores de desempenho para apoiar decisões estratégicas relacionadas à precificação, compras e fidelização.
3. **Conformidade e Segurança**: Implementação de práticas alinhadas à Lei Geral de Proteção de Dados Pessoais (LGPD) e ISO/IEC 27001.

O sistema transforma dados brutos coletados manualmente em registros digitais estruturados, permitindo análise, monitoramento e previsão de receita com segurança e confiabilidade.

---

## 👥 Integrantes

* **André Calebe Santos de Andrade**
* **Fábio Rezende Dias Silva**
* **Gabriel Amorim de Almeida Custódio**
* **Giselle Lucy Souza Coelho**
* **Henrique Avelar Amaral**
* **Pedro Mário Xavier Guimarães**

---

## 👨‍🏫 Orientador

* **Prof. Simone Fernandes Queiroz** - PUC Minas

---

## 📊 Contexto da Empresa

### Apresentação

A empresa objeto deste estudo é um microempreendimento informal de comercialização de roupas femininas, operado exclusivamente pela proprietária (Maria Aparecida). O negócio está localizado em Belo Horizonte, bairro Liberdade, e atende predominantemente uma clientela local composta por vizinhos, amigos e conhecidos.

**Características operacionais:**
- Negócio domiciliar sem estrutura física formal
- Estoque armazenado em cômodos residenciais
- Negociações presenciais ou via WhatsApp
- Caráter informal (sem registro como MEI)
- Gestão centralizada pela proprietária

### Problemas Identificados

| Problema | Impacto |
|----------|--------|
| Controle manual em caderno físico | Risco de perda, ilegibilidade, falta de estrutura |
| Ausência de registro de estoque | Impossibilidade de análise de giro e demanda |
| Dificuldade de controlar inadimplência | Risco de perdas financeiras |
| Falta de relatórios consolidados | Impossibilidade de análise de lucratividade |
| Registro desorganizado do "fiado" | Dificuldade em cobranças e fluxo de caixa |

### Matriz SWOT

**Forças:**
- Baixo custo operacional (estrutura domiciliar)
- Relacionamento pessoal com clientes

**Fraquezas:**
- Ausência de sistema de informação
- Sem controle de estoque ou emissão de comprovantes
- Risco de perdas por falta de controle do fiado

**Oportunidades:**
- Adoção de sistema digital para modernização
- Formação de banco de clientes para marketing
- Ferramentas de baixo custo (apps, planilhas)

**Ameaças:**
- Concorrência de plataformas digitais (Shein, Shopee)
- Inadimplência de clientes que compram fiado
- Sazonalidade de moda

---

## 🎯 Objetivo do Sistema

O sistema objetiva:

✅ Digitalizar o registo de vendas e pagamentos  
✅ Estruturar o controle de crédito informal (fiado)  
✅ Reduzir risco de inadimplência por meio de monitoramento contínuo  
✅ Gerar inteligência competitiva através de KPIs e relatórios  
✅ Apoiar decisões estratégicas de precificação, compras e fidelização  
✅ Garantir conformidade com LGPD e boas práticas de segurança  

---

## 🏗️ Stack Tecnológico

### Backend
- **Linguagem**: C# / .NET 10.0
- **Framework**: ASP.NET Core Web API
- **ORM**: Entity Framework Core
- **Autenticação**: JWT Bearer Tokens
- **Validação**: Fluent Validation

### Frontend
- **Biblioteca**: React 19+
- **Linguagem**: TypeScript
- **Build Tool**: Vite
- **Estilos**: Tailwind CSS + PostCSS
- **Roteamento**: React Router DOM
- **HTTP Client**: Axios
- **Gráficos**: Recharts

### Banco de Dados
- **SGBD**: MariaDB 10.6+
- **Versionamento**: Entity Framework Core Migrations
- **Backup**: Scripts SQL nativos

### Infraestrutura
- **Implantação**: On-Premisse (local)
- **API Port**: 5000
- **Frontend Port**: 5173
- **Segurança**: HTTPS, CORS configurado

---

## 📁 Estrutura do Projeto

```
.
├── code/
│   ├── backend/                           # Backend .NET Core
│   │   ├── MariaAparecida.Retail.sln      # Solution do Visual Studio
│   │   ├── MariaAparecida.Retail.Api/     # Web API Principal
│   │   │   ├── Controllers/               # Endpoints REST
│   │   │   ├── Program.cs                 # Configuração da aplicação
│   │   │   └── appsettings.json           # Config de ambiente
│   │   ├── MariaAparecida.Retail.Application/
│   │   │   ├── DTOs/                      # Data Transfer Objects
│   │   │   ├── Services/                  # Lógica de negócio
│   │   │   └── Validators/                # Validações FluentValidation
│   │   ├── MariaAparecida.Retail.Domain/
│   │   │   ├── Entities/                  # Modelos de domínio
│   │   │   ├── Enums/                     # Enumerações
│   │   │   └── Abstractions/              # Interfaces
│   │   ├── MariaAparecida.Retail.Persistence/
│   │   │   ├── Data/                      # DbContext
│   │   │   ├── Repositories/              # Padrão Repository
│   │   │   └── Migrations/                # Versionamento BD
│   │   └── MariaAparecida.Retail.Tests/   # Testes Unitários
│   │
│   └── frontend/                          # Frontend React + TypeScript
│       ├── src/
│       │   ├── components/                # Componentes React
│       │   ├── pages/                     # Páginas da aplicação
│       │   ├── services/                  # Chamadas à API
│       │   ├── hooks/                     # Hooks customizados
│       │   ├── context/                   # Context API
│       │   ├── types/                     # TypeScript types
│       │   ├── utils/                     # Funções utilitárias
│       │   ├── App.tsx                    # Componente raiz
│       │   └── main.tsx                   # Ponto de entrada
│       ├── index.html
│       ├── package.json
│       ├── vite.config.ts
│       └── tailwind.config.js
│
├── docs/                                  # Documentação TCC
┗── README.md                              # Este arquivo
```

---

## 🚀 Como Instalar e Executar

### Pré-requisitos

- **.NET SDK 10.0+** - [Download](https://dotnet.microsoft.com/download)
- **Node.js 20+ e npm/yarn** - [Download](https://nodejs.org)
- **MariaDB 10.6+** - [Download](https://mariadb.org/download)
- **Visual Studio Code** ou **Visual Studio 2022+** (recomendado)

### Backend Setup

#### 1. Clonar o repositório

```bash
git clone https://github.com/seu-usuario/pmv-si-2026-1-pe8-t1-eixo-8-t1-g5-controle-vendas.git
cd code/backend
```

#### 2. Restaurar dependências NuGet

```bash
dotnet restore
```

#### 3. Configurar conexão com MariaDB

Editar `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=mariaparecida_retail;User=root;Password=sua_senha;"
  }
}
```

#### 4. Executar migrations

```bash
cd MariaAparecida.Retail.Api
dotnet ef database update --project ../MariaAparecida.Retail.Persistence
```

#### 5. Iniciar o servidor

```bash
dotnet run --project MariaAparecida.Retail.Api
```

A API estará disponível em: `http://localhost:5000`  
Swagger UI: `http://localhost:5000/swagger`

### Frontend Setup

#### 1. Navegar para o diretório frontend

```bash
cd code/frontend
```

#### 2. Instalar dependências

```bash
npm install
# ou
yarn install
```

#### 3. Criar arquivo de environment (opcional)

```bash
cp .env.example .env.local
```

**Conteúdo do `.env.local`:**
```
VITE_API_URL=http://localhost:5000
```

#### 4. Iniciar o servidor de desenvolvimento

```bash
npm run dev
# ou
yarn dev
```

A aplicação estará disponível em: `http://localhost:5173`

### Testes

#### Backend - Rodar testes unitários

```bash
cd code/backend
dotnet test
```

#### Frontend - Rodar testes

```bash
cd code/frontend
npm run test
# ou para watch mode
npm run test -- --watch
```

---

## 📱 Funcionalidades Principais

### 1. **Gestão de Clientes**
- ✅ Cadastro completo de clientes (nome, telefone, endereço)
- ✅ Consulta instantânea de saldo devedor
- ✅ Histórico de compras por cliente
- ✅ Alertas para clientes em atraso

### 2. **Registro de Vendas**
- ✅ Venda à vista ou a prazo (fiado)
- ✅ Limite de crédito configurável por cliente
- ✅ Validação automática antes de autorizar venda a prazo
- ✅ Histórico completo de transações

### 3. **Controle de Fiado**
- ✅ Registro de pagamentos parciais e totais
- ✅ Cálculo automático de saldo devedor
- ✅ Timeline de pagamentos por cliente
- ✅ Alertas de inadimplência

### 4. **Dashboard e KPIs**
- ✅ Total de recebíveis em aberto
- ✅ Taxa de inadimplência mensal
- ✅ Prazo médio de recebimento
- ✅ Receita por período
- ✅ Ticket médio de vendas
- ✅ Índice de concentração de crédito

### 5. **Relatórios**
- ✅ Contas a receber por cliente e período
- ✅ Receita consolidada
- ✅ Análise de inadimplência
- ✅ Exportação em CSV e PDF

### 6. **Segurança e Compliance**
- ✅ Autenticação JWT com 2FA
- ✅ Criptografia de dados em repouso
- ✅ Logs de auditoria completos
- ✅ Conformidade LGPD
- ✅ Backup e restauração de dados

---

## 🔐 Segurança e Compliance

### LGPD (Lei Geral de Proteção de Dados)

O sistema implementa as seguintes práticas:

- **Minimização de dados**: Coleta apenas de informações essenciais
- **Consentimento informado**: Identificação clara da finalidade de uso dos dados
- **Controle de acesso**: Autenticação obrigatória e baseada em papéis
- **Direitos dos titulares**: Funcionalidade para acesso, correção e exclusão de dados
- **Retenção**: Dados mantidos por 5 anos conforme regulamentação

### Segurança da Informação

- 🔒 Senha com hash bcrypt/Argon2
- 🔐 Comunicação via HTTPS/TLS
- 🗝️ JWT Tokens para autenticação stateless
- 🛡️ CORS e Content Security Policy configurados
- 📋 Auditoria e logging de todas as operações críticas
- 💾 Backup criptografado do banco de dados

---

## 📊 Inteligência Competitiva (IC)

O projeto implementa um **Plano de Inteligência Competitiva** estruturado com:

### Key Intelligence Topic (KIT)
**Controle, Monitoramento e Mitigação de Riscos do Crédito Informal (Fiado)**

### Key Intelligence Questions (KIQs)

1. **KIQ1**: Volume e proporção do fiado em relação ao faturamento
2. **KIQ2**: Perfil de inadimplência e comportamento de pagamento
3. **KIQ3**: Prazo médio de recebimento
4. **KIQ4**: Concentração de risco de crédito
5. **KIQ5**: Correlação entre valor da venda e inadimplência

### KPIs Monitorados

| KPI | Frequência | Fórmula |
|-----|-----------|--------|
| Taxa de Inadimplência | Mensal | (Clientes com fiado vencido / Total com fiado) × 100 |
| Volume de Recebíveis | Semanal | Σ Saldos devedores em aberto |
| Prazo Médio Recebimento | Mensal | Média de dias entre venda a prazo e quitação |
| Ticket Médio | Mensal | Receita total / Número de vendas |
| Receita Total | Mensal | Σ Valores recebidos |
| Índice Concentração | Mensal | (3 maiores devedores / Total em aberto) × 100 |

---

## 🧪 Testes

O projeto inclui testes abrangentes:

### Backend
- Testes unitários de serviços
- Testes de integração de controllers
- Testes de validações
- Testes de performance

Executar:
```bash
dotnet test
```

### Frontend
- Testes de componentes React
- Testes de integração com API
- Testes de hooks

Executar:
```bash
npm run test
```

---

## 📚 Documentação Adicional

- **TCC Completo**: Veja `/docs/TCC DOC.docx`
- **BPMN do Processo**: Documentado no TCC
- **Diagrama ER**: Estrutura do banco de dados
- **Diagrama de Arquitetura**: Veja `3.3.1` do documento TCC

---

## 🤝 Padrões e Boas Práticas

O projeto segue:

- **Clean Code**: Nomenclatura clara, métodos pequenos e focados
- **SOLID Principles**: Separação de responsabilidades, inversão de controle
- **RESTful API**: Endpoints seguindo convenções REST
- **Git Flow**: Commits semânticos e ramificação organizada
- **Error Handling**: Tratamento estruturado de exceções
- **.gitignore**: Exclusão de arquivos desnecessários

---

## 🛠️ Ambiente de Desenvolvimento

### Visual Studio Code - Extensões Recomendadas

Para **backend (.NET)**:
- C# Dev Kit
- REST Client
- C# Extensions

Para **frontend (React)**:
- ES7+ React/Redux/React-Native snippets
- Tailwind CSS IntelliSense
- TypeScript Vue Plugin (Volar)

### Linting e Formatting

```bash
# Frontend - ESLint
npm run lint

# Frontend - Prettier (formatação)
npm run format
```

---

## 📞 Suporte e Contato

Para dúvidas ou relatos de bugs:
1. Abra uma **issue** no repositório
2. Descreva o problema com detalhes
3. Inclua prints ou logs quando possível

---

## 📄 Licença

Este projeto é desenvolvido como **Trabalho de Conclusão de Curso** da PUC Minas e segue as diretrizes da instituição.

---

## 📖 Referências

- **LGPD**: Lei Geral de Proteção de Dados Pessoais. Lei nº 13.709/2018. Brasil.
- **ABNT NBR ISO/IEC 27001:2022** - Segurança da Informação
- **Código de Defesa do Consumidor** - Lei nº 8.078/1990
- **Microsoft .NET Documentation**: https://learn.microsoft.com/dotnet
- **React Documentation**: https://react.dev
- **Recharts**: https://recharts.org

---

**Desenvolvido com ❤️ para o comércio de Maria Aparecida Santos de Andrade**

Belo Horizonte, MG — 2026
