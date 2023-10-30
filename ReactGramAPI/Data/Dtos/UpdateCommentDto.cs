using System.ComponentModel.DataAnnotations;

namespace ReactGramAPI.Data.Dtos;

public class UpdateCommentDto
{
    [Required]
    public string Content { get; set; }
}
