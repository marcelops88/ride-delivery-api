using Domain.Entities;

namespace Domain.Interfaces.Repositories
{
    public interface IEntregadorRepository : IRepository<Entregador>
    {
        Task<Entregador> FindByCnpjAsync(string cnpj);
        Task<Entregador> FindByNumeroCnhAsync(string numeroCNH);
    }
}
