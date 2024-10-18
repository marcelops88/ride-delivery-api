using Bogus;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using NSubstitute;

namespace Tests.Domain.Services
{
    public class DevolucaoServiceTests
    {
        private readonly ILocacaoRepository _locacaoRepository;
        private readonly ILogger<DevolucaoService> _logger;
        private readonly DevolucaoService _devolucaoService;
        private readonly Faker _faker;

        public DevolucaoServiceTests()
        {
            _locacaoRepository = Substitute.For<ILocacaoRepository>();
            _logger = Substitute.For<ILogger<DevolucaoService>>();

            _devolucaoService = new DevolucaoService(_locacaoRepository, _logger);

            _faker = new Faker("pt_BR");
        }

        [Fact]
        public async Task ProcessarDevolucaoAsync_ShouldReturnSuccessfulDevolucao_WithLateReturn()
        {
            var locacao = new Locacao
            {
                Id = ObjectId.GenerateNewId(),
                DataInicio = DateTime.Now.AddDays(-10), 
                DataPrevisaoTermino = DateTime.Now.AddDays(-5),
                Plano = 7 
            };

            var expectedMulta = 50M; 

            var locacaoRepository = Substitute.For<ILocacaoRepository>();
            locacaoRepository.GetById(Arg.Any<ObjectId>()).Returns(locacao);

            var logger = Substitute.For<ILogger<DevolucaoService>>();

            var devolucaoService = new DevolucaoService(locacaoRepository, logger);

            // Act
            var result = await devolucaoService.ProcessarDevolucaoAsync(locacao.Id.ToString(), DateTime.Now.AddDays(-4)); 

            // Assert
            result.Multa.Should().Be(expectedMulta); 
            result.ValorTotal.Should().BeGreaterThan(0); 
        }

        [Fact]
        public async Task ProcessarDevolucaoAsync_ShouldThrowErrorWhenLocacaoNotFound()
        {
            // Arrange
            var logger = Substitute.For<ILogger<DevolucaoService>>();
            var locacaoRepository = Substitute.For<ILocacaoRepository>();

            // Ajuste para ObjectId
            locacaoRepository.GetById(Arg.Any<ObjectId>()).Returns((Locacao)null);

            var service = new DevolucaoService(locacaoRepository, logger);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                service.ProcessarDevolucaoAsync(ObjectId.GenerateNewId().ToString(), DateTime.Now));
        }

        private Locacao GenerateFakeLocacao()
        {
            return new Faker<Locacao>("pt_BR")
                .RuleFor(l => l.Id, f => ObjectId.GenerateNewId())
                .RuleFor(l => l.Plano, f => f.PickRandom(new[] { 7, 15, 30, 45, 50 }))
                .RuleFor(l => l.DataInicio, f => f.Date.Past(1))
                .RuleFor(l => l.DataPrevisaoTermino, (f, l) => l.DataInicio.AddDays(l.Plano))
                .Generate();
        }
    }

}
