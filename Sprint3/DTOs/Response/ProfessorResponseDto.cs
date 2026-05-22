namespace HilbertoSilva.DTOs.Response;

public class ProfessorResponseDto
{
    public int Id { get; set; }
    public string Cpf { get; set; }
    public string Nome { get; set; }
    public string Telefone { get; set; }
    public string? Especialidade { get; set; }
}