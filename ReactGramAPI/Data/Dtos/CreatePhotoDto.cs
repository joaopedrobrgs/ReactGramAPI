using System.ComponentModel.DataAnnotations;

namespace ReactGramAPI.Data.Dtos;

public class CreatePhotoDto
{
    [Required]
    public string Title { get; set; }
    [Required]
    public IFormFile File { get; set; }
}
