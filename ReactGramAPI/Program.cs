//Fazendo importações:
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ReactGramAPI.Data;
using ReactGramAPI.Models;
using ReactGramAPI.Services;
using System.Text;

////Criando builder:
var builder = WebApplication.CreateBuilder(args);

////Add services to the container.
//Adicionandos controllers que nós criamos:
builder.Services.AddControllers();
//Fazendo conexão com o banco de dados:
var connectionString = builder.Configuration["ConnectionStrings:ReactgramConnection"];
builder.Services.AddDbContext<ReactgramDbContext>(opts =>
{
    opts.UseLazyLoadingProxies().UseSqlServer(connectionString);
});
//Serviço do AutoMapper (utilização de DTOs):
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
//Serviços do Identity (utilizando token para autenticação do usuário):
builder.Services
    .AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ReactgramDbContext>()
    .AddDefaultTokenProviders();
//Fazendo a injeção do TOKEN:
builder.Services.AddAuthentication(options =>
{
    //Passando algumas options para esse TOKEN:
    //Definindo qual vai ser o schema que o nosso TOKEN irá utilizar:
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    //Fazendo a injeção do tipo de token que iremos utilizar, que nesse caso será o bearer token:
}).AddJwtBearer(options =>
{
    //Passando algumas options para esse bearer token:
    //Definindo quais serão os parâmetros que serão válidados para esse token:
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        //1º parâmetro: ValidateIssuerSigningKey - validar chave de segurança que nós defimos na hora de criar o token (deve ser igual a que está lá no TokenService):
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["SymmetricSecurityKey"])),
        //2º parâmetro: ValidateAudience - evita ataques de redirecionamento:
        ValidateAudience = false,
        //3º parâmetro: ValidateIssuer - forma de segurança para trafego do token via web:
        ValidateIssuer = false,
        //4º parâmetro: ClockSkew - determina o alinhamento do relógio que estamos utilizando para questão de validação de tempo do token:
        ClockSkew = TimeSpan.Zero
    };
});
//Serviço do NewtonsoftJSON (para fazer requisições do tipo PATCH):
builder.Services.AddControllers().AddNewtonsoftJson();
//Serviços do swagger:
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//Serviços personalizados:
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TokenService>();

////Criando servidor:
var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
