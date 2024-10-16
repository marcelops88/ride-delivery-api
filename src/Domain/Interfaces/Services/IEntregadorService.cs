using Domain.Models.Inputs;
using Domain.Models.Outputs;

namespace Domain.Interfaces.Services
{
    public interface IEntregadorService
    {
        Task<EntregadorOutput> CreateEntregadorAsync(EntregadorInput entregadorInput);
        Task UpdateImagemCNHAsync(string identificador, string Base64ImagemCNH);
    }
}
