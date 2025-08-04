import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import { useAgentStore } from '@/store/agentStore'

const Evolution = () => {
  const { executeAgentTask, isLoading } = useAgentStore()

  const capabilityGaps = [
    {
      id: 1,
      area: 'ProjectManagement',
      description: 'Need specialized agent for project coordination and planning',
      priority: 9,
      status: 'identified',
      requiredCapabilities: ['project-planning', 'task-coordination', 'resource-allocation'],
      targetPlatform: 'web',
      estimatedEffort: 'Medium',
      icon: '📋'
    },
    {
      id: 2,
      area: 'CodeReview',
      description: 'Automated code review and quality assessment capabilities missing',
      priority: 7,
      status: 'planned',
      requiredCapabilities: ['code-analysis', 'quality-metrics', 'best-practices'],
      targetPlatform: 'web',
      estimatedEffort: 'High',
      icon: '🔍'
    },
    {
      id: 3,
      area: 'Testing',
      description: 'Automated testing agent for test generation and execution',
      priority: 6,
      status: 'identified',
      requiredCapabilities: ['test-generation', 'test-execution', 'coverage-analysis'],
      targetPlatform: 'web',
      estimatedEffort: 'High',
      icon: '🧪'
    },
    {
      id: 4,
      area: 'Deployment',
      description: 'CI/CD automation and deployment management',
      priority: 5,
      status: 'identified',
      requiredCapabilities: ['ci-cd', 'deployment', 'infrastructure'],
      targetPlatform: 'web',
      estimatedEffort: 'Medium',
      icon: '🚀'
    },
    {
      id: 5,
      area: 'Monitoring',
      description: 'System monitoring, alerting, and performance tracking',
      priority: 4,
      status: 'identified',
      requiredCapabilities: ['monitoring', 'alerting', 'metrics'],
      targetPlatform: 'web',
      estimatedEffort: 'Low',
      icon: '📊'
    }
  ]

  const evolutionHistory = [
    {
      id: 1,
      version: '0.1.0',
      date: '2025-08-04',
      type: 'system_initialization',
      changes: ['Initial 3-agent system created', 'MCP tool integration completed'],
      success: true,
      duration: '2.3s'
    },
    {
      id: 2,
      version: '0.1.1',
      date: '2025-08-04',
      type: 'capability_enhancement',
      changes: ['Enhanced agent coordination', 'Improved error handling'],
      success: true,
      duration: '1.8s'
    }
  ]

  const getPriorityColor = (priority: number) => {
    if (priority >= 8) return 'bg-red-100 text-red-800'
    if (priority >= 6) return 'bg-orange-100 text-orange-800'
    if (priority >= 4) return 'bg-yellow-100 text-yellow-800'
    return 'bg-green-100 text-green-800'
  }

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'completed':
        return 'bg-green-100 text-green-800'
      case 'in_progress':
        return 'bg-blue-100 text-blue-800'
      case 'planned':
        return 'bg-purple-100 text-purple-800'
      default:
        return 'bg-gray-100 text-gray-800'
    }
  }

  const handleTriggerEvolution = async () => {
    await executeAgentTask('evolution-agent', {
      type: 'evolution-cycle',
      trigger: 'manual-full-cycle'
    })
  }

  const handleCreateAgent = async (gapId: number) => {
    const gap = capabilityGaps.find(g => g.id === gapId)
    if (gap) {
      await executeAgentTask('evolution-agent', {
        type: 'create-agent',
        specification: `Create a ${gap.area}Agent with capabilities: ${gap.requiredCapabilities.join(', ')}`
      })
    }
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Evolution Engine</h1>
          <p className="text-muted-foreground">
            System self-analysis and autonomous improvement
          </p>
        </div>
        <Button 
          onClick={handleTriggerEvolution}
          variant="agent"
          disabled={isLoading}
          className="flex items-center space-x-2"
        >
          <span>🧬</span>
          <span>{isLoading ? 'Evolving...' : 'Trigger Evolution'}</span>
        </Button>
      </div>

      {/* Evolution Status */}
      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Next Evolution</CardTitle>
            <span className="text-2xl">⏰</span>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">25min</div>
            <p className="text-xs text-muted-foreground">
              Scheduled cycle
            </p>
          </CardContent>
        </Card>
        
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Capability Gaps</CardTitle>
            <span className="text-2xl">📋</span>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-orange-600">{capabilityGaps.length}</div>
            <p className="text-xs text-muted-foreground">
              Identified gaps
            </p>
          </CardContent>
        </Card>
        
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Success Rate</CardTitle>
            <span className="text-2xl">✅</span>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-green-600">100%</div>
            <p className="text-xs text-muted-foreground">
              Evolution success
            </p>
          </CardContent>
        </Card>
        
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Next Agent</CardTitle>
            <span className="text-2xl">🤖</span>
          </CardHeader>
          <CardContent>
            <div className="text-lg font-bold text-pronetheia-blue">ProjectAgent</div>
            <p className="text-xs text-muted-foreground">
              Recommended creation
            </p>
          </CardContent>
        </Card>
      </div>

      {/* Capability Gaps */}
      <Card>
        <CardHeader>
          <CardTitle>Capability Gap Analysis</CardTitle>
          <CardDescription>
            Identified areas for system improvement and agent creation
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="space-y-4">
            {capabilityGaps.map((gap) => (
              <div key={gap.id} className="border rounded-lg p-4">
                <div className="flex items-start justify-between">
                  <div className="flex items-start space-x-3 flex-1">
                    <div className="text-2xl">{gap.icon}</div>
                    <div className="flex-1">
                      <div className="flex items-center space-x-2 mb-2">
                        <h4 className="font-semibold">{gap.area}</h4>
                        <Badge className={getPriorityColor(gap.priority)}>
                          Priority {gap.priority}
                        </Badge>
                        <Badge className={getStatusColor(gap.status)}>
                          {gap.status}
                        </Badge>
                      </div>
                      <p className="text-sm text-muted-foreground mb-3">
                        {gap.description}
                      </p>
                      
                      <div className="grid md:grid-cols-3 gap-4 text-sm">
                        <div>
                          <span className="font-medium">Required Capabilities:</span>
                          <div className="mt-1 flex flex-wrap gap-1">
                            {gap.requiredCapabilities.map((cap) => (
                              <Badge key={cap} variant="outline" className="text-xs">
                                {cap}
                              </Badge>
                            ))}
                          </div>
                        </div>
                        <div>
                          <span className="font-medium">Platform:</span>
                          <div className="mt-1">{gap.targetPlatform}</div>
                        </div>
                        <div>
                          <span className="font-medium">Effort:</span>
                          <div className="mt-1">{gap.estimatedEffort}</div>
                        </div>
                      </div>
                    </div>
                  </div>
                  <div className="flex flex-col space-y-2">
                    <Button
                      size="sm"
                      variant="outline"
                      onClick={() => handleCreateAgent(gap.id)}
                      disabled={isLoading || gap.status === 'completed'}
                    >
                      🤖 Create Agent
                    </Button>
                    <Button size="sm" variant="outline">
                      📋 View Details
                    </Button>
                  </div>
                </div>
              </div>
            ))}
          </div>
        </CardContent>
      </Card>

      {/* Evolution History */}
      <Card>
        <CardHeader>
          <CardTitle>Evolution History</CardTitle>
          <CardDescription>
            Record of system improvements and agent creation
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="space-y-3">
            {evolutionHistory.map((evolution) => (
              <div key={evolution.id} className="flex items-center justify-between p-3 bg-gray-50 rounded-lg">
                <div className="flex items-center space-x-3">
                  <div className={`w-3 h-3 rounded-full ${evolution.success ? 'bg-green-500' : 'bg-red-500'}`}></div>
                  <div>
                    <div className="font-medium text-sm">
                      Version {evolution.version} - {evolution.type.replace('_', ' ')}
                    </div>
                    <div className="text-xs text-muted-foreground">
                      {evolution.changes.join(', ')}
                    </div>
                  </div>
                </div>
                <div className="flex items-center space-x-4 text-sm">
                  <Badge variant={evolution.success ? "active" : "error"} className="text-xs">
                    {evolution.success ? 'Success' : 'Failed'}
                  </Badge>
                  <span className="text-muted-foreground">{evolution.duration}</span>
                  <span className="text-muted-foreground">{evolution.date}</span>
                </div>
              </div>
            ))}
          </div>
        </CardContent>
      </Card>

      {/* Evolution Roadmap */}
      <Card>
        <CardHeader>
          <CardTitle>Evolution Roadmap</CardTitle>
          <CardDescription>
            Planned development path for the multi-agent system
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="space-y-6">
            <div className="grid md:grid-cols-3 gap-4">
              <div className="text-center p-4 bg-blue-50 rounded-lg">
                <div className="text-2xl mb-2">🌱</div>
                <h4 className="font-semibold">Phase 0.1 - SEED</h4>
                <p className="text-sm text-muted-foreground mt-1">
                  3-agent foundation with MCP tools
                </p>
                <Badge variant="active" className="mt-2">Current</Badge>
              </div>
              <div className="text-center p-4 bg-green-50 rounded-lg">
                <div className="text-2xl mb-2">🌿</div>
                <h4 className="font-semibold">Phase 0.2 - GROWTH</h4>
                <p className="text-sm text-muted-foreground mt-1">
                  Agent self-evolution and specialization
                </p>
                <Badge variant="outline" className="mt-2">Next</Badge>
              </div>
              <div className="text-center p-4 bg-purple-50 rounded-lg">
                <div className="text-2xl mb-2">🌳</div>
                <h4 className="font-semibold">Phase 0.3 - SCALE</h4>
                <p className="text-sm text-muted-foreground mt-1">
                  Multi-agent swarm intelligence
                </p>
                <Badge variant="outline" className="mt-2">Future</Badge>
              </div>
            </div>
            
            <div className="border-t pt-4">
              <h4 className="font-semibold mb-3">Evolution Principles</h4>
              <div className="grid md:grid-cols-2 gap-4 text-sm">
                <div>
                  <h5 className="font-medium mb-2">🎯 Goal-Oriented Evolution</h5>
                  <p className="text-muted-foreground">
                    Agents identify capability gaps and create specialized agents to fill them
                  </p>
                </div>
                <div>
                  <h5 className="font-medium mb-2">🔄 Continuous Improvement</h5>
                  <p className="text-muted-foreground">
                    Regular analysis cycles ensure system adaptation and optimization
                  </p>
                </div>
                <div>
                  <h5 className="font-medium mb-2">🤝 Collaborative Intelligence</h5>
                  <p className="text-muted-foreground">
                    Agents coordinate and share knowledge for enhanced system capabilities
                  </p>
                </div>
                <div>
                  <h5 className="font-medium mb-2">🛡️ Safe Evolution</h5>
                  <p className="text-muted-foreground">
                    All changes validated and tested before deployment with rollback capability
                  </p>
                </div>
              </div>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  )
}

export default Evolution