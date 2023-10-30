using Microsoft.AspNetCore.Identity;

namespace ReactGramAPI.Models;

public class User : IdentityUser
{

    public string? ProfileImage { get; set; }
    public string? Biography { get; set; }

    public User() : base() { }

    //Tabelas/Modelos com os quais se relaciona:
    //Relação 1 : n
    public virtual ICollection<Photo>? Photos { get; set; }
    //Relação 1 : n
    public virtual ICollection<Comment>? Comments { get; set; }
    //Relação 1 : n
    public virtual ICollection<Like>? Likes { get; set; }


}
