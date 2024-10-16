using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests
{
    public class UpdateImagemCNHRequest
    {
        private const string Base64Pattern = @"^(data:image\/(png|bmp);base64,)?[A-Za-z0-9+/=]+$";

        [Required(ErrorMessage = "A imagem da CNH é obrigatória.")]
        [RegularExpression(Base64Pattern, ErrorMessage = "A imagem deve estar em formato Base64 e ser do tipo PNG ou BMP.")]
        public string Base64ImagemCNH { get; set; }
    }
}
