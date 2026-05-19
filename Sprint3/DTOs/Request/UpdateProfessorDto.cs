using System.ComponentModel.DataAnnotations;

namespace HilbertoSilva.DTOs.Request;

public class UpdateProfessorDto
{
    [Required(ErrorMessage = "O nome do professor é obrigatório.")]
    [StringLength(255)]
    public string Nome { get; set; }

    [Required(ErrorMessage = "O telefone do professor é obrigatório.")]
    [StringLength(50)]
    public string Telefone { get; set; }

    [StringLength(100)]
    public string? Especialidade { get; set; }
}
