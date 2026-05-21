using HilbertoSilva.DTOs.Request.Create;
using HilbertoSilva.DTOs.Request.Update;
using HilbertoSilva.DTOs.Response;

namespace HilbertoSilva.Interfaces;

public interface IAlunoService
{
    Task<IEnumerable<AlunoResponseDto>> ObterTodosAsync();
    Task<AlunoResponseDto> ObterPorIdAsync(int id);
    Task<AlunoResponseDto> CriarUsuarioETransacaoAsync(CreateAlunoComUsuarioDto dto);
    Task<bool> AtualizarAsync(int id, UpdateAlunoDto dto);
    Task<bool> DeletarAsync(int id);
}
