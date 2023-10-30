using System.ComponentModel.DataAnnotations;

namespace ReactGramAPI.Data.Dtos;

public class ReadCommentDto
{
    public int Id { get; set; }
    public string Content { get; set; }
    public string UserId { get; set; }
}
