using Domain.Entities;
using Domain.Interfaces.Repositories;
using MongoDB.Driver;

namespace Data.Repositories
{
    public class LocacaoRepository : MongoDbRepository<Locacao>, ILocacaoRepository
    {
        private const string COLLECTION_NAME = "Locacoes";

        public LocacaoRepository(IMongoDatabase database) : base(database, COLLECTION_NAME)
        {
        }
    }

}
