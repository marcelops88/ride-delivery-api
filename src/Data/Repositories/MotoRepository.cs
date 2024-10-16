using Domain.Entities;
using Domain.Interfaces.Repositories;
using MongoDB.Driver;

namespace Data.Repositories
{
    public class MotoRepository : MongoDbRepository<Moto>, IMotoRepository
    {
        private const string COLLECTION_NAME = "Motos";

        public MotoRepository(IMongoDatabase database) : base(database, COLLECTION_NAME)
        {
        }

        public async Task<Moto> FindByIdentificadorOrPlacaAsync(string? identificador, string? placa)
        {
            if (!string.IsNullOrWhiteSpace(identificador))
            {
                var motoByIdentificador = await _collection
                    .Find(m => m.Identificador == identificador)
                    .FirstOrDefaultAsync();

                if (motoByIdentificador != null)
                {
                    return motoByIdentificador;
                }
            }

            if (!string.IsNullOrWhiteSpace(placa))
            {
                var motoByPlaca = await _collection
                    .Find(m => m.Placa == placa)
                    .FirstOrDefaultAsync();

                return motoByPlaca;
            }

            return null;
        }
    }
}
