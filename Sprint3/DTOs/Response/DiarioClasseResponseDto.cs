namespace HilbertoSilva.DTOs.Response;

public class DiarioClasseResponseDto
{
    public int Id { get; set; }
    public int TurmaId { get; set; }
    public string NomeTurma { get; set; }
    public int DisciplinaId { get; set; }
    public string NomeDisciplina { get; set; }
    public int ProfessorId { get; set; }
    public string NomeProfessor { get; set; }
}