using HilbertoSilva.Models.Enum;

namespace HilbertoSilva.DTOs.Response;

public class TurmaResponseDto
{
    public int Id { get; set; }
    public string NomeTurma { get; set; }
    public AnoEscolar AnoEscolar { get; set; }
    public int AnoLetivo { get; set; }
    public TurnoTurma Turno { get; set; }

    // Propriedade para exibir a quantidade de alunos na turma
    //public int QuantidadeAlunos { get; set; }
}