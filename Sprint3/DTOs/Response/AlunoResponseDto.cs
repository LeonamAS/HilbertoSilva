namespace HilbertoSilva.DTOs.Response;

public class AlunoResponseDto
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public DateTime DataNascimento { get; set; }
    public string Matricula { get; set; }

    // Dados do Responsável
    public string NomeResponsavel { get; set; }
    public string CpfResponsavel { get; set; }
    public string TelefoneResponsavel { get; set; }
}