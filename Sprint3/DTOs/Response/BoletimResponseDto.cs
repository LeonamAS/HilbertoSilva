namespace HilbertoSilva.DTOs.Response;

public class BoletimResponseDto
{
    public int Id { get; set; }
    public decimal NotaU1 { get; set; }
    public decimal NotaU2 { get; set; }
    public decimal NotaU3 { get; set; }
    public decimal Frequencia { get; set; }
    
    public string NomeAluno { get; set; }
    public string NomeDisciplina { get; set; }

    // Calcula a média final
    public decimal MediaFinal => (NotaU1 + NotaU2 + NotaU3) / 3;
}