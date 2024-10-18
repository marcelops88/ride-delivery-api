using AutoMapper;
using Bogus;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Models.Inputs;
using Domain.Models.Outputs;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using NSubstitute;

namespace Tests.Domain.Services
{
    public class LocacaoServiceTests
    {
        private readonly ILocacaoRepository _locacaoRepositoryMock;
        private readonly IEntregadorService _entregadorServiceMock;
        private readonly ILogger<LocacaoService> _loggerMock;
        private readonly IMapper _mapperMock;
        private readonly LocacaoService _locacaoService;

        private readonly Faker<LocacaoInput> _locacaoInputFaker;
        private readonly Faker<Locacao> _locacaoFaker;

        public LocacaoServiceTests()
        {
            // Mocks
            _locacaoRepositoryMock = Substitute.For<ILocacaoRepository>();
            _entregadorServiceMock = Substitute.For<IEntregadorService>();
            _loggerMock = Substitute.For<ILogger<LocacaoService>>();
            _mapperMock = Substitute.For<IMapper>();

            // Serviço
            _locacaoService = new LocacaoService(_locacaoRepositoryMock, _entregadorServiceMock, _loggerMock, _mapperMock);

            // Faker para gerar dados de teste
            _locacaoInputFaker = new Faker<LocacaoInput>()
                .RuleFor(l => l.Plano, f => f.PickRandom(new[] { 7, 15, 30, 45, 50 }))
                .RuleFor(l => l.DataTermino, f => f.Date.Future());

            _locacaoFaker = new Faker<Locacao>()
                .RuleFor(l => l.Id, f => ObjectId.GenerateNewId()) 
                .RuleFor(l => l.Plano, f => f.PickRandom(new[] { 7, 15, 30, 45, 50 }))
                .RuleFor(l => l.ValorTotal, f => f.Finance.Amount(100, 1000));
        }

        [Fact]
        public async Task CreateLocacaoAsync_ShouldCreateLocacaoAndReturnOutput()
        {
            // Arrange
            var locacaoInput = _locacaoInputFaker.Generate();
            var locacao = _locacaoFaker.Generate();
            locacaoInput.Plano = 7;
            locacao.ValorTotal = locacaoInput.Plano * 30; 

            var locacaoOutput = new LocacaoOutput
            {
                Identificador = locacao.Identificador,
                ValorDiaria = 30m
            };

            _mapperMock.Map<Locacao>(locacaoInput).Returns(locacao);
            _mapperMock.Map<LocacaoOutput>(locacao).Returns(locacaoOutput);

            // Act
            var result = await _locacaoService.CreateLocacaoAsync(locacaoInput);

            // Assert
            result.Should().NotBeNull();
            result.ValorDiaria.Should().Be(30m); 

            _locacaoRepositoryMock.Received(1).Add(locacao);
        }

        [Fact]
        public async Task CreateLocacaoAsync_ShouldThrowException_WhenInvalidDateRange()
        {
            // Arrange
            var locacaoInput = _locacaoInputFaker.Generate();
            locacaoInput.DataTermino = DateTime.Now.AddDays(-1); 

            // Act
            Func<Task> act = async () => await _locacaoService.CreateLocacaoAsync(locacaoInput);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Ocorreu um erro ao criar a locação. Por favor, tente novamente.");
        }


        [Fact]
        public async Task FindByIdAsync_ShouldReturnLocacaoOutput_WhenLocacaoExists()
        {
            // Arrange
            var locacao = _locacaoFaker.Generate();
            locacao.Plano = 7;
            var locacaoOutput = new LocacaoOutput
            {
                Identificador = locacao.Identificador,
                ValorDiaria = 30m
            };

            _locacaoRepositoryMock.FindByIdentificadorAsync(locacao.Identificador).Returns(Task.FromResult(locacao));
            _mapperMock.Map<LocacaoOutput>(locacao).Returns(locacaoOutput);

            // Act
            var result = await _locacaoService.FindByIdAsync(locacao.Identificador);

            // Assert
            result.Should().NotBeNull();
            result.Identificador.Should().Be(locacao.Identificador);
            result.ValorDiaria.Should().Be(30m);
        }

        [Fact]
        public async Task FindByIdAsync_ShouldThrowException_WhenLocacaoNotFound()
        {
            // Arrange
            var locacaoId = Guid.NewGuid().ToString();
            _locacaoRepositoryMock.FindByIdentificadorAsync(locacaoId).Returns(Task.FromResult<Locacao>(null));

            // Act
            Func<Task> act = async () => await _locacaoService.FindByIdAsync(locacaoId);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("Locação não encontrada.");
        }
    }

}
