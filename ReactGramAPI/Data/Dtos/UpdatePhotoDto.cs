using System.ComponentModel.DataAnnotations;

namespace ReactGramAPI.Data.Dtos;

public class UpdatePhotoDto
{
    [Required]
    public string Title { get; set; }
}
