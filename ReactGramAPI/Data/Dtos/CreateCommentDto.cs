using System.ComponentModel.DataAnnotations;

namespace ReactGramAPI.Data.Dtos;

public class CreateCommentDto
{
    [Required]
    public string Content { get; set; }
}
