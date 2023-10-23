namespace ReactGramAPI.Data.Dtos;

public class ReadLoggedUserDto
{
    public string? Id { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }

    public string? ProfileImage { get; set; }
    public string? Biography { get; set; }
}
