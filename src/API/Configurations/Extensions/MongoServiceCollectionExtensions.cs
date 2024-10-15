using Data.Repositories;
using Domain.Interfaces.Repositories;
using MongoDB.Driver;

namespace API.Configurations.Extensions
{
    /// <summary>
    /// Classe de extensão para adicionar serviços do Mongo à coleção de serviços do ASP.NET Core.
    /// </summary>
    public static class MongoServiceCollectionExtensions
    {
        /// <summary>
        /// Adiciona os serviços básicos do Mongo à coleção de serviços.
        /// </summary>
        /// <param name="services">A coleção de serviços do ASP.NET Core.</param>
        public static void AddMongoServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("MongoDB");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("A string de conexão do MongoDB não está configurada.");
            }

            var mongoUrl = new MongoUrl(connectionString);
            var databaseName = mongoUrl.DatabaseName ?? "rideanddeliverdb"; 

            var mongoClient = new MongoClient(connectionString);
            var database = mongoClient.GetDatabase(databaseName);

            services.AddSingleton<IMongoClient>(mongoClient);
            services.AddSingleton(database);

            services.AddScoped(typeof(IRepository<>), typeof(MongoDbRepository<>));
        }

    }
}
