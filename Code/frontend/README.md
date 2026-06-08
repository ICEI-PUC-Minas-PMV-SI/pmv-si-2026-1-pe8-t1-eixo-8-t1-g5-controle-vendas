# Maria Aparecida - Frontend React

Frontend da aplicação de gestão de vendas para Maria Aparecida, desenvolvido em React 18 + TypeScript + Tailwind CSS.

## 🚀 Quick Start

### Pré-requisitos
- Node.js 18+
- npm ou yarn

### Instalação

```bash
# Instalar dependências
npm install

# Criar arquivo .env.local a partir do .env.example
cp .env.example .env.local
```

### Desenvolvimento

```bash
# Iniciar servidor de desenvolvimento
npm run dev

# O servidor abrirá em http://localhost:3000
```

### Build para produção

```bash
# Compilar para produção
npm run build

# Preview da build
npm run preview
```

### Testes

```bash
# Rodar testes unitários
npm run test

# Rodar testes com coverage
npm run test:coverage

# Rodar testes E2E
npm run e2e
```

## 📁 Estrutura do Projeto

```
frontend/
├── src/
│   ├── components/       # Componentes React reutilizáveis
│   ├── pages/           # Páginas da aplicação
│   ├── services/        # Serviços (API, storage, etc)
│   ├── hooks/           # Custom hooks
│   ├── context/         # Context API
│   ├── types/           # TypeScript types
│   ├── utils/           # Funções utilitárias
│   ├── styles/          # Estilos globais
│   ├── routes/          # Configuração de rotas
│   ├── App.tsx          # App root
│   └── index.tsx        # Entry point
├── public/              # Assets estáticos
├── tailwind.config.js   # Tailwind CSS config
├── vite.config.ts       # Vite config
└── package.json
```

## 🎨 Stack Tecnológico

- **React 18** - UI library
- **TypeScript** - Type safety
- **Tailwind CSS** - Utility-first CSS framework
- **React Router v6** - Client-side routing
- **Axios** - HTTP client com interceptors JWT
- **Recharts** - Charts library
- **Vite** - Build tool
- **Jest + React Testing Library** - Testing

## 🔐 Autenticação

A autenticação é feita via JWT (JSON Web Token):

1. Usuário faz login em `/login`
2. Backend retorna um JWT token
3. Token é armazenado em `localStorage`
4. Axios interceptor injeta o token em todas as requisições
5. PrivateRoute valida autenticação antes de renderizar páginas protegidas

### Credenciais de Teste (Development)

```
Email: maria@aparecida.com
Senha: teste123
```

## 🔄 Fluxo de Dados

```
Component
    ↓
useAuth Hook / useApi Hook
    ↓
axios-config (interceptor JWT)
    ↓
Backend API (.NET Core)
    ↓
Response Handler
    ↓
Context/State Update
    ↓
UI Re-render
```

## 📝 Fases de Implementação

### ✅ Fase 1: Foundation (Completa)
- Setup React + Vite + TypeScript + Tailwind
- Axios com interceptor JWT
- AuthContext + useAuth hook
- Login page
- PrivateRoute + routing
- Header + Sidebar
- Build funcionando

### 🔄 Fase 2: Core CRUD (Próxima)
- Listagem de Clientes
- Cadastro de Clientes
- Histórico de Vendas
- Registrar Pagamentos
- Testes de integração

### ⏳ Fase 3: Dashboard & Relatórios
- Dashboard com KPIs
- Gráficos (receita, devedores)
- Relatórios completos
- Exportação CSV/PDF

### ⏳ Fase 4: Polish & Optimization
- Performance optimization
- Testes E2E
- Deploy

## 📚 Documentação

- [Plano Completo](../silly-hugging-fairy.md)
- [API Documentation](../Documents/API_DOCUMENTATION.md)
- [Integration Guide](../Documents/CLIENT_INTEGRATION_GUIDE.md)

## 🛠️ Troubleshooting

### Erro de compilação TypeScript

```bash
# Limpar cache
rm -rf node_modules/.cache tsconfig.app.tsbuildinfo

# Reinstalar dependências
rm -rf node_modules package-lock.json
npm install
```

### Servidor de desenvolvimento não inicia

```bash
# Verificar se a porta 3000 está em uso
# Se estiver, vite usará a próxima porta disponível (3001, 3002, etc)

# Ou iniciar em porta específica
npm run dev -- --port 3000
```

### Erros de CORS

Se receber erros de CORS ao conectar na API:

1. Verificar se a API está rodando em `http://localhost:5000`
2. Verificar se CORS está configurado no backend para `http://localhost:3000`
3. Verificar `.env.local` tem `REACT_APP_API_URL` correto

## 📞 Suporte

Para dúvidas ou problemas, consulte a documentação do projeto ou o plano de implementação.

---

**Desenvolvido para:** PUC Minas - Trabalho de Conclusão de Curso  
**Projeto:** Maria Aparecida - Sistema de Gestão de Vendas  
**Data:** 2026-04-29

The React Compiler is not enabled on this template because of its impact on dev & build performances. To add it, see [this documentation](https://react.dev/learn/react-compiler/installation).

## Expanding the ESLint configuration

If you are developing a production application, we recommend updating the configuration to enable type-aware lint rules:

```js
export default defineConfig([
  globalIgnores(['dist']),
  {
    files: ['**/*.{ts,tsx}'],
    extends: [
      // Other configs...

      // Remove tseslint.configs.recommended and replace with this
      tseslint.configs.recommendedTypeChecked,
      // Alternatively, use this for stricter rules
      tseslint.configs.strictTypeChecked,
      // Optionally, add this for stylistic rules
      tseslint.configs.stylisticTypeChecked,

      // Other configs...
    ],
    languageOptions: {
      parserOptions: {
        project: ['./tsconfig.node.json', './tsconfig.app.json'],
        tsconfigRootDir: import.meta.dirname,
      },
      // other options...
    },
  },
])
```

You can also install [eslint-plugin-react-x](https://github.com/Rel1cx/eslint-react/tree/main/packages/plugins/eslint-plugin-react-x) and [eslint-plugin-react-dom](https://github.com/Rel1cx/eslint-react/tree/main/packages/plugins/eslint-plugin-react-dom) for React-specific lint rules:

```js
// eslint.config.js
import reactX from 'eslint-plugin-react-x'
import reactDom from 'eslint-plugin-react-dom'

export default defineConfig([
  globalIgnores(['dist']),
  {
    files: ['**/*.{ts,tsx}'],
    extends: [
      // Other configs...
      // Enable lint rules for React
      reactX.configs['recommended-typescript'],
      // Enable lint rules for React DOM
      reactDom.configs.recommended,
    ],
    languageOptions: {
      parserOptions: {
        project: ['./tsconfig.node.json', './tsconfig.app.json'],
        tsconfigRootDir: import.meta.dirname,
      },
      // other options...
    },
  },
])
```
