using Microsoft.EntityFrameworkCore;
using HilbertoSilva.Data;
using HilbertoSilva.DTOs.Request;
using HilbertoSilva.DTOs.Response;
using HilbertoSilva.Interfaces;
using HilbertoSilva.Models;

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
        // Busca todos os professores e projeta direto para o DTO de resposta
        return await _context.Professores
            .Select(p => new ProfessorResponseDto
            {
                Id = p.Id,
                Nome = p.Nome,
                Telefone = p.Telefone,
                Especialidade = p.Especialidade
            })
            .ToListAsync();
    }

    public async Task<ProfessorResponseDto?> ObterPorIdAsync(int id)
    {
        // Busca o registro por ID (Chave Primária)
        var professor = await _context.Professores.FindAsync(id);

        if (professor == null)
            return null;

        // Retorna o mapeamento para o DTO
        return new ProfessorResponseDto
        {
            Id = professor.Id,
            Nome = professor.Nome,
            Telefone = professor.Telefone,
            Especialidade = professor.Especialidade
        };
    }

    public async Task<ProfessorResponseDto> CriarAsync(CreateProfessorDto dto)
    {
        // Instancia a entidade real do banco mapeando os dados do DTO
        var novoProfessor = new Professor
        {
            //UsuarioId = dto.UsuarioId,
            Nome = dto.Nome,
            Telefone = dto.Telefone,
            Especialidade = dto.Especialidade
        };

        // Adiciona e commita as alterações no MySQL via Pomelo
        await _context.Professores.AddAsync(novoProfessor);
        await _context.SaveChangesAsync();

        // Retorna o DTO de resposta contendo o ID auto-incrementado gerado pelo banco
        return new ProfessorResponseDto
        {
            Id = novoProfessor.Id,
            Nome = novoProfessor.Nome,
            Telefone = novoProfessor.Telefone,
            Especialidade = novoProfessor.Especialidade
        };
    }

    public async Task<bool> AtualizarAsync(int id, UpdateProfessorDto dto)
    {
        var professor = await _context.Professores.FindAsync(id);

        if (professor == null)
            return false;

        // Atualiza apenas as propriedades permitidas pelo DTO de atualização
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