import { useState } from 'react'
import { Link, useLocation } from 'react-router-dom'

interface NavItem {
  label: string
  href: string
  icon: string
}

const navItems: NavItem[] = [
  { label: 'Dashboard', href: '/dashboard', icon: '📊' },
  { label: 'Clientes', href: '/clientes', icon: '👥' },
  { label: 'Vendas', href: '/vendas', icon: '🛍️' },
  { label: 'Fiado', href: '/fiado', icon: '📝' },
  { label: 'Relatórios', href: '/relatorios', icon: '📈' },
]

export function Sidebar() {
  const location = useLocation()
  const [isOpen, setIsOpen] = useState(true)

  const isActive = (href: string) =>
    location.pathname === href || location.pathname.startsWith(`${href}/`)

  return (
    <aside
      className={`${
        isOpen ? 'w-64' : 'w-20'
      } bg-gray-900 text-white transition-all duration-300 flex flex-col`}
    >
      {/* Botão toggle */}
      <div className="p-4 flex justify-end">
        <button
          onClick={() => setIsOpen(!isOpen)}
          className="p-2 rounded-lg hover:bg-gray-800 transition"
          title={isOpen ? 'Fechar menu' : 'Abrir menu'}
        >
          {isOpen ? '←' : '→'}
        </button>
      </div>

      {/* Navegação */}
      <nav className="flex-1 px-4 space-y-2">
        {navItems.map((item) => (
          <Link
            key={item.href}
            to={item.href}
            className={`flex items-center space-x-3 px-4 py-3 rounded-lg transition ${
              isActive(item.href)
                ? 'bg-blue-600 text-white'
                : 'text-gray-300 hover:bg-gray-800'
            }`}
            title={!isOpen ? item.label : undefined}
          >
            <span className="text-xl">{item.icon}</span>
            {isOpen && <span className="font-medium">{item.label}</span>}
          </Link>
        ))}
      </nav>

      {/* Rodapé */}
      <div className="p-4 border-t border-gray-700">
        <p className="text-xs text-gray-400 text-center">
          {isOpen ? 'v1.0.0' : 'v1'}
        </p>
      </div>
    </aside>
  )
}

export default Sidebar
