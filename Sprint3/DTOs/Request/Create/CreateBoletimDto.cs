using System.ComponentModel.DataAnnotations;

namespace HilbertoSilva.DTOs.Request.Create;

public class CreateBoletimDto
{
    [Required(ErrorMessage = "O aluno é obrigatório.")]
    public int AlunoId { get; set; }

    [Required(ErrorMessage = "A disciplina/turma é obrigatória.")]
    public int TurmaDisciplinaId { get; set; }

    // Ao criar, as notas geralmente iniciam em zero.
    public decimal NotaU1 { get; set; } = 0.0m;
    public decimal NotaU2 { get; set; } = 0.0m;
    public decimal NotaU3 { get; set; } = 0.0m;

    // Frequência padrão ao criar = 100%
    public decimal Frequencia { get; set; } = 100.00m;
}
