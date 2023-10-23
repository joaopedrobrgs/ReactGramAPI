using System.ComponentModel.DataAnnotations;

namespace ReactGramAPI.Data.Dtos;

public class CreateUserDto
{
    [Required(ErrorMessage = "O nome de usuário é obrigatório")]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "O nome de usuário deve ter entre 3 e 20 caracteres.")]
    [DataType(DataType.Text)]
    public string Username { get; set; }
    [Required(ErrorMessage = "O email é obrigatório")]
    [DataType(DataType.EmailAddress)]
    [EmailAddress]
    public string Email { get; set; }
    [Required(ErrorMessage = "A senha é obrigatória")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [Required(ErrorMessage = "Confirme a senha")]
    [Compare("Password")]
    public string PasswordConfirm { get; set; }
}
