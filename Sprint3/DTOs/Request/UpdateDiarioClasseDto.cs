using System.ComponentModel.DataAnnotations;

namespace HilbertoSilva.DTOs.Request;

public class UpdateDiarioClasseDto
{
    [Required(ErrorMessage = "O novo professor responsável é obrigatório.")]
    public int ProfessorId { get; set; }
}
