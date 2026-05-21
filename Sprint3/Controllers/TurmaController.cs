using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HilbertoSilva.Interfaces;
using HilbertoSilva.DTOs.Request.Create;

namespace HilbertoSilva.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TurmaController : ControllerBase
{
    private readonly ITurmaService _turmaService;

    public TurmaController(ITurmaService turmaService)
    {
        _turmaService = turmaService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterTodas()
    {
        var turmas = await _turmaService.ObterTodasAsync();
        return Ok(turmas);
    }

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

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Criar([FromBody] TurmaRequestDto dto)
    {
        var turmaCriada = await _turmaService.CriarAsync(dto);

        return CreatedAtAction(nameof(ObterPorId), new { id = turmaCriada.Id }, turmaCriada);
    }

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