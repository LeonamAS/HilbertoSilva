using System.ComponentModel.DataAnnotations;

namespace HilbertoSilva.DTOs.Request;

public class CreateUsuarioDto
{
    [Required(ErrorMessage = "O CPF é obrigatório.")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "O CPF deve conter exatamente 11 números.")]
    [RegularExpression(@"^[0-9]*$", ErrorMessage = "O CPF deve conter apenas números.")]
    public string Cpf { get; set; }

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [StringLength(255, MinimumLength = 6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")]
    public string Senha { get; set; }

    [Required(ErrorMessage = "O tipo de usuário é obrigatório.")]
    public string TipoUsuario { get; set; }
}
