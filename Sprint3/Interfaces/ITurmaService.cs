using HilbertoSilva.DTOs.Request;
using HilbertoSilva.DTOs.Response;

namespace HilbertoSilva.Interfaces;

public interface ITurmaService
{
    Task<IEnumerable<TurmaResponseDto>> ObterTodasAsync();
    Task<TurmaResponseDto?> ObterPorIdAsync(int id);
    Task<TurmaResponseDto> CriarAsync(TurmaRequestDto dto);
    Task<bool> AtualizarAsync(int id, TurmaRequestDto dto);
    Task<bool> DeletarAsync(int id);
}