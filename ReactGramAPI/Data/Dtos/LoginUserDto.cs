using System.ComponentModel.DataAnnotations;

namespace ReactGramAPI.Data.Dtos;

public class LoginUserDto
{
    [Required(ErrorMessage = "Digite o nome de usuário")]
    [DataType(DataType.Text)]
    public string Username { get; set; }
    [Required(ErrorMessage = "Digite a senha")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}