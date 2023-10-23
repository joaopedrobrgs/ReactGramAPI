using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using ReactGramAPI.Data.Dtos;
using ReactGramAPI.Models;
using ReactGramAPI.Services;
using System.Security.Claims;

namespace ReactGramAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{

    private UserService _userService;
    private IMapper _mapper;

    public UserController(UserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto userDto)
    {
        TokenInfo token = await _userService.RegisterUser(userDto);
        return Ok(token);
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> LoginUser([FromBody] LoginUserDto userDto)
    {
        ReadAuthenticationUserDto user = await _userService.LoginUser(userDto);
        return Ok(user);
    }

    [HttpGet]
    [Authorize]
    [Route("")]
    public IActionResult GetCurrentUser()
    {
        ClaimsIdentity identity = HttpContext.User.Identity as ClaimsIdentity;
        if (identity == null)
        {
            throw new Exception("Error");
        }
        User user = _userService.GetCurrentUser(identity);
        //return Ok(_mapper.Map<ReadLoggedUserData>(user));
        return Ok(user);
    }

    [HttpGet("{id}")]
    [Route("")]   
    public IActionResult GetUserById(string id)
    {
        ReadUserDto userInfo = _userService.GetUserById(id);
        return Ok(userInfo);
    }

    [HttpPut]
    [Authorize]
    [Route("")]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto updateUserDto)
    {
        ClaimsIdentity identity = HttpContext.User.Identity as ClaimsIdentity;
        if (identity == null)
        {
            throw new Exception("Error");
        }
        User user = _userService.GetCurrentUser(identity);
        User updatedUser = await _userService.UpdateUser(user, updateUserDto);
        return Ok(_mapper.Map<ReadLoggedUserDto>(updatedUser));
    }

    [HttpPatch]
    [Authorize]
    [Route("")]
    public async Task<IActionResult> PartialUpdateUser([FromBody] JsonPatchDocument<UpdateUserDto> patch)
    {
        //Pegando Claims que estão salvas no TOKEN:
        ClaimsIdentity identity = HttpContext.User.Identity as ClaimsIdentity;
        if (identity == null)
        {
            throw new Exception("Error");
        }
        //Pegando usuário logado através desse TOKEN:
        User user = _userService.GetCurrentUser(identity);
        //Convertendo de tipo User para UpdateUserDto:
        UpdateUserDto userDto = _mapper.Map<UpdateUserDto>(user);
        //Validando se o patch enviado pela requisição é válido para atualizar o usuário e aplicando mudanças ao userDto:
        patch.ApplyTo(userDto, ModelState);
        //Validando mudança:
        if (!TryValidateModel(userDto))
        {
            //Se não for válida, retornaremos um erro de validação (normalmente ocorre quando JSON enviado pelo usuário não condiz com que o modelo espera)
            return ValidationProblem(ModelState);
        }
        //Se for válida, executar o método de fazer esse update:
        User updatedUser = await _userService.UpdateUser(user, userDto);
        //Retornando usuário atualizado:
        return Ok(_mapper.Map<ReadLoggedUserDto>(updatedUser));
    }

    [HttpDelete]
    [Authorize]
    [Route("")]
    public async Task<IActionResult> DeleteCurrentUser()
    {
        //Pegando Claims que estão salvas no TOKEN:
        ClaimsIdentity identity = HttpContext.User.Identity as ClaimsIdentity;
        if (identity == null)
        {
            throw new Exception("Error");
        }
        //Pegando usuário logado através desse TOKEN:
        User user = _userService.GetCurrentUser(identity);
        //Executando método de deletar usuário:
        await _userService.DeleteCurrentUser(user);
        //Retornando resposta ao usuário (ação de no content - código 204):
        return NoContent();
    }
}
