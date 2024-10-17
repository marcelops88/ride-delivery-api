using Domain.Interfaces.Services;
using Domain.Models.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Domain.Services
{

    public class ImagemService : IImagemService
    {
        private readonly ILogger<ImagemService> _logger;
        private readonly StorageSettings _storageSettings;

        public ImagemService(ILogger<ImagemService> logger, IOptions<StorageSettings> storageSettings)
        {
            _logger = logger;
            _storageSettings = storageSettings.Value;
        }
        public async Task<string> SalvarImagemCNHAsync(string identificador, string base64ImagemCNH)
        {
            const string pngPrefix = "data:image/png;base64,";
            const string bmpPrefix = "data:image/bmp;base64,";

            string extensao = ".png";

            if (base64ImagemCNH.StartsWith(pngPrefix))
            {
                base64ImagemCNH = base64ImagemCNH.Substring(pngPrefix.Length);
                extensao = ".png";
            }
            else if (base64ImagemCNH.StartsWith(bmpPrefix))
            {
                base64ImagemCNH = base64ImagemCNH.Substring(bmpPrefix.Length);
                extensao = ".bmp";
            }
            else
            {
                _logger.LogWarning("Formato de imagem não suportado.");
                throw new ArgumentException("Formato de imagem não suportado. Apenas PNG e BMP são aceitos.");
            }

            byte[] imagemBytes = Convert.FromBase64String(base64ImagemCNH);

            string diretorio = _storageSettings.ImageDirectory;
            string nomeArquivo = $"{identificador}_CNH{extensao}";
            string caminhoCompleto = Path.Combine(diretorio, nomeArquivo);

            CriarDiretorioSeNecessario(diretorio);

            await File.WriteAllBytesAsync(caminhoCompleto, imagemBytes);
            _logger.LogInformation("Imagem da CNH salva em: {CaminhoCompleto}", caminhoCompleto);

            return caminhoCompleto;
        }

        private void CriarDiretorioSeNecessario(string diretorio)
        {
            if (!Directory.Exists(diretorio))
            {
                Directory.CreateDirectory(diretorio);
                _logger.LogInformation("Diretório criado: {Diretorio}", diretorio);
            }
        }
    }
}
