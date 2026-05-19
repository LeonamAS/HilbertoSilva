using System.ComponentModel.DataAnnotations;

namespace HilbertoSilva.DTOs.Request;

public class UpdateAlunoDto
{
    public int? TurmaId { get; set; }

    [Required(ErrorMessage = "O nome do aluno é obrigatório.")]
    [StringLength(255)]
    public string Nome { get; set; }

    [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
    public DateTime DataNascimento { get; set; }

    [Required(ErrorMessage = "O nome do responsável é obrigatório.")]
    [StringLength(255)]
    public string NomeResponsavel { get; set; }

    [Required(ErrorMessage = "O telefone do responsável é obrigatório.")]
    [StringLength(50)]
    public string TelefoneResponsavel { get; set; }

    [Required]
    [StringLength(11, MinimumLength = 11)]
    [RegularExpression(@"^[0-9]*$")]
    public string CpfResponsavel { get; set; }
}
