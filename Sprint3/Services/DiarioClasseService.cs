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
        public async Task<IEnumerable<TurmaDisciplinaResponseDto>> ObterTurmasPorProfessorUsuarioIdAsync(int usuarioId)
        {
            return await _context.DiarioClasse
                .AsNoTracking()
                .Where(d => d.Professor.FkUsuario == usuarioId)
                .Select(d => new TurmaDisciplinaResponseDto
                {
                    TurmaId = d.FkTurma,
                    NomeTurma = d.Turma.AnoEscolar + " - " + d.Turma.NomeTurma,
                    DisciplinaId = d.FkDisciplina,
                    NomeDisciplina = d.Disciplina.Nome
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<AlunosDiarioResponseDto>> ObterAlunosPorTurmaEDisciplinaAsync(int turmaId, int disciplinaId)
        {
            var diario = await _context.DiarioClasse
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.FkTurma == turmaId && d.FkDisciplina == disciplinaId);

            if (diario == null)
                return Enumerable.Empty<AlunosDiarioResponseDto>();

            return await _context.Alunos
                .AsNoTracking()
                .Where(a => a.FkTurma == turmaId)
                .Select(a => new AlunosDiarioResponseDto
                {
                    AlunoId = a.Id,
                    Matricula = a.Matricula,
                    NomeAluno = a.Nome,
                    NotaU1 = _context.Boletins.Where(b => b.FkAluno == a.Id && b.FkTurmaDisciplina == diario.Id).Select(b => b.NotaU1).FirstOrDefault(),
                    NotaU2 = _context.Boletins.Where(b => b.FkAluno == a.Id && b.FkTurmaDisciplina == diario.Id).Select(b => b.NotaU2).FirstOrDefault(),
                    NotaU3 = _context.Boletins.Where(b => b.FkAluno == a.Id && b.FkTurmaDisciplina == diario.Id).Select(b => b.NotaU3).FirstOrDefault(),
                    Frequencia = _context.Boletins.Where(b => b.FkAluno == a.Id && b.FkTurmaDisciplina == diario.Id).Select(b => b.Frequencia).FirstOrDefault()
                })
                .ToListAsync();
        }

        public async Task<bool> LancarNotasAsync(LancarNotasDto request)
        {
            var alunoValido = await _context.Alunos.AnyAsync(a => a.Id == request.AlunoId && a.FkTurma == request.TurmaId);
            if (!alunoValido) return false;

            var diario = await _context.DiarioClasse
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.FkTurma == request.TurmaId && d.FkDisciplina == request.DisciplinaId);

            if (diario == null) return false;

            var boletim = await _context.Boletins
                .FirstOrDefaultAsync(b => b.FkAluno == request.AlunoId && b.FkTurmaDisciplina == diario.Id);

            if (boletim == null)
            {
                boletim = new Boletim
                {
                    FkAluno = request.AlunoId,
                    FkTurmaDisciplina = diario.Id,
                    NotaU1 = request.NotaU1,
                    NotaU2 = request.NotaU2,
                    NotaU3 = request.NotaU3,
                    Frequencia = request.Frequencia
                };
                _context.Boletins.Add(boletim);
            }
            else
            {
                boletim.NotaU1 = request.NotaU1;
                boletim.NotaU2 = request.NotaU2;
                boletim.NotaU3 = request.NotaU3;
                boletim.Frequencia = request.Frequencia;
                _context.Boletins.Update(boletim);
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}