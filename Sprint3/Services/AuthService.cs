using HilbertoSilva.Data;
using HilbertoSilva.DTOs.Request;
using HilbertoSilva.Interfaces;
using HilbertoSilva.Models;
using Microsoft.EntityFrameworkCore;

namespace HilbertoSilva.Services
{
    public class AuthService : IAuthService
    {
        private readonly EscolaDbContext _context;

        public AuthService(EscolaDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> ValidarCredenciaisAsync(LoginDto dto)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Cpf == dto.Cpf);

            if (usuario == null)
                return null;

            if (usuario.Senha == dto.Senha)
            {
                return usuario;
            }

            return null;
        }

        public async Task<object> RegistrarAsync(CreateUsuarioDto dto)
        {
            var usuarioExistente = await _context.Usuarios
                .AnyAsync(u => u.Cpf == dto.Cpf);

            if (usuarioExistente)
            {
                throw new InvalidOperationException("Este CPF ou usuário já está cadastrado.");
            }

            var novoUsuario = new Usuario
            {
                Cpf = dto.Cpf,
                Senha = dto.Senha,
                TipoUsuario = dto.TipoUsuario,
                DataCadastro = DateTime.UtcNow
            };

            await _context.Usuarios.AddAsync(novoUsuario);
            await _context.SaveChangesAsync();

            return new
            {
                Mensagem = "Usuário cadastrado com sucesso no banco de dados!",
                Id = novoUsuario.Id,
                Cpf = novoUsuario.Cpf,
                TipoUsuario = novoUsuario.TipoUsuario
            };
        }

        public async Task<bool> AlterarSenhaAsync(int usuarioId, UsuarioAlterarSenhaDto dto)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);

            if (usuario == null)
                return false;

            if (usuario.Senha != dto.SenhaAtual)
                return false;

            usuario.Senha = dto.NovaSenha;
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}