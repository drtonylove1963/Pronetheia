import { useEffect } from 'react'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { useAgentStore } from '@/store/agentStore'
import { getAgentTypeIcon } from '@/lib/utils'

const Agents = () => {
  const { agents, isLoading, getAgentStatus, executeAgentTask } = useAgentStore()

  useEffect(() => {
    getAgentStatus()
  }, [getAgentStatus])

  const handleRestartAgent = async (agentId: string) => {
    await executeAgentTask(agentId, { action: 'restart' })
  }

  const handleTestAgent = async (agentId: string) => {
    await executeAgentTask(agentId, { action: 'health-check' })
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div>
        <h1 className="text-3xl font-bold tracking-tight">Agent Management</h1>
        <p className="text-muted-foreground">
          Monitor and manage individual agents in your system
        </p>
      </div>

      {/* Agent Grid */}
      <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
        {agents.map((agent) => (
          <Card key={agent.id} className="relative">
            <CardHeader>
              <div className="flex items-center justify-between">
                <div className="flex items-center space-x-3">
                  <div className="text-3xl">{getAgentTypeIcon(agent.type)}</div>
                  <div>
                    <CardTitle className="text-xl">{agent.name}</CardTitle>
                    <CardDescription>
                      {agent.type.charAt(0).toUpperCase() + agent.type.slice(1)} Agent
                    </CardDescription>
                  </div>
                </div>
                <Badge variant={agent.status as any} className="text-xs">
                  {agent.status}
                </Badge>
              </div>
            </CardHeader>
            
            <CardContent className="space-y-4">
              {/* Status Information */}
              <div className="grid grid-cols-2 gap-4 text-sm">
                <div>
                  <span className="font-medium">Health:</span>
                  <div className={`mt-1 ${agent.isHealthy ? 'text-green-600' : 'text-red-600'}`}>
                    {agent.isHealthy ? '✅ Healthy' : '❌ Issues'}
                  </div>
                </div>
                <div>
                  <span className="font-medium">Last Activity:</span>
                  <div className="mt-1 text-muted-foreground">
                    {new Date(agent.lastActivity).toLocaleString()}
                  </div>
                </div>
              </div>

              {/* Capabilities */}
              <div>
                <span className="font-medium text-sm">Capabilities:</span>
                <div className="mt-2 flex flex-wrap gap-1">
                  {agent.capabilities.map((capability) => (
                    <Badge key={capability} variant="outline" className="text-xs">
                      {capability}
                    </Badge>
                  ))}
                </div>
              </div>

              {/* Metrics */}
              {agent.metrics && Object.keys(agent.metrics).length > 0 && (
                <div>
                  <span className="font-medium text-sm">Metrics:</span>
                  <div className="mt-2 space-y-1">
                    {Object.entries(agent.metrics).map(([key, value]) => (
                      <div key={key} className="flex justify-between text-xs">
                        <span>{key}:</span>
                        <span className="font-mono">{value}</span>
                      </div>
                    ))}
                  </div>
                </div>
              )}

              {/* Agent Actions */}
              <div className="flex space-x-2 pt-2">
                <Button
                  size="sm"
                  variant="outline"
                  onClick={() => handleTestAgent(agent.id)}
                  disabled={isLoading}
                >
                  🔍 Test
                </Button>
                <Button
                  size="sm"
                  variant="outline"
                  onClick={() => handleRestartAgent(agent.id)}
                  disabled={isLoading || agent.status === 'busy'}
                >
                  🔄 Restart
                </Button>
              </div>

              {/* Agent-Specific Info */}
              <div className="pt-2 border-t">
                {agent.type === 'chat' && (
                  <div className="text-xs text-muted-foreground">
                    Handles user interactions and routes messages to other agents
                  </div>
                )}
                {agent.type === 'evolution' && (
                  <div className="text-xs text-muted-foreground">
                    Analyzes system capabilities and creates new agents
                  </div>
                )}
                {agent.type === 'tool' && (
                  <div className="text-xs text-muted-foreground">
                    Orchestrates MCP tool execution and result processing
                  </div>
                )}
                {agent.type === 'projectManagement' && (
                  <div className="text-xs text-muted-foreground">
                    ✨ First Evolved Agent - Coordinates projects, optimizes workflows, and manages resources across the agent network
                  </div>
                )}
              </div>
            </CardContent>
          </Card>
        ))}
      </div>

      {/* System Overview */}
      <Card>
        <CardHeader>
          <CardTitle>System Overview</CardTitle>
          <CardDescription>
            Overview of agent coordination and communication
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="space-y-4">
            {/* Agent Communication Flow */}
            <div>
              <h4 className="font-semibold mb-2">4-Agent System Architecture</h4>
              <div className="bg-gray-50 rounded-lg p-4">
                <div className="grid grid-cols-2 gap-4">
                  {/* Top Row */}
                  <div className="text-center">
                    <div className="w-12 h-12 bg-blue-500 rounded-full flex items-center justify-center text-white font-bold mb-2 mx-auto">
                      💬
                    </div>
                    <div className="text-xs font-medium">ChatAgent</div>
                    <div className="text-xs text-muted-foreground">User Interface</div>
                  </div>
                  
                  <div className="text-center">
                    <div className="w-12 h-12 bg-purple-500 rounded-full flex items-center justify-center text-white font-bold mb-2 mx-auto">
                      📋
                    </div>
                    <div className="text-xs font-medium">ProjectManagementAgent</div>
                    <div className="text-xs text-muted-foreground">✨ First Evolved</div>
                  </div>
                  
                  {/* Bottom Row */}
                  <div className="text-center">
                    <div className="w-12 h-12 bg-green-500 rounded-full flex items-center justify-center text-white font-bold mb-2 mx-auto">
                      🧬
                    </div>
                    <div className="text-xs font-medium">EvolutionAgent</div>
                    <div className="text-xs text-muted-foreground">Self-Evolution</div>
                  </div>
                  
                  <div className="text-center">
                    <div className="w-12 h-12 bg-orange-500 rounded-full flex items-center justify-center text-white font-bold mb-2 mx-auto">
                      🔧
                    </div>
                    <div className="text-xs font-medium">ToolAgent</div>
                    <div className="text-xs text-muted-foreground">MCP Tools</div>
                  </div>
                </div>
                
                {/* Evolution Status */}
                <div className="mt-4 text-center">
                  <div className="inline-flex items-center space-x-2 bg-green-100 text-green-800 px-3 py-1 rounded-full text-xs">
                    <span>🎉</span>
                    <span>Evolution Cycle 1 Complete</span>
                    <span>🎉</span>
                  </div>
                </div>
              </div>
            </div>

            {/* Agent Statistics */}
            <div>
              <h4 className="font-semibold mb-2">Agent Statistics</h4>
              <div className="grid grid-cols-2 md:grid-cols-4 gap-4 text-sm">
                <div className="text-center p-3 bg-gray-50 rounded-lg">
                  <div className="text-2xl font-bold text-green-600">{agents.filter(a => a.status === 'active').length}</div>
                  <div className="text-muted-foreground">Active</div>
                </div>
                <div className="text-center p-3 bg-gray-50 rounded-lg">
                  <div className="text-2xl font-bold text-orange-600">{agents.filter(a => a.status === 'busy').length}</div>
                  <div className="text-muted-foreground">Busy</div>
                </div>
                <div className="text-center p-3 bg-gray-50 rounded-lg">
                  <div className="text-2xl font-bold text-gray-600">{agents.filter(a => a.status === 'idle').length}</div>
                  <div className="text-muted-foreground">Idle</div>
                </div>
                <div className="text-center p-3 bg-gray-50 rounded-lg">
                  <div className="text-2xl font-bold text-green-600">{agents.filter(a => a.isHealthy).length}</div>
                  <div className="text-muted-foreground">Healthy</div>
                </div>
              </div>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  )
}

export default Agents