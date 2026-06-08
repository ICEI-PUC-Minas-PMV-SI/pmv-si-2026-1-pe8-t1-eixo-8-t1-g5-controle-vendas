import { useState, useCallback } from 'react'

interface UseApiState<T> {
  data: T | null
  isLoading: boolean
  error: string | null
}

interface UseApiResult<T> extends UseApiState<T> {
  execute: (fn: () => Promise<T>) => Promise<T | null>
  reset: () => void
  setData: (data: T | null) => void
  setError: (error: string | null) => void
}

/**
 * Hook useApi
 * Gerencia estado de loading, erro e dados para chamadas à API
 */
export function useApi<T = any>(initialData: T | null = null): UseApiResult<T> {
  const [data, setData] = useState<T | null>(initialData)
  const [isLoading, setIsLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const execute = useCallback(async (fn: () => Promise<T>): Promise<T | null> => {
    setIsLoading(true)
    setError(null)

    try {
      const result = await fn()
      setData(result)
      return result
    } catch (err: any) {
      const errorMessage =
        err?.message || 'Erro ao processar a requisição'

      setError(errorMessage)
      console.error('API Error:', err)
      return null
    } finally {
      setIsLoading(false)
    }
  }, [])

  const reset = useCallback(() => {
    setData(initialData)
    setIsLoading(false)
    setError(null)
  }, [initialData])

  return {
    data,
    isLoading,
    error,
    execute,
    reset,
    setData,
    setError,
  }
}
