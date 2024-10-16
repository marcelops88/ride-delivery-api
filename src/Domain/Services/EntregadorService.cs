using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Models.Inputs;
using Domain.Models.Outputs;
using Microsoft.Extensions.Logging;

public class EntregadorService : IEntregadorService
{
    private readonly IEntregadorRepository _entregadorRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<EntregadorService> _logger;

    public EntregadorService(IEntregadorRepository entregadorRepository, IMapper mapper, ILogger<EntregadorService> logger)
    {
        _entregadorRepository = entregadorRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<EntregadorOutput> CreateEntregadorAsync(EntregadorInput entregadorInput)
    {
        _logger.LogInformation("Iniciando o cadastro do entregador: {Identificador}", entregadorInput.Identificador);

        var existingCnpj = await _entregadorRepository.FindByCnpjAsync(entregadorInput.Cnpj);
        if (existingCnpj != null)
        {
            _logger.LogWarning("Tentativa de cadastro de entregador falhou. CNPJ já existe: {CNPJ}", entregadorInput.Cnpj);
            throw new Exception("Já existe um entregador cadastrado com esse CNPJ.");
        }

        var existingCnh = await _entregadorRepository.FindByNumeroCnhAsync(entregadorInput.NumeroCNH);
        if (existingCnh != null)
        {
            _logger.LogWarning("Tentativa de cadastro de entregador falhou. Número da CNH já existe: {NumeroCNH}", entregadorInput.NumeroCNH);
            throw new Exception("Já existe um entregador cadastrado com esse número de CNH.");
        }

        var entregador = _mapper.Map<Entregador>(entregadorInput);

        entregador.ImagemCNH = string.Empty;

        _entregadorRepository.Add(entregador);

        var entregadorOutput = _mapper.Map<EntregadorOutput>(entregador);

        _logger.LogInformation("Cadastro do entregador {Identificador} concluído com sucesso.", entregadorOutput.Identificador);
        return entregadorOutput;
    }

    public Task UpdateImagemCNHAsync(string identificador, string Base64ImagemCNH)
    {
        throw new NotImplementedException();
    }
}
