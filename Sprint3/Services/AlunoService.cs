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

        public async Task<IEnumerable<AlunoResponseDto>> ObterTodosAsync()
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

        public async Task<AlunoResponseDto?> ObterPorIdAsync(int id)
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

        public async Task<AlunoResponseDto> CriarUsuarioETransacaoAsync(CreateAlunoComUsuarioDto dto)
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

                var novoAluno = new Aluno
                {
                    FkUsuario = novoUsuario.Id,
                    FkTurma = dto.Aluno.TurmaId,
                    Nome = dto.Aluno.Nome,
                    DataNascimento = dto.Aluno.DataNascimento,
                    Matricula = dto.Aluno.Matricula,
                    NomeResponsavel = dto.Aluno.NomeResponsavel,
                    CpfResponsavel = dto.Aluno.CpfResponsavel,
                    TelefoneResponsavel = dto.Aluno.TelefoneResponsavel
                };

                _context.Set<Aluno>().Add(novoAluno);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new AlunoResponseDto
                {
                    Id = novoAluno.Id,
                    Nome = novoAluno.Nome,
                    DataNascimento = novoAluno.DataNascimento,
                    Matricula = novoAluno.Matricula,
                    NomeResponsavel = novoAluno.NomeResponsavel,
                    CpfResponsavel = novoAluno.CpfResponsavel,
                    TelefoneResponsavel = novoAluno.TelefoneResponsavel
                };
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> AtualizarAsync(int id, UpdateAlunoDto dto)
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

        public async Task<bool> DeletarAsync(int id)
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