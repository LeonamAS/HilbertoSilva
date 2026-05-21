using Microsoft.EntityFrameworkCore;
using HilbertoSilva.Data;
using HilbertoSilva.Models;
using HilbertoSilva.DTOs.Request.Create;
using HilbertoSilva.DTOs.Request.Update;
using HilbertoSilva.DTOs.Response;
using HilbertoSilva.Services.Interfaces;

namespace HilbertoSilva.Services
{
    public class DiarioClasseService : IDiarioClasseService
    {
        private readonly EscolaDbContext _context;

        public DiarioClasseService(EscolaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DiarioClasseResponseDto>> ObterTodosAsync()
        {
            return await _context.DiarioClasse
                .AsNoTracking()
                .Select(d => new DiarioClasseResponseDto
                {
                    Id = d.Id,
                    TurmaId = d.FkTurma,
                    NomeTurma = d.Turma.AnoEscolar + " - " + d.Turma.NomeTurma,
                    DisciplinaId = d.FkDisciplina,
                    NomeDisciplina = d.Disciplina.Nome,
                    ProfessorId = d.FkProfessor,
                    NomeProfessor = d.Professor.Nome
                })
                .ToListAsync();
        }

        public async Task<DiarioClasseResponseDto?> ObterPorIdAsync(int id)
        {
            return await _context.DiarioClasse
                .AsNoTracking()
                .Where(d => d.Id == id)
                .Select(d => new DiarioClasseResponseDto
                {
                    Id = d.Id,
                    TurmaId = d.FkTurma,
                    NomeTurma = d.Turma.AnoEscolar + " - " + d.Turma.NomeTurma,
                    DisciplinaId = d.FkDisciplina,
                    NomeDisciplina = d.Disciplina.Nome,
                    ProfessorId = d.FkProfessor,
                    NomeProfessor = d.Professor.Nome
                })
                .FirstOrDefaultAsync();
        }

        public async Task<DiarioClasseResponseDto> CriarAsync(CreateDiarioClasseDto request)
        {
            var diario = new DiarioClasse
            {
                FkTurma = request.TurmaId,
                FkDisciplina = request.DisciplinaId,
                FkProfessor = request.ProfessorId
            };

            _context.DiarioClasse.Add(diario);
            await _context.SaveChangesAsync();

            return await ObterPorIdAsync(diario.Id);
        }

        public async Task<bool> AtualizarAsync(int id, UpdateDiarioClasseDto request)
        {
            var diario = await _context.DiarioClasse.FindAsync(id);

            if (diario == null) return false;

            diario.FkProfessor = request.ProfessorId;

            _context.DiarioClasse.Update(diario);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeletarAsync(int id)
        {
            var diario = await _context.DiarioClasse.FindAsync(id);

            if (diario == null) return false;

            _context.DiarioClasse.Remove(diario);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}