using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HilbertoSilva.DTOs.Request.Create;
using HilbertoSilva.DTOs.Request.Update;
using HilbertoSilva.DTOs.Response;
using HilbertoSilva.Services.Interfaces;

namespace HilbertoSilva.Controllers;

[Authorize]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN")]
public class DiarioClasseController : ControllerBase
{
    private readonly IDiarioClasseService _diarioService;

    public DiarioClasseController(IDiarioClasseService diarioService)
    {
        _diarioService = diarioService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DiarioClasseResponseDto>>> ObterTodos()
    {
        var diarios = await _diarioService.ObterTodosAsync();
        return Ok(diarios);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DiarioClasseResponseDto>> ObterPorId(int id)
    {
        var diario = await _diarioService.ObterPorIdAsync(id);

        if (diario == null)
        {
            return NotFound(new { mensagem = "Diário de classe não encontrado." });
        }

        return Ok(diario);
    }

    [HttpPost]
    public async Task<ActionResult<DiarioClasseResponseDto>> Criar([FromBody] CreateDiarioClasseDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var novoDiario = await _diarioService.CriarAsync(request);

        return CreatedAtAction(nameof(ObterPorId), new { id = novoDiario.Id }, novoDiario);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] UpdateDiarioClasseDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var atualizado = await _diarioService.AtualizarAsync(id, request);

        if (!atualizado)
        {
            return NotFound(new { mensagem = "Diário de classe não encontrado para atualização." });
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Deletar(int id)
    {
        var deletado = await _diarioService.DeletarAsync(id);

        if (!deletado)
        {
            return NotFound(new { mensagem = "Diário de classe não encontrado para exclusão." });
        }

        return NoContent();
    }
}