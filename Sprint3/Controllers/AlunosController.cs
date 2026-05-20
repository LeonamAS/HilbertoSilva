using HilbertoSilva.DTOs.Request;
using HilbertoSilva.DTOs.Response;
using HilbertoSilva.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HilbertoSilva.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlunosController : ControllerBase
{
    private readonly IAlunoService _alunoService;

    public AlunosController(IAlunoService alunoService)
    {
        _alunoService = alunoService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AlunoResponseDto>>> GetAlunos()
    {
        var alunos = await _alunoService.ObterAlunosAsync();
        return Ok(alunos);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AlunoResponseDto>> GetAlunoPorId(int id)
    {
        var aluno = await _alunoService.ObterAlunoPorIdAsync(id);

        if (aluno == null)
            return NotFound(new { mensagem = "Aluno não encontrado." });

        return Ok(aluno);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AlunoResponseDto>> Create([FromBody] CreateAlunoDto createAlunoDto)
    {
        var novoAluno = await _alunoService.CriarAlunoAsync(createAlunoDto);


        return CreatedAtAction(nameof(GetAlunoPorId), new { id = novoAluno.Id }, novoAluno);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAluno(int id, [FromBody] UpdateAlunoDto updateAlunoDto)
    {
        var atualizado = await _alunoService.AtualizarAlunoAsync(id, updateAlunoDto);

        if (!atualizado)
            return NotFound(new { mensagem = "Aluno não encontrado para atualização." });

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAluno(int id)
    {
        var deletado = await _alunoService.DeletarAlunoAsync(id);

        if (!deletado)
            return NotFound(new { mensagem = "Aluno não encontrado para exclusão." });

        return NoContent();
    }
}