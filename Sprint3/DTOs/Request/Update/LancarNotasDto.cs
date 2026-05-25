using System.ComponentModel.DataAnnotations;

namespace HilbertoSilva.DTOs.Request.Update;

public class LancarNotasDto
{
    [Required(ErrorMessage = "O identificador do aluno é obrigatório.")]
    public int AlunoId { get; set; }

    [Required(ErrorMessage = "O identificador da turma é obrigatório.")]
    public int TurmaId { get; set; }

    [Required(ErrorMessage = "O identificador da disciplina é obrigatório.")]
    public int DisciplinaId { get; set; }

    [Range(0, 10, ErrorMessage = "A nota da Unidade 1 deve estar entre 0 e 10.")]
    public decimal? NotaU1 { get; set; }

    [Range(0, 10, ErrorMessage = "A nota da Unidade 2 deve estar entre 0 e 10.")]
    public decimal? NotaU2 { get; set; }

    [Range(0, 10, ErrorMessage = "A nota da Unidade 3 deve estar entre 0 e 10.")]
    public decimal? NotaU3 { get; set; }

    [Range(0, 100, ErrorMessage = "A frequência deve estar entre 0 e 100.")]
    public decimal? Frequencia { get; set; }
}