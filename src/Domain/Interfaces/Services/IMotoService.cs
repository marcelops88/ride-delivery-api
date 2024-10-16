using Domain.Entities;
using Domain.Models.Inputs;
using MongoDB.Bson;

namespace Domain.Interfaces.Services
{
    public interface IMotoService
    {
        Task<Moto> CreateMotoAsync(MotoInput moto);

        Task<IEnumerable<Moto>> GetAllMotosAsync();

        Task<Moto> GetMotoByIdAsync(ObjectId id);

        Task UpdateMotoPlateAsync(ObjectId id, string novaPlaca);

        Task DeleteMotoAsync(ObjectId id);
    }
}
