import { useState } from 'react'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Badge } from '@/components/ui/badge'
import { useAgentStore } from '@/store/agentStore'
import { formatTimestamp, getAgentTypeIcon } from '@/lib/utils'

const Chat = () => {
  const [inputMessage, setInputMessage] = useState('')
  const { messages, isLoading, sendMessage, clearMessages, agents } = useAgentStore()

  const handleSendMessage = async (e: React.FormEvent) => {
    e.preventDefault()
    if (inputMessage.trim() && !isLoading) {
      await sendMessage(inputMessage.trim())
      setInputMessage('')
    }
  }

  const getAgentInfo = (agentId?: string) => {
    if (!agentId) return null
    const agent = agents.find(a => a.id === agentId)
    return agent
  }

  return (
    <div className="max-w-4xl mx-auto space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Multi-Agent Chat</h1>
          <p className="text-muted-foreground">
            Interact with your agent system through natural language
          </p>
        </div>
        <Button 
          onClick={clearMessages}
          variant="outline"
          className="flex items-center space-x-2"
        >
          <span>🗑️</span>
          <span>Clear Chat</span>
        </Button>
      </div>

      {/* Active Agents */}
      <Card>
        <CardHeader>
          <CardTitle className="text-lg">Active Agents</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="flex flex-wrap gap-2">
            {agents.map((agent) => (
              <div key={agent.id} className="flex items-center space-x-2 bg-gray-50 rounded-lg px-3 py-2">
                <span className="text-lg">{getAgentTypeIcon(agent.type)}</span>
                <span className="font-medium">{agent.name}</span>
                <Badge variant={agent.status as any} className="text-xs">
                  {agent.status}
                </Badge>
              </div>
            ))}
          </div>
        </CardContent>
      </Card>

      {/* Chat Messages */}
      <Card className="min-h-[500px]">
        <CardContent className="p-0">
          <div className="flex flex-col h-[500px]">
            {/* Messages Area */}
            <div className="flex-1 overflow-y-auto p-4 space-y-4">
              {messages.length === 0 ? (
                <div className="flex flex-col items-center justify-center h-full text-center text-muted-foreground">
                  <div className="text-6xl mb-4">🤖</div>
                  <h3 className="text-lg font-semibold mb-2">Start a conversation</h3>
                  <p>Type a message below to interact with your agents</p>
                  <div className="mt-4 space-y-2 text-sm">
                    <p>Try asking:</p>
                    <ul className="space-y-1">
                      <li>• "Analyze the current system capabilities"</li>
                      <li>• "Generate a new React component"</li>
                      <li>• "Execute file operations"</li>
                      <li>• "Create a new agent for project management"</li>
                    </ul>
                  </div>
                </div>
              ) : (
                messages.map((message) => {
                  const agent = getAgentInfo(message.agentId)
                  return (
                    <div key={message.id} className={`flex ${message.role === 'user' ? 'justify-end' : 'justify-start'}`}>
                      <div className={`max-w-[80%] rounded-lg p-3 ${
                        message.role === 'user' 
                          ? 'bg-pronetheia-blue text-white' 
                          : 'bg-gray-100 text-gray-900'
                      }`}>
                        {/* Message Header */}
                        {message.role !== 'user' && (
                          <div className="flex items-center space-x-2 mb-2">
                            {agent && (
                              <>
                                <span className="text-lg">{getAgentTypeIcon(agent.type)}</span>
                                <span className="font-semibold text-sm">{agent.name}</span>
                                <Badge variant={agent.status as any} className="text-xs">
                                  {agent.status}
                                </Badge>
                              </>
                            )}
                          </div>
                        )}
                        
                        {/* Message Content */}
                        <div className="space-y-2">
                          <p className="whitespace-pre-wrap">{message.content}</p>
                          
                          {/* Metadata */}
                          {message.metadata && (
                            <div className="text-xs opacity-75 mt-2">
                              <details>
                                <summary className="cursor-pointer">View details</summary>
                                <pre className="mt-1 p-2 bg-black/10 rounded text-xs overflow-x-auto">
                                  {JSON.stringify(message.metadata, null, 2)}
                                </pre>
                              </details>
                            </div>
                          )}
                        </div>
                        
                        {/* Timestamp */}
                        <div className={`text-xs mt-2 ${message.role === 'user' ? 'text-white/70' : 'text-gray-500'}`}>
                          {formatTimestamp(message.timestamp)}
                        </div>
                      </div>
                    </div>
                  )
                })
              )}
              
              {/* Loading Indicator */}
              {isLoading && (
                <div className="flex justify-start">
                  <div className="max-w-[80%] rounded-lg p-3 bg-gray-100">
                    <div className="flex items-center space-x-2">
                      <div className="flex space-x-1">
                        <div className="w-2 h-2 bg-gray-400 rounded-full animate-bounce"></div>
                        <div className="w-2 h-2 bg-gray-400 rounded-full animate-bounce" style={{animationDelay: '0.1s'}}></div>
                        <div className="w-2 h-2 bg-gray-400 rounded-full animate-bounce" style={{animationDelay: '0.2s'}}></div>
                      </div>
                      <span className="text-sm text-gray-600">Agents are processing...</span>
                    </div>
                  </div>
                </div>
              )}
            </div>
            
            {/* Input Area */}
            <div className="border-t p-4">
              <form onSubmit={handleSendMessage} className="flex space-x-2">
                <Input
                  value={inputMessage}
                  onChange={(e) => setInputMessage(e.target.value)}
                  placeholder="Type your message to the agents..."
                  disabled={isLoading}
                  className="flex-1"
                />
                <Button 
                  type="submit" 
                  disabled={!inputMessage.trim() || isLoading}
                  variant="agent"
                >
                  {isLoading ? '⏳' : '📤'} Send
                </Button>
              </form>
              <div className="mt-2 text-xs text-gray-500">
                Messages are automatically routed to the appropriate agent based on content analysis
              </div>
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Quick Actions */}
      <Card>
        <CardHeader>
          <CardTitle className="text-lg">Quick Actions</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-2 md:grid-cols-4 gap-2">
            <Button 
              variant="outline" 
              onClick={() => setInputMessage("Analyze current system capabilities")}
              className="text-left justify-start h-auto p-3"
            >
              <div>
                <div className="font-semibold">🔍 System Analysis</div>
                <div className="text-xs text-muted-foreground">Analyze capabilities</div>
              </div>
            </Button>
            <Button 
              variant="outline" 
              onClick={() => setInputMessage("Generate a React component for user authentication")}
              className="text-left justify-start h-auto p-3"
            >
              <div>
                <div className="font-semibold">💻 Code Generation</div>
                <div className="text-xs text-muted-foreground">Generate code</div>
              </div>
            </Button>
            <Button 
              variant="outline" 
              onClick={() => setInputMessage("Execute file operations to create a new project structure")}
              className="text-left justify-start h-auto p-3"
            >
              <div>
                <div className="font-semibold">📁 File Operations</div>
                <div className="text-xs text-muted-foreground">Manage files</div>
              </div>
            </Button>
            <Button 
              variant="outline" 
              onClick={() => setInputMessage("Create a new specialized agent for project management")}
              className="text-left justify-start h-auto p-3"
            >
              <div>
                <div className="font-semibold">🧬 Agent Evolution</div>
                <div className="text-xs text-muted-foreground">Create new agents</div>
              </div>
            </Button>
          </div>
        </CardContent>
      </Card>
    </div>
  )
}

export default Chat