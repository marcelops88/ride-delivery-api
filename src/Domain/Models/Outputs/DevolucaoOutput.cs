namespace Domain.Models.Outputs
{
    public class DevolucaoOutput
    {
        public string IdentificadorLocacao { get; set; }
        public DateTime DataDevolucao { get; set; }
        public decimal ValorDiaria { get; set; }
        public decimal Multa { get; set; }
        public decimal ValorTotal { get; set; }
        public string Mensagem { get; set; }
    }
}
