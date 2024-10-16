namespace Domain.Interfaces.Services
{
    public interface IImagemService
    {
        Task<string> SalvarImagemCNHAsync(string identificador, string base64ImagemCNH);
    }

}
