namespace HilbertoSilva.Services.Interfaces
{
    public interface IBaseService<TCreateDto, TUpdateDto, TResponseDto>
    {
        Task<IEnumerable<TResponseDto>> ObterTodosAsync();
        Task<TResponseDto?> ObterPorIdAsync(int id);
        Task<TResponseDto> CriarAsync(TCreateDto dto);
        Task<bool> AtualizarAsync(int id, TUpdateDto dto);
        Task<bool> DeletarAsync(int id);
    }
}