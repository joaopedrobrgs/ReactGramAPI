using ReactGramAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace ReactGramAPI.Data.Dtos;

public class ReadPhotoDto
{
    //Atributos comuns:
    public string Title { get; set; }
    public string FilePath { get; set; }

    //Atributos vindos de outros modelos:
    public ReadUserDto User { get; set; }
    public ICollection<ReadLikeDto>? Likes { get; set; }
    public ICollection<ReadCommentDto>? Comments { get; set; }
}
