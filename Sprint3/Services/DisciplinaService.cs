using HilbertoSilva.Models;
using HilbertoSilva.DTOs.Request.Create;
using HilbertoSilva.DTOs.Response;
using HilbertoSilva.Services.Interfaces;
using HilbertoSilva.Repositories.Interfaces;

namespace HilbertoSilva.Services;

public class DisciplinaService : IDisciplinaService
{
    private readonly IBaseRepository<Disciplina> _repositorio;

    public DisciplinaService(IBaseRepository<Disciplina> repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<IEnumerable<DisciplinaResponseDto>> ObterTodosAsync()
    {
        var disciplinas = await _repositorio.ObterTodosAsync();

        return disciplinas.Select(d => new DisciplinaResponseDto
        {
            Id = d.Id,
            Nome = d.Nome,
            CargaHoraria = d.CargaHoraria
        });
    }

    public async Task<DisciplinaResponseDto?> ObterPorIdAsync(int id)
    {
        var disciplina = await _repositorio.ObterPorIdAsync(id);
        if (disciplina == null) return null;

        return new DisciplinaResponseDto
        {
            Id = disciplina.Id,
            Nome = disciplina.Nome,
            CargaHoraria = disciplina.CargaHoraria
        };
    }

    public async Task<DisciplinaResponseDto> CriarAsync(DisciplinaRequestDto request)
    {
        var disciplina = new Disciplina
        {
            Nome = request.Nome,
            CargaHoraria = request.CargaHoraria
        };

        var criada = await _repositorio.CriarAsync(disciplina);

        return new DisciplinaResponseDto
        {
            Id = criada.Id,
            Nome = criada.Nome,
            CargaHoraria = criada.CargaHoraria
        };
    }

    public async Task<bool> AtualizarAsync(int id, DisciplinaRequestDto request)
    {
        var disciplina = await _repositorio.ObterPorIdAsync(id);
        if (disciplina == null) return false;

        disciplina.Nome = request.Nome;
        disciplina.CargaHoraria = request.CargaHoraria;

        await _repositorio.AtualizarAsync(disciplina);
        return true;
    }

    public async Task<bool> DeletarAsync(int id)
    {
        var disciplina = await _repositorio.ObterPorIdAsync(id);
        if (disciplina == null) return false;

        await _repositorio.DeletarAsync(disciplina);
        return true;
    }
}