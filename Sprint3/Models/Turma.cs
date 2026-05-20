using HilbertoSilva.Models.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HilbertoSilva.Models;

public class Turma
{
    [Key]
    [Column("turma_id")]
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome da turma é obrigatório.")]
    [StringLength(50, ErrorMessage = "O nome da turma não pode ultrapassar 50 caracteres.")]
    [Column("nome_turma")]
    public string NomeTurma { get; set; }

    [Required(ErrorMessage = "O ano escolar é obrigatório.")]
    [Column("ano_escolar")]
    public string AnoEscolar { get; set; }

    [Required(ErrorMessage = "O ano letivo é obrigatório.")]
    [Column("ano_letivo")]
    public int AnoLetivo { get; set; }

    [Required(ErrorMessage = "O turno é obrigatório.")]
    [Column("turno")]
    public TurnoTurma Turno { get; set; }

    // Propriedade de Navegação (Uma turma tem vários alunos)
    public ICollection<Aluno> Alunos { get; set; } = new List<Aluno>();
}
