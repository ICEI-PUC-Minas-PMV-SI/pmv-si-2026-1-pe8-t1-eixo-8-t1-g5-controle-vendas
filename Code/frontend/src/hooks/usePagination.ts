import { useCallback, useMemo, useState } from 'react'

export interface UsePaginationResult {
  currentPage: number
  goToPage: (page: number) => void
  goToNextPage: () => void
  goToPreviousPage: () => void
  resetPagination: () => void
}

export function usePagination(initialPage = 1): UsePaginationResult {
  const [currentPage, setCurrentPage] = useState(initialPage)

  const goToPage = useCallback((page: number) => {
    setCurrentPage(Math.max(1, page))
  }, [])

  const goToNextPage = useCallback(() => {
    setCurrentPage((page) => page + 1)
  }, [])

  const goToPreviousPage = useCallback(() => {
    setCurrentPage((page) => Math.max(1, page - 1))
  }, [])

  const resetPagination = useCallback(() => {
    setCurrentPage(1)
  }, [])

  return useMemo(() => ({
    currentPage,
    goToPage,
    goToNextPage,
    goToPreviousPage,
    resetPagination,
  }), [currentPage, goToNextPage, goToPage, goToPreviousPage, resetPagination])
}
