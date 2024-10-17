using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities
{
    public class Locacao : EntityBase
    {
        [BsonElement("entregador_id")]
        public string IdentificadorEntregador { get; set; }

        [BsonElement("moto_id")]
        public string IdentificadorMoto { get; set; }

        [BsonElement("data_inicio")]
        public DateTime DataInicio { get; set; } = DateTime.Now.AddDays(1);

        [BsonElement("data_termino")]
        public DateTime? DataTermino { get; set; }

        [BsonElement("data_previsao_termino")]
        public DateTime DataPrevisaoTermino { get; set; }

        [BsonElement("plano")]
        public int Plano { get; set; }

        [BsonElement("valor_total")]
        public decimal ValorTotal { get; set; }
    }

}
