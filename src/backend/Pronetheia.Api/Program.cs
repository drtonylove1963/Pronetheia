using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Pronetheia.Api.Data;
using Pronetheia.Api.Hubs;
using Pronetheia.Api.Services;
using Pronetheia.Api.Services.Agents;
using Pronetheia.Api.Services.MCP;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure the API to listen on all interfaces
builder.WebHost.UseUrls("http://0.0.0.0:80");

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/pronetheia-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Pronetheia API", 
        Version = "v0.1-SEED",
        Description = "Agent-Native Bootstrap Self-Evolution Platform API"
    });
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configure Entity Framework
builder.Services.AddDbContext<PronetheiaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key not configured"));

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ClockSkew = TimeSpan.Zero
    };
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        builder => builder
            .WithOrigins("http://localhost:3000", "http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

// Register application services
builder.Services.AddSingleton<IOpenRouterService, OpenRouterService>();
builder.Services.AddSingleton<IAgentCommunicationHub, AgentCommunicationHub>();
builder.Services.AddScoped<IMCPToolRegistry, MCPToolRegistry>();
builder.Services.AddScoped<IAgentOrchestrationService, AgentOrchestrationService>();
builder.Services.AddScoped<IEvolutionEngine, EvolutionEngine>();
builder.Services.AddScoped<ICapabilityAnalyzer, CapabilityAnalyzer>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Register Agents
builder.Services.AddScoped<ChatAgent>();
builder.Services.AddScoped<EvolutionAgent>();
builder.Services.AddScoped<ToolAgent>();
builder.Services.AddScoped<ProjectManagementAgent>(); // ✨ First Evolved Agent - Created by EvolutionAgent Phase 0.3

// Register MCP Tools
builder.Services.AddScoped<FileOperationsMCP>();
builder.Services.AddScoped<CodeGenerationMCP>();
builder.Services.AddScoped<AnalysisMCP>();
builder.Services.AddScoped<DatabaseMCP>();
builder.Services.AddScoped<WebSearchMCP>();

// Add SignalR for real-time communication
builder.Services.AddSignalR();

// Add HttpClient for external API calls
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pronetheia API v0.1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<AgentHub>("/hubs/agent");

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PronetheiaDbContext>();
    try
    {
        // Apply migrations - this will create database if it doesn't exist
        dbContext.Database.Migrate();
        Log.Information("Database migration completed successfully");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "An error occurred while migrating the database");
        // Log the connection string (without password) for debugging
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        var safeConnectionString = connectionString?.Replace("Password=PronetheiaDB2024!", "Password=***");
        Log.Information("Connection string: {ConnectionString}", safeConnectionString);
    }
}

// Initialize Agent System
using (var scope = app.Services.CreateScope())
{
    var agentOrchestration = scope.ServiceProvider.GetRequiredService<IAgentOrchestrationService>();
    await agentOrchestration.InitializeAgents();
    Log.Information("Agent system initialized successfully");
}

Log.Information("Pronetheia API started successfully");

app.Run();