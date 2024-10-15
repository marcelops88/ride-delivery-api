using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities
{
    public class Entregador : EntityBase
    {
        [BsonElement("identificador")]
        public string Identificador { get; set; }

        [BsonElement("nome")]
        public string Nome { get; set; }

        [BsonElement("cnpj")]
        public string CNPJ { get; set; }

        [BsonElement("data_nascimento")]
        public DateTime DataNascimento { get; set; }

        [BsonElement("numero_cnh")]
        public string NumeroCNH { get; set; }

        [BsonElement("tipo_cnh")]
        public string TipoCNH { get; set; }

        [BsonElement("imagem_cnh")]
        public string ImagemCNH { get; set; }
    }
}
