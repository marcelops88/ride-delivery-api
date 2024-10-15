using Domain.Entities;
using Domain.Interfaces.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Adapters.Mongo.Repositories
{
    public class MongoDbRepository<TEntity> : IRepository<TEntity> where TEntity : EntityBase
    {
        protected readonly IMongoCollection<TEntity> _collection;

        public MongoDbRepository(IMongoDatabase database, string? collectionName = null)
        {
            _collection = database.GetCollection<TEntity>(collectionName ?? nameof(TEntity));
        }

        public void Add(TEntity entity)
        {
            _collection.InsertOne(entity);
        }

        public void Update(TEntity entity)
        {
            var filter = Builders<TEntity>.Filter.Eq(e => e.Id, entity.Id);
            var modifiedProperties = new List<UpdateDefinition<TEntity>>();

            foreach (var property in typeof(TEntity).GetProperties())
            {
                if (property.DeclaringType != typeof(EntityBase))
                {
                    var value = property.GetValue(entity);
                    modifiedProperties.Add(Builders<TEntity>.Update.Set(property.Name, value));
                }
            }

            if (modifiedProperties.Count > 0)
            {
                modifiedProperties.Add(Builders<TEntity>.Update.Set("UpdatedAt", DateTime.UtcNow));
                modifiedProperties.Add(Builders<TEntity>.Update.Set("Version", entity.Version + 1));
            }

            var updated = Builders<TEntity>.Update.Combine(modifiedProperties);
            _collection.UpdateOne(filter, updated);
        }

        public void Delete(ObjectId id)
        {
            var filter = Builders<TEntity>.Filter.Eq(e => e.Id, id);
            var entityType = typeof(TEntity);
            var isActiveProperty = entityType.GetProperty("Active");

            if (isActiveProperty != null && isActiveProperty.PropertyType == typeof(bool))
            {
                var updated = Builders<TEntity>.Update.Set("Active", false);
                _collection.UpdateOne(filter, updated);
            }
        }

        public TEntity GetById(ObjectId id)
        {
            return _collection.Find(e => e.Id == id && e.Active).FirstOrDefault();
        }

        public IQueryable<TEntity> GetAll()
        {
            return _collection.AsQueryable().Where(entity => entity.Active);
        }
    }
}
