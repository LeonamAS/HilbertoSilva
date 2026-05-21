using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HilbertoSilva.DTOs.Request.Create;
using HilbertoSilva.DTOs.Request.Update;
using HilbertoSilva.DTOs.Response;
using HilbertoSilva.Services.Interfaces;

namespace HilbertoSilva.Controllers;

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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BoletimResponseDto>>> ObterTodos()
    {
        var boletins = await _boletimService.ObterTodosAsync();
        return Ok(boletins);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BoletimResponseDto>> ObterPorId(int id)
    {
        var boletim = await _boletimService.ObterPorIdAsync(id);

        if (boletim == null)
        {
            return NotFound(new { mensagem = "Boletim não encontrado." });
        }

        return Ok(boletim);
    }

    [HttpPost]
    public async Task<ActionResult<BoletimResponseDto>> Criar([FromBody] CreateBoletimDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var novoBoletim = await _boletimService.CriarAsync(request);

        return CreatedAtAction(nameof(ObterPorId), new { id = novoBoletim.Id }, novoBoletim);
    }

    [HttpPut("{id}")]
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

    [HttpDelete("{id}")]
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