using Microsoft.EntityFrameworkCore;
using HilbertoSilva.Data;
using HilbertoSilva.Models;
using HilbertoSilva.DTOs.Request.Create;
using HilbertoSilva.DTOs.Request.Update;
using HilbertoSilva.DTOs.Response;
using HilbertoSilva.Services.Interfaces;

namespace HilbertoSilva.Services
{
    public class BoletimService : IBoletimService
    {
        private readonly EscolaDbContext _context;

        public BoletimService(EscolaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BoletimResponseDto>> ObterTodosAsync()
        {
            return await _context.Boletins
                .AsNoTracking()
                .Select(b => new BoletimResponseDto
                {
                    Id = b.Id,
                    AlunoId = b.FkAluno,
                    NotaU1 = b.NotaU1,
                    NotaU2 = b.NotaU2,
                    NotaU3 = b.NotaU3,
                    Frequencia = b.Frequencia,
                    NomeAluno = b.Aluno.Nome,
                    NomeDisciplina = b.DiarioClasse.Disciplina.Nome,
                    Matricula = b.Aluno.Matricula,
                    NomeTurma = b.Aluno.Turma != null ? b.Aluno.Turma.NomeTurma : "Sem Turma"
                })
                .ToListAsync();
        }

        public async Task<BoletimResponseDto?> ObterPorIdAsync(int id)
        {
            return await _context.Boletins
                .AsNoTracking()
                .Where(b => b.Id == id)
                .Select(b => new BoletimResponseDto
                {
                    Id = b.Id,
                    AlunoId = b.FkAluno,
                    NotaU1 = b.NotaU1,
                    NotaU2 = b.NotaU2,
                    NotaU3 = b.NotaU3,
                    Frequencia = b.Frequencia,
                    NomeAluno = b.Aluno.Nome,
                    NomeDisciplina = b.DiarioClasse.Disciplina.Nome,
                    Matricula = b.Aluno.Matricula,
                    NomeTurma = b.Aluno.Turma != null ? b.Aluno.Turma.NomeTurma : "Sem Turma"
                })
                .FirstOrDefaultAsync();
        }

        public async Task<BoletimResponseDto> CriarAsync(CreateBoletimDto request)
        {
            var boletim = new Boletim
            {
                FkAluno = request.AlunoId,
                FkTurmaDisciplina = request.TurmaDisciplinaId,
                NotaU1 = request.NotaU1,
                NotaU2 = request.NotaU2,
                NotaU3 = request.NotaU3,
                Frequencia = request.Frequencia
            };

            _context.Boletins.Add(boletim);
            await _context.SaveChangesAsync();

            return await ObterPorIdAsync(boletim.Id);
        }

        public async Task<bool> AtualizarAsync(int id, UpdateBoletimDto request)
        {
            var boletim = await _context.Boletins.FindAsync(id);

            if (boletim == null) return false;

            boletim.NotaU1 = request.NotaU1;
            boletim.NotaU2 = request.NotaU2;
            boletim.NotaU3 = request.NotaU3;
            boletim.Frequencia = request.Frequencia;

            _context.Boletins.Update(boletim);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeletarAsync(int id)
        {
            var boletim = await _context.Boletins.FindAsync(id);

            if (boletim == null) return false;

            _context.Boletins.Remove(boletim);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}