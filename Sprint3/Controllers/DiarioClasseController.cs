using HilbertoSilva.DTOs.Request.Create;
using HilbertoSilva.DTOs.Request.Update;
using HilbertoSilva.DTOs.Response;
using HilbertoSilva.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HilbertoSilva.Controllers;

/// <summary>
/// Gerencia as operações referentes aos diários de classe.
/// </summary>
[Authorize]
[Route("api/[controller]")]
[Authorize]
public class DiarioClasseController : ControllerBase
{
    private readonly IDiarioClasseService _diarioService;

    public DiarioClasseController(IDiarioClasseService diarioService)
    {
        _diarioService = diarioService;
    }

    /// <summary>
    /// Obtém as turmas e disciplinas vinculadas ao professor autenticado.
    /// </summary>
    /// <response code="200">Lista de turmas retornada com sucesso.</response>
    /// <response code="401">Sessão inválida.</response>
    [HttpGet("minhas-turmas")]
    [Authorize(Roles = "PROFESSOR")]
    public async Task<IActionResult> ObterMinhasTurmas()
    {
        // Pega o ID do Usuário direto do Token JWT (Igual ao seu "meu-boletim")
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdClaim, out int usuarioId))
            return Unauthorized(new { mensagem = "Sessão inválida." });

        // O Service deve buscar apenas as turmas que esse ID de usuário (Professor) leciona
        var turmas = await _diarioService.ObterTurmasPorProfessorUsuarioIdAsync(usuarioId);
        return Ok(turmas);
    }

    /// <summary>
    /// Obtém os alunos e suas respectivas notas filtrados por Turma e Disciplina para o Diário.
    /// </summary>
    [HttpGet("turma/{turmaId}/disciplina/{disciplinaId}")]
    [Authorize(Roles = "PROFESSOR,ADMIN")]
    public async Task<IActionResult> ObterAlunosDiario(int turmaId, int disciplinaId)
    {
        var alunosDiario = await _diarioService.ObterAlunosPorTurmaEDisciplinaAsync(turmaId, disciplinaId);
        return Ok(alunosDiario);
    }

    /// <summary>
    /// Lança ou atualiza as notas e a frequência de um aluno específico no diário.
    /// </summary>
    [HttpPut("lancar-notas")]
    [Authorize(Roles = "PROFESSOR,ADMIN")]
    public async Task<IActionResult> LancarNotas([FromBody] LancarNotasDto request) // <-- Mudamos aqui!
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var atualizado = await _diarioService.LancarNotasAsync(request);

        if (!atualizado)
        {
            return NotFound(new { mensagem = "Registro de diário não encontrado para os parâmetros informados." });
        }

        return NoContent();
    }

    /// <summary>
    /// Obtém a lista de todos os diários de classe cadastrados no sistema.
    /// </summary>
    /// <returns>Retorna a lista de diários de classe.</returns>
    /// <response code="200">Lista retornada com sucesso.</response>
    [HttpGet]
    [Authorize(Roles = "ADMIN")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<DiarioClasseResponseDto>>> ObterTodos()
    {
        var diarios = await _diarioService.ObterTodosAsync();
        return Ok(diarios);
    }

    /// <summary>
    /// Obtém um diário de classe específico pelo seu ID.
    /// </summary>
    /// <param name="id">O identificador único do diário de classe.</param>
    /// <returns>Retorna os detalhes do diário de classe solicitado.</returns>
    /// <response code="200">Diário de classe encontrado e retornado com sucesso.</response>
    /// <response code="404">Nenhum diário de classe encontrado com o ID fornecido.</response>
    [HttpGet("{id}")]
    [Authorize(Roles = "ADMIN")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DiarioClasseResponseDto>> ObterPorId(int id)
    {
        var diario = await _diarioService.ObterPorIdAsync(id);

        if (diario == null)
        {
            return NotFound(new { mensagem = "Diário de classe não encontrado." });
        }

        return Ok(diario);
    }

    /// <summary>
    /// Cria um novo diário de classe.
    /// </summary>
    /// <param name="request">Objeto contendo os dados do diário a ser criado.</param>
    /// <returns>Retorna o diário de classe recém-criado.</returns>
    /// <response code="201">Diário de classe criado com sucesso.</response>
    /// <response code="400">Dados inválidos fornecidos na requisição.</response>
    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DiarioClasseResponseDto>> Criar([FromBody] CreateDiarioClasseDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var novoDiario = await _diarioService.CriarAsync(request);

        return CreatedAtAction(nameof(ObterPorId), new { id = novoDiario.Id }, novoDiario);
    }

    /// <summary>
    /// Atualiza os dados de um diário de classe existente.
    /// </summary>
    /// <param name="id">O identificador do diário de classe que será atualizado.</param>
    /// <param name="request">Objeto contendo os novos dados do diário de classe.</param>
    /// <response code="204">Diário de classe atualizado com sucesso.</response>
    /// <response code="400">Dados inválidos fornecidos na requisição.</response>
    /// <response code="404">Nenhum diário de classe encontrado com o ID fornecido.</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "ADMIN")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Exclui um diário de classe do sistema.
    /// </summary>
    /// <param name="id">O identificador do diário de classe a ser excluído.</param>
    /// <response code="204">Diário de classe excluído com sucesso.</response>
    /// <response code="404">Nenhum diário de classe encontrado com o ID fornecido.</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "ADMIN")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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