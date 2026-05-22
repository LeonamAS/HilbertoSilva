using Microsoft.EntityFrameworkCore;
using HilbertoSilva.Data;
using HilbertoSilva.DTOs.Response;
using HilbertoSilva.Services.Interfaces;
using HilbertoSilva.Models;
using HilbertoSilva.DTOs.Request.Create;
using HilbertoSilva.DTOs.Request.Update;

namespace HilbertoSilva.Services;

public class ProfessorService : IProfessorService
{
    private readonly EscolaDbContext _context;

    public ProfessorService(EscolaDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProfessorResponseDto>> ObterTodosAsync()
    {
        return await _context.Professores
            .Select(p => new ProfessorResponseDto
            {
                Id = p.Id,
                Nome = p.Nome,
                Telefone = p.Telefone,
                Especialidade = p.Especialidade,
                Cpf = p.Usuario != null ? p.Usuario.Cpf : string.Empty
            })
            .ToListAsync();
    }

    public async Task<ProfessorResponseDto?> ObterPorIdAsync(int id)
    {
        var professor = await _context.Professores
        .Include(p => p.Usuario)
        .FirstOrDefaultAsync(p => p.Id == id);

        if (professor == null)
            return null;

        return new ProfessorResponseDto
        {
            Id = professor.Id,
            Nome = professor.Nome,
            Telefone = professor.Telefone,
            Especialidade = professor.Especialidade,
            Cpf = professor.Usuario != null ? professor.Usuario.Cpf : string.Empty
        };
    }

    public async Task<ProfessorResponseDto> CriarAsync(CreateProfessorComUsuarioDto dto)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var novoUsuario = new Usuario
            {
                Cpf = dto.Usuario.Cpf,
                Senha = dto.Usuario.Senha,
                TipoUsuario = dto.Usuario.TipoUsuario
            };

            _context.Set<Usuario>().Add(novoUsuario);
            await _context.SaveChangesAsync();

            var novoProfessor = new Professor
            {
                FkUsuario = novoUsuario.Id,
                Nome = dto.Professor.Nome,
                Telefone = dto.Professor.Telefone,
                Especialidade = dto.Professor.Especialidade
            };

            _context.Set<Professor>().Add(novoProfessor);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return new ProfessorResponseDto
            {
                Id = novoProfessor.Id,
                Nome = novoProfessor.Nome,
                Telefone = novoProfessor.Telefone,
                Especialidade = novoProfessor.Especialidade
            };
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> AtualizarAsync(int id, UpdateProfessorDto dto)
    {
        var professor = await _context.Professores.FindAsync(id);

        if (professor == null)
            return false;

        professor.Nome = dto.Nome;
        professor.Telefone = dto.Telefone;
        professor.Especialidade = dto.Especialidade;

        _context.Professores.Update(professor);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeletarAsync(int id)
    {
        var professor = await _context.Professores.FindAsync(id);

        if (professor == null)
            return false;

        _context.Professores.Remove(professor);
        await _context.SaveChangesAsync();

        return true;
    }
}