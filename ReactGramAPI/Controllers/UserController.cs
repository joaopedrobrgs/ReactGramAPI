using Microsoft.AspNetCore.Mvc;
using ReactGramAPI.Data.Dtos;
using ReactGramAPI.Models;
using ReactGramAPI.Services;

namespace ReactGramAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{

    private UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> RegisterUser([FromBody] CreateUserDto userDto)
    {
        TokenInfo token = await _userService.RegisterUser(userDto);
        return Ok(token);
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> LoginUser([FromBody] LoginUserDto userDto)
    {
        TokenInfo token = await _userService.LoginUser(userDto);
        return Ok(token);
    }
}
