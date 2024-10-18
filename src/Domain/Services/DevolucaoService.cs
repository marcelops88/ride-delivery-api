using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Models.Outputs;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

public class DevolucaoService : IDevolucaoService
{
    private readonly ILocacaoRepository _locacaoRepository;
    private readonly ILogger<DevolucaoService> _logger;

    public DevolucaoService(ILocacaoRepository locacaoRepository, ILogger<DevolucaoService> logger)
    {
        _locacaoRepository = locacaoRepository;
        _logger = logger;
    }

    public async Task<DevolucaoOutput> ProcessarDevolucaoAsync(string identificadorLocacao, DateTime dataDevolucao)
    {
        try
        {
            _logger.LogInformation("Tentando processar a devolução da locação com identificador: {IdentificadorLocacao} na data: {DataDevolucao}", identificadorLocacao, dataDevolucao);

            var locacao = _locacaoRepository.GetById(ObjectId.Parse(identificadorLocacao));

            if (locacao == null)
            {
                _logger.LogWarning("Locação com identificador {IdentificadorLocacao} não encontrada.", identificadorLocacao);
                throw new KeyNotFoundException("Locação não encontrada.");
            }

            var valorTotal = CalcularValorTotal(locacao, dataDevolucao, out decimal multa);

            var devolucaoOutput = new DevolucaoOutput
            {
                IdentificadorLocacao = locacao.Id.ToString(),
                DataDevolucao = dataDevolucao,
                ValorDiaria = ObterValorDiaria(locacao.Plano),
                Multa = multa,
                ValorTotal = valorTotal,
                Mensagem = multa > 0 ? "Multa aplicada devido à devolução antecipada ou tardia." : "Devolução sem multa."
            };

            _logger.LogInformation("Devolução processada com sucesso para a locação: {IdentificadorLocacao}", identificadorLocacao);
            return devolucaoOutput;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar devolução para a locação com identificador {IdentificadorLocacao}.", identificadorLocacao);
            throw;
        }
    }

    private decimal CalcularValorTotal(Locacao locacao, DateTime dataDevolucao, out decimal multa)
    {
        int diasLocados = (locacao.DataPrevisaoTermino - locacao.DataInicio).Days;
        decimal valorDiaria = ObterValorDiaria(locacao.Plano);

        decimal valorTotalBase = diasLocados * valorDiaria;
        multa = 0;

        if (dataDevolucao < locacao.DataPrevisaoTermino)
        {
            int diasRestantes = (locacao.DataPrevisaoTermino - dataDevolucao).Days;

            if (locacao.Plano == 7)
                multa = 0.20m * (diasRestantes * valorDiaria);
            if (locacao.Plano == 15)
                multa = 0.40m * (diasRestantes * valorDiaria);

            return valorTotalBase - (diasRestantes * valorDiaria) + multa;
        }

        if (dataDevolucao > locacao.DataPrevisaoTermino)
        {
            int diasAdicionais = (dataDevolucao - locacao.DataPrevisaoTermino).Days;
            multa = diasAdicionais * 50;

            return valorTotalBase + multa;
        }
        return valorTotalBase;
    }


    private decimal ObterValorDiaria(int plano)
    {
        return plano switch
        {
            7 => 30.00m,
            15 => 28.00m,
            30 => 22.00m,
            45 => 20.00m,
            50 => 18.00m,
            _ => throw new ArgumentException("Plano inválido")
        };
    }
}
