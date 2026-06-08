import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  ResponsiveContainer,
} from 'recharts'

interface ReceitaMensalChartProps {
  data: Array<{
    mes: string
    receita: number
  }>
  height?: number
}

export function ReceitaMensalChart({
  data,
  height = 300,
}: ReceitaMensalChartProps) {
  if (!data || data.length === 0) {
    return (
      <div className="flex items-center justify-center h-80 bg-gray-50 rounded-lg border border-dashed border-gray-300">
        <p className="text-gray-500">Sem dados para exibir</p>
      </div>
    )
  }

  return (
    <ResponsiveContainer width="100%" height={height}>
      <BarChart
        data={data}
        margin={{
          top: 20,
          right: 30,
          left: 0,
          bottom: 5,
        }}
      >
        <CartesianGrid strokeDasharray="3 3" stroke="#e5e7eb" />
        <XAxis
          dataKey="mes"
          tick={{ fill: '#6b7280', fontSize: 12 }}
          axisLine={{ stroke: '#e5e7eb' }}
        />
        <YAxis
          tick={{ fill: '#6b7280', fontSize: 12 }}
          axisLine={{ stroke: '#e5e7eb' }}
          label={{
            value: 'R$',
            angle: -90,
            position: 'insideLeft',
            fill: '#6b7280',
          }}
        />
        <Tooltip
          contentStyle={{
            backgroundColor: '#fff',
            border: '1px solid #e5e7eb',
            borderRadius: '8px',
          }}
          formatter={(value: any) =>
            `R$ ${(value as number).toLocaleString('pt-BR', {
              minimumFractionDigits: 2,
              maximumFractionDigits: 2,
            })}`
          }
        />
        <Legend />
        <Bar
          dataKey="receita"
          fill="#3b82f6"
          radius={[8, 8, 0, 0]}
          name="Receita"
        />
      </BarChart>
    </ResponsiveContainer>
  )
}

export default ReceitaMensalChart
