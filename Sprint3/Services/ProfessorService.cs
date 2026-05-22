using Microsoft.EntityFrameworkCore;
using HilbertoSilva.Data;
using HilbertoSilva.DTOs.Response;
using HilbertoSilva.Services.Interfaces;
using HilbertoSilva.Models;
using HilbertoSilva.DTOs.Request.Create;
using HilbertoSilva.DTOs.Request.Update;
using System.Text.RegularExpressions;

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
        var cpfLimpo = ValidarELimparCpf(dto.Usuario.Cpf);
        var cpfJaExiste = await _context.Usuarios.AnyAsync(u => u.Cpf == cpfLimpo);

        if (cpfJaExiste)
        {
            throw new InvalidOperationException("O CPF informado já está cadastrado no sistema.");
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var novoUsuario = new Usuario
            {
                Cpf = cpfLimpo,
                Senha = dto.Usuario.Senha,
                TipoUsuario = dto.Usuario.TipoUsuario,
                DataCadastro = DateTime.Now
            };

            _context.Set<Usuario>().Add(novoUsuario);
            await _context.SaveChangesAsync();

            var novoProfessor = new Professor
            {
                FkUsuario = novoUsuario.Id,
                Nome = dto.Professor.Nome.Trim(),
                Telefone = ManterApenasNumeros(dto.Professor.Telefone),
                Especialidade = dto.Professor.Especialidade?.Trim()
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

        professor.Nome = dto.Nome.Trim();
        professor.Telefone = ManterApenasNumeros(dto.Telefone);
        professor.Especialidade = dto.Especialidade?.Trim();

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

    private string ValidarELimparCpf(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            throw new ArgumentException("O CPF não pode ser vazio.");

        var cpfNumeros = ManterApenasNumeros(cpf);

        if (cpfNumeros.Length != 11)
            throw new ArgumentException("O CPF está no formato incorreto. Ele deve conter exatamente 11 números.");

        return cpfNumeros;
    }

    private string ManterApenasNumeros(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            return string.Empty;

        return Regex.Replace(valor, @"[^\d]", "");
    }
}