using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Pronetheia.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Agents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "idle"),
                    Capabilities = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "[]"),
                    Configuration = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastActivity = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CapabilityGaps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GapType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "identified"),
                    ProposedSolution = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstimatedEffort = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TargetPlatform = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "web"),
                    IdentifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CapabilityGaps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EvolutionHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EvolutionType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TriggerReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Changes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Result = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Success = table.Column<bool>(type: "bit", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvolutionHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MCPTools",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InputSchema = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OutputSchema = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SecurityLevel = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "safe"),
                    ExecutionTimeout = table.Column<int>(type: "int", nullable: false, defaultValue: 30000),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Version = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MCPTools", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AgentCommunications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromAgentId = table.Column<int>(type: "int", nullable: false),
                    ToAgentId = table.Column<int>(type: "int", nullable: true),
                    MessageType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "pending"),
                    RequiresResponse = table.Column<bool>(type: "bit", nullable: false),
                    ResponseToId = table.Column<int>(type: "int", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentCommunications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgentCommunications_AgentCommunications_ResponseToId",
                        column: x => x.ResponseToId,
                        principalTable: "AgentCommunications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AgentCommunications_Agents_FromAgentId",
                        column: x => x.FromAgentId,
                        principalTable: "Agents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AgentCommunications_Agents_ToAgentId",
                        column: x => x.ToAgentId,
                        principalTable: "Agents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AgentMetrics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AgentId = table.Column<int>(type: "int", nullable: false),
                    MetricName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MetricValue = table.Column<decimal>(type: "decimal(10,4)", precision: 10, scale: 4, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    RecordedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Context = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentMetrics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgentMetrics_Agents_AgentId",
                        column: x => x.AgentId,
                        principalTable: "Agents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatSessions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BasePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MCPToolExecutions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ToolId = table.Column<int>(type: "int", nullable: false),
                    RequestingAgentId = table.Column<int>(type: "int", nullable: false),
                    InputParameters = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OutputResult = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "pending"),
                    ExecutionTime = table.Column<int>(type: "int", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MCPToolExecutions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MCPToolExecutions_Agents_RequestingAgentId",
                        column: x => x.RequestingAgentId,
                        principalTable: "Agents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MCPToolExecutions_MCPTools_ToolId",
                        column: x => x.ToolId,
                        principalTable: "MCPTools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SessionId = table.Column<int>(type: "int", nullable: false),
                    AgentId = table.Column<int>(type: "int", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMessages_Agents_AgentId",
                        column: x => x.AgentId,
                        principalTable: "Agents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ChatMessages_ChatSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "ChatSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Hash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectFiles_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Insert seed data
            migrationBuilder.InsertData(
                table: "Agents",
                columns: new[] { "Id", "Name", "Type", "Status", "Capabilities", "IsActive" },
                values: new object[,]
                {
                    { 1, "ChatAgent", "chat", "idle", "[\"naturalLanguageProcessing\",\"conversationManagement\",\"userIntentRecognition\",\"agentCoordination\",\"contextRetention\"]", true },
                    { 2, "EvolutionAgent", "evolution", "idle", "[\"selfAnalysis\",\"capabilityGapIdentification\",\"agentGeneration\",\"codeGeneration\",\"systemOptimization\"]", true },
                    { 3, "ToolAgent", "tool", "idle", "[\"mcpToolOrchestration\",\"toolExecution\",\"resultProcessing\",\"securityValidation\",\"performanceMonitoring\"]", true }
                });

            migrationBuilder.InsertData(
                table: "MCPTools",
                columns: new[] { "Id", "Name", "Category", "Description", "InputSchema", "OutputSchema", "SecurityLevel", "ExecutionTimeout", "IsActive" },
                values: new object[,]
                {
                    { 1, "FileOperationsMCP", "file_ops", "File system operations including create, read, update, delete, and directory listing", "{\"type\":\"object\",\"properties\":{\"operation\":{\"type\":\"string\",\"enum\":[\"create\",\"read\",\"update\",\"delete\",\"list\"]},\"path\":{\"type\":\"string\"},\"content\":{\"type\":\"string\"}}}", "{\"type\":\"object\",\"properties\":{\"success\":{\"type\":\"boolean\"},\"result\":{\"type\":\"any\"},\"error\":{\"type\":\"string\"}}}", "elevated", 10000, true },
                    { 2, "CodeGenerationMCP", "code_gen", "AI-powered code generation, analysis, refactoring, test generation, and syntax validation", "{\"type\":\"object\",\"properties\":{\"action\":{\"type\":\"string\",\"enum\":[\"generate\",\"analyze\",\"refactor\",\"test\",\"validate\"]},\"prompt\":{\"type\":\"string\"},\"language\":{\"type\":\"string\"},\"code\":{\"type\":\"string\"}}}", "{\"type\":\"object\",\"properties\":{\"success\":{\"type\":\"boolean\"},\"code\":{\"type\":\"string\"},\"analysis\":{\"type\":\"object\"},\"error\":{\"type\":\"string\"}}}", "safe", 60000, true },
                    { 3, "AnalysisMCP", "analysis", "Codebase analysis including pattern identification, capability assessment, dependency analysis, and metrics calculation", "{\"type\":\"object\",\"properties\":{\"analysisType\":{\"type\":\"string\",\"enum\":[\"codebase\",\"patterns\",\"capabilities\",\"dependencies\",\"metrics\"]},\"targetPath\":{\"type\":\"string\"},\"options\":{\"type\":\"object\"}}}", "{\"type\":\"object\",\"properties\":{\"success\":{\"type\":\"boolean\"},\"analysis\":{\"type\":\"object\"},\"error\":{\"type\":\"string\"}}}", "safe", 30000, true },
                    { 4, "DatabaseMCP", "database", "Database operations including query execution, schema management, and CRUD operations", "{\"type\":\"object\",\"properties\":{\"operation\":{\"type\":\"string\",\"enum\":[\"query\",\"create\",\"insert\",\"update\",\"delete\"]},\"query\":{\"type\":\"string\"},\"table\":{\"type\":\"string\"},\"data\":{\"type\":\"object\"}}}", "{\"type\":\"object\",\"properties\":{\"success\":{\"type\":\"boolean\"},\"result\":{\"type\":\"any\"},\"rowsAffected\":{\"type\":\"number\"},\"error\":{\"type\":\"string\"}}}", "elevated", 15000, true },
                    { 5, "WebSearchMCP", "web_search", "Web search and content analysis including search, content fetching, website analysis, and documentation lookup", "{\"type\":\"object\",\"properties\":{\"action\":{\"type\":\"string\",\"enum\":[\"search\",\"fetch\",\"analyze\",\"documentation\",\"trends\"]},\"query\":{\"type\":\"string\"},\"url\":{\"type\":\"string\"},\"options\":{\"type\":\"object\"}}}", "{\"type\":\"object\",\"properties\":{\"success\":{\"type\":\"boolean\"},\"results\":{\"type\":\"array\"},\"content\":{\"type\":\"string\"},\"analysis\":{\"type\":\"object\"},\"error\":{\"type\":\"string\"}}}", "safe", 20000, true }
                });

            // Create indexes
            migrationBuilder.CreateIndex(
                name: "IX_AgentCommunications_FromAgentId",
                table: "AgentCommunications",
                column: "FromAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_AgentCommunications_ResponseToId",
                table: "AgentCommunications",
                column: "ResponseToId");

            migrationBuilder.CreateIndex(
                name: "IX_AgentCommunications_ToAgentId",
                table: "AgentCommunications",
                column: "ToAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_AgentMetrics_AgentId",
                table: "AgentMetrics",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_AgentId",
                table: "ChatMessages",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_SessionId",
                table: "ChatMessages",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatSessions_UserId",
                table: "ChatSessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MCPToolExecutions_RequestingAgentId",
                table: "MCPToolExecutions",
                column: "RequestingAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_MCPToolExecutions_ToolId",
                table: "MCPToolExecutions",
                column: "ToolId");

            migrationBuilder.CreateIndex(
                name: "IX_MCPTools_Name",
                table: "MCPTools",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectFiles_ProjectId",
                table: "ProjectFiles",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_UserId",
                table: "Projects",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AgentCommunications");

            migrationBuilder.DropTable(
                name: "AgentMetrics");

            migrationBuilder.DropTable(
                name: "CapabilityGaps");

            migrationBuilder.DropTable(
                name: "ChatMessages");

            migrationBuilder.DropTable(
                name: "EvolutionHistories");

            migrationBuilder.DropTable(
                name: "MCPToolExecutions");

            migrationBuilder.DropTable(
                name: "ProjectFiles");

            migrationBuilder.DropTable(
                name: "ChatSessions");

            migrationBuilder.DropTable(
                name: "Agents");

            migrationBuilder.DropTable(
                name: "MCPTools");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}