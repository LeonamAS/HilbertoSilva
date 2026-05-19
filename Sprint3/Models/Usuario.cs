using HilbertoSilva.Models.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HilbertoSilva.Models;

public class Usuario
{
    [Key]
    [Column("usuario_id")]
    public int Id { get; set; }

    [Required(ErrorMessage = "O CPF é obrigatório.")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "O CPF deve conter exatamente 11 números.")]
    [RegularExpression(@"^[0-9]*$", ErrorMessage = "O CPF deve conter apenas números.")]
    [Column("cpf")]
    public string Cpf { get; set; }

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [StringLength(255)]
    [Column("senha")]
    public string Senha { get; set; }

    [Required(ErrorMessage = "O tipo de usuário é obrigatório.")]
    [Column("tipo_usuario")]
    public TipoUsuario TipoUsuario { get; set; }

    [Column("data_cadastro")]
    public DateTime DataCadastro { get; set; } = DateTime.Now;

    // Propriedades de Navegação (Relacionamentos 1:1)
    public Aluno? Aluno { get; set; }
    public Professor? Professor { get; set; }
}