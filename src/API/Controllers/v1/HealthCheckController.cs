using API.Configurations.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.v1
{
    /// <summary>
    /// Verificações de integridade da API v1
    /// </summary>
    [ApiController]
    [ApiV1Route("[controller]")]
    [Produces("application/json")]
    public class HealthCheckController : ControllerBase
    {
        /// <summary>
        /// HealthCheck
        /// </summary>
        /// <remarks>
        /// Obtém o status de integridade da API v1.
        /// </remarks>
        /// <response code="200">Retorna uma mensagem "Healthy!"</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult HealthCheck()
        {
            return Ok("Healthy!");
        }
    }
}
