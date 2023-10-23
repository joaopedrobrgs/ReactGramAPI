using ReactGramAPI.Models;

namespace ReactGramAPI.Data.Dtos;

public class ReadAuthenticationUserDto
{
    public string Id { get; set; }
    public TokenInfo TokenInfo { get; set; }
    public string ProfileImage { get; set; }

    //public ReadUserDto(string id, TokenInfo token, string? profileImage)
    //{
    //    Id = id;
    //    Token = token;
    //    ProfileImage = profileImage;
    //}
}
