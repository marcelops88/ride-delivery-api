using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Models.Inputs;
using Domain.Models.Outputs;
using Microsoft.Extensions.Logging;
public class LocacaoService : ILocacaoService
{
    private readonly ILocacaoRepository _locacaoRepository;
    private readonly IEntregadorService _entregadorService;
    private readonly ILogger<LocacaoService> _logger;
    private readonly IMapper _mapper;

    public LocacaoService(
        ILocacaoRepository locacaoRepository,
        IEntregadorService entregadorService,
        ILogger<LocacaoService> logger,
        IMapper mapper)
    {
        _locacaoRepository = locacaoRepository;
        _entregadorService = entregadorService;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<LocacaoOutput> CreateLocacaoAsync(LocacaoInput locacaoInput)
    {
        try
        {
            _logger.LogInformation("Iniciando criação da locação.");

            var locacao = _mapper.Map<Locacao>(locacaoInput);

            locacao.ValorTotal = CalcularValorTotal(locacaoInput);

            _locacaoRepository.Add(locacao);

            _logger.LogInformation("Locação criada com sucesso. ID: {LocacaoId}", locacao.Id);

            var locacaoOutput = _mapper.Map<LocacaoOutput>(locacao);

            locacaoOutput.ValorDiaria = ValorDiaria(locacaoInput.Plano);

            return locacaoOutput;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar locação: {Message}", ex.Message);
            throw new Exception("Ocorreu um erro ao criar a locação. Por favor, tente novamente.");
        }
    }

    public async Task<LocacaoOutput> FindByIdAsync(string identificadorLocacao)
    {
        try
        {
            var locacao = await _locacaoRepository.FindByIdentificadorAsync(identificadorLocacao);
            if (locacao == null)
            {
                _logger.LogWarning("Locação não encontrada: {IdentificadorLocacao}", identificadorLocacao);
                throw new KeyNotFoundException("Locação não encontrada.");
            }

            _logger.LogInformation("Locação encontrada: {IdentificadorLocacao}", identificadorLocacao);

            var locacaoOutput = _mapper.Map<LocacaoOutput>(locacao);
            locacaoOutput.ValorDiaria = ValorDiaria(locacao.Plano);

            return locacaoOutput;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar locação: {IdentificadorLocacao}", identificadorLocacao);
            throw;
        }
    }

    private decimal CalcularValorTotal(LocacaoInput locacaoInput)
    {
        decimal valorDiaria = ValorDiaria(locacaoInput.Plano);

        DateTime dataInicioReal = DateTime.Now.AddDays(1);
        int diasDeLocacao = (locacaoInput.DataTermino - dataInicioReal).Days + 1;

        if (diasDeLocacao < 0)
        {
            throw new InvalidOperationException("A data de término não pode ser anterior à data de início.");
        }

        return valorDiaria * diasDeLocacao;
    }
    private decimal ValorDiaria(int plano)
    {
        return
            plano switch
            {
                7 => 30m,
                15 => 28m,
                30 => 22m,
                45 => 20m,
                50 => 18m,
                _ => throw new InvalidOperationException("Plano de locação inválido."),
            };
    }
}
