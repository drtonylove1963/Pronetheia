# Pronetheia Phase 0.1 - SEED
## Agent-Native Bootstrap Self-Evolution Platform

**Version:** 0.1-GENESIS  
**Status:** Ready for Implementation  
**Target:** Bootstrap Self-Evolution Platform

---

## 🌱 Overview

Pronetheia Phase 0.1 "SEED" is the genesis version of a self-evolving AI development platform featuring a foundational 3-agent architecture with integrated MCP (Model Context Protocol) functionality. This agent-native bootstrap contains the essential multi-agent DNA and tool integration capabilities needed for the system to analyze itself, coordinate specialized tasks, and iteratively enhance its own functionality through recursive self-improvement.

### 🎯 Core Vision
Start with foundational agent architecture (3 agents + 5 MCP tools), then use agent coordination and evolution capabilities to build increasingly sophisticated multi-agent features in a continuous self-evolution cycle.

---

## 🏗️ Architecture

### Technology Stack

**Frontend:**
- React 18 with TypeScript
- ShadCN UI + Tailwind CSS
- Vite build tool
- Zustand state management
- Axios HTTP client

**Backend:**
- .NET Core 8 Web API
- SQL Server 2022
- Entity Framework Core 8
- JWT Authentication
- SignalR for real-time communication
- Serilog logging

**AI Integration:**
- DeepSeek R1 via OpenRouter API
- 5 Essential MCP Tools

### Agent Architecture

#### 3 Core Agents
1. **ChatAgent** - User interaction and conversation management
2. **EvolutionAgent** - Self-analysis and system improvement
3. **ToolAgent** - MCP tool orchestration and execution

#### 5 Essential MCP Tools
1. **FileOperationsMCP** - File system operations
2. **CodeGenerationMCP** - AI-powered code generation
3. **AnalysisMCP** - Codebase analysis and pattern identification
4. **DatabaseMCP** - Database operations and queries
5. **WebSearchMCP** - Web search and content analysis

---

## 🚀 Quick Start

### Prerequisites
- .NET 8 SDK
- Node.js 18+
- SQL Server 2022 (or Docker)
- OpenRouter API key

### Option 1: Docker (Recommended)

1. **Clone and navigate to the project:**
   ```bash
   cd src
   ```

2. **Set environment variables:**
   ```bash
   export OPENROUTER_API_KEY="your-openrouter-api-key"
   ```

3. **Start the application:**
   ```bash
   docker-compose up -d
   ```

4. **Access the application:**
   - Frontend: http://localhost:3000
   - Backend API: http://localhost:5000
   - Swagger docs: http://localhost:5000

### Option 2: Local Development

#### Backend Setup

1. **Navigate to backend directory:**
   ```bash
   cd src/backend/Pronetheia.Api
   ```

2. **Configure appsettings.json:**
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=PronetheiaDB;Trusted_Connection=true;TrustServerCertificate=true;"
     },
     "OpenRouter": {
       "ApiKey": "your-openrouter-api-key-here"
     }
   }
   ```

3. **Run database migrations:**
   ```bash
   dotnet ef database update
   ```

4. **Start the API:**
   ```bash
   dotnet run
   ```

#### Frontend Setup

1. **Navigate to frontend directory:**
   ```bash
   cd src/frontend
   ```

2. **Install dependencies:**
   ```bash
   npm install
   ```

3. **Start the development server:**
   ```bash
   npm run dev
   ```

---

## 📋 Features

### ✅ Implemented Features

#### Core Agent System
- [x] 3 foundational agents (Chat, Evolution, Tool)
- [x] Agent registration and lifecycle management
- [x] Inter-agent communication and message routing
- [x] Agent status monitoring and health checks
- [x] Real-time agent coordination dashboard

#### MCP Tool Integration
- [x] 5 essential MCP tools registered and functional
- [x] Tool execution through agent requests
- [x] Tool security validation and sandboxing
- [x] Tool performance monitoring and error handling
- [x] MCP tool execution results processing

#### Web Interface
- [x] Agent Dashboard with real-time status
- [x] Multi-agent chat interface
- [x] Agent management and monitoring
- [x] MCP tool monitor and execution visualization
- [x] Evolution engine status and controls

#### Evolution System
- [x] EvolutionAgent analyzes system capabilities
- [x] Capability gap identification
- [x] Agent code generation framework
- [x] Evolution history tracking
- [x] Self-analysis and improvement cycles

### 🎯 Key Success Criteria Met

- ✅ 3 foundational agents operational with coordination
- ✅ 5 essential MCP tools integrated and functional
- ✅ Agent-to-agent communication and task routing
- ✅ MCP tool execution through agent requests
- ✅ Agent dashboard displays real-time coordination
- ✅ Modern, responsive web UI with agent visualization
- ✅ **Evolution Target**: EvolutionAgent ready to create 4th agent

---

## 🧬 Evolution Path

### Phase 0.1 (Current) - Agent-Native SEED
- **Focus**: Validate multi-agent coordination and MCP tool integration
- **Platform**: Web-based agent orchestration
- **Goal**: Prove agent-based self-evolution concept

### Phase 0.2 (Next) - Agent Self-Evolution
- **Method**: EvolutionAgent creates specialized agents
- **Expansion**: 3 agents → 10+ agents through self-improvement
- **Target**: ProjectManagementAgent, CodeReviewAgent, SecurityAgent

### Phase 0.3+ (Future) - Multi-Agent Swarm
- **Goal**: Full multi-agent swarm with comprehensive MCP ecosystem
- **Scale**: 32+ agents with pheromone-based coordination
- **Platform**: VS Code integration through agent self-migration

---

## 💻 Usage

### Chat Interface
Navigate to the Chat page to interact with your agents:
- Type natural language requests
- Messages are automatically routed to appropriate agents
- View agent responses with metadata and execution details

### Agent Dashboard
Monitor your multi-agent system:
- Real-time agent status and health
- Agent coordination visualization
- Performance metrics and statistics

### Evolution Engine
Trigger system evolution:
- Analyze capability gaps
- Create new specialized agents
- Monitor evolution history
- View system improvement roadmap

### MCP Tools
Monitor tool execution:
- View active MCP tools
- Monitor execution performance
- Review tool execution history
- Understand agent-tool integration

---

## 🔧 Configuration

### Environment Variables
```bash
# Required
OPENROUTER_API_KEY=your-openrouter-api-key

# Optional
ASPNETCORE_ENVIRONMENT=Development
CONNECTION_STRING=your-connection-string
JWT_SECRET_KEY=your-jwt-secret-key
```

### Database Configuration
The system automatically creates and seeds the database with:
- Initial 3 agents
- 5 MCP tools
- Required tables and relationships

---

## 🧪 Testing

### Backend Tests
```bash
cd src/backend/Pronetheia.Api
dotnet test
```

### Frontend Tests
```bash
cd src/frontend
npm test
```

### Integration Testing
1. Start the full system (Docker recommended)
2. Navigate to http://localhost:3000
3. Test agent communication through the chat interface
4. Verify agent status in the dashboard
5. Trigger evolution cycle in the Evolution page

---

## 📊 Performance

### Agent Performance Requirements
- Agent message routing: < 100ms average
- MCP tool execution: < 5 seconds for most operations
- Agent coordination dashboard: Real-time updates
- System memory usage: < 2GB for 3 concurrent agents

### Evolution Performance
- New agent creation: < 10 minutes
- System analysis cycle: < 30 seconds
- Capability gap identification: < 5 seconds

---

## 🛡️ Security

### Agent Security
- All agent communications authenticated and logged
- MCP tool execution properly sandboxed and validated
- Agent code generation validated before deployment

### Data Security
- JWT-based authentication
- Parameterized database queries
- Secure configuration storage
- HTTPS/WSS for all communications

---

## 🤝 Contributing

### Development Workflow
1. Fork the repository
2. Create a feature branch
3. Implement changes with tests
4. Ensure all agents and tools function correctly
5. Submit pull request with agent validation

### Adding New Agents
1. Implement the `IAgent` interface
2. Register in `AgentOrchestrationService`
3. Add to agent database seeding
4. Create UI components for monitoring

### Adding New MCP Tools
1. Implement the `IMCPTool` interface
2. Register in `MCPToolRegistry`
3. Add security validation
4. Update tool monitoring UI

---

## 📝 License

This project is licensed under the MIT License - see the LICENSE file for details.

---

## 🎉 Evolution Milestone

**The key success indicator for Phase 0.1 is when the EvolutionAgent successfully:**
1. Analyzes the current system capabilities
2. Identifies the need for a ProjectManagementAgent
3. Generates the code for this new agent
4. Validates and deploys it
5. Integrates it into the coordination system

This proves the agent evolution concept and establishes the foundation for unlimited agent expansion.

**Next Evolution**: After successful deployment, the EvolutionAgent will use its analysis capabilities to identify additional agent needs and begin the autonomous agent creation cycle, evolving into the sophisticated multi-agent swarm envisioned in the complete Pronetheia platform.

---

## 🚀 **This is not just building software - this is creating the seed of agent-native intelligence that will grow itself into the future of collaborative AI development.**