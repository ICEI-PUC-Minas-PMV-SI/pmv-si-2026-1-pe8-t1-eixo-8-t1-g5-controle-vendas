// Tipos de erro customizados

export class ApiError extends Error {
  constructor(
    public message: string,
    public statusCode?: number,
    public details?: any
  ) {
    super(message)
    this.name = 'ApiError'
  }
}

export class AuthError extends ApiError {
  constructor(message: string = 'Erro de autenticação', details?: any) {
    super(message, 401, details)
    this.name = 'AuthError'
  }
}

export class ValidationError extends ApiError {
  constructor(message: string = 'Erro de validação', public fields?: Record<string, string[]>) {
    super(message, 400, fields)
    this.name = 'ValidationError'
  }
}

export class BusinessError extends ApiError {
  constructor(message: string = 'Erro de negócio', details?: any) {
    super(message, 409, details)
    this.name = 'BusinessError'
  }
}

export class NotFoundError extends ApiError {
  constructor(message: string = 'Recurso não encontrado', details?: any) {
    super(message, 404, details)
    this.name = 'NotFoundError'
  }
}

export class NetworkError extends ApiError {
  constructor(message: string = 'Erro de conexão', details?: any) {
    super(message, 0, details)
    this.name = 'NetworkError'
  }
}
