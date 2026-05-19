using System.ComponentModel.DataAnnotations;

namespace HilbertoSilva.DTOs.Request;

public class UsuarioAlterarSenhaDto
{
    [Required(ErrorMessage = "A senha atual é obrigatória.")]
    public string SenhaAtual { get; set; }

    [Required(ErrorMessage = "A nova senha é obrigatória.")]
    [StringLength(255, MinimumLength = 6, ErrorMessage = "A nova senha deve ter no mínimo 6 caracteres.")]
    public string NovaSenha { get; set; }

    [Required(ErrorMessage = "A confirmação da nova senha é obrigatória.")]
    [Compare("NovaSenha", ErrorMessage = "As senhas não conferem.")]
    public string ConfirmarNovaSenha { get; set; }
}
