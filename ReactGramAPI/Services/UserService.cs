using AutoMapper;
using Microsoft.AspNetCore.Identity;
using ReactGramAPI.Data.Dtos;
using ReactGramAPI.Models;

namespace ReactGramAPI.Services;

public class UserService
{
    private IMapper _mapper;
    private UserManager<User> _userManager;
    private SignInManager<User> _signInManager;
    private TokenService _tokenService;

    public UserService(IMapper mapper, UserManager<User> userManager, SignInManager<User> signInManager, TokenService tokenService)
    {
        _mapper = mapper;
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    //Método para cadastrar um usuário no banco de dados:
    public async Task<TokenInfo> RegisterUser(CreateUserDto userDto)
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

    public async Task<TokenInfo> LoginUser(LoginUserDto userDto)
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
        if(user == null)
        {
            throw new Exception("Usuário não autenticado");
        }
        //Se deu tudo certo, executando método de gerar Token para usuário cadastrado:
        TokenInfo token = _tokenService.GenerateToken(user);
        //Retornando token:
        return token;
    }
}
