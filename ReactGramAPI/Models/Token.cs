namespace ReactGramAPI.Models;

public class TokenInfo
{
    public string Token { get; set; }
    public DateTime? Expires { get; set; }

    public TokenInfo(string token, DateTime? expires)
    {
        Token = token;
        Expires = expires;
    }
}
