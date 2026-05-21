using System.ComponentModel.DataAnnotations;

namespace HilbertoSilva.DTOs.Request.Create;

public class CreateProfessorComUsuarioDto
{
    [Required(ErrorMessage = "Os dados de acesso do usuário são obrigatórios.")]
    public CreateUsuarioDto Usuario { get; set; }

    [Required(ErrorMessage = "Os dados cadastrais do professor são obrigatórios.")]
    public CreateProfessorDto Professor { get; set; }
}