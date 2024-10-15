using API.Configurations.Attributes;
using API.DTOs.Requests;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace API.Controllers.v1
{
    [ApiController]
    [ApiV1Route("[controller]")]
    [Produces("application/json")]
    public class MotoController : ControllerBase
    {
        private readonly IRepository<Moto> _motoRepository;

        public MotoController(IRepository<Moto> motoRepository)
        {
            _motoRepository = motoRepository;
        }

        /// <summary>
        /// Cadastrar uma nova moto.
        /// </summary>
        /// <param name="request">Os dados da moto a serem cadastrados.</param>
        /// <returns>Um status de sucesso ou erro.</returns>
        [HttpPost]
        public IActionResult CadastrarMoto([FromBody] MotoRequest request)
        {
            var moto = new Moto
            {
                Identificador = request.Identificador,
                Ano = request.Ano.ToString(),
                Modelo = request.Modelo,
                Placa = request.Placa
            };

            _motoRepository.Add(moto);

            // Publicar evento de moto cadastrada (mensageria aqui)

            return CreatedAtAction(nameof(ConsultarMotoPorId), new { id = moto.Id }, moto);
        }

        /// <summary>
        /// Consultar motos existentes.
        /// </summary>
        /// <returns>Lista de motos cadastradas.</returns>
        [HttpGet]
        public IActionResult ConsultarMotos()
        {
            var motos = _motoRepository.GetAll().ToList();
            return Ok(motos);
        }

        /// <summary>
        /// Modificar a placa de uma moto.
        /// </summary>
        /// <param name="id">ID da moto a ser modificada.</param>
        /// <param name="request">Um objeto contendo a nova placa da moto.</param>
        /// <returns>Status de sucesso ou erro.</returns>
        [HttpPut("{id}/placa")]
        public IActionResult ModificarPlacaMoto([FromRoute] ObjectId id, [FromBody] ModificarPlacaRequest request)
        {
            var moto = _motoRepository.GetById(id);

            if (moto == null)
            {
                return NotFound("Moto não encontrada.");
            }

            moto.Placa = request.NovaPlaca;
            _motoRepository.Update(moto);

            return NoContent();
        }

        /// <summary>
        /// Consultar uma moto existente por ID.
        /// </summary>
        /// <param name="id">ID da moto a ser consultada.</param>
        /// <returns>Dados da moto consultada.</returns>
        [HttpGet("{id}")]
        public IActionResult ConsultarMotoPorId(ObjectId id)
        {
            var moto = _motoRepository.GetById(id);

            if (moto == null)
            {
                return NotFound("Moto não encontrada.");
            }

            return Ok(moto);
        }

        /// <summary>
        /// Remover uma moto.
        ///// </summary>
        /// <param name="id">ID da moto a ser removida.</param>
        /// <returns>Status de sucesso ou erro.</returns>
        [HttpDelete("{id}")]
        public IActionResult RemoverMoto(ObjectId id)
        {
            var moto = _motoRepository.GetById(id);

            if (moto == null)
            {
                return NotFound("Moto não encontrada.");
            }

            _motoRepository.Delete(id);
            return NoContent();
        }
    }
}
