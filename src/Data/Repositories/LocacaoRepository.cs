using Domain.Entities;
using Domain.Interfaces.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Data.Repositories
{
    public class LocacaoRepository : MongoDbRepository<Locacao>, ILocacaoRepository
    {
        private const string COLLECTION_NAME = "Locacoes";

        public LocacaoRepository(IMongoDatabase database) : base(database, COLLECTION_NAME)
        {
        }

        public async Task<Locacao> FindByIdentificadorAsync(string identificadorLocacao)
        {
            var filter = Builders<Locacao>.Filter.Eq(l => l.Identificador, identificadorLocacao);
            var locacao = await _collection.Find(filter).FirstOrDefaultAsync();
            return locacao;
        }

        public async Task<bool> TemLocacoesAtivasAsync(string identificadorMoto)
        {
            var pipeline = new[]
            {
                new BsonDocument("$match", new BsonDocument("moto_id", identificadorMoto)),
                new BsonDocument("$match", new BsonDocument("Active", true)),
                new BsonDocument("$count", "count")
            };

            var result = await _collection.Aggregate<BsonDocument>(pipeline).ToListAsync();

            return result.Count > 0;
        }
    }
}
