using API.Configurations.Attributes;
using API.DTOs.Requests;
using AutoMapper;
using Domain.Interfaces.Services;
using Domain.Models.Inputs;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace API.Controllers.v1
{
    [ApiController]
    [ApiV1Route("[controller]")]
    [Produces("application/json")]
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
        [HttpPost]
        public async Task<IActionResult> CadastrarMoto([FromBody] MotoRequest request)
        {
            var motoInput = _mapper.Map<MotoInput>(request);

            await _motoService.CreateMotoAsync(motoInput);

            return CreatedAtAction(nameof(ConsultarMotoPorId), new { id = motoInput.Identificador }, motoInput);
        }

        /// <summary>
        /// Consultar motos existentes.
        /// </summary>
        /// <returns>Lista de motos cadastradas.</returns>
        [HttpGet]
        public async Task<IActionResult> ConsultarMotos()
        {
            var motos = await _motoService.GetAllMotosAsync();
            return Ok(motos);
        }

        /// <summary>
        /// Modificar a placa de uma moto.
        /// </summary>
        /// <param name="id">ID da moto a ser modificada.</param>
        /// <param name="request">Um objeto contendo a nova placa da moto.</param>
        /// <returns>Status de sucesso ou erro.</returns>
        [HttpPut("{id}/placa")]
        public async Task<IActionResult> ModificarPlacaMoto([FromRoute] ObjectId id, [FromBody] ModificarPlacaRequest request)
        {
            var moto = await _motoService.GetMotoByIdAsync(id);

            if (moto == null)
            {
                return NotFound("Moto não encontrada.");
            }

            await _motoService.UpdateMotoPlateAsync(id, request.NovaPlaca);

            return NoContent();
        }

        /// <summary>
        /// Consultar uma moto existente por ID.
        /// </summary>
        /// <param name="id">ID da moto a ser consultada.</param>
        /// <returns>Dados da moto consultada.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> ConsultarMotoPorId(ObjectId id)
        {
            var moto = await _motoService.GetMotoByIdAsync(id);

            if (moto == null)
            {
                return NotFound("Moto não encontrada.");
            }

            return Ok(moto);
        }

        /// <summary>
        /// Remover uma moto.
        /// </summary>
        /// <param name="id">ID da moto a ser removida.</param>
        /// <returns>Status de sucesso ou erro.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoverMoto(ObjectId id)
        {
            var moto = await _motoService.GetMotoByIdAsync(id);

            if (moto == null)
            {
                return NotFound("Moto não encontrada.");
            }

            await _motoService.DeleteMotoAsync(id);
            return NoContent();
        }
    }
}
