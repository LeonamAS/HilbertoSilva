using System.ComponentModel.DataAnnotations;

namespace HilbertoSilva.DTOs.Request.Create;

public class DisciplinaRequestDto
{
    [Required(ErrorMessage = "O nome da disciplina é obrigatório.")]
    [StringLength(50)]
    public string Nome { get; set; }

    [Required(ErrorMessage = "A carga horária é obrigatória.")]
    public int CargaHoraria { get; set; }
}
