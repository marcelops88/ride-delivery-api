using Domain.Entities;

namespace Domain.Interfaces.Repositories
{
    public interface ILocacaoRepository : IRepository<Locacao>
    {
        Task<Locacao> FindByIdentificadorAsync(string identificadorLocacao);
        Task<bool> TemLocacoesAtivasAsync(string identificadorMoto);
    }
}
