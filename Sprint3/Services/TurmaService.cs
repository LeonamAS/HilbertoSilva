using Microsoft.EntityFrameworkCore;
using HilbertoSilva.Data;
using HilbertoSilva.DTOs.Request;
using HilbertoSilva.DTOs.Response;
using HilbertoSilva.Interfaces;
using HilbertoSilva.Models;

namespace HilbertoSilva.Services;

public class TurmaService : ITurmaService
{
    private readonly EscolaDbContext _context;

    public TurmaService(EscolaDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TurmaResponseDto>> ObterTodasAsync()
    {
        // Busca todas as turmas e projeta diretamente para o DTO de resposta
        return await _context.Turmas
            .Select(t => new TurmaResponseDto
            {
                Id = t.Id,
                NomeTurma = t.NomeTurma,
                AnoEscolar = t.AnoEscolar,
                AnoLetivo = t.AnoLetivo,
                Turno = t.Turno
            })
            .ToListAsync();
    }

    public async Task<TurmaResponseDto?> ObterPorIdAsync(int id)
    {
        // Busca a turma no banco pela Chave Primária (ID)
        var turma = await _context.Turmas.FindAsync(id);

        if (turma == null)
            return null;

        // Mapeia os dados encontrados para o DTO de resposta
        return new TurmaResponseDto
        {
            Id = turma.Id,
            NomeTurma = turma.NomeTurma,
            AnoEscolar = turma.AnoEscolar,
            AnoLetivo = turma.AnoLetivo,
            Turno = turma.Turno
        };
    }

    public async Task<TurmaResponseDto> CriarAsync(TurmaRequestDto dto)
    {
        // Transforma o DTO de requisição na entidade física que o banco conhece
        var novaTurma = new Turma
        {
            NomeTurma = dto.NomeTurma,
            AnoEscolar = dto.AnoEscolar,
            AnoLetivo = dto.AnoLetivo,
            Turno = dto.Turno
        };

        // Adiciona e commita a nova linha no MySQL via Pomelo
        await _context.Turmas.AddAsync(novaTurma);
        await _context.SaveChangesAsync();

        // Retorna a resposta contendo o ID auto-incrementado gerado pelo banco
        return new TurmaResponseDto
        {
            Id = novaTurma.Id,
            NomeTurma = novaTurma.NomeTurma,
            AnoEscolar = novaTurma.AnoEscolar,
            AnoLetivo = novaTurma.AnoLetivo,
            Turno = novaTurma.Turno
        };
    }

    public async Task<bool> AtualizarAsync(int id, TurmaRequestDto dto)
    {
        var turma = await _context.Turmas.FindAsync(id);

        if (turma == null)
            return false; // Turma não encontrada para atualizar

        // Atualiza os campos com os dados validados que vieram da requisição
        turma.NomeTurma = dto.NomeTurma;
        turma.AnoEscolar = dto.AnoEscolar;
        turma.AnoLetivo = dto.AnoLetivo;
        turma.Turno = dto.Turno;

        _context.Turmas.Update(turma);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeletarAsync(int id)
    {
        var turma = await _context.Turmas.FindAsync(id);

        if (turma == null)
            return false; // Turma não existe no banco

        _context.Turmas.Remove(turma);
        await _context.SaveChangesAsync();

        return true;
    }
}