using HilbertoSilva.DTOs.Request;
using HilbertoSilva.DTOs.Response;

namespace HilbertoSilva.Interfaces;

public interface IAlunoService
{
    Task<IEnumerable<AlunoResponseDto>> ObterTodosAsync();
    Task<AlunoResponseDto> ObterPorIdAsync(int id);
    Task<AlunoResponseDto> CriarComUsuarioETransacaoAsync(CreateAlunoDto dto);
    Task<bool> AtualizarAsync(int id, UpdateAlunoDto dto);
    Task<bool> DeletarAsync(int id);
}
