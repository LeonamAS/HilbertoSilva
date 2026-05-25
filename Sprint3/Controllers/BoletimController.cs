using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using HilbertoSilva.DTOs.Request.Create;
using HilbertoSilva.DTOs.Request.Update;
using HilbertoSilva.DTOs.Response;
using HilbertoSilva.Services.Interfaces;

namespace HilbertoSilva.Controllers;

/// <summary>
/// Gerencia as operações relacionadas aos boletins escolares.
/// </summary>
[Authorize]
[Route("api/[controller]")]
[ApiController]
public class BoletimController : ControllerBase
{
    private readonly IBoletimService _boletimService;

    public BoletimController(IBoletimService boletimService)
    {
        _boletimService = boletimService;
    }

    /// <summary>
    /// Obtém os boletins do aluno autenticado com base no seu CPF.
    /// </summary>
    /// <returns>Retorna uma lista de boletins do aluno logado.</returns>
    /// <response code="200">Lista de boletins retornada com sucesso (pode ser vazia se não houver registros).</response>
    /// <response code="401">Sessão inválida ou token corrompido.</response>
    [HttpGet("meu-boletim")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<BoletimResponseDto>>> ObterMeuBoletim()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdClaim, out int usuarioId))
            return Unauthorized(new { mensagem = "Sessão inválida." });

        var boletins = await _boletimService.ObterPorUsuarioIdAsync(usuarioId);

        if (boletins == null || !boletins.Any())
            return Ok(new List<BoletimResponseDto>());

        return Ok(boletins);
    }

    /// <summary>
    /// Obtém a lista de todos os boletins cadastrados no sistema.
    /// </summary>
    /// <returns>Retorna a lista de boletins.</returns>
    /// <response code="200">Lista retornada com sucesso.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BoletimResponseDto>>> ObterTodos()
    {
        var boletins = await _boletimService.ObterTodosAsync();
        return Ok(boletins);
    }

    /// <summary>
    /// Obtém um boletim específico pelo seu ID.
    /// </summary>
    /// <param name="id">O identificador único do boletim.</param>
    /// <returns>Retorna os detalhes do boletim solicitado.</returns>
    /// <response code="200">Boletim encontrado e retornado com sucesso.</response>
    /// <response code="404">Nenhum boletim encontrado com o ID fornecido.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BoletimResponseDto>> ObterPorId(int id)
    {
        var boletim = await _boletimService.ObterPorIdAsync(id);

        if (boletim == null)
        {
            return NotFound(new { mensagem = "Boletim não encontrado." });
        }

        return Ok(boletim);
    }

    /// <summary>
    /// Cria um novo boletim escolar.
    /// </summary>
    /// <param name="request">Objeto contendo os dados do boletim a ser criado.</param>
    /// <returns>Retorna o boletim recém-criado.</returns>
    /// <response code="201">Boletim criado com sucesso.</response>
    /// <response code="400">Dados inválidos fornecidos na requisição.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BoletimResponseDto>> Criar([FromBody] CreateBoletimDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var novoBoletim = await _boletimService.CriarAsync(request);

        return CreatedAtAction(nameof(ObterPorId), new { id = novoBoletim.Id }, novoBoletim);
    }

    /// <summary>
    /// Atualiza os dados de um boletim existente.
    /// </summary>
    /// <param name="id">O identificador do boletim que será atualizado.</param>
    /// <param name="request">Objeto contendo os novos dados do boletim.</param>
    /// <response code="204">Boletim atualizado com sucesso.</response>
    /// <response code="400">Dados inválidos fornecidos na requisição.</response>
    /// <response code="404">Nenhum boletim encontrado com o ID fornecido.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] UpdateBoletimDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var atualizado = await _boletimService.AtualizarAsync(id, request);

        if (!atualizado)
        {
            return NotFound(new { mensagem = "Boletim não encontrado para atualização." });
        }

        return NoContent();
    }

    /// <summary>
    /// Exclui um boletim do sistema.
    /// </summary>
    /// <param name="id">O identificador do boletim a ser excluído.</param>
    /// <response code="204">Boletim excluído com sucesso.</response>
    /// <response code="404">Nenhum boletim encontrado com o ID fornecido.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deletar(int id)
    {
        var deletado = await _boletimService.DeletarAsync(id);

        if (!deletado)
        {
            return NotFound(new { mensagem = "Boletim não encontrado para exclusão." });
        }

        return NoContent();
    }
}