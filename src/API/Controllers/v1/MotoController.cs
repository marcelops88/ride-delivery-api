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
    [ApiExplorerSettings(GroupName = "Motos")]
    public class MotoController : ControllerBase
    {
        private readonly IMotoService _motoService;
        private readonly IMapper _mapper;

        public MotoController(IMotoService motoService, IMapper mapper)
        {
            _motoService = motoService;
            _mapper = mapper;
        }

        /// <summary>
        /// Cadastrar uma nova moto.
        /// </summary>
        /// <param name="request">Os dados da moto a serem cadastrados.</param>
        /// <returns>Um status de sucesso ou erro.</returns>
        /// <response code="201">Moto cadastrada com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        /// <response code="500">Erro interno do servidor.</response>
        [HttpPost]
        [ProducesResponseType(typeof(MotoResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CadastrarMoto([FromBody] MotoRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorResponse { Message = "Dados inválidos" });

            var motoInput = _mapper.Map<MotoInput>(request);

            var motoOutput = await _motoService.CreateMotoAsync(motoInput);
            var motoResponse = _mapper.Map<MotoResponse>(motoOutput);

            return CreatedAtAction(nameof(CadastrarMoto), new { id = motoResponse.Identificador }, motoResponse);
        }

        /// <summary>
        /// Consultar motos existentes.
        /// </summary>
        /// <returns>Lista de motos cadastradas.</returns>
        /// <response code="200">Retorna a lista de motos cadastradas.</response>
        /// <response code="404">Se nenhuma moto for encontrada.</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<MotoResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public IActionResult ConsultarMotos()
        {
            var motos = _motoService.GetAllMotos();

            if (motos == null || !motos.Any())
            {
                return NotFound(new ErrorResponse { Message = "Nenhuma moto encontrada." });
            }

            var motosResponse = _mapper.Map<List<MotoResponse>>(motos);

            return Ok(motosResponse);
        }

        /// <summary>
        /// Modificar a placa de uma moto.
        /// </summary>
        /// <param name="identificador">Identificador da moto a ser modificada.</param>
        /// <param name="request">Um objeto contendo a nova placa da moto.</param>
        /// <returns>Status de sucesso ou erro.</returns>
        /// <response code="200">Placa da moto modificada com sucesso.</response>
        /// <response code="404">Moto não encontrada.</response>
        [HttpPut("{identificador}/placa")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ModificarPlacaMoto([FromRoute] string identificador, [FromBody] ModificarPlacaRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorResponse { Message = "Dados inválidos" });

            var moto = await _motoService.GetMotoByIdAsync(identificador);

            if (moto == null)
                return NotFound("Moto não encontrada.");

            await _motoService.UpdateMotoPlateAsync(identificador, request.NovaPlaca);

            return Ok();
        }

        /// <summary>
        /// Consultar uma moto existente pelo Identificador.
        /// </summary>
        /// <param name="identificador">Identificador da moto a ser consultada.</param>
        /// <returns>Dados da moto consultada.</returns>
        /// <response code="200">Dados da moto encontrada.</response>
        /// <response code="404">Moto não encontrada.</response>
        [HttpGet("{identificador}")]
        [ProducesResponseType(typeof(MotoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ConsultarMotoPorId(string identificador)
        {
            var moto = await _motoService.GetMotoByIdAsync(identificador);

            if (moto == null)
            {
                return NotFound("Moto não encontrada.");
            }
            var motoResponse = _mapper.Map<MotoResponse>(moto);

            return Ok(motoResponse);
        }

        /// <summary>
        /// Remover uma moto.
        /// </summary>
        /// <param name="identificador">Identificador da moto a ser removida.</param>
        /// <returns>Status de sucesso ou erro.</returns>
        /// <response code="204">Moto removida com sucesso.</response>
        /// <response code="404">Moto não encontrada.</response>
        [HttpDelete("{identificador}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoverMoto(string identificador)
        {
            var moto = await _motoService.GetMotoByIdAsync(identificador);

            if (moto == null)
            {
                return NotFound("Moto não encontrada.");
            }

            await _motoService.DeleteMotoAsync(identificador);
            return NoContent();
        }

    }
}
