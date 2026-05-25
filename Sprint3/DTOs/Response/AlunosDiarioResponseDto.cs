namespace HilbertoSilva.DTOs.Response;

public class AlunosDiarioResponseDto
{
    /// <summary>
    /// Identificador exclusivo do aluno (útil para referenciar no front).
    /// </summary>
    public int AlunoId { get; set; }

    /// <summary>
    /// Matrícula interna da escola.
    /// </summary>
    public string Matricula { get; set; } = string.Empty;

    /// <summary>
    /// Nome completo do aluno para exibição na chamada.
    /// </summary>
    public string NomeAluno { get; set; } = string.Empty;

    /// <summary>
    /// Nota do aluno no 1º Bimestre / Unidade 1 (Pode ser nula se ainda não lançada).
    /// </summary>
    public decimal? NotaU1 { get; set; }

    /// <summary>
    /// Nota do aluno no 2º Bimestre / Unidade 2 (Pode ser nula se ainda não lançada).
    /// </summary>
    public decimal? NotaU2 { get; set; }

    /// <summary>
    /// Nota do aluno no 3º Bimestre / Unidade 3 (Pode ser nula se ainda não lançada).
    /// </summary>
    public decimal? NotaU3 { get; set; }

    /// <summary>
    /// Percentual de frequência acumulado pelo aluno de 0 a 100 (Pode ser nulo).
    /// </summary>
    public decimal? Frequencia { get; set; }
}