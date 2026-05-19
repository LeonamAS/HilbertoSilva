using System.ComponentModel.DataAnnotations;

namespace HilbertoSilva.DTOs.Request;

public class CreateDiarioClasseDto
{
    [Required(ErrorMessage = "A turma é obrigatória.")]
    public int TurmaId { get; set; }

    [Required(ErrorMessage = "A disciplina é obrigatória.")]
    public int DisciplinaId { get; set; }

    [Required(ErrorMessage = "O professor é obrigatório.")]
    public int ProfessorId { get; set; }
}
