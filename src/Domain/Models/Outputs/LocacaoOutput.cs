namespace Domain.Models.Outputs
{
    public class LocacaoOutput
    {
        public string Identificador { get; set; }
        public decimal ValorDiaria { get; set; }
        public string IdentificadorEntregador { get; set; }
        public string IdentificadorMoto { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataTermino { get; set; }
        public DateTime DataPrevisaoTermino { get; set; }
        public DateTime DataDevolucao { get; set; }

    }
}
