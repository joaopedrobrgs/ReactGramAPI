//Fazendo importa��es:
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
//Adicionandos controllers que n�s criamos:
builder.Services.AddControllers();
//Fazendo conex�o com o banco de dados:
var connectionString = builder.Configuration["ConnectionStrings:ReactgramConnection"];
builder.Services.AddDbContext<ReactgramDbContext>(opts =>
{
    opts.UseLazyLoadingProxies().UseSqlServer(connectionString);
});
//Servi�o do AutoMapper (utiliza��o de DTOs):
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
//Servi�os do Identity (utilizando token para autentica��o do usu�rio):
builder.Services
    .AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ReactgramDbContext>()
    .AddDefaultTokenProviders();
//Fazendo a inje��o do TOKEN:
builder.Services.AddAuthentication(options =>
{
    //Passando algumas options para esse TOKEN:
    //Definindo qual vai ser o schema que o nosso TOKEN ir� utilizar:
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    //Fazendo a inje��o do tipo de token que iremos utilizar, que nesse caso ser� o bearer token:
}).AddJwtBearer(options =>
{
    //Passando algumas options para esse bearer token:
    //Definindo quais ser�o os par�metros que ser�o v�lidados para esse token:
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        //1� par�metro: ValidateIssuerSigningKey - validar chave de seguran�a que n�s defimos na hora de criar o token (deve ser igual a que est� l� no TokenService):
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["SymmetricSecurityKey"])),
        //2� par�metro: ValidateAudience - evita ataques de redirecionamento:
        ValidateAudience = false,
        //3� par�metro: ValidateIssuer - forma de seguran�a para trafego do token via web:
        ValidateIssuer = false,
        //4� par�metro: ClockSkew - determina o alinhamento do rel�gio que estamos utilizando para quest�o de valida��o de tempo do token:
        ClockSkew = TimeSpan.Zero
    };
});
//Servi�o do NewtonsoftJSON (para fazer requisi��es do tipo PATCH):
builder.Services.AddControllers().AddNewtonsoftJson();
//Servi�os do swagger:
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//Servi�os personalizados:
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
