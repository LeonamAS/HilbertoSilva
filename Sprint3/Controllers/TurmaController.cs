using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HilbertoSilva.Interfaces;
using HilbertoSilva.DTOs.Request.Create;

namespace HilbertoSilva.Controllers;

/// <summary>
/// Gerencia as operações referentes às turmas da escola.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN")]
public class TurmaController : ControllerBase
{
    private readonly ITurmaService _turmaService;

    public TurmaController(ITurmaService turmaService)
    {
        _turmaService = turmaService;
    }

    /// <summary>
    /// Obtém a lista de todas as turmas cadastradas no sistema.
    /// </summary>
    /// <returns>Retorna a lista de turmas.</returns>
    /// <response code="200">Lista de turmas retornada com sucesso.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterTodas()
    {
        var turmas = await _turmaService.ObterTodosAsync();
        return Ok(turmas);
    }

    /// <summary>
    /// Obtém uma turma específica pelo seu ID.
    /// </summary>
    /// <param name="id">O identificador único da turma.</param>
    /// <returns>Retorna os detalhes da turma solicitada.</returns>
    /// <response code="200">Turma encontrada e retornada com sucesso.</response>
    /// <response code="404">Nenhuma turma encontrada com o ID fornecido.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var turma = await _turmaService.ObterPorIdAsync(id);

        if (turma == null)
            return NotFound(new { mensagem = "Turma não encontrada." });

        return Ok(turma);
    }

    /// <summary>
    /// Cria uma nova turma no sistema.
    /// </summary>
    /// <param name="dto">Objeto contendo os dados da turma a ser criada.</param>
    /// <returns>Retorna a turma recém-criada.</returns>
    /// <response code="201">Turma criada com sucesso.</response>
    /// <response code="400">Dados inválidos fornecidos na requisição.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Criar([FromBody] TurmaRequestDto dto)
    {
        var turmaCriada = await _turmaService.CriarAsync(dto);

        return CreatedAtAction(nameof(ObterPorId), new { id = turmaCriada.Id }, turmaCriada);
    }

    /// <summary>
    /// Atualiza os dados de uma turma existente.
    /// </summary>
    /// <param name="id">O identificador da turma que será atualizada.</param>
    /// <param name="dto">Objeto contendo os novos dados da turma.</param>
    /// <response code="204">Turma atualizada com sucesso.</response>
    /// <response code="400">Dados inválidos fornecidos na requisição.</response>
    /// <response code="404">Nenhuma turma encontrada com o ID fornecido.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] TurmaRequestDto dto)
    {
        var sucesso = await _turmaService.AtualizarAsync(id, dto);

        if (!sucesso)
            return NotFound(new { mensagem = "Não foi possível atualizar. Turma não encontrada." });

        return NoContent();
    }

    /// <summary>
    /// Exclui uma turma do sistema.
    /// </summary>
    /// <param name="id">O identificador da turma a ser excluída.</param>
    /// <response code="204">Turma excluída com sucesso.</response>
    /// <response code="404">Nenhuma turma encontrada com o ID fornecido.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deletar(int id)
    {
        var sucesso = await _turmaService.DeletarAsync(id);

        if (!sucesso)
            return NotFound(new { mensagem = "Não foi possível deletar. Turma não encontrada." });

        return NoContent();
    }
}