using Microsoft.AspNetCore.Identity;

namespace ReactGramAPI.Models;

public class User : IdentityUser
{

    public string? ProfileImage { get; set; }
    public string? Biography { get; set; }

    public User() : base() { }

}
