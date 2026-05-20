using HilbertoSilva.DTOs.Request;
using HilbertoSilva.Models;

namespace HilbertoSilva.Interfaces;

public interface IAuthService
{
    // Retorna o token JWT caso o login tenha sucesso, ou null/empty em caso de falha
    Task<Usuario?> ValidarCredenciaisAsync(LoginDto dto);

    // Retorna um objeto com dados do usuário criado
    Task<object> RegistrarAsync(CreateUsuarioDto dto);

    // Valida a senha atual e salva o hash da nova senha
    Task<bool> AlterarSenhaAsync(int usuarioId, UsuarioAlterarSenhaDto dto);
}