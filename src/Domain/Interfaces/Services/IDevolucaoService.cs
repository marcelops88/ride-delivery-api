using Domain.Models.Outputs;

namespace Domain.Interfaces.Services
{
    public interface IDevolucaoService
    {
        Task<DevolucaoOutput> ProcessarDevolucaoAsync(string identificadorLocacao, DateTime dataDevolucao);
    }

}
