using HilbertoSilva.DTOs.Request.Create;
using HilbertoSilva.DTOs.Request.Update;
using HilbertoSilva.DTOs.Response;

namespace HilbertoSilva.Services.Interfaces;

public interface IDiarioClasseService : IBaseService<CreateDiarioClasseDto, UpdateDiarioClasseDto, DiarioClasseResponseDto>
{
    Task<IEnumerable<TurmaDisciplinaResponseDto>> ObterTurmasPorProfessorUsuarioIdAsync(int usuarioId);
    Task<IEnumerable<AlunosDiarioResponseDto>> ObterAlunosPorTurmaEDisciplinaAsync(int turmaId, int disciplinaId);
    Task<bool> LancarNotasAsync(LancarNotasDto request);
}