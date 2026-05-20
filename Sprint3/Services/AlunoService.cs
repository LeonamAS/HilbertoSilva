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

        public async Task<AlunoResponseDto> CriarComUsuarioETransacaoAsync(CreateAlunoComUsuarioDto dto)
        {
            // 1. Inicia a transação garantindo a atomicidade (tudo ou nada)
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 2. Instancia o Usuário com base no modelo real 'Usuario'
                var novoUsuario = new Usuario
                {
                    Cpf = dto.Usuario.Cpf,
                    Senha = dto.Usuario.Senha, // Caso use hash de senha, aplique aqui (ex: BCrypt ou Identity)
                    TipoUsuario = dto.Usuario.TipoUsuario
                    // Note que o 'DataCadastro' já inicializa sozinho com DateTime.Now no seu modelo!
                };

                // Adiciona e salva para gerar o 'usuario_id' no banco de dados
                _context.Set<Usuario>().Add(novoUsuario);
                await _context.SaveChangesAsync();

                // 3. Instancia o Aluno usando os mapeamentos exatos do seu modelo real
                var novoAluno = new Aluno
                {
                    FkUsuario = novoUsuario.Id,          // <-- Usando a propriedade real 'FkUsuario'
                    FkTurma = dto.Aluno.TurmaId,         // <-- Mapeando o int? para 'FkTurma'
                    Nome = dto.Aluno.Nome,
                    DataNascimento = dto.Aluno.DataNascimento,
                    Matricula = dto.Aluno.Matricula,
                    NomeResponsavel = dto.Aluno.NomeResponsavel,
                    CpfResponsavel = dto.Aluno.CpfResponsavel,
                    TelefoneResponsavel = dto.Aluno.TelefoneResponsavel
                };

                _context.Set<Aluno>().Add(novoAluno);
                await _context.SaveChangesAsync();

                // 4. Se o fluxo chegou aqui sem erros, confirma a operação no banco definitivamente
                await transaction.CommitAsync();

                // 5. Retorna o DTO de resposta preenchido
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
                // Caso ocorra qualquer erro (Ex: CPF ou Matrícula duplicada), desfaz tudo
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