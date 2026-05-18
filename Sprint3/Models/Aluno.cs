using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HilbertoSilva.Models;

public class Aluno
{
    [Key]
    [Column("aluno_id")]
    public int Id { get; set; }

    [Required]
    [Column("fk_usuario")]
    public int FkUsuario { get; set; }

    [ForeignKey(nameof(FkUsuario))]
    public Usuario Usuario { get; set; }

    [Column("fk_turma")]
    public int? FkTurma { get; set; }

    [ForeignKey(nameof(FkTurma))]
    public Turma? Turma { get; set; }

    [Required(ErrorMessage = "O nome do aluno é obrigatório.")]
    [StringLength(255, ErrorMessage = "O nome do aluno não pode ultrapassar 255 caracteres.")]
    [Column("nome")]
    public string Nome { get; set; }

    [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
    [Column("data_nascimento")]
    public DateTime DataNascimento { get; set; }

    [Required(ErrorMessage = "A matrícula do aluno é obrigatória.")]
    [StringLength(50, ErrorMessage = "A matrícula não pode ultrapassar 50 caracteres.")]
    [Column("matricula")]
    public string Matricula { get; set; }

    [Required(ErrorMessage = "O nome do responsável é obrigatório.")]
    [StringLength(255, ErrorMessage = "O nome do responsável não pode ultrapassar 255 caracteres.")]
    [Column("nome_responsavel")]
    public string NomeResponsavel { get; set; }

    [Required(ErrorMessage = "O CPF do responsável é obrigatório.")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "O CPF do responsável deve conter exatamente 11 números.")]
    [RegularExpression(@"^[0-9]*$", ErrorMessage = "O CPF do responsável deve conter apenas números.")]
    [Column("cpf_responsavel")]
    public string CpfResponsavel { get; set; }

    [Required(ErrorMessage = "O telefone do responsável é obrigatório.")]
    [StringLength(50, ErrorMessage = "O telefone não pode ultrapassar 50 caracteres.")]
    [Column("telefone_responsavel")]
    public string TelefoneResponsavel { get; set; }
}
