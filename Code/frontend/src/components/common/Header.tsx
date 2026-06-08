import { useState } from 'react'
import { useAuth } from '../../hooks'
import { useNavigate } from 'react-router-dom'

export function Header() {
  const { user, logout } = useAuth()
  const navigate = useNavigate()
  const [showUserMenu, setShowUserMenu] = useState(false)

  const handleLogout = async () => {
    await logout()
    navigate('/login', { replace: true })
  }

  return (
    <header className="bg-white shadow border-b border-gray-200">
      <div className="px-6 py-4 flex items-center justify-between">
        {/* Logo/Titulo */}
        <div className="flex items-center space-x-3">
          <div className="text-2xl font-bold text-blue-600">MA</div>
          <div>
            <h1 className="text-lg font-semibold text-gray-900">
              Maria Aparecida
            </h1>
            <p className="text-xs text-gray-600">Gestão de Vendas</p>
          </div>
        </div>

        {/* Menu do usuário */}
        <div className="relative">
          <button
            onClick={() => setShowUserMenu(!showUserMenu)}
            className="flex items-center space-x-3 px-4 py-2 rounded-lg hover:bg-gray-100 transition"
          >
            <div className="text-right">
              <p className="text-sm font-medium text-gray-900">
                {user?.nomeCompleto || 'Usuário'}
              </p>
              <p className="text-xs text-gray-600">{user?.email}</p>
            </div>
            <div className="w-10 h-10 rounded-full bg-blue-500 text-white flex items-center justify-center font-bold">
              {(user?.nomeCompleto?.[0] || 'U').toUpperCase()}
            </div>
          </button>

          {/* Dropdown menu */}
          {showUserMenu && (
            <div className="absolute right-0 mt-2 w-48 bg-white rounded-lg shadow-lg border border-gray-200 py-2 z-50">
              <button
                onClick={handleLogout}
                className="w-full text-left px-4 py-2 text-red-600 hover:bg-red-50 transition text-sm font-medium"
              >
                Sair
              </button>
            </div>
          )}
        </div>
      </div>
    </header>
  )
}

export default Header
