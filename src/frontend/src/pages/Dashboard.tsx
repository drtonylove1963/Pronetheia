import { useEffect } from 'react'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { useAgentStore } from '@/store/agentStore'
import { getAgentTypeIcon } from '@/lib/utils'

const Dashboard = () => {
  const { agents, getAgentStatus, executeAgentTask } = useAgentStore()

  useEffect(() => {
    // Refresh agent status on load
    getAgentStatus()
    
    // Set up periodic refresh
    const interval = setInterval(getAgentStatus, 5000)
    return () => clearInterval(interval)
  }, [getAgentStatus])

  const handleTriggerEvolution = async () => {
    await executeAgentTask('evolution-agent', {
      type: 'evolution-cycle',
      trigger: 'manual'
    })
  }

  const agentStats = {
    total: agents.length,
    active: agents.filter(a => a.status === 'active').length,
    busy: agents.filter(a => a.status === 'busy').length,
    healthy: agents.filter(a => a.isHealthy).length,
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Agent Dashboard</h1>
          <p className="text-muted-foreground">
            Monitor and coordinate your multi-agent system
          </p>
        </div>
        <Button 
          onClick={handleTriggerEvolution}
          variant="agent"
          className="flex items-center space-x-2"
        >
          <span>🧬</span>
          <span>Trigger Evolution</span>
        </Button>
      </div>

      {/* Stats Cards */}
      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Total Agents</CardTitle>
            <span className="text-2xl">🤖</span>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{agentStats.total}</div>
            <p className="text-xs text-muted-foreground">
              Multi-agent system
            </p>
          </CardContent>
        </Card>
        
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Active Agents</CardTitle>
            <span className="text-2xl">⚡</span>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-green-600">{agentStats.active}</div>
            <p className="text-xs text-muted-foreground">
              Currently running
            </p>
          </CardContent>
        </Card>
        
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Busy Agents</CardTitle>
            <span className="text-2xl">🔄</span>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-orange-600">{agentStats.busy}</div>
            <p className="text-xs text-muted-foreground">
              Processing tasks
            </p>
          </CardContent>
        </Card>
        
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">System Health</CardTitle>
            <span className="text-2xl">💚</span>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-green-600">
              {Math.round((agentStats.healthy / agentStats.total) * 100)}%
            </div>
            <p className="text-xs text-muted-foreground">
              All systems operational
            </p>
          </CardContent>
        </Card>
      </div>

      {/* Agent Status Grid */}
      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
        {agents.map((agent) => (
          <Card key={agent.id} className="relative">
            <CardHeader>
              <div className="flex items-center justify-between">
                <div className="flex items-center space-x-2">
                  <span className="text-2xl">{getAgentTypeIcon(agent.type)}</span>
                  <CardTitle className="text-lg">{agent.name}</CardTitle>
                </div>
                <Badge variant={agent.status as any}>
                  {agent.status}
                </Badge>
              </div>
              <CardDescription>
                {agent.type.charAt(0).toUpperCase() + agent.type.slice(1)} Agent
              </CardDescription>
            </CardHeader>
            <CardContent>
              <div className="space-y-3">
                {/* Capabilities */}
                <div>
                  <p className="text-sm font-medium mb-1">Capabilities:</p>
                  <div className="flex flex-wrap gap-1">
                    {agent.capabilities.slice(0, 3).map((capability) => (
                      <Badge key={capability} variant="outline" className="text-xs">
                        {capability}
                      </Badge>
                    ))}
                    {agent.capabilities.length > 3 && (
                      <Badge variant="outline" className="text-xs">
                        +{agent.capabilities.length - 3}
                      </Badge>
                    )}
                  </div>
                </div>

                {/* Health Status */}
                <div className="flex items-center justify-between text-sm">
                  <span>Health:</span>
                  <span className={agent.isHealthy ? 'text-green-600' : 'text-red-600'}>
                    {agent.isHealthy ? '✅ Healthy' : '❌ Issues'}
                  </span>
                </div>

                {/* Last Activity */}
                <div className="flex items-center justify-between text-sm">
                  <span>Last Activity:</span>
                  <span className="text-muted-foreground">
                    {new Date(agent.lastActivity).toLocaleTimeString()}
                  </span>
                </div>
              </div>
            </CardContent>
          </Card>
        ))}
      </div>

      {/* Agent Coordination Visualization */}
      <Card>
        <CardHeader>
          <CardTitle>Agent Coordination Network</CardTitle>
          <CardDescription>
            Real-time view of agent communication and task routing
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="flex items-center justify-center h-48 bg-gray-50 rounded-lg">
            <div className="text-center space-y-2">
              <div className="flex items-center justify-center space-x-8">
                {agents.map((agent) => (
                  <div key={agent.id} className="text-center">
                    <div className={`w-16 h-16 rounded-full flex items-center justify-center text-white font-bold ${
                      agent.status === 'active' ? 'bg-green-500' :
                      agent.status === 'busy' ? 'bg-orange-500' :
                      agent.status === 'error' ? 'bg-red-500' : 'bg-gray-400'
                    }`}>
                      {getAgentTypeIcon(agent.type)}
                    </div>
                    <p className="text-xs mt-1">{agent.name}</p>
                  </div>
                ))}
              </div>
              <div className="flex items-center justify-center space-x-4 mt-4">
                <div className="flex items-center space-x-1">
                  <div className="w-2 h-2 bg-blue-500 rounded-full"></div>
                  <span className="text-xs">Message Flow</span>
                </div>
                <div className="flex items-center space-x-1">
                  <div className="w-2 h-2 bg-green-500 rounded-full animate-pulse"></div>
                  <span className="text-xs">Active Coordination</span>
                </div>
              </div>
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Evolution Status */}
      <Card>
        <CardHeader>
          <CardTitle>Evolution Engine Status</CardTitle>
          <CardDescription>
            Self-analysis and improvement capabilities
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="space-y-4">
            <div className="flex items-center justify-between">
              <span>Next Evolution Cycle:</span>
              <Badge variant="outline">Scheduled in 25 minutes</Badge>
            </div>
            <div className="flex items-center justify-between">
              <span>Capability Gaps Identified:</span>
              <Badge variant="outline">5 critical gaps</Badge>
            </div>
            <div className="flex items-center justify-between">
              <span>Recommended Next Agent:</span>
              <Badge variant="outline">ProjectManagementAgent</Badge>
            </div>
            <div className="flex items-center justify-between">
              <span>Evolution Success Rate:</span>
              <span className="text-green-600 font-semibold">100%</span>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  )
}

export default Dashboard