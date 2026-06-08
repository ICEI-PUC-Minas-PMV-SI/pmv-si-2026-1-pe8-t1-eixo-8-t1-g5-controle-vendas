import React, { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAuth } from '../../hooks'

export function Login() {
  const navigate = useNavigate()
  const { login, error, clearError } = useAuth()

  const [formData, setFormData] = useState({
    email: '',
    senha: '',
  })

  const [localError, setLocalError] = useState<string | null>(null)

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }))
    // Limpar erro ao digitar
    if (localError) setLocalError(null)
    if (error) clearError()
  }

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault()

    // Validação básica
    if (!formData.email.trim() || !formData.senha.trim()) {
      setLocalError('Preencha todos os campos')
      return
    }

    if (!formData.email.includes('@')) {
      setLocalError('Email inválido')
      return
    }

    try {
      console.log('Tentando fazer login com:', formData)
      console.log('[Login] Chamando useAuth().login()...')
      await login(formData.email, formData.senha)
      // Redirecionar para dashboard ao fazer login com sucesso
      console.log('Login bem-sucedido, redirecionando para dashboard...')
      navigate('/dashboard', { replace: true })
    } catch (err: any) {
      console.error('[Login] Erro ao fazer login:', err)
      setLocalError(err?.message || 'Erro ao fazer login')
    }
  }

  const displayError = localError || error

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 flex items-center justify-center px-4 py-12">
      <div className="flex-col items-center justify-center">
        {/* Card de login */}
        <div className="bg-white rounded-lg shadow-lg p-8 space-y-6 max-w-6xl">
          {/* Header */}
          <div className="text-center">
            <h1 className="text-3xl font-bold text-gray-900">
              Maria Aparecida
            </h1>
            <p className="text-gray-600 mt-2">
              Sistema de Gestão de Vendas
            </p>
          </div>

          {/* Erro */}
          {displayError && (
            <div className="bg-red-50 border border-red-200 rounded-md p-4">
              <p className="text-red-700 text-sm font-medium">
                {displayError}
              </p>
            </div>
          )}

          {/* Formulário */}
          <form onSubmit={handleSubmit} className="space-y-4">
            {/* Email */}
            <div>
              <label htmlFor="email" className="block text-sm font-medium text-gray-700 mb-2">
                Email
              </label>
              <input
                type="email"
                id="email"
                name="email"
                value={formData.email}
                onChange={handleChange}
                className="input-field"
                placeholder="seu.email@example.com"
              />
            </div>

            {/* Senha */}
            <div>
              <label htmlFor="senha" className="block text-sm font-medium text-gray-700 mb-2">
                Senha
              </label>
              <input
                type="password"
                id="senha"
                name="senha"
                value={formData.senha}
                onChange={handleChange}
                className="input-field"
                placeholder="••••••••"
              />
            </div>

            {/* Botão de login */}
            <button
              type="submit"
              className="w-full btn-primary py-3 font-semibold text-white"
            >Entrar
            </button>
          </form>

          {/* Dica de credenciais (desenvolvimento) */}
          {import.meta.env.DEV && (
            <div className="bg-blue-50 border border-blue-200 rounded-md p-3 text-xs text-blue-700">
              <p className="font-semibold mb-1">Teste (desenvolvimento):</p>
              <p>Email: maria@aparecida.com</p>
              <p>Senha: teste123</p>
            </div>
          )}
        </div>
      </div>
    </div>
  )
}

export default Login
