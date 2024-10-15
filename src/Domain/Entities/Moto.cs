using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities
{
    public class Moto : EntityBase
    {
        [BsonElement("placa")]
        public string Placa { get; set; }

        [BsonElement("modelo")]
        public string Modelo { get; set; }

        [BsonElement("ano")]
        public string Ano { get; set; }
    }
}
