import { Link, useLocation } from 'react-router-dom'
import { Button } from './ui/button'

const Navigation = () => {
  const location = useLocation()

  const navItems = [
    { path: '/', label: 'Dashboard', icon: '📊' },
    { path: '/chat', label: 'Chat', icon: '💬' },
    { path: '/agents', label: 'Agents', icon: '🤖' },
    { path: '/tools', label: 'Tools', icon: '🔧' },
    { path: '/evolution', label: 'Evolution', icon: '🧬' },
  ]

  return (
    <nav className="border-b bg-white shadow-sm">
      <div className="container mx-auto px-4">
        <div className="flex items-center justify-between h-16">
          {/* Logo */}
          <div className="flex items-center space-x-2">
            <div className="text-2xl">🔮</div>
            <h1 className="text-xl font-bold text-pronetheia-blue">
              Pronetheia <span className="text-sm text-gray-500">v0.1-SEED</span>
            </h1>
          </div>

          {/* Navigation Links */}
          <div className="flex items-center space-x-1">
            {navItems.map((item) => (
              <Link key={item.path} to={item.path}>
                <Button
                  variant={location.pathname === item.path ? "agent" : "ghost"}
                  size="sm"
                  className="flex items-center space-x-2"
                >
                  <span>{item.icon}</span>
                  <span>{item.label}</span>
                </Button>
              </Link>
            ))}
          </div>

          {/* Status Indicator */}
          <div className="flex items-center space-x-2">
            <div className="flex items-center space-x-1">
              <div className="w-2 h-2 bg-green-500 rounded-full animate-pulse"></div>
              <span className="text-sm text-gray-600">3 Agents Active</span>
            </div>
          </div>
        </div>
      </div>
    </nav>
  )
}

export default Navigation