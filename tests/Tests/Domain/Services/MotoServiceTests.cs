using AutoMapper;
using Bogus;
using Domain.Entities;
using Domain.Interfaces.Messaging;
using Domain.Interfaces.Repositories;
using Domain.Models;
using Domain.Models.Inputs;
using Domain.Models.Outputs;
using Domain.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using NSubstitute;

namespace Tests.Domain.Services
{
    public class MotoServiceTests
    {
        private readonly IMotoRepository _motoRepository;
        private readonly ILocacaoRepository _locacaoRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<MotoService> _logger;
        private readonly IProducer<MotoInput> _motoProducer;
        private readonly MotoService _motoService;

        public MotoServiceTests()
        {
            _motoRepository = Substitute.For<IMotoRepository>();
            _locacaoRepository = Substitute.For<ILocacaoRepository>();
            _mapper = Substitute.For<IMapper>();
            _logger = Substitute.For<ILogger<MotoService>>();
            _motoProducer = Substitute.For<IProducer<MotoInput>>();

            _motoService = new MotoService(_motoRepository, _mapper, _logger, _motoProducer, _locacaoRepository);
        }

        [Fact]
        public async Task CreateMotoAsync_ShouldThrowException_WhenMotoAlreadyExists()
        {
            // Arrange
            var faker = new Faker<MotoInput>()
                .RuleFor(m => m.Identificador, f => ObjectId.GenerateNewId().ToString())
                .RuleFor(m => m.Placa, f => f.Vehicle.Vin());

            var motoInput = faker.Generate();

            _motoRepository.FindByIdentificadorOrPlacaAsync(motoInput.Identificador, motoInput.Placa)
                     .Returns(new MotoPlaca { Moto = new Moto { Active = true } });

            // Act
            Func<Task> act = async () => await _motoService.CreateMotoAsync(motoInput);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Já existe uma moto ativa com esse identificador ou placa.");
        }

        [Fact]
        public async Task DeleteMotoAsync_ShouldRemoveMotoSuccessfully()
        {
            // Arrange
            var identificador = ObjectId.GenerateNewId().ToString();
            var moto = new Moto { Identificador = identificador, Active = true };
            _motoRepository.FindByIdentificadorOrPlacaAsync(identificador, null)
                .Returns(new MotoPlaca { Moto = moto });

            _locacaoRepository.TemLocacoesAtivasAsync(identificador).Returns(false);

            // Act
            await _motoService.DeleteMotoAsync(identificador);

            // Assert
            await _motoRepository.Received(1).FindByIdentificadorOrPlacaAsync(identificador, null);
            _motoRepository.Received(1).Delete(moto.Id);
        }

        [Fact]
        public async Task DeleteMotoAsync_ShouldThrowException_WhenMotoNotFound()
        {
            // Arrange
            var identificador = ObjectId.GenerateNewId().ToString();
            _motoRepository.FindByIdentificadorOrPlacaAsync(identificador, null)
                       .Returns(new MotoPlaca { Moto = null });

            // Act
            Func<Task> act = async () => await _motoService.DeleteMotoAsync(identificador);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Moto não encontrada.");
        }

        [Fact]
        public async Task DeleteMotoAsync_ShouldThrowException_WhenMotoHasActiveRentals()
        {
            // Arrange
            var identificador = ObjectId.GenerateNewId().ToString();
            var moto = new Moto { Identificador = identificador, Active = true };
            _motoRepository.FindByIdentificadorOrPlacaAsync(identificador, null)
                      .Returns(new MotoPlaca { Moto = moto });

            _locacaoRepository.TemLocacoesAtivasAsync(identificador).Returns(true);

            // Act
            Func<Task> act = async () => await _motoService.DeleteMotoAsync(identificador);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Não é possível remover a moto. A moto possui locações.");
        }

        [Fact]
        public void GetAllMotos_ShouldReturnAllMotos()
        {
            // Arrange
            var motos = new List<Moto>
                {
                    new Moto { Identificador = ObjectId.GenerateNewId().ToString(), Placa = "ABC-1234", Active = true },
                    new Moto { Identificador = ObjectId.GenerateNewId().ToString(), Placa = "DEF-5678", Active = false }
                }.AsQueryable();

            _motoRepository.GetAll().Returns(motos);

            var motoOutputs = new List<MotoOutput>
                {
                    new MotoOutput { Identificador = motos.ElementAt(0).Identificador, Placa = motos.ElementAt(0).Placa, Active = true },
                    new MotoOutput { Identificador = motos.ElementAt(1).Identificador, Placa = motos.ElementAt(1).Placa, Active = false }
                };

            _mapper.Map<IEnumerable<MotoOutput>>(motos).Returns(motoOutputs);

            // Act
            var result = _motoService.GetAllMotos();

            // Assert
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(motoOutputs);
        }

        [Fact]
        public async Task GetMotoByIdAsync_ShouldThrowException_WhenMotoNotFound()
        {
            // Arrange
            var identificador = ObjectId.GenerateNewId().ToString();
            _motoRepository.FindByIdentificadorOrPlacaAsync(identificador, null)
                           .Returns(new MotoPlaca { Moto = null });


            // Act
            Func<Task> act = async () => await _motoService.GetMotoByIdAsync(identificador);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Moto não encontrada.");
        }

        [Fact]
        public async Task UpdateMotoPlateAsync_ShouldUpdatePlateSuccessfully()
        {
            // Arrange
            var identificador = ObjectId.GenerateNewId().ToString();
            var novaPlaca = "XYZ-9876";
            var moto = new Moto { Identificador = identificador, Placa = "ABC-1234", Active = true };
            _motoRepository.FindByIdentificadorOrPlacaAsync(identificador, novaPlaca)
                           .Returns(new MotoPlaca { Moto = moto, PlacaExistente = false });


            // Act
            await _motoService.UpdateMotoPlateAsync(identificador, novaPlaca);

            // Assert
            moto.Placa.Should().Be(novaPlaca);
            _motoRepository.Received(1).Update(moto);
        }
    }
}
