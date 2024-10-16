using API.DTOs.Validation;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests
{
    public class EntregadorRequest
    {
        private const string CnpjPattern = @"^\d{2}\.\d{3}\.\d{3}\/\d{4}-\d{2}$";
        private const string NumeroCNHPattern = @"^[0-9]{11,12}$";

        [Required(ErrorMessage = "O CNPJ é obrigatório.")]
        [RegularExpression(CnpjPattern, ErrorMessage = "O CNPJ deve estar no formato XX.XXX.XXX/XXXX-XX.")]
        public string Cnpj { get; set; }

        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        [DataType(DataType.Date)]
        [CustomDateOfBirthValidation(ErrorMessage = "O entregador deve ter pelo menos 18 anos.")]
        public DateTime DataNascimento { get; set; }

        [Required(ErrorMessage = "O número da CNH é obrigatório.")]
        [RegularExpression(NumeroCNHPattern, ErrorMessage = "O número da CNH deve conter entre 11 e 12 dígitos.")]
        public string NumeroCNH { get; set; }

        [Required(ErrorMessage = "O tipo da CNH é obrigatório.")]
        [RegularExpression("^(A|B|A\\+B)$", ErrorMessage = "O tipo da CNH deve ser 'A', 'B' ou 'A+B'.")]
        public string TipoCNH { get; set; }
    }
}
