using HilbertoSilva.DTOs.Request.Create;
using HilbertoSilva.DTOs.Request.Update;
using HilbertoSilva.DTOs.Response;
using HilbertoSilva.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HilbertoSilva.Controllers;

/// <summary>
/// Gerencia as operações referentes aos alunos do sistema.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN")]
public class AlunosController : ControllerBase
{
    private readonly IAlunoService _alunoService;

    public AlunosController(IAlunoService alunoService)
    {
        _alunoService = alunoService;
    }

    /// <summary>
    /// Obtém a lista de todos os alunos cadastrados no sistema.
    /// </summary>
    /// <returns>Retorna a lista de alunos.</returns>
    /// <response code="200">Lista de alunos retornada com sucesso.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AlunoResponseDto>>> GetAlunos()
    {
        var alunos = await _alunoService.ObterTodosAsync();
        return Ok(alunos);
    }

    /// <summary>
    /// Obtém um aluno específico pelo seu ID.
    /// </summary>
    /// <param name="id">O identificador único do aluno.</param>
    /// <returns>Retorna os detalhes do aluno solicitado.</returns>
    /// <response code="200">Aluno encontrado e retornado com sucesso.</response>
    /// <response code="404">Nenhum aluno encontrado com o ID fornecido.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AlunoResponseDto>> GetAlunoPorId(int id)
    {
        var aluno = await _alunoService.ObterPorIdAsync(id);

        if (aluno == null)
            return NotFound(new { mensagem = "Aluno não encontrado." });

        return Ok(aluno);
    }

    /// <summary>
    /// Cria um novo aluno vinculado a um usuário de acesso.
    /// </summary>
    /// <param name="dto">Objeto contendo os dados do aluno e do usuário a ser criado.</param>
    /// <returns>Retorna o aluno recém-criado.</returns>
    /// <response code="201">Aluno criado com sucesso.</response>
    /// <response code="400">Dados inválidos fornecidos na requisição ou violação de regras de negócio.</response>
    [HttpPost("com-usuario")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AlunoResponseDto>> CreateComUsuario([FromBody] CreateAlunoComUsuarioDto dto)
    {
        try
        {
            var novoAluno = await _alunoService.CriarAsync(dto);
            return CreatedAtAction(nameof(GetAlunoPorId), new { id = novoAluno.Id }, novoAluno);
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
    /// Atualiza os dados de um aluno existente.
    /// </summary>
    /// <param name="id">O identificador do aluno que será atualizado.</param>
    /// <param name="updateAlunoDto">Objeto contendo os novos dados do aluno.</param>
    /// <response code="204">Aluno atualizado com sucesso.</response>
    /// <response code="400">Dados inválidos fornecidos na requisição.</response>
    /// <response code="404">Nenhum aluno encontrado com o ID fornecido.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAluno(int id, [FromBody] UpdateAlunoDto updateAlunoDto)
    {
        try
        {
            var atualizado = await _alunoService.AtualizarAsync(id, updateAlunoDto);

            if (!atualizado)
                return NotFound(new { mensagem = "Aluno não encontrado para atualização." });

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Exclui um aluno do sistema.
    /// </summary>
    /// <param name="id">O identificador do aluno a ser excluído.</param>
    /// <response code="204">Aluno excluído com sucesso.</response>
    /// <response code="404">Nenhum aluno encontrado com o ID fornecido.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAluno(int id)
    {
        var deletado = await _alunoService.DeletarAsync(id);

        if (!deletado)
            return NotFound(new { mensagem = "Aluno não encontrado para exclusão." });

        return NoContent();
    }
}