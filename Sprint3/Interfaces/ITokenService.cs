using HilbertoSilva.Models;

namespace HilbertoSilva.Interfaces;

public interface ITokenService
{
    string GerarToken(Usuario usuario);
}
