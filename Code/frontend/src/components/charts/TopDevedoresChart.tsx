import {
  PieChart,
  Pie,
  Cell,
  Legend,
  Tooltip,
  ResponsiveContainer,
} from 'recharts'

interface TopDevedoresChartProps {
  data: Array<{
    nome: string
    valor: number
  }>
  height?: number
}

const COLORS = ['#3b82f6', '#ef4444', '#f59e0b', '#10b981', '#8b5cf6']

export function TopDevedoresChart({
  data,
  height = 300,
}: TopDevedoresChartProps) {
  if (!data || data.length === 0) {
    return (
      <div className="flex items-center justify-center h-80 bg-gray-50 rounded-lg border border-dashed border-gray-300">
        <p className="text-gray-500">Sem dados para exibir</p>
      </div>
    )
  }

  // Limita a top 5
  const chartData = data.slice(0, 5)
  const total = chartData.reduce((sum, item) => sum + item.valor, 0)

  const renderLabel = (entry: any) => {
    const percent = ((entry.valor / total) * 100).toFixed(0)
    return `${entry.nome} (${percent}%)`
  }

  return (
    <ResponsiveContainer width="100%" height={height}>
      <PieChart margin={{ top: 20, right: 20, left: 20, bottom: 20 }}>
        <Pie
          data={chartData}
          cx="50%"
          cy="50%"
          labelLine={false}
          label={renderLabel}
          outerRadius={80}
          fill="#8884d8"
          dataKey="valor"
        >
          {chartData.map((_, index) => (
            <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
          ))}
        </Pie>
        <Tooltip
          formatter={(value: any) =>
            `R$ ${Number(value).toLocaleString('pt-BR', {
              minimumFractionDigits: 2,
              maximumFractionDigits: 2,
            })}`
          }
        />
        <Legend />
      </PieChart>
    </ResponsiveContainer>
  )
}

export default TopDevedoresChart
