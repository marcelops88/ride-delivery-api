using Domain.Models.Inputs;
using Domain.Models.Outputs;

namespace Domain.Interfaces.Services
{
    public interface IMotoService
    {
        Task<MotoOutput> CreateMotoAsync(MotoInput moto);

        IEnumerable<MotoOutput> GetAllMotos();

        Task<MotoOutput> GetMotoByIdAsync(string identificador);

        Task UpdateMotoPlateAsync(string identificador, string novaPlaca);

        Task DeleteMotoAsync(string identificador);
    }
}
