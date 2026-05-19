using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HilbertoSilva.Models;

public class Disciplina
{
    [Key]
    [Column("disciplina_id")]
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome da disciplina é obrigatório.")]
    [StringLength(50, ErrorMessage = "O nome da disciplina não pode ultrapassar 50 caracteres.")]
    [Column("nome")]
    public string Nome { get; set; }

    [Required(ErrorMessage = "A carga horária é obrigatória.")]
    [Column("carga_horaria")]
    public int CargaHoraria { get; set; }

    // Propriedade de Navegação
    public ICollection<DiarioClasse> DiariosClasses { get; set; } = new List<DiarioClasse>();
}
