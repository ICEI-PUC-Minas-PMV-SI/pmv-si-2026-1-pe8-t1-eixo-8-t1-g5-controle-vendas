import { useCallback, useMemo, useState, type Dispatch, type SetStateAction } from 'react'

type ValidationErrors<T> = Partial<Record<keyof T | string, string>>

export interface UseFormResult<T> {
  values: T
  errors: ValidationErrors<T>
  setValues: Dispatch<SetStateAction<T>>
  setErrors: Dispatch<SetStateAction<ValidationErrors<T>>>
  setFieldValue: <K extends keyof T>(field: K, value: T[K]) => void
  validateForm: () => boolean
  resetForm: (nextValues?: T) => void
}

export function useForm<T extends object>(
  initialValues: T,
  validate?: (values: T) => ValidationErrors<T>
): UseFormResult<T> {
  const [values, setValues] = useState<T>(initialValues)
  const [errors, setErrors] = useState<ValidationErrors<T>>({})

  const setFieldValue = useCallback(<K extends keyof T>(field: K, value: T[K]) => {
    setValues((currentValues) => ({
      ...currentValues,
      [field]: value,
    }))
  }, [])

  const validateForm = useCallback(() => {
    const nextErrors = validate ? validate(values) : {}
    setErrors(nextErrors)
    return Object.keys(nextErrors).length === 0
  }, [validate, values])

  const resetForm = useCallback((nextValues?: T) => {
    setValues(nextValues ?? initialValues)
    setErrors({})
  }, [initialValues])

  return useMemo(() => ({
    values,
    errors,
    setValues,
    setErrors,
    setFieldValue,
    validateForm,
    resetForm,
  }), [errors, resetForm, setFieldValue, validateForm, values])
}
