import { create } from 'zustand'
import { agentApi } from '../services/api'

export interface Agent {
  id: string
  name: string
  type: 'chat' | 'evolution' | 'tool' | 'projectManagement'
  status: 'active' | 'idle' | 'busy' | 'error' | 'processing'
  capabilities: string[]
  lastActivity: string
  isHealthy: boolean
  metrics: Record<string, any>
}

export interface MCPTool {
  id: string
  name: string
  category: string
  description: string
  securityLevel: string
  isActive: boolean
}

export interface ChatMessage {
  id: string
  content: string
  role: 'user' | 'assistant' | 'system'
  agentId?: string
  timestamp: string
  metadata?: Record<string, any>
}

interface AgentState {
  agents: Agent[]
  tools: MCPTool[]
  messages: ChatMessage[]
  isLoading: boolean
  error: string | null
  
  // Actions
  initializeAgents: () => Promise<void>
  sendMessage: (message: string) => Promise<void>
  getAgentStatus: () => Promise<void>
  executeAgentTask: (agentId: string, task: any) => Promise<void>
  addMessage: (message: ChatMessage) => void
  clearMessages: () => void
  setError: (error: string | null) => void
}

export const useAgentStore = create<AgentState>((set, get) => ({
  agents: [],
  tools: [],
  messages: [],
  isLoading: false,
  error: null,

  initializeAgents: async () => {
    set({ isLoading: true, error: null })
    try {
      // Get initial agent statuses
      const response = await agentApi.getAgentStatus()
      
      // Transform response to match our interface
      const agents: Agent[] = response.data.map((status: any) => ({
        id: status.agentId,
        name: status.agentId.split('-').map((part: string) => 
          part.charAt(0).toUpperCase() + part.slice(1)).join(''),
        type: status.agentId.includes('chat') ? 'chat' : 
              status.agentId.includes('evolution') ? 'evolution' : 
              status.agentId.includes('project-management') ? 'projectManagement' : 'tool',
        status: status.status === 0 ? 'idle' : 
                status.status === 1 ? 'active' : 
                status.status === 2 ? 'busy' : 
                status.status === 3 ? 'processing' : 'error',
        capabilities: [],
        lastActivity: status.lastActivity,
        isHealthy: status.isHealthy,
        metrics: status.metrics || {}
      }))
      
      set({ agents, isLoading: false })
    } catch (error) {
      console.error('Failed to initialize agents:', error)
      set({ 
        error: 'Failed to initialize agents', 
        isLoading: false,
        // Set default agents for demo
        agents: [
          {
            id: 'chat-agent',
            name: 'ChatAgent',
            type: 'chat',
            status: 'active',
            capabilities: ['naturalLanguageProcessing', 'conversationManagement'],
            lastActivity: new Date().toISOString(),
            isHealthy: true,
            metrics: {}
          },
          {
            id: 'evolution-agent',
            name: 'EvolutionAgent',
            type: 'evolution',
            status: 'idle',
            capabilities: ['selfAnalysis', 'agentGeneration'],
            lastActivity: new Date().toISOString(),
            isHealthy: true,
            metrics: {}
          },
          {
            id: 'tool-agent',
            name: 'ToolAgent',
            type: 'tool',
            status: 'active',
            capabilities: ['mcpToolOrchestration', 'toolExecution'],
            lastActivity: new Date().toISOString(),
            isHealthy: true,
            metrics: {}
          }
        ]
      })
    }
  },

  sendMessage: async (message: string) => {
    const userMessage: ChatMessage = {
      id: Date.now().toString(),
      content: message,
      role: 'user',
      timestamp: new Date().toISOString()
    }
    
    set({ isLoading: true })
    get().addMessage(userMessage)
    
    try {
      const response = await agentApi.sendChatMessage(message, 'user-1')
      
      const assistantMessage: ChatMessage = {
        id: (Date.now() + 1).toString(),
        content: response.data.result?.message || 'No response',
        role: 'assistant',
        agentId: response.data.agentId,
        timestamp: new Date().toISOString(),
        metadata: response.data.metadata
      }
      
      get().addMessage(assistantMessage)
    } catch (error) {
      console.error('Failed to send message:', error)
      const errorMessage: ChatMessage = {
        id: (Date.now() + 1).toString(),
        content: 'Sorry, I encountered an error while processing your message.',
        role: 'assistant',
        timestamp: new Date().toISOString()
      }
      get().addMessage(errorMessage)
    } finally {
      set({ isLoading: false })
    }
  },

  getAgentStatus: async () => {
    try {
      const response = await agentApi.getAgentStatus()
      const agents: Agent[] = response.data.map((status: any) => ({
        id: status.agentId,
        name: status.agentId.split('-').map((part: string) => 
          part.charAt(0).toUpperCase() + part.slice(1)).join(''),
        type: status.agentId.includes('chat') ? 'chat' : 
              status.agentId.includes('evolution') ? 'evolution' : 
              status.agentId.includes('project-management') ? 'projectManagement' : 'tool',
        status: status.status === 0 ? 'idle' : 
                status.status === 1 ? 'active' : 
                status.status === 2 ? 'busy' : 
                status.status === 3 ? 'processing' : 'error',
        capabilities: [],
        lastActivity: status.lastActivity,
        isHealthy: status.isHealthy,
        metrics: status.metrics || {}
      }))
      
      set({ agents })
    } catch (error) {
      console.error('Failed to get agent status:', error)
    }
  },

  executeAgentTask: async (agentId: string, task: any) => {
    set({ isLoading: true })
    try {
      await agentApi.executeAgentTask(agentId, task)
      // Refresh agent status after task execution
      await get().getAgentStatus()
    } catch (error) {
      console.error('Failed to execute agent task:', error)
      set({ error: 'Failed to execute agent task' })
    } finally {
      set({ isLoading: false })
    }
  },

  addMessage: (message: ChatMessage) => {
    set((state) => ({
      messages: [...state.messages, message]
    }))
  },

  clearMessages: () => {
    set({ messages: [] })
  },

  setError: (error: string | null) => {
    set({ error })
  }
}))