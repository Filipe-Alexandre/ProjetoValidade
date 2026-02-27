using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models; // Namespace necessário para as definições de segurança do Swagger
using System.Text;
using ValiKop.Api.Data;
using ValiKop.Api.Middlewares;
using ValiKop.Api.Services;
using ValiKop.Shared.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// --- PASSO 1: CONFIGURAÇÃO DO BANCO DE DADOS ---
// Registra o DbContext usando a ConnectionString definida no appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- PASSO 2: INJEÇÃO DE DEPENDÊNCIA (SERVICES) ---
// Registra as suas classes de negócio para que possam ser usadas nas Controllers
builder.Services.AddScoped<IProdutoService, ProdutoService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ISalgadoService, SalgadoService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

// --- PASSO 3: CONFIGURAÇÃO DE AUTENTICAÇÃO JWT ---
// O uso do "?? " evita o erro de ArgumentNullException se a chave não estiver no JSON
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// --- PASSO 4: CONFIGURAÇÃO DO SWAGGER (CADEADO) ---
// Ajustado para as versões 10.x dos pacotes para evitar erro de 'Reference'
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ValiKop API", Version = "v1" });

    // Definimos o esquema de segurança Bearer
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Informe o token JWT. Exemplo: Bearer {seu_token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", securityScheme);

    // Aplica a exigência do cadeado em todos os endpoints
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});

// --- PASSO 5: POLÍTICA DE CORS ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCors", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddAuthorization();
builder.Services.AddControllers();

var app = builder.Build();

// --- PASSO 6: PIPELINE DE MIDDLEWARES (A ORDEM IMPORTA!) ---

// Habilita o Swagger apenas em ambiente de desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Middleware de CORS deve vir antes da Autenticação
app.UseCors("DefaultCors");

// Middleware de tratamento global de erros
app.UseMiddleware<ExceptionMiddleware>();

// Ordem obrigatória: Autenticação ANTES da Autorização
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();