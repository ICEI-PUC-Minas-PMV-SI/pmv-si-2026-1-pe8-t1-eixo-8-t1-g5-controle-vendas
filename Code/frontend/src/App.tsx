import { AuthProvider } from './context/AuthContext'
import { Router } from './routes/router'

function App() {
  return (
    <AuthProvider>
      <Router />
    </AuthProvider>
  )
}

export default App
