using AutoMapper;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Domain.Models.Inputs;
using Domain.Models.Outputs;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Tests.Domain.Services
{
    public class EntregadorServiceTests
    {
        private readonly IEntregadorRepository _entregadorRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<EntregadorService> _logger;
        private readonly IImagemService _imageService;
        private readonly EntregadorService _entregadorService;

        public EntregadorServiceTests()
        {
            _entregadorRepository = Substitute.For<IEntregadorRepository>();
            _mapper = Substitute.For<IMapper>();
            _logger = Substitute.For<ILogger<EntregadorService>>();
            _imageService = Substitute.For<IImagemService>();
            _entregadorService = new EntregadorService(_entregadorRepository, _mapper, _logger, _imageService);
        }

        [Fact]
        public async Task CreateEntregadorAsync_ShouldThrowErrorWhenCnpjExists()
        {
            // Arrange
            var entregadorInput = new EntregadorInput { Cnpj = "12345678000195" };
            _entregadorRepository.FindByCnpjAsync(entregadorInput.Cnpj).Returns(new Entregador());

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _entregadorService.CreateEntregadorAsync(entregadorInput));
            Assert.Equal("Já existe um entregador cadastrado com esse CNPJ.", ex.Message);
        }

        [Fact]
        public async Task CreateEntregadorAsync_ShouldThrowErrorWhenNumeroCnhExists()
        {
            // Arrange
            var entregadorInput = new EntregadorInput { NumeroCNH = "12345678900" };
            _entregadorRepository.FindByCnpjAsync(Arg.Any<string>()).Returns((Entregador)null);
            _entregadorRepository.FindByNumeroCnhAsync(entregadorInput.NumeroCNH).Returns(new Entregador());

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _entregadorService.CreateEntregadorAsync(entregadorInput));
            Assert.Equal("Já existe um entregador cadastrado com esse número de CNH.", ex.Message);
        }

        [Fact]
        public async Task CreateEntregadorAsync_ShouldCreateEntregadorSuccessfully()
        {
            // Arrange
            var entregadorInput = new EntregadorInput { Cnpj = "12345678000195", NumeroCNH = "12345678900" };
            _entregadorRepository.FindByCnpjAsync(Arg.Any<string>()).Returns((Entregador)null);
            _entregadorRepository.FindByNumeroCnhAsync(Arg.Any<string>()).Returns((Entregador)null);

            var entregador = new Entregador { Identificador = "1", CNPJ = entregadorInput.Cnpj, NumeroCNH = entregadorInput.NumeroCNH };
            _mapper.Map<Entregador>(entregadorInput).Returns(entregador);
            _mapper.Map<EntregadorOutput>(entregador).Returns(new EntregadorOutput { Identificador = entregador.Identificador });

            // Act
            var result = await _entregadorService.CreateEntregadorAsync(entregadorInput);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(entregador.Identificador, result.Identificador);
             _entregadorRepository.Received(1).Add(entregador);
        }

        [Fact]
        public async Task UpdateImagemCNHAsync_ShouldThrowErrorWhenEntregadorNotFound()
        {
            // Arrange
            var identificador = "1";
            _entregadorRepository.FindByIdentificadorAsync(identificador).Returns((Entregador)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _entregadorService.UpdateImagemCNHAsync(identificador, "base64Image"));
            Assert.Equal("Entregador não encontrado.", ex.Message);
        }

        [Fact]
        public async Task UpdateImagemCNHAsync_ShouldUpdateImagemCNHSuccessfully()
        {
            // Arrange
            var identificador = "1";
            var entregador = new Entregador { Identificador = identificador };
            _entregadorRepository.FindByIdentificadorAsync(identificador).Returns(entregador);

            var caminhoImagem = "caminho/para/imagem.jpg";
            _imageService.SalvarImagemCNHAsync(identificador, Arg.Any<string>()).Returns(caminhoImagem);

            // Act
            await _entregadorService.UpdateImagemCNHAsync(identificador, "base64Image");

            // Assert
            Assert.Equal(caminhoImagem, entregador.ImagemCNH);
             _entregadorRepository.Received(1).Update(entregador);
        }
    }

}
