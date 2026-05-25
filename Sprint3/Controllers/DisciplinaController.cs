using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HilbertoSilva.DTOs.Request.Create;
using HilbertoSilva.DTOs.Response;
using HilbertoSilva.Services.Interfaces;

namespace HilbertoSilva.Controllers
{
    /// <summary>
    /// Gerencia as operações referentes às disciplinas oferecidas na escola.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [Authorize(Roles = "ADMIN")]
    public class DisciplinaController : ControllerBase
    {
        private readonly IDisciplinaService _disciplinaService;

        public DisciplinaController(IDisciplinaService disciplinaService)
        {
            _disciplinaService = disciplinaService;
        }

        /// <summary>
        /// Obtém a lista de todas as disciplinas cadastradas no sistema.
        /// </summary>
        /// <returns>Retorna a lista de disciplinas.</returns>
        /// <response code="200">Lista retornada com sucesso.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DisciplinaResponseDto>>> ObterTodos()
        {
            var disciplinas = await _disciplinaService.ObterTodosAsync();
            return Ok(disciplinas);
        }

        /// <summary>
        /// Obtém uma disciplina específica pelo seu ID.
        /// </summary>
        /// <param name="id">O identificador único da disciplina.</param>
        /// <returns>Retorna os detalhes da disciplina solicitada.</returns>
        /// <response code="200">Disciplina encontrada e retornada com sucesso.</response>
        /// <response code="404">Nenhuma disciplina encontrada com o ID fornecido.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DisciplinaResponseDto>> ObterPorId(int id)
        {
            var disciplina = await _disciplinaService.ObterPorIdAsync(id);

            if (disciplina == null)
            {
                return NotFound(new { mensagem = "Disciplina não encontrada." });
            }

            return Ok(disciplina);
        }

        /// <summary>
        /// Cria uma nova disciplina.
        /// </summary>
        /// <param name="request">Objeto contendo os dados da disciplina a ser criada.</param>
        /// <returns>Retorna a disciplina recém-criada.</returns>
        /// <response code="201">Disciplina criada com sucesso.</response>
        /// <response code="400">Dados inválidos fornecidos na requisição.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DisciplinaResponseDto>> Criar([FromBody] DisciplinaRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var novaDisciplina = await _disciplinaService.CriarAsync(request);

            return CreatedAtAction(nameof(ObterPorId), new { id = novaDisciplina.Id }, novaDisciplina);
        }

        /// <summary>
        /// Atualiza os dados de uma disciplina existente.
        /// </summary>
        /// <param name="id">O identificador da disciplina que será atualizada.</param>
        /// <param name="request">Objeto contendo os novos dados da disciplina.</param>
        /// <response code="204">Disciplina atualizada com sucesso.</response>
        /// <response code="400">Dados inválidos fornecidos na requisição.</response>
        /// <response code="404">Nenhuma disciplina encontrada com o ID fornecido.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Atualizar(int id, [FromBody] DisciplinaRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var atualizado = await _disciplinaService.AtualizarAsync(id, request);

            if (!atualizado)
            {
                return NotFound(new { mensagem = "Disciplina não encontrada para atualização." });
            }

            return NoContent();
        }

        /// <summary>
        /// Exclui uma disciplina do sistema.
        /// </summary>
        /// <param name="id">O identificador da disciplina a ser excluída.</param>
        /// <response code="204">Disciplina excluída com sucesso.</response>
        /// <response code="404">Nenhuma disciplina encontrada com o ID fornecido.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Deletar(int id)
        {
            var deletado = await _disciplinaService.DeletarAsync(id);

            if (!deletado)
            {
                return NotFound(new { mensagem = "Disciplina não encontrada para exclusão." });
            }

            return NoContent();
        }
    }
}