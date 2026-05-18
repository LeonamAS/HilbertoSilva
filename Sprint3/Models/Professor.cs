using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HilbertoSilva.Models;

public class Professor
{
    [Key]
    [Column("professor_id")]
    public int Id { get; set; }

    [Required]
    [Column("fk_usuario")]
    public int FkUsuario { get; set; }

    [ForeignKey(nameof(FkUsuario))]
    public Usuario Usuario { get; set; }

    [Required(ErrorMessage = "O nome do professor é obrigatório.")]
    [StringLength(255, ErrorMessage = "O nome do professor não pode ultrapassar 255 caracteres.")]
    [Column("nome")]
    public string Nome { get; set; }

    [Required(ErrorMessage = "O telefone do professor é obrigatório.")]
    [StringLength(50, ErrorMessage = "O telefone não pode ultrapassar 50 caracteres.")]
    [Column("telefone")]
    public string Telefone { get; set; }

    [StringLength(100, ErrorMessage = "A especialidade não pode ultrapassar 100 caracteres.")]
    [Column("especialidade")]
    public string? Especialidade { get; set; }
}
