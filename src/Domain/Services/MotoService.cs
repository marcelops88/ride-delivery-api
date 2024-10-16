using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Models.Inputs;
using Domain.Models.Outputs;
using Microsoft.Extensions.Logging;

namespace Domain.Services
{
    internal class MotoService : IMotoService
    {
        private readonly IMotoRepository _motoRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<MotoService> _logger;

        public MotoService(IMotoRepository motoRepository, IMapper mapper, ILogger<MotoService> logger)
        {
            _motoRepository = motoRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<MotoOutput> CreateMotoAsync(MotoInput motoInput)
        {
            try
            {
                _logger.LogInformation("Tentando criar uma nova moto com identificador {Identificador} e placa {Placa}.", motoInput.Identificador, motoInput.Placa);

                var resultado = await _motoRepository.FindByIdentificadorOrPlacaAsync(motoInput.Identificador, motoInput.Placa);

                if (resultado.Moto != null)
                {
                    _logger.LogWarning("Já existe uma moto com esse identificador ou placa.");
                    throw new Exception("Já existe uma moto com esse identificador ou placa.");
                }

                var moto = _mapper.Map<Moto>(motoInput);
                _motoRepository.Add(moto);

                _logger.LogInformation("Moto criada com sucesso: {Identificador}.", moto.Identificador);
                return _mapper.Map<MotoOutput>(moto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar moto.");
                throw;
            }
        }
        public async Task DeleteMotoAsync(string identificador)
        {
            try
            {
                _logger.LogInformation("Tentando remover a moto com identificador {Identificador}.", identificador);

                var resultado = await _motoRepository.FindByIdentificadorOrPlacaAsync(null, identificador);
                if (resultado.Moto == null)
                {
                    _logger.LogWarning("Moto não encontrada para remoção.");
                    throw new Exception("Moto não encontrada.");
                }

                // adicionar a lógica de verificação se a moto está ativa ou tem locações

                _motoRepository.Delete(resultado.Moto.Id);
                _logger.LogInformation("Moto removida com sucesso: {Identificador}.", identificador);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover moto.");
                throw;
            }
        }

        public IEnumerable<MotoOutput> GetAllMotos()
        {
            try
            {
                _logger.LogInformation("Consultando todas as motos.");
                var motos = _motoRepository.GetAll();
                return _mapper.Map<IEnumerable<MotoOutput>>(motos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar motos.");
                throw;
            }
        }

        public async Task<MotoOutput> GetMotoByIdAsync(string identificador)
        {
            try
            {
                _logger.LogInformation("Consultando moto com identificador {Identificador}.", identificador);
                var resultado = await _motoRepository.FindByIdentificadorOrPlacaAsync(null, identificador);

                if (resultado.Moto == null)
                {
                    _logger.LogWarning("Moto não encontrada: {Identificador}.", identificador);
                    throw new Exception("Moto não encontrada.");
                }

                return _mapper.Map<MotoOutput>(resultado.Moto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar moto por ID.");
                throw;
            }
        }

        public async Task UpdateMotoPlateAsync(string identificador, string novaPlaca)
        {
            try
            {
                _logger.LogInformation("Tentando atualizar a placa da moto com identificador {Identificador}.", identificador);
                var resultado = await _motoRepository.FindByIdentificadorOrPlacaAsync(identificador, novaPlaca);

                if (resultado.Moto == null)
                {
                    _logger.LogWarning("Moto não encontrada ao tentar atualizar placa: {Identificador}.", identificador);
                    throw new Exception("Moto não encontrada.");
                }

                if (resultado.PlacaExistente && resultado.Moto.Placa != novaPlaca)
                {
                    _logger.LogWarning("Já existe uma moto com essa placa: {Placa}.", novaPlaca);
                    throw new Exception("Já existe uma moto com essa placa.");
                }

                resultado.Moto.Placa = novaPlaca;
                _motoRepository.Update(resultado.Moto);
                _logger.LogInformation("Placa da moto atualizada com sucesso: {Identificador}.", identificador);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar placa da moto.");
                throw;
            }
        }
    }
}
