using System.ComponentModel.DataAnnotations;

namespace ReactGramAPI.Models;

public class Like
{
    //Chave primária:
    [Key]
    [Required]
    public int Id { get; set; }

    //Chaves estrangeiras:
    [Required]
    public int PhotoId { get; set; }
    public string? UserId { get; set; }

    //Tabelas com as quais se relaciona:
    //Relação n : 1
    public virtual Photo Photo { get; set; }
    //Relação n : 1
    public virtual User User { get; set; }

    //Contrutor vazio para poder inicializar um objeto sem passar nenhum argumento:
    public Like()
    {
    }
}
