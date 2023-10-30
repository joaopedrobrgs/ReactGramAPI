using System.ComponentModel.DataAnnotations;

namespace ReactGramAPI.Models;

public class Photo
{
    //Chave primária:
    [Key]
    [Required]
    public int Id { get; set; }

    //Atributos comuns:
    [Required]
    public string Title { get; set; }
    //Arquivo não vai ficar armazenado no banco de dados, por isso colocamos apenas o caminho:
    [Required]
    public string FilePath { get; set; }

    //Chaves estrangeiras:
    [Required]
    public string UserId { get; set; }


    //Tabelas/Modelos com os quais se relaciona:
    //Relação n : 1
    public virtual User User { get; set; }
    //Relação 1 : n
    public virtual ICollection<Like>? Likes { get; set; }
    //Relação 1 : n
    public virtual ICollection<Comment>? Comments { get; set; }
    //Tabelas/Modelos com os quais se relaciona:

}
