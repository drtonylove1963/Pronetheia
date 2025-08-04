# Pronetheia 🤖

**Agent-Native Bootstrap Self-Evolution Platform**

[![CI/CD Pipeline](https://github.com/drtonylove1963/Pronetheia/actions/workflows/ci-cd.yml/badge.svg)](https://github.com/drtonylove1963/Pronetheia/actions/workflows/ci-cd.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET 8](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![React 18](https://img.shields.io/badge/React-18-blue.svg)](https://reactjs.org/)
[![Docker](https://img.shields.io/badge/Docker-Enabled-blue.svg)](https://www.docker.com/)

> *A revolutionary multi-agent system that autonomously evolves itself by creating new specialized agents based on identified capability gaps.*

## 🌟 Overview

Pronetheia is a cutting-edge **Agent-Native Bootstrap Self-Evolution Platform** that demonstrates autonomous AI system evolution. Starting with 3 core agents, the system can identify capability gaps and autonomously create new specialized agents to fill those gaps, effectively evolving itself without human intervention.

### 🎯 Current Status: Phase 0.2 - Production Deployment

**✅ DEPLOYED & OPERATIONAL**
- 3-Agent Architecture: ChatAgent, EvolutionAgent, ToolAgent
- 5 MCP Tools for comprehensive system capabilities
- Full Docker containerization with CI/CD pipeline
- OpenRouter API integration with DeepSeek R1
- Real-time React frontend with SignalR communication

**🚀 Next: Phase 0.3 - First Evolution Cycle**
Ready for EvolutionAgent to autonomously create ProjectManagementAgent (3→4 agents)

## 🏗️ Architecture

### Core Agents (3)
- **🗣️ ChatAgent** - Natural language interaction and user communication
- **🧬 EvolutionAgent** - Analyzes capability gaps and creates new agents
- **🔧 ToolAgent** - Manages and executes MCP tools

### MCP Tools (5)
- **📁 FileOperationsMCP** - File system operations
- **💻 CodeGenerationMCP** - Dynamic code generation
- **📊 AnalysisMCP** - System analysis and metrics
- **🗄️ DatabaseMCP** - Data persistence operations
- **🌐 WebSearchMCP** - External information retrieval

### Technology Stack

| Component | Technology | Version |
|-----------|------------|---------|
| **Backend** | .NET Core | 8.0 |
| **Frontend** | React + TypeScript | 18.x |
| **Database** | SQL Server | 2022 |
| **AI Gateway** | OpenRouter → DeepSeek R1 | Latest |
| **Real-time** | SignalR | 8.0 |
| **Containerization** | Docker + Compose | Latest |
| **CI/CD** | GitHub Actions | Latest |
| **Testing** | Puppeteer | Latest |

## 🚀 Quick Start

### Prerequisites
- Docker & Docker Compose
- OpenRouter API Key
- Git

### 1. Clone Repository
```bash
git clone https://github.com/drtonylove1963/Pronetheia.git
cd Pronetheia
```

### 2. Configure Environment
```bash
cd src
cp .env.example .env
```

Edit `.env` and add your OpenRouter API key:
```env
OPENROUTER_API_KEY=your-openrouter-api-key-here
```

### 3. Deploy with Docker
```bash
docker-compose up -d
```

### 4. Access Applications
- **Frontend**: http://localhost:3000
- **API**: http://localhost:6789
- **Swagger UI**: http://localhost:6789/swagger
- **Database**: localhost:1433

### 5. Verify Deployment
```bash
# Run integration tests
cd tests
npm install
npm test
```

## 🧪 Testing

### Automated Testing Suite
The project includes comprehensive testing:

```bash
# Backend .NET Tests
cd src/backend/Pronetheia.Api
dotnet test

# Frontend Build Test
cd src/frontend
npm run build

# End-to-End Integration Tests
cd tests
npm test
```

### Manual Testing
1. **Chat Interface**: Navigate to http://localhost:3000/chat
2. **Agent Status**: Check /agents page for agent health
3. **Evolution Monitoring**: Watch /evolution page for autonomous agent creation

## 🔄 Evolution Process

Pronetheia's unique self-evolution capability works through:

1. **Gap Analysis**: EvolutionAgent continuously monitors system capabilities
2. **Agent Generation**: Creates new agent specifications when gaps are identified  
3. **Code Generation**: Uses CodeGenerationMCP to implement new agents
4. **Integration**: Automatically integrates new agents into the system
5. **Validation**: Tests new agents before activation

### First Evolution Target
The system is primed to create its first evolved agent:
- **ProjectManagementAgent**: Will handle project planning, task coordination, and workflow management

## 📊 System Metrics

### Current System Status
- **Agents**: 3 Active (ChatAgent, EvolutionAgent, ToolAgent)
- **MCP Tools**: 5 Operational
- **Evolution Cycles**: 0 (Ready for first evolution)
- **Deployment**: Production Ready with CI/CD
- **API Endpoints**: 15+ RESTful endpoints
- **Real-time Channels**: SignalR hub active

## 🛠️ Development

### Local Development Setup
```bash
# Backend
cd src/backend/Pronetheia.Api
dotnet run

# Frontend  
cd src/frontend
npm install
npm run dev

# Database
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=PronetheiaDB2024!" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest
```

### CI/CD Pipeline
Automated GitHub Actions workflow includes:
- ✅ .NET Build & Test
- ✅ React Build & Test  
- ✅ Docker Image Building
- ✅ Integration Testing
- ✅ Deployment to Container Registry

## 📈 Roadmap

### Phase 0.3 - First Evolution Cycle
- [ ] Trigger autonomous agent creation
- [ ] ProjectManagementAgent implementation
- [ ] Validate 4-agent system operation

### Phase 0.4 - Multi-Evolution
- [ ] Support multiple simultaneous agent creation
- [ ] Advanced capability gap detection
- [ ] Agent specialization optimization

### Phase 1.0 - Production Evolution Platform
- [ ] Agent marketplace and sharing
- [ ] Advanced evolution algorithms
- [ ] Enterprise deployment features

## 🔐 Security

- JWT-based authentication
- API rate limiting
- Input validation and sanitization
- Secure environment variable management
- Docker security best practices

## 📚 Documentation

- **[Agent Specifications](docs/complete_agent_specifications.md)** - Detailed agent documentation
- **[MCP Integration](docs/mcp_integration_prd.md)** - Tool integration guide
- **[Evolution Process](docs/pronetheia-phase-01-prd.md)** - Self-evolution mechanics
- **[API Reference](src/backend/Pronetheia.Api/Controllers/)** - REST API documentation

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- **OpenRouter** for AI model access infrastructure
- **DeepSeek** for the R1 reasoning model
- **Microsoft** for .NET 8 and SQL Server
- **Meta** for React ecosystem
- **Docker** for containerization platform

## 📞 Support

- **Issues**: [GitHub Issues](https://github.com/drtonylove1963/Pronetheia/issues)
- **Discussions**: [GitHub Discussions](https://github.com/drtonylove1963/Pronetheia/discussions)
- **Documentation**: [Project Wiki](https://github.com/drtonylove1963/Pronetheia/wiki)

---

**🧬 "Evolving AI systems, one agent at a time."** - Pronetheia Team

[![Pronetheia Evolution](https://img.shields.io/badge/Evolution-Ready-green.svg)](#)
[![Agents](https://img.shields.io/badge/Agents-3%20Active-blue.svg)](#)
[![Status](https://img.shields.io/badge/Status-Production-green.svg)](#)