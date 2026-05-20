using HilbertoSilva.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace HilbertoSilva.DTOs.Request;

public class TurmaRequestDto
{
    [Required(ErrorMessage = "O nome da turma é obrigatório.")]
    [StringLength(50)]
    public string NomeTurma { get; set; }

    [Required(ErrorMessage = "O ano escolar é obrigatório.")]
    public string AnoEscolar { get; set; }

    [Required(ErrorMessage = "O ano letivo é obrigatório.")]
    public int AnoLetivo { get; set; }

    [Required(ErrorMessage = "O turno é obrigatório.")]
    public TurnoTurma Turno { get; set; }
}
