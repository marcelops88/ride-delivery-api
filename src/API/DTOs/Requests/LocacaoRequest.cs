namespace API.DTOs.Requests
{
    public class LocacaoRequest
    {
        public string Identificador { get; set; }
        public string IdentificadorEntregador { get; set; }
        public string IdentificadorMoto { get; set; }
        public DateTime DataTermino { get; set; }
        public DateTime DataPrevisaoTermino { get; set; }
        public int Plano { get; set; }
    }
}
