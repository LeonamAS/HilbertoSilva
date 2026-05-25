namespace HilbertoSilva.DTOs.Response;

public class TurmaDisciplinaResponseDto
{
    /// <summary>
    /// Identificador exclusivo da Turma.
    /// </summary>
    public int TurmaId { get; set; }

    /// <summary>
    /// Nome descritivo da turma (Ex: "3º Ano A - Ensino Médio").
    /// </summary>
    public string NomeTurma { get; set; } = string.Empty;

    /// <summary>
    /// Identificador exclusivo da Disciplina vinculada.
    /// </summary>
    public int DisciplinaId { get; set; }

    /// <summary>
    /// Nome da disciplina lecionada nessa turma (Ex: "Matemática").
    /// </summary>
    public string NomeDisciplina { get; set; } = string.Empty;
}