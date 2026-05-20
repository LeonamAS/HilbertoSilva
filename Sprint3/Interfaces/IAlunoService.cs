using HilbertoSilva.DTOs.Request;
using HilbertoSilva.DTOs.Response;

namespace HilbertoSilva.Interfaces;

public interface IAlunoService
{
    Task<IEnumerable<AlunoResponseDto>> ObterAlunosAsync();
    Task<AlunoResponseDto> ObterAlunoPorIdAsync(int id);
    Task<AlunoResponseDto> CriarAlunoAsync(CreateAlunoDto dto);
    Task<bool> AtualizarAlunoAsync(int id, UpdateAlunoDto dto);
    Task<bool> DeletarAlunoAsync(int id);
}
