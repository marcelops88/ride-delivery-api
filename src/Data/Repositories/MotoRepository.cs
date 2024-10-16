using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Models;
using MongoDB.Driver;

namespace Data.Repositories
{
    public class MotoRepository : MongoDbRepository<Moto>, IMotoRepository
    {
        private const string COLLECTION_NAME = "Motos";

        public MotoRepository(IMongoDatabase database) : base(database, COLLECTION_NAME)
        {
        }

        public async Task<MotoPlaca> FindByIdentificadorOrPlacaAsync(string? identificador, string? placa)
        {
            var motoByIdentificador = await _collection
                .Find(m => m.Identificador == identificador)
                .FirstOrDefaultAsync();

            var placaExistente = !string.IsNullOrWhiteSpace(placa) && await PlacaExistenteAsync(placa);

            return new MotoPlaca
            {
                Moto = motoByIdentificador,
                PlacaExistente = placaExistente
            };
        }
        private async Task<bool> PlacaExistenteAsync(string placa)
        {
            return await _collection.CountDocumentsAsync(m => m.Placa == placa) > 0;
        }
    }
}
