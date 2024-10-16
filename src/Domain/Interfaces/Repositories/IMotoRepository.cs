
using Domain.Entities;
using Domain.Models;

namespace Domain.Interfaces.Repositories
{
    public interface IMotoRepository : IRepository<Moto>
    {
        Task<MotoPlaca> FindByIdentificadorOrPlacaAsync(string? identificador, string? placa);
    }
}
