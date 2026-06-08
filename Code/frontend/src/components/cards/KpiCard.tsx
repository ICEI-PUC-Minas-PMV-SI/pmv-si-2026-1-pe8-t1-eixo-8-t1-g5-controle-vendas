import React from 'react'

interface KpiCardProps {
  title: string
  value: number | string
  unit?: string // 'R$', '%', 'Dias', etc
  trend?: number // +5, -2, null
  trendLabel?: string // 'vs mês anterior', etc
  icon?: React.ReactNode
  bgColor?: 'blue' | 'green' | 'red' | 'orange' | 'purple'
  textColor?: string // tailwind class override
  onClick?: () => void
}

const colorMap = {
  blue: 'from-blue-50 to-blue-100',
  green: 'from-green-50 to-green-100',
  red: 'from-red-50 to-red-100',
  orange: 'from-orange-50 to-orange-100',
  purple: 'from-purple-50 to-purple-100',
}

const textColorMap = {
  blue: 'text-blue-700',
  green: 'text-green-700',
  red: 'text-red-700',
  orange: 'text-orange-700',
  purple: 'text-purple-700',
}

export function KpiCard({
  title,
  value,
  unit = '',
  trend,
  trendLabel,
  icon,
  bgColor = 'blue',
  textColor,
  onClick,
}: KpiCardProps) {
  const bgGradient = colorMap[bgColor]
  const valueColor = textColorMap[bgColor]
  const isTrendPositive = trend !== null && trend !== undefined && trend > 0
  const trendColor = isTrendPositive ? 'text-green-600' : 'text-red-600'

  const displayValue = typeof value === 'number' ? value.toLocaleString('pt-BR') : value

  return (
    <div
      onClick={onClick}
      className={`card bg-gradient-to-br ${bgGradient} hover:shadow-lg transition cursor-pointer ${
        onClick ? 'cursor-pointer' : 'cursor-default'
      }`}
    >
      {/* Header com ícone e título */}
      <div className="flex items-start justify-between mb-3">
        <div className="flex-1">
          <p className="text-sm font-medium text-gray-600">{title}</p>
        </div>
        {icon && <div className="text-2xl ml-2">{icon}</div>}
      </div>

      {/* Valor principal */}
      <div className="mb-2">
        <p className={`text-3xl font-bold ${textColor || valueColor}`}>
          {displayValue}
          {unit && <span className="text-lg ml-1 font-semibold">{unit}</span>}
        </p>
      </div>

      {/* Tendência */}
      {trend !== null && trend !== undefined && (
        <div className="flex items-center gap-2">
          <span className={`text-sm font-semibold ${trendColor}`}>
            {isTrendPositive ? '↑' : '↓'} {Math.abs(trend)}%
          </span>
          {trendLabel && <span className="text-xs text-gray-500">{trendLabel}</span>}
        </div>
      )}
    </div>
  )
}

export default KpiCard
