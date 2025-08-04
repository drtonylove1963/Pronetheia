import { BrowserRouter as Router, Routes, Route } from 'react-router-dom'
import { Toaster } from './components/ui/toaster'
import Dashboard from './pages/Dashboard'
import Chat from './pages/Chat'
import Agents from './pages/Agents'
import Tools from './pages/Tools'
import Evolution from './pages/Evolution'
import Navigation from './components/Navigation'
import { useAgentStore } from './store/agentStore'
import { useEffect } from 'react'

function App() {
  const initializeAgents = useAgentStore((state) => state.initializeAgents)

  useEffect(() => {
    // Initialize agents on app start
    initializeAgents()
  }, [initializeAgents])

  return (
    <Router>
      <div className="min-h-screen bg-background">
        <Navigation />
        <main className="container mx-auto px-4 py-8">
          <Routes>
            <Route path="/" element={<Dashboard />} />
            <Route path="/chat" element={<Chat />} />
            <Route path="/agents" element={<Agents />} />
            <Route path="/tools" element={<Tools />} />
            <Route path="/evolution" element={<Evolution />} />
          </Routes>
        </main>
        <Toaster />
      </div>
    </Router>
  )
}

export default App