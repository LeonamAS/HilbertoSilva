using System.ComponentModel.DataAnnotations;

namespace HilbertoSilva.DTOs.Request;

public class LoginDto
{
    [Required(ErrorMessage = "O campo de login (CPF ou Usuário) é obrigatório.")]
    public string Cpf { get; set; }

    [Required(ErrorMessage = "A senha é obrigatória.")]
    public string Senha { get; set; }
}