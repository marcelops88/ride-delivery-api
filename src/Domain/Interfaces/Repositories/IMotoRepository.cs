
using Domain.Entities;

namespace Domain.Interfaces.Repositories
{
    public interface IMotoRepository : IRepository<Moto>
    {
        Task<Moto> FindByIdentificadorOrPlacaAsync(string? identificador, string? placa);
    }
}
