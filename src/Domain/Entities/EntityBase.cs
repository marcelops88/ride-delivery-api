using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities
{
    public abstract class EntityBase
    {
        [BsonElement("identificador")]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        public bool Active { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = null;
        public int Version { get; set; }
    }
}
