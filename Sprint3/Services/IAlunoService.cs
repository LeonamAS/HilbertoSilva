using HilbertoSilva.DTOs.Request;
using HilbertoSilva.DTOs.Response;

namespace HilbertoSilva.Services;

public interface IAlunoService
{
    Task<IEnumerable<AlunoResponseDto>> ObterTodosAsync();
    Task<AlunoResponseDto> ObterPorIdAsync(int id);
    Task<AlunoResponseDto> CriarAsync(CreateAlunoDto dto);
    Task<bool> AtualizarAsync(int id, UpdateAlunoDto dto);
    Task<bool> DeletarAsync(int id);
}
