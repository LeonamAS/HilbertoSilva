using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HilbertoSilva.Models;

public class TurmaDisciplina
{
    [Key]
    [Column("turma_disciplina_id")]
    public int Id { get; set; }

    [Required]
    [Column("fk_turma")]
    public int FkTurma { get; set; }

    [ForeignKey(nameof(FkTurma))]
    public Turma Turma { get; set; }

    [Required]
    [Column("fk_disciplina")]
    public int FkDisciplina { get; set; }

    [ForeignKey(nameof(FkDisciplina))]
    public Disciplina Disciplina { get; set; }

    [Required]
    [Column("fk_professor")]
    public int FkProfessor { get; set; }

    [ForeignKey(nameof(FkProfessor))]
    public Professor Professor { get; set; }

    // Propriedade de Navegação (Lista de boletins vinculados a esta disciplina nesta turma)
    public ICollection<Boletim> Boletins { get; set; } = new List<Boletim>();
}
