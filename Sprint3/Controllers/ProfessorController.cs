using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HilbertoSilva.DTOs.Request;
using HilbertoSilva.DTOs.Response;
using HilbertoSilva.Interfaces;

namespace HilbertoSilva.Controllers;

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

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterTodos()
    {
        var professores = await _professorService.ObterTodosAsync();
        return Ok(professores);
    }

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

    [HttpPost("com-usuario")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProfessorResponseDto>> CreateComUsuario([FromBody] CreateProfessorComUsuarioDto dto)
    {
        var novoProfessor = await _professorService.CriarUsuarioETransacaoAsync(dto);

        return CreatedAtAction(nameof(ObterPorId), new { id = novoProfessor.Id }, novoProfessor);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] UpdateProfessorDto dto)
    {
        var sucesso = await _professorService.AtualizarAsync(id, dto);

        if (!sucesso)
            return NotFound(new { mensagem = "Não foi possível atualizar. Professor não encontrado." });

        return NoContent();
    }

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