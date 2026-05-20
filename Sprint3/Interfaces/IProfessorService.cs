using HilbertoSilva.DTOs.Request;
using HilbertoSilva.DTOs.Response;

namespace HilbertoSilva.Interfaces;

public interface IProfessorService
{
    Task<IEnumerable<ProfessorResponseDto>> ObterTodosAsync();
    Task<ProfessorResponseDto?> ObterPorIdAsync(int id);
    Task<ProfessorResponseDto> CriarUsuarioETransacaoAsync(CreateProfessorComUsuarioDto dto);
    Task<bool> AtualizarAsync(int id, UpdateProfessorDto dto);
    Task<bool> DeletarAsync(int id);
}