using Domain.Entities;
using Domain.Interfaces.Repositories;
using MongoDB.Driver;

namespace Data.Repositories
{
    public class EntregadorRepository : MongoDbRepository<Entregador>, IEntregadorRepository
    {
        private const string COLLECTION_NAME = "Entregadores";

        public EntregadorRepository(IMongoDatabase database) : base(database, COLLECTION_NAME)
        {
        }

        public Task<Entregador> FindByCnpjAsync(string cnpj)
        {
            return _collection
                        .Find(entregador => entregador.CNPJ == cnpj)
                        .FirstOrDefaultAsync();
        }

        public Task<Entregador> FindByIdentificadorAsync(string identificador)
        {
            return _collection
                  .Find(entregador => entregador.Identificador == identificador)
                  .FirstOrDefaultAsync();
        }

        public Task<Entregador> FindByNumeroCnhAsync(string numeroCNH)
        {
            return _collection
                    .Find(entregador => entregador.NumeroCNH == numeroCNH)
                    .FirstOrDefaultAsync();
        }
    }
}
