using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests
{
    public class ModificarPlacaRequest
    {
        [Required(ErrorMessage = "A placa da moto é obrigatória.")]
        [RegularExpression(@"^[A-Z]{3}-\d{4}$|^[A-Z]{3}\d[A-Z]\d{2}$", ErrorMessage = "A placa deve estar no formato ABC-1234 ou ABC1D23.")]
        public string NovaPlaca { get; set; }
    }
}
