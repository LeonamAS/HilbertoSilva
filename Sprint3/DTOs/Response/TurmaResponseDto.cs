namespace HilbertoSilva.DTOs.Response;

public class TurmaResponseDto
{
    public int Id { get; set; }
    public string NomeTurma { get; set; }
    public string AnoEscolar { get; set; }
    public int AnoLetivo { get; set; }
    public string Turno { get; set; }

    // Propriedade para exibir a quantidade de alunos na turma
    //public int QuantidadeAlunos { get; set; }
}