using Domain.Entities;
using MongoDB.Bson;

namespace Domain.Interfaces.Repositories
{
    public interface IRepository<T> where T : EntityBase
    {
        void Add(T entity);
        void Update(T entity);
        void Delete(ObjectId id);
        T GetById(ObjectId id);
        IQueryable<T> GetAll();
    }
}
