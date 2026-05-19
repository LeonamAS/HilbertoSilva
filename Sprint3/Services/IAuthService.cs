using HilbertoSilva.DTOs.Request;

namespace HilbertoSilva.Services;

public interface IAuthService
{
    // Retorna o token JWT caso o login tenha sucesso, ou null/empty em caso de falha
    Task<string> LoginAsync(LoginDto dto);

    // Retorna um objeto com dados do usuário criado (evite retornar a entidade com senha)
    Task<object> RegistrarAsync(CreateUsuarioDto dto);

    // Valida a senha atual e salva o hash da nova senha
    Task<bool> AlterarSenhaAsync(int usuarioId, UsuarioAlterarSenhaDto dto);
}
