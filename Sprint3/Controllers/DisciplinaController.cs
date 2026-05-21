using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using HilbertoSilva.DTOs.Request.Create;
using HilbertoSilva.DTOs.Response;
using HilbertoSilva.Services.Interfaces;

namespace HilbertoSilva.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DisciplinaController : ControllerBase
    {
        private readonly IDisciplinaService _disciplinaService;

        public DisciplinaController(IDisciplinaService disciplinaService)
        {
            _disciplinaService = disciplinaService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DisciplinaResponseDto>>> ObterTodos()
        {
            var disciplinas = await _disciplinaService.ObterTodosAsync();
            return Ok(disciplinas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DisciplinaResponseDto>> ObterPorId(int id)
        {
            var disciplina = await _disciplinaService.ObterPorIdAsync(id);

            if (disciplina == null)
            {
                return NotFound(new { mensagem = "Disciplina não encontrada." });
            }

            return Ok(disciplina);
        }

        [HttpPost]
        public async Task<ActionResult<DisciplinaResponseDto>> Criar([FromBody] DisciplinaRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var novaDisciplina = await _disciplinaService.CriarAsync(request);

            return CreatedAtAction(nameof(ObterPorId), new { id = novaDisciplina.Id }, novaDisciplina);
        }

        [HttpPut("{id}")]
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

        [HttpDelete("{id}")]
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