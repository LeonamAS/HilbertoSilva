using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HilbertoSilva.Models;

public class Boletim
{
    [Key]
    [Column("boletim_id")]
    public int Id { get; set; }

    [Required]
    [Column("fk_aluno")]
    public int FkAluno { get; set; }

    [ForeignKey(nameof(FkAluno))]
    public Aluno Aluno { get; set; }

    [Required]
    [Column("fk_turma_disciplina")]
    public int FkTurmaDisciplina { get; set; }

    [ForeignKey(nameof(FkTurmaDisciplina))]
    public TurmaDisciplina TurmaDisciplina { get; set; }

    [Column("nota_u1")]
    public decimal NotaU1 { get; set; } = 0.0m;

    [Column("nota_u2")]
    public decimal NotaU2 { get; set; } = 0.0m;

    [Column("nota_u3")]
    public decimal NotaU3 { get; set; } = 0.0m;

    [Column("frequencia")]
    public decimal Frequencia { get; set; } = 100.00m;
}
