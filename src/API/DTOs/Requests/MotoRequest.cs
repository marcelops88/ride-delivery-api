using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests
{
    public class MotoRequest
    {
        [Required(ErrorMessage = "O identificador da moto é obrigatório.")]
        public string Identificador { get; set; }

        [Required(ErrorMessage = "O ano da moto é obrigatório.")]
        [Range(1900, 2100, ErrorMessage = "O ano deve ser válido.")]
        public int Ano { get; set; }

        [Required(ErrorMessage = "O modelo da moto é obrigatório.")]
        public string Modelo { get; set; }

        [Required(ErrorMessage = "A placa da moto é obrigatória.")]
        [RegularExpression(@"^[A-Z]{3}-\d{4}$", ErrorMessage = "A placa deve estar no formato ABC-1234.")]
        public string Placa { get; set; }
    }
}
