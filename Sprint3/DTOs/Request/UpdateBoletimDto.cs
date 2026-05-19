using System.ComponentModel.DataAnnotations;

namespace HilbertoSilva.DTOs.Request;

public class UpdateBoletimDto
{
    // Apenas as notas e faltas podem ser atualizadas
    [Range(0, 10, ErrorMessage = "A nota deve estar entre 0 e 10.")]
    public decimal NotaU1 { get; set; }

    [Range(0, 10, ErrorMessage = "A nota deve estar entre 0 e 10.")]
    public decimal NotaU2 { get; set; }

    [Range(0, 10, ErrorMessage = "A nota deve estar entre 0 e 10.")]
    public decimal NotaU3 { get; set; }

    [Range(0, 100, ErrorMessage = "A frequência deve estar entre 0 e 100.")]
    public decimal Frequencia { get; set; }
}
