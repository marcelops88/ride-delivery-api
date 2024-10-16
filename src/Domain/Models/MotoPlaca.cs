using Domain.Entities;

namespace Domain.Models
{
    public class MotoPlaca
    {
        public Moto Moto { get; set; }
        public bool PlacaExistente { get; set; }
    }
}
