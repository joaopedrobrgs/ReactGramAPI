using Microsoft.IdentityModel.Tokens;
using ReactGramAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ReactGramAPI.Services;

public class TokenService
{

    private IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    //Método para gerar um token para usuário:
    public TokenInfo GenerateToken(User user)
    {
        //Informações que estarão armazenadas no TOKEN:
        Claim[] claims = new Claim[]
        {
            new Claim("id", user.Id),
            new Claim("username", user.UserName),
            new Claim ("email", user.Email),
            new Claim ("loginTimeStamp", DateTime.UtcNow.ToString())
        };

        //Tempo de expiração do token:
        DateTime expiresTime = DateTime.Now.AddDays(1);

        //Credenciais de Login:
        ////Gerando chave:
        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_configuration["SymmetricSecurityKey"]));
        ////Utilizando chave para gerar um credencial:
        SigningCredentials signingCredentials = new(key, SecurityAlgorithms.HmacSha256);

        //Gerando Token:
        JwtSecurityToken token = new(
                expires: expiresTime,
                claims: claims,
                signingCredentials: signingCredentials
            );

        //Retornando token na forma de string:
        //return new JwtSecurityTokenHandler().WriteToken(token);

        //Retornando token na forma do modelo TokenInfo, que é um objeto que criamos que retorna o token e sua data de expiração:
        return new TokenInfo(new JwtSecurityTokenHandler().WriteToken(token), expiresTime);
    }
}
