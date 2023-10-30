using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ReactGramAPI.Data;
using ReactGramAPI.Data.Dtos;
using ReactGramAPI.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace ReactGramAPI.Services;

public class UserService
{
    private ReactgramDbContext _context;
    private IMapper _mapper;
    private UserManager<User> _userManager;
    private SignInManager<User> _signInManager;
    private TokenService _tokenService;

    public UserService(IMapper mapper, UserManager<User> userManager, SignInManager<User> signInManager, TokenService tokenService, ReactgramDbContext context)
    {
        _mapper = mapper;
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _context = context;
    }

    //Método para cadastrar um usuário no banco de dados:
    public async Task<TokenInfo> RegisterUser(RegisterUserDto userDto)
    {
        //Convertendo de CreateUserDto para User:
        User user = _mapper.Map<User>(userDto);
        //Aguardando resultado da requisição para Cadastrar objeto no banco de dados:
        IdentityResult result = await _userManager.CreateAsync(user, userDto.Password);
        //Retornando se houve algum erro nesse cadastro:
        if (!result.Succeeded)
        {
            throw new Exception("Falha ao cadastrar usuário");
        }
        //Se deu tudo certo, executando método de gerar Token para usuário cadastrado:
        TokenInfo token = _tokenService.GenerateToken(user);
        //Retornando Token:
        return token;
    }

    public async Task<ReadAuthenticationUserDto> LoginUser(LoginUserDto userDto)
    {
        //Aguardando resultado da requisição para Logar:
        SignInResult result = await _signInManager.PasswordSignInAsync(userDto.Username, userDto.Password, false, false);
        //Retornando se houve algum erro nesse login:
        if (!result.Succeeded)
        {
            throw new Exception("Usuário não autenticado");
        }
        //Encontrando usuário cadastrado no banco de dados (esse usuário vai vir com a tipagem "User", porém colocamos como variável para o compilador não reclamar, pois pode ser que a função FirstOrDefault não encontre resultado):
        var user = _signInManager
            .UserManager
            .Users
            .FirstOrDefault(user => userDto.Username.ToUpper() == user.NormalizedUserName); //Colocamos ambos os criterios de comparação em letra maiuscula para evitar problemas
        //Verificando se foi encontrado usuário de fato (apenas para que compilador não reclame):
        if (user == null)
        {
            throw new Exception("Usuário não autenticado");
        }
        //Se deu tudo certo, executando método de gerar Token para usuário cadastrado:
        TokenInfo token = _tokenService.GenerateToken(user);
        //Gerando resposta da requisição
        ReadAuthenticationUserDto response = _mapper.Map<ReadAuthenticationUserDto>(user);
        response.TokenInfo = token;
        //ReadUserDto response = new UserLoggedInfo(user.Id, token, user.ProfileImage);
        //Retornando token:
        return response;
    }

    public User GetCurrentUser(ClaimsIdentity identity)
    {
        var id = identity.FindFirst("id").Value;
        var user = _context.Users.FirstOrDefault(user => user.Id == id);
        if (user == null)
        {
            throw new Exception("Usuário não autenticado");
        }
        return user;
    }

    public User GetUserById(string id)
    {
        var user = _context.Users.FirstOrDefault(user => user.Id == id);
        if(user == null)
        {
            throw new Exception("Usuário não encontrado");
        }
        return user;
    }

    public async Task<User> UpdateUser(User user, UpdateUserDto userDto)
    {
        _mapper.Map(userDto, user);
        IdentityResult result = await _userManager.UpdateAsync(user);
        if(!result.Succeeded)
        {
            throw new Exception("Algo deu errado");
        }
        return user;
    }

    public async Task DeleteCurrentUser(User user)
    {
        IdentityResult result = await _userManager.DeleteAsync(user);
        if(!result.Succeeded)
        {
            throw new Exception("Algo deu errado");
        } 
    }

    internal List<User> SearchByUsername(string username)
    {
        try
        {
            List<User> users = _context.Users.FromSqlRaw($"SELECT * FROM AspNetUsers WHERE USERNAME LIKE '%{username}%'").ToList();
            if (users.Count == 0)
            {
                throw new Exception("Nenhum resultado encontrado");
            }
            return users;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

    }
}