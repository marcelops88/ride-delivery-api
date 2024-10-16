using API.Configurations.Attributes;
using API.DTOs.Requests;
using API.DTOs.Responses;
using AutoMapper;
using Domain.Interfaces.Services;
using Domain.Models.Inputs;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.v1
{
    [ApiController]
    [ApiV1Route("[controller]")]
    [Produces("application/json")]
    [ApiExplorerSettings(GroupName = "Entregadores")]
    public class EntregadorController : ControllerBase
    {
        private readonly IEntregadorService _entregadorService;
        private readonly IMapper _mapper;

        public EntregadorController(IEntregadorService entregadorService, IMapper mapper)
        {
            _entregadorService = entregadorService;
            _mapper = mapper;
        }

        /// <summary>
        /// Cadastra um novo entregador na plataforma.
        /// </summary>
        /// <param name="request">Os dados do entregador a serem cadastrados.</param>
        /// <returns>Status de sucesso ou erro.</returns>
        /// <response code="201">Entregador cadastrado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        [HttpPost]
        [ProducesResponseType(typeof(EntregadorResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CadastrarEntregador([FromBody] EntregadorRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorResponse { Message = "Dados inválidos." });

            var entregadorInput = _mapper.Map<EntregadorInput>(request);
            var entregadorOutput = await _entregadorService.CreateEntregadorAsync(entregadorInput);
            var entregadorResponse = _mapper.Map<EntregadorResponse>(entregadorOutput);

            return CreatedAtAction(nameof(CadastrarEntregador), new { id = entregadorResponse.Id }, entregadorResponse);
        }

        /// <summary>
        /// Envia a imagem da CNH de um entregador para atualização de seu cadastro.
        /// O formato da imagem deve ser PNG ou BMP.
        /// A imagem será armazenada localmente, e o caminho será salvo no cadastro do entregador.
        /// </summary>
        /// <param name="identificador">O identificador único do entregador.</param>
        /// <param name="UpdateImagemCNHRequest">Objeto que contém a imagem da CNH em formato Base64. O formato aceito é PNG ou BMP.</param>
        /// <returns>Status de sucesso ou erro.</returns>
        /// <response code="204">Imagem da CNH enviada e armazenada com sucesso.</response>
        /// <response code="400">Nenhuma imagem foi enviada ou o formato da imagem é inválido.</response>
        /// <response code="404">Entregador não encontrado.</response>
        /// <response code="500">Erro interno ao processar o arquivo.</response>
        [HttpPost("{identificador}/cnh")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> EnviarImagemCNH(string identificador, [FromBody] UpdateImagemCNHRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorResponse { Message = "Dados inválidos." });
            }

            try
            {
                await _entregadorService.UpdateImagemCNHAsync(identificador, request.Base64ImagemCNH);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse { Message = ex.Message });
            }
        }
    }
}
