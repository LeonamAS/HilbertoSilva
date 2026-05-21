using HilbertoSilva.DTOs.Request.Create;
using HilbertoSilva.DTOs.Response;

namespace HilbertoSilva.Services.Interfaces;

public interface IDisciplinaService
{
    Task<IEnumerable<DisciplinaResponseDto>> ObterTodosAsync();
    Task<DisciplinaResponseDto> ObterPorIdAsync(int id);
    Task<DisciplinaResponseDto> CriarAsync(DisciplinaRequestDto request);
    Task<bool> AtualizarAsync(int id, DisciplinaRequestDto request);
    Task<bool> DeletarAsync(int id);
}