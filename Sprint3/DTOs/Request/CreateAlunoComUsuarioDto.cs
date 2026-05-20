using System.ComponentModel.DataAnnotations;

namespace HilbertoSilva.DTOs.Request;

public class CreateAlunoComUsuarioDto
{
    [Required(ErrorMessage = "Os dados de acesso do usuário são obrigatórios.")]
    public CreateUsuarioDto Usuario { get; set; }

    [Required(ErrorMessage = "Os dados cadastrais do aluno são obrigatórios.")]
    public CreateAlunoDto Aluno { get; set; }
}