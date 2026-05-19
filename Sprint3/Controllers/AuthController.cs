using HilbertoSilva.DTOs.Request;
using HilbertoSilva.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HilbertoSilva.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var token = await _authService.LoginAsync(loginDto);

        if (string.IsNullOrEmpty(token))
            return Unauthorized(new { mensagem = "CPF ou senha inválidos." });

        return Ok(new { token });
    }

    [HttpPost("registrar")]
    [AllowAnonymous] // Nota: Se apenas Admins puderem criar usuários, troque por [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Registrar([FromBody] CreateUsuarioDto createUsuarioDto)
    {
        var usuarioCriado = await _authService.RegistrarAsync(createUsuarioDto);

        // Retornando 201 Created. Em um cenário real, aponte para um GetById de usuários.
        return StatusCode(StatusCodes.Status201Created, usuarioCriado);
    }

    [HttpPut("alterar-senha")]
    [Authorize] // Exige que o usuário envie um Bearer token válido
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AlterarSenha([FromBody] UsuarioAlterarSenhaDto alterarSenhaDto)
    {
        // Extrai o ID do usuário diretamente do Token JWT (claim NameIdentifier ou a que você configurou no seu gerador de JWT)
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdClaim, out int usuarioId))
            return Unauthorized(new { mensagem = "Usuário inválido ou token corrompido." });

        var sucesso = await _authService.AlterarSenhaAsync(usuarioId, alterarSenhaDto);

        if (!sucesso)
            return BadRequest(new { mensagem = "Não foi possível alterar a senha. Verifique se a senha atual está correta." });

        return NoContent();
    }
}