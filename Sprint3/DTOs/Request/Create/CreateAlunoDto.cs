using System.ComponentModel.DataAnnotations;

namespace HilbertoSilva.DTOs.Request.Create;

public class CreateAlunoDto
{
    [Required(ErrorMessage = "A identificação da turma é obrigatória.")]
    public int TurmaId { get; set; }

    [Required(ErrorMessage = "O nome do aluno é obrigatório.")]
    [StringLength(255, ErrorMessage = "O nome do aluno não pode ultrapassar 255 caracteres.")]
    public string Nome { get; set; }

    [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
    public DateTime DataNascimento { get; set; }

    public string? Matricula { get; set; }

    [Required(ErrorMessage = "O nome do responsável é obrigatório.")]
    [StringLength(255, ErrorMessage = "O nome não pode ultrapassar 255 caracteres.")]
    public string NomeResponsavel { get; set; }

    [Required(ErrorMessage = "O CPF do responsável é obrigatório.")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "O CPF deve conter exatamente 11 números.")]
    [RegularExpression(@"^[0-9]*$", ErrorMessage = "O CPF deve conter apenas números.")]
    public string CpfResponsavel { get; set; }

    [Required(ErrorMessage = "O telefone do responsável é obrigatório.")]
    [StringLength(50, ErrorMessage = "O telefone não pode ultrapassar 50 caracteres.")]
    public string TelefoneResponsavel { get; set; }
}
