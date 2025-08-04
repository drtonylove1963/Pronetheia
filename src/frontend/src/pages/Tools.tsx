import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'

const Tools = () => {
  const mcpTools = [
    {
      id: 'file-operations',
      name: 'FileOperationsMCP',
      category: 'file_ops',
      description: 'File system operations including create, read, update, delete, and directory listing',
      securityLevel: 'elevated',
      isActive: true,
      icon: '📁',
      capabilities: ['create', 'read', 'update', 'delete', 'list'],
      executionTime: '2ms',
      successRate: '99.8%'
    },
    {
      id: 'code-generation',
      name: 'CodeGenerationMCP',
      category: 'code_gen',
      description: 'AI-powered code generation, analysis, refactoring, test generation, and syntax validation',
      securityLevel: 'safe',
      isActive: true,
      icon: '💻',
      capabilities: ['generate', 'analyze', 'refactor', 'test', 'validate'],
      executionTime: '847ms',
      successRate: '95.2%'
    },
    {
      id: 'analysis',
      name: 'AnalysisMCP',
      category: 'analysis',
      description: 'Codebase analysis including pattern identification, capability assessment, dependency analysis',
      securityLevel: 'safe',
      isActive: true,
      icon: '🔍',
      capabilities: ['codebase', 'patterns', 'capabilities', 'dependencies', 'metrics'],
      executionTime: '1.2s',
      successRate: '97.1%'
    },
    {
      id: 'database',
      name: 'DatabaseMCP',
      category: 'database',
      description: 'Database operations including query execution, schema management, and CRUD operations',
      securityLevel: 'elevated',
      isActive: true,
      icon: '🗄️',
      capabilities: ['query', 'insert', 'update', 'delete'],
      executionTime: '45ms',
      successRate: '99.5%'
    },
    {
      id: 'web-search',
      name: 'WebSearchMCP',
      category: 'web_search',
      description: 'Web search and content analysis including search, content fetching, website analysis',
      securityLevel: 'safe',
      isActive: true,
      icon: '🌐',
      capabilities: ['search', 'fetch', 'analyze', 'documentation'],
      executionTime: '890ms',
      successRate: '92.3%'
    }
  ]

  const getSecurityColor = (level: string) => {
    switch (level) {
      case 'safe':
        return 'bg-green-100 text-green-800'
      case 'elevated':
        return 'bg-yellow-100 text-yellow-800'
      case 'dangerous':
        return 'bg-red-100 text-red-800'
      default:
        return 'bg-gray-100 text-gray-800'
    }
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div>
        <h1 className="text-3xl font-bold tracking-tight">MCP Tools Monitor</h1>
        <p className="text-muted-foreground">
          Monitor and manage Model Context Protocol (MCP) tools
        </p>
      </div>

      {/* Summary Stats */}
      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Total Tools</CardTitle>
            <span className="text-2xl">🔧</span>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{mcpTools.length}</div>
            <p className="text-xs text-muted-foreground">
              MCP tools registered
            </p>
          </CardContent>
        </Card>
        
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Active Tools</CardTitle>
            <span className="text-2xl">⚡</span>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-green-600">{mcpTools.filter(t => t.isActive).length}</div>
            <p className="text-xs text-muted-foreground">
              Currently available
            </p>
          </CardContent>
        </Card>
        
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Avg Response Time</CardTitle>
            <span className="text-2xl">⏱️</span>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-blue-600">453ms</div>
            <p className="text-xs text-muted-foreground">
              Across all tools
            </p>
          </CardContent>
        </Card>
        
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Success Rate</CardTitle>
            <span className="text-2xl">✅</span>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-green-600">96.8%</div>
            <p className="text-xs text-muted-foreground">
              Overall reliability
            </p>
          </CardContent>
        </Card>
      </div>

      {/* Tools Grid */}
      <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
        {mcpTools.map((tool) => (
          <Card key={tool.id} className="relative">
            <CardHeader>
              <div className="flex items-center justify-between">
                <div className="flex items-center space-x-3">
                  <div className="text-3xl">{tool.icon}</div>
                  <div>
                    <CardTitle className="text-lg">{tool.name}</CardTitle>
                    <CardDescription className="capitalize">
                      {tool.category.replace('_', ' ')} Tool
                    </CardDescription>
                  </div>
                </div>
                <div className="flex flex-col items-end space-y-1">
                  <Badge variant={tool.isActive ? "active" : "idle"} className="text-xs">
                    {tool.isActive ? 'Active' : 'Inactive'}
                  </Badge>
                  <Badge className={`text-xs ${getSecurityColor(tool.securityLevel)}`}>
                    {tool.securityLevel}
                  </Badge>
                </div>
              </div>
            </CardHeader>
            
            <CardContent className="space-y-4">
              {/* Description */}
              <p className="text-sm text-muted-foreground">
                {tool.description}
              </p>

              {/* Capabilities */}
              <div>
                <span className="font-medium text-sm">Capabilities:</span>
                <div className="mt-1 flex flex-wrap gap-1">
                  {tool.capabilities.map((capability) => (
                    <Badge key={capability} variant="outline" className="text-xs">
                      {capability}
                    </Badge>
                  ))}
                </div>
              </div>

              {/* Performance Metrics */}
              <div className="grid grid-cols-2 gap-4 text-sm">
                <div>
                  <span className="font-medium">Response Time:</span>
                  <div className="mt-1 text-muted-foreground">
                    {tool.executionTime}
                  </div>
                </div>
                <div>
                  <span className="font-medium">Success Rate:</span>
                  <div className="mt-1 text-green-600 font-semibold">
                    {tool.successRate}
                  </div>
                </div>
              </div>

              {/* Tool Actions */}
              <div className="flex space-x-2 pt-2">
                <Button size="sm" variant="outline">
                  🔍 Test Tool
                </Button>
                <Button size="sm" variant="outline">
                  📊 View Logs
                </Button>
              </div>
            </CardContent>
          </Card>
        ))}
      </div>

      {/* Tool Execution History */}
      <Card>
        <CardHeader>
          <CardTitle>Recent Tool Executions</CardTitle>
          <CardDescription>
            Recent MCP tool execution history and performance
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="space-y-3">
            {[
              { tool: 'CodeGenerationMCP', action: 'generate', status: 'completed', time: '2 min ago', duration: '1.2s' },
              { tool: 'FileOperationsMCP', action: 'create', status: 'completed', time: '5 min ago', duration: '3ms' },
              { tool: 'AnalysisMCP', action: 'codebase', status: 'completed', time: '8 min ago', duration: '2.1s' },
              { tool: 'WebSearchMCP', action: 'search', status: 'completed', time: '12 min ago', duration: '890ms' },
              { tool: 'DatabaseMCP', action: 'query', status: 'completed', time: '15 min ago', duration: '45ms' },
            ].map((execution, index) => (
              <div key={index} className="flex items-center justify-between p-3 bg-gray-50 rounded-lg">
                <div className="flex items-center space-x-3">
                  <div className="w-2 h-2 bg-green-500 rounded-full"></div>
                  <div>
                    <div className="font-medium text-sm">{execution.tool}</div>
                    <div className="text-xs text-muted-foreground">
                      Action: {execution.action}
                    </div>
                  </div>
                </div>
                <div className="flex items-center space-x-4 text-sm">
                  <Badge variant="active" className="text-xs">
                    {execution.status}
                  </Badge>
                  <span className="text-muted-foreground">{execution.duration}</span>
                  <span className="text-muted-foreground">{execution.time}</span>
                </div>
              </div>
            ))}
          </div>
        </CardContent>
      </Card>

      {/* Tool Integration Guide */}
      <Card>
        <CardHeader>
          <CardTitle>MCP Tool Integration</CardTitle>
          <CardDescription>
            How agents use MCP tools for enhanced capabilities
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="space-y-4">
            <div className="grid md:grid-cols-3 gap-4">
              <div className="text-center p-4 bg-blue-50 rounded-lg">
                <div className="text-2xl mb-2">🤖</div>
                <h4 className="font-semibold">Agent Request</h4>
                <p className="text-sm text-muted-foreground mt-1">
                  Agent identifies need for tool execution
                </p>
              </div>
              <div className="text-center p-4 bg-green-50 rounded-lg">
                <div className="text-2xl mb-2">🔧</div>
                <h4 className="font-semibold">Tool Execution</h4>
                <p className="text-sm text-muted-foreground mt-1">
                  ToolAgent orchestrates MCP tool execution
                </p>
              </div>
              <div className="text-center p-4 bg-purple-50 rounded-lg">
                <div className="text-2xl mb-2">📤</div>
                <h4 className="font-semibold">Result Processing</h4>
                <p className="text-sm text-muted-foreground mt-1">
                  Results processed and returned to agent
                </p>
              </div>
            </div>
            
            <div className="mt-4">
              <h4 className="font-semibold mb-2">Security Levels</h4>
              <div className="space-y-2 text-sm">
                <div className="flex items-center space-x-2">
                  <Badge className="bg-green-100 text-green-800">safe</Badge>
                  <span>No system access required, read-only operations</span>
                </div>
                <div className="flex items-center space-x-2">
                  <Badge className="bg-yellow-100 text-yellow-800">elevated</Badge>
                  <span>Requires file system or database access</span>
                </div>
                <div className="flex items-center space-x-2">
                  <Badge className="bg-red-100 text-red-800">dangerous</Badge>
                  <span>Can modify system state or execute commands</span>
                </div>
              </div>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  )
}

export default Tools