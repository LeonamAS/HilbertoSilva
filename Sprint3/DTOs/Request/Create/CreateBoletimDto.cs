using System.ComponentModel.DataAnnotations;

namespace HilbertoSilva.DTOs.Request.Create;

public class CreateBoletimDto
{
    [Required(ErrorMessage = "O aluno é obrigatório.")]
    public int AlunoId { get; set; }

    [Required(ErrorMessage = "A disciplina/turma é obrigatória.")]
    public int TurmaDisciplinaId { get; set; }
    public decimal? NotaU1 { get; set; }
    public decimal? NotaU2 { get; set; }
    public decimal? NotaU3 { get; set; }
    public decimal? Frequencia { get; set; }
}
