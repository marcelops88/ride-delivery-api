using Domain.Entities;

namespace Domain.Interfaces.Repositories
{
    public interface ILocacaoRepository : IRepository<Locacao>
    {
        Task<bool> TemLocacoesAtivasAsync(string identificadorMoto);
    }
}
