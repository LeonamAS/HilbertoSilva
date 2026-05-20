using Microsoft.EntityFrameworkCore;
using HilbertoSilva.Data;
using HilbertoSilva.DTOs.Request;
using HilbertoSilva.DTOs.Response;
using HilbertoSilva.Interfaces;
using HilbertoSilva.Models;

namespace HilbertoSilva.Services
{
    public class AlunoService : IAlunoService
    {
        private readonly EscolaDbContext _context;

        public AlunoService(EscolaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AlunoResponseDto>> ObterAlunosAsync()
        {
            return await _context.Alunos
                .Select(aluno => new AlunoResponseDto
                {
                    Id = aluno.Id,
                    Nome = aluno.Nome,
                    Matricula = aluno.Matricula,
                    DataNascimento = aluno.DataNascimento,
                    NomeResponsavel = aluno.NomeResponsavel
                })
                .ToListAsync();
        }

        public async Task<AlunoResponseDto?> ObterAlunoPorIdAsync(int id)
        {
            var aluno = await _context.Alunos.FindAsync(id);

            if (aluno == null)
                return null;

            return new AlunoResponseDto
            {
                Id = aluno.Id,
                Nome = aluno.Nome,
                Matricula = aluno.Matricula,
                DataNascimento = aluno.DataNascimento,
                NomeResponsavel = aluno.NomeResponsavel
            };
        }

        public async Task<AlunoResponseDto> CriarAlunoAsync(CreateAlunoDto dto)
        {
            var novoAluno = new Aluno
            {
                //UsuarioId = dto.UsuarioId,
                //TurmaId = dto.TurmaId,
                Nome = dto.Nome,
                DataNascimento = dto.DataNascimento,
                Matricula = dto.Matricula,
                NomeResponsavel = dto.NomeResponsavel,
                CpfResponsavel = dto.CpfResponsavel,
                TelefoneResponsavel = dto.TelefoneResponsavel
            };

            await _context.Alunos.AddAsync(novoAluno);
            await _context.SaveChangesAsync();

            return new AlunoResponseDto
            {
                Id = novoAluno.Id,
                Nome = novoAluno.Nome,
                Matricula = novoAluno.Matricula,
                DataNascimento = novoAluno.DataNascimento,
                NomeResponsavel = novoAluno.NomeResponsavel
                // Se o seu AlunoResponseDto tiver as propriedades abaixo, pode descomentá-las:
                // CpfResponsavel = novoAluno.CpfResponsavel,
                // TelefoneResponsavel = novoAluno.TelefoneResponsavel,
                // UsuarioId = novoAluno.UsuarioId,
                // TurmaId = novoAluno.TurmaId
            };
        }

        public async Task<bool> AtualizarAlunoAsync(int id, UpdateAlunoDto dto)
        {
            var aluno = await _context.Alunos.FindAsync(id);

            if (aluno == null)
                return false;

            aluno.Nome = dto.Nome;
            aluno.DataNascimento = dto.DataNascimento;
            aluno.NomeResponsavel = dto.NomeResponsavel;
            aluno.TelefoneResponsavel = dto.TelefoneResponsavel;
            aluno.CpfResponsavel = dto.CpfResponsavel;

            _context.Alunos.Update(aluno);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeletarAlunoAsync(int id)
        {
            var aluno = await _context.Alunos.FindAsync(id);

            if (aluno == null)
                return false;

            _context.Alunos.Remove(aluno);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}