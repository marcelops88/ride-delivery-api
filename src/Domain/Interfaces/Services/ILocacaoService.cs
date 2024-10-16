using Domain.Models.Inputs;
using Domain.Models.Outputs;

namespace Domain.Interfaces.Services
{
    public interface ILocacaoService
    {
        Task<LocacaoOutput> CreateLocacaoAsync(LocacaoInput locacaoInput);
        Task<LocacaoOutput> FindByIdAsync(object id);
    }
}
