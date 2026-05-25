using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HilbertoSilva.DTOs.Response;
using HilbertoSilva.Services.Interfaces;
using HilbertoSilva.DTOs.Request.Create;
using HilbertoSilva.DTOs.Request.Update;

namespace HilbertoSilva.Controllers;

/// <summary>
/// Gerencia as operações referentes aos professores do sistema.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfessorController : ControllerBase
{
    private readonly IProfessorService _professorService;

    public ProfessorController(IProfessorService professorService)
    {
        _professorService = professorService;
    }

    /// <summary>
    /// Obtém a lista de todos os professores cadastrados no sistema.
    /// </summary>
    /// <returns>Retorna a lista de professores.</returns>
    /// <response code="200">Lista retornada com sucesso.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterTodos()
    {
        var professores = await _professorService.ObterTodosAsync();
        return Ok(professores);
    }

    /// <summary>
    /// Obtém um professor específico pelo seu ID.
    /// </summary>
    /// <param name="id">O identificador único do professor.</param>
    /// <returns>Retorna os detalhes do professor solicitado.</returns>
    /// <response code="200">Professor encontrado e retornado com sucesso.</response>
    /// <response code="404">Nenhum professor encontrado com o ID fornecido.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var professor = await _professorService.ObterPorIdAsync(id);

        if (professor == null)
            return NotFound(new { mensagem = "Professor não encontrado no sistema." });

        return Ok(professor);
    }

    /// <summary>
    /// Cria um novo professor vinculado a um usuário de acesso.
    /// </summary>
    /// <param name="dto">Objeto contendo os dados do professor e do usuário a ser criado.</param>
    /// <returns>Retorna o professor recém-criado.</returns>
    /// <response code="201">Professor criado com sucesso.</response>
    /// <response code="400">Dados inválidos fornecidos na requisição ou violação de regras de negócio.</response>
    [HttpPost("com-usuario")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProfessorResponseDto>> CreateComUsuario([FromBody] CreateProfessorComUsuarioDto dto)
    {
        try
        {
            var novoProfessor = await _professorService.CriarAsync(dto);
            return CreatedAtAction(nameof(ObterPorId), new { id = novoProfessor.Id }, novoProfessor);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Atualiza os dados de um professor existente.
    /// </summary>
    /// <param name="id">O identificador do professor que será atualizado.</param>
    /// <param name="dto">Objeto contendo os novos dados do professor.</param>
    /// <response code="204">Professor atualizado com sucesso.</response>
    /// <response code="400">Dados inválidos fornecidos na requisição.</response>
    /// <response code="404">Nenhum professor encontrado com o ID fornecido.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] UpdateProfessorDto dto)
    {
        try
        {
            var sucesso = await _professorService.AtualizarAsync(id, dto);

            if (!sucesso)
                return NotFound(new { mensagem = "Não foi possível atualizar. Professor não encontrado." });

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Exclui um professor do sistema.
    /// </summary>
    /// <param name="id">O identificador do professor a ser excluído.</param>
    /// <response code="204">Professor excluído com sucesso.</response>
    /// <response code="404">Nenhum professor encontrado com o ID fornecido.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deletar(int id)
    {
        var sucesso = await _professorService.DeletarAsync(id);

        if (!sucesso)
            return NotFound(new { mensagem = "Não foi possível deletar. Professor não encontrado." });

        return NoContent();
    }
}