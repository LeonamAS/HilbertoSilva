using HilbertoSilva.DTOs.Request;
using HilbertoSilva.DTOs.Request.Create;
using HilbertoSilva.DTOs.Request.Update;
using HilbertoSilva.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HilbertoSilva.Controllers;

/// <summary>
/// Gerencia a autenticação e registro de usuários no sistema.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ITokenService _tokenService;

    public AuthController(IAuthService authService, ITokenService tokenService)
    {
        _authService = authService;
        _tokenService = tokenService;
    }

    /// <summary>
    /// Realiza o login de um usuário e retorna um Token JWT.
    /// </summary>
    /// <param name="loginDto">Objeto contendo CPF e senha do usuário.</param>
    /// <returns>Retorna um Token JWT em caso de sucesso.</returns>
    /// <response code="200">Login realizado com sucesso.</response>
    /// <response code="401">CPF ou senha inválidos.</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var usuario = await _authService.ValidarCredenciaisAsync(loginDto);

        if (usuario == null)
            return Unauthorized(new { mensagem = "CPF ou senha inválidos." });

        var token = _tokenService.GerarToken(usuario);

        return Ok(new { token });
    }

    /// <summary>
    /// Registra um novo usuário no sistema. (Restrito a administradores).
    /// </summary>
    /// <param name="createUsuarioDto">Objeto com os dados de criação do usuário.</param>
    /// <response code="201">Usuário criado com sucesso.</response>
    /// <response code="400">Erros de validação na requisição.</response>
    [HttpPost("registrar")]
    [Authorize(Roles = "ADMIN")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Registrar([FromBody] CreateUsuarioDto createUsuarioDto)
    {
        var usuarioCriado = await _authService.RegistrarAsync(createUsuarioDto);
        return StatusCode(StatusCodes.Status201Created, usuarioCriado);
    }

    /// <summary>
    /// Altera a senha do usuário autenticado.
    /// </summary>
    /// <param name="alterarSenhaDto">Objeto contendo a senha atual e a nova senha desejada.</param>
    /// <response code="204">Senha alterada com sucesso. (Não retorna conteúdo)</response>
    /// <response code="400">Não foi possível alterar a senha (ex: a senha atual informada está incorreta).</response>
    /// <response code="401">Usuário não autenticado, sessão inválida ou token corrompido.</response>
    [HttpPut("alterar-senha")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AlterarSenha([FromBody] UsuarioAlterarSenhaDto alterarSenhaDto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdClaim, out int usuarioId))
            return Unauthorized(new { mensagem = "Usuário inválido ou token corrompido." });

        var sucesso = await _authService.AlterarSenhaAsync(usuarioId, alterarSenhaDto);

        if (!sucesso)
            return BadRequest(new { mensagem = "Não foi possível alterar a senha. Verifique se a senha atual está correta." });

        return NoContent();
    }
}