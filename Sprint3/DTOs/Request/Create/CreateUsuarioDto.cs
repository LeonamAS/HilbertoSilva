using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using HilbertoSilva.Models.Enum;

namespace HilbertoSilva.DTOs.Request.Create;

public class CreateUsuarioDto
{
    [Required(ErrorMessage = "O CPF ou Usuário é obrigatório.")]
    [StringLength(20, ErrorMessage = "O identificador não pode ultrapassar 20 caracteres.")]
    public string Cpf { get; set; }

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [StringLength(255, MinimumLength = 6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")]
    public string Senha { get; set; }

    [Required(ErrorMessage = "O tipo de usuário é obrigatório.")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TipoUsuario TipoUsuario { get; set; }
}