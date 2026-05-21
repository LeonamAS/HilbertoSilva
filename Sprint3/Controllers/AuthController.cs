using HilbertoSilva.DTOs.Request;
using HilbertoSilva.DTOs.Request.Create;
using HilbertoSilva.DTOs.Request.Update;
using HilbertoSilva.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HilbertoSilva.Controllers;

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

    [HttpPost("registrar")]
    [Authorize(Roles = "ADMIN")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Registrar([FromBody] CreateUsuarioDto createUsuarioDto)
    {
        var usuarioCriado = await _authService.RegistrarAsync(createUsuarioDto);
        return StatusCode(StatusCodes.Status201Created, usuarioCriado);
    }

    [HttpPut("alterar-senha")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AlterarSenha([FromBody] UsuarioAlterarSenhaDto alterarSenhaDto)
    {
        // Extrai o ID do usuário diretamente do Token JWT
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdClaim, out int usuarioId))
            return Unauthorized(new { mensagem = "Usuário inválido ou token corrompido." });

        var sucesso = await _authService.AlterarSenhaAsync(usuarioId, alterarSenhaDto);

        if (!sucesso)
            return BadRequest(new { mensagem = "Não foi possível alterar a senha. Verifique se a senha atual está correta." });

        return NoContent();
    }
}