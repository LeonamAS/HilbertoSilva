using Microsoft.EntityFrameworkCore;
using HilbertoSilva.Data;
using HilbertoSilva.Models;
using HilbertoSilva.DTOs.Request.Create;
using HilbertoSilva.DTOs.Response;
using HilbertoSilva.Services.Interfaces;

namespace HilbertoSilva.Services
{
    public class DisciplinaService : IDisciplinaService
    {
        private readonly EscolaDbContext _context;

        public DisciplinaService(EscolaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DisciplinaResponseDto>> ObterTodosAsync()
        {
            return await _context.Disciplinas
                .AsNoTracking()
                .Select(d => new DisciplinaResponseDto
                {
                    Id = d.Id,
                    Nome = d.Nome,
                    CargaHoraria = d.CargaHoraria
                })
                .ToListAsync();
        }

        public async Task<DisciplinaResponseDto> ObterPorIdAsync(int id)
        {
            var disciplina = await _context.Disciplinas
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);

            if (disciplina == null) return null;

            return new DisciplinaResponseDto
            {
                Id = disciplina.Id,
                Nome = disciplina.Nome,
                CargaHoraria = disciplina.CargaHoraria
            };
        }

        public async Task<DisciplinaResponseDto> CriarAsync(DisciplinaRequestDto request)
        {
            var disciplina = new Disciplina
            {
                Nome = request.Nome,
                CargaHoraria = request.CargaHoraria
            };

            _context.Disciplinas.Add(disciplina);
            await _context.SaveChangesAsync();

            return new DisciplinaResponseDto
            {
                Id = disciplina.Id,
                Nome = disciplina.Nome,
                CargaHoraria = disciplina.CargaHoraria
            };
        }

        public async Task<bool> AtualizarAsync(int id, DisciplinaRequestDto request)
        {
            var disciplina = await _context.Disciplinas.FindAsync(id);

            if (disciplina == null) return false;

            disciplina.Nome = request.Nome;
            disciplina.CargaHoraria = request.CargaHoraria;

            _context.Disciplinas.Update(disciplina);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeletarAsync(int id)
        {
            var disciplina = await _context.Disciplinas.FindAsync(id);

            if (disciplina == null) return false;

            _context.Disciplinas.Remove(disciplina);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}