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
    private readonly IImagemService _imageService;

    public EntregadorService(IEntregadorRepository entregadorRepository, IMapper mapper, ILogger<EntregadorService> logger, IImagemService imageService)
    {
        _entregadorRepository = entregadorRepository;
        _mapper = mapper;
        _logger = logger;
        _imageService = imageService;
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
    public async Task UpdateImagemCNHAsync(string identificador, string base64ImagemCNH)
    {
        _logger.LogInformation("Atualizando imagem da CNH para o entregador: {Identificador}", identificador);

        var entregador = await ValidarEntregadorExistente(identificador);

        string caminhoImagem = await _imageService.SalvarImagemCNHAsync(identificador, base64ImagemCNH);

        AtualizarCaminhoImagemCNH(entregador, caminhoImagem);

        _logger.LogInformation("Caminho da imagem da CNH atualizado para o entregador: {Identificador}", identificador);
    }

    private async Task<Entregador> ValidarEntregadorExistente(string identificador)
    {
        var entregador = await _entregadorRepository.FindByIdentificadorAsync(identificador);
        if (entregador == null)
        {
            _logger.LogWarning("Entregador não encontrado para o identificador: {Identificador}", identificador);
            throw new Exception("Entregador não encontrado.");
        }
        return entregador;
    }
    private void AtualizarCaminhoImagemCNH(Entregador entregador, string caminhoCompleto)
    {
        entregador.ImagemCNH = caminhoCompleto;
        _entregadorRepository.Update(entregador);
    }
}

