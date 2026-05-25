using HilbertoSilva.DTOs.Request.Create;
using HilbertoSilva.DTOs.Request.Update;
using HilbertoSilva.DTOs.Response;

namespace HilbertoSilva.Services.Interfaces;


public interface IBoletimService : IBaseService<CreateBoletimDto, UpdateBoletimDto, BoletimResponseDto>
{
    Task<IEnumerable<BoletimResponseDto>> ObterPorUsuarioIdAsync(int usuarioId);
}