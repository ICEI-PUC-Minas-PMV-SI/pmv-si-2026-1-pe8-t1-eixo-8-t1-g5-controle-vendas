/** @type {import('tailwindcss').Config} */
export default {
  content: [
    './index.html',
    './src/**/*.{js,ts,jsx,tsx}',
  ],
  theme: {
    extend: {
      colors: {
        // Brand colors
        primary: '#3B82F6',      // Azul - ações principais
        success: '#10B981',      // Verde - sucesso
        danger: '#EF4444',       // Vermelho - erro/alerta
        warning: '#F59E0B',      // Laranja - avisos
        neutral: '#6B7280',      // Cinza - texto secondary
        background: '#F9FAFB',   // Cinza claro - fundo

        // Status colors
        error: '#DC2626',        // Vermelho escuro para atrasos
        info: '#06B6D4',         // Ciano para info
      },
      spacing: {
        xs: '0.25rem',
        sm: '0.5rem',
        md: '1rem',
        lg: '1.5rem',
        xl: '2rem',
      },
      borderRadius: {
        sm: '0.375rem',
        md: '0.5rem',
        lg: '0.75rem',
        full: '9999px',
      },
      fontSize: {
        xs: ['0.75rem', { lineHeight: '1rem' }],
        sm: ['0.875rem', { lineHeight: '1.25rem' }],
        base: ['1rem', { lineHeight: '1.5rem' }],
        lg: ['1.125rem', { lineHeight: '1.75rem' }],
        xl: ['1.25rem', { lineHeight: '1.75rem' }],
        '2xl': ['1.5rem', { lineHeight: '2rem' }],
      },
    },
  },
  plugins: [],
}
