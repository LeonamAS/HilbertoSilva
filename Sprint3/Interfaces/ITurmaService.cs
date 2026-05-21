using HilbertoSilva.DTOs.Request.Create;
using HilbertoSilva.DTOs.Response;
using HilbertoSilva.Services.Interfaces;

namespace HilbertoSilva.Interfaces;

public interface ITurmaService : IBaseService<TurmaRequestDto, TurmaRequestDto, TurmaResponseDto>
{
}