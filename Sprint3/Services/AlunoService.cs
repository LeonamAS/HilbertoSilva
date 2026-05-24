using Microsoft.EntityFrameworkCore;
using HilbertoSilva.Data;
using HilbertoSilva.DTOs.Response;
using HilbertoSilva.Services.Interfaces;
using HilbertoSilva.Models;
using HilbertoSilva.DTOs.Request.Create;
using HilbertoSilva.DTOs.Request.Update;
using System.Text.RegularExpressions;

namespace HilbertoSilva.Services;

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
                NomeResponsavel = aluno.NomeResponsavel,
                CpfResponsavel = aluno.CpfResponsavel,
                TelefoneResponsavel = aluno.TelefoneResponsavel,
                Cpf = aluno.Usuario != null ? aluno.Usuario.Cpf : string.Empty
            })
            .ToListAsync();
    }

    public async Task<AlunoResponseDto?> ObterPorIdAsync(int id)
    {
        var aluno = await _context.Alunos
            .Include(a => a.Usuario)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (aluno == null)
            return null;

        return new AlunoResponseDto
        {
            Id = aluno.Id,
            Nome = aluno.Nome,
            Matricula = aluno.Matricula,
            DataNascimento = aluno.DataNascimento,
            NomeResponsavel = aluno.NomeResponsavel,
            CpfResponsavel = aluno.CpfResponsavel,
            TelefoneResponsavel = aluno.TelefoneResponsavel,
            Cpf = aluno.Usuario != null ? aluno.Usuario.Cpf : string.Empty
        };
    }

    public async Task<AlunoResponseDto> CriarAsync(CreateAlunoComUsuarioDto dto)
    {
        var cpfUsuarioLimpo = ValidarELimparCpf(dto.Usuario.Cpf, "Usuário");
        var cpfResponsavelLimpo = ValidarELimparCpf(dto.Aluno.CpfResponsavel, "Responsável");

        var cpfJaExiste = await _context.Usuarios.AnyAsync(u => u.Cpf == cpfUsuarioLimpo);
        if (cpfJaExiste)
        {
            throw new InvalidOperationException("O CPF informado para o usuário já está cadastrado no sistema.");
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var novoUsuario = new Usuario
            {
                Cpf = cpfUsuarioLimpo,
                Senha = dto.Usuario.Senha,
                TipoUsuario = dto.Usuario.TipoUsuario,
                DataCadastro = DateTime.Now
            };

            _context.Set<Usuario>().Add(novoUsuario);
            await _context.SaveChangesAsync();

            int anoAtual = DateTime.Now.Year;
            string prefixoMatricula = $"MAT{anoAtual}-";

            var ultimaMatricula = await _context.Alunos
                .Where(a => a.Matricula.StartsWith(prefixoMatricula))
                .OrderByDescending(a => a.Matricula)
                .Select(a => a.Matricula)
                .FirstOrDefaultAsync();

            int proximoNumero = 1;

            if (!string.IsNullOrEmpty(ultimaMatricula))
            {
                string sufixoNumero = ultimaMatricula.Substring(prefixoMatricula.Length);

                if (int.TryParse(sufixoNumero, out int ultimoNumeroConvertido))
                {
                    proximoNumero = ultimoNumeroConvertido + 1;
                }
            }

            string novaMatriculaGerada = $"{prefixoMatricula}{proximoNumero:D3}";

            var novoAluno = new Aluno
            {
                FkUsuario = novoUsuario.Id,
                FkTurma = dto.Aluno.TurmaId,
                Nome = dto.Aluno.Nome.Trim(),
                DataNascimento = dto.Aluno.DataNascimento,
                Matricula = novaMatriculaGerada,
                NomeResponsavel = dto.Aluno.NomeResponsavel.Trim(),
                CpfResponsavel = cpfResponsavelLimpo,
                TelefoneResponsavel = ManterApenasNumeros(dto.Aluno.TelefoneResponsavel)
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

        var cpfResponsavelLimpo = ValidarELimparCpf(dto.CpfResponsavel, "Responsável");

        aluno.Nome = dto.Nome.Trim();
        aluno.DataNascimento = dto.DataNascimento;
        aluno.NomeResponsavel = dto.NomeResponsavel.Trim();
        aluno.TelefoneResponsavel = ManterApenasNumeros(dto.TelefoneResponsavel);
        aluno.CpfResponsavel = cpfResponsavelLimpo;
        aluno.FkTurma = dto.TurmaId;

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

    private string ValidarELimparCpf(string cpf, string tipo)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            throw new ArgumentException($"O CPF do {tipo} não pode ser vazio.");

        var cpfNumeros = ManterApenasNumeros(cpf);

        if (cpfNumeros.Length != 11)
            throw new ArgumentException($"O CPF do {tipo} está no formato incorreto. Ele deve conter exatamente 11 números.");

        return cpfNumeros;
    }

    private string ManterApenasNumeros(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            return string.Empty;

        return Regex.Replace(valor, @"[^\d]", "");
    }
}