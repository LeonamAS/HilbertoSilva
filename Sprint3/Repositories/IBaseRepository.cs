namespace HilbertoSilva.Repositories.Interfaces;

public interface IBaseRepository<T> where T : class
{
    Task<IEnumerable<T>> ObterTodosAsync();
    Task<T?> ObterPorIdAsync(int id);
    Task<T> CriarAsync(T entity);
    Task AtualizarAsync(T entity);
    Task DeletarAsync(T entity);
}