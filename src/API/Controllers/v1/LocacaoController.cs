using API.Configurations.Attributes;
using API.DTOs.Requests;
using API.DTOs.Responses;
using AutoMapper;
using Domain.Interfaces.Services;
using Domain.Models.Inputs;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

[ApiController]
[ApiV1Route("[controller]")]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "Locacao")]
public class LocacaoController : ControllerBase
{
    private readonly ILocacaoService _locacaoService;
    private readonly IDevolucaoService _devolucaoService;
    private readonly IMapper _mapper;
    public LocacaoController(ILocacaoService locacaoService, IMapper mapper, IDevolucaoService devolucaoService)
    {
        _locacaoService = locacaoService;
        _mapper = mapper;
        _devolucaoService = devolucaoService;
    }

    /// <summary>
    /// Aluga uma moto para o entregador.
    /// </summary>
    /// <param name="request">Os dados necessários para realizar a locação da moto.</param>
    /// <returns>Um objeto LocacaoResponse contendo os detalhes da locação criada.</returns>
    /// <response code="201">Retorna os detalhes da locação criada com sucesso.</response>
    /// <response code="400">Dados inválidos fornecidos pelo usuário.</response>
    [HttpPost]
    [ProducesResponseType(typeof(LocacaoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AlugarMoto([FromBody] LocacaoRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ErrorResponse { Message = "Dados inválidos." });

        var locacaoInput = _mapper.Map<LocacaoInput>(request);

        var locacao = await _locacaoService.CreateLocacaoAsync(locacaoInput);
        var locacaoResponse = _mapper.Map<LocacaoResponse>(locacao);

        return CreatedAtAction(nameof(AlugarMoto), new { id = locacaoResponse.Identificador }, locacaoResponse);
    }

    /// <summary>
    /// Retorna os detalhes de uma locação pelo identificador.
    /// </summary>
    /// <param name="identificadorLocacao">O identificador da locação (obrigatório).</param>
    /// <returns>Um objeto LocacaoResponse contendo os detalhes da locação.</returns>
    /// <response code="200">Retorna os detalhes da locação.</response>
    /// <response code="400">Identificador da locação inválido ou ausente.</response>
    /// <response code="404">Locação não encontrada.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(LocacaoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLocacaoById([FromRoute][Required] string identificadorLocacao)
    {
        if (string.IsNullOrEmpty(identificadorLocacao))
        {
            return BadRequest(new ErrorResponse { Message = "O identificador da locação é obrigatório." });
        }

        var locacao = await _locacaoService.FindByIdAsync(identificadorLocacao);
        if (locacao == null)
        {
            return NotFound(new ErrorResponse { Message = "Locação não encontrada." });
        }

        var locacaoResponse = _mapper.Map<LocacaoResponse>(locacao);
        return Ok(locacaoResponse);
    }

    /// <summary>
    /// Processa a devolução de uma moto alugada e calcula o valor total, incluindo multas, se aplicável.
    /// </summary>
    /// <param name="identificadorLocacao">Identificador único da locação.</param>
    /// <param name="dataDevolucao">Data de devolução informada pelo entregador.</param>
    /// <returns>Retorna os detalhes da devolução, incluindo o valor total e multas, se houver.</returns>
    /// <response code="200">Devolução processada com sucesso. Retorna os detalhes da locação e o valor total a ser pago.</response>
    /// <response code="404">Locação não encontrada com o identificador fornecido.</response>
    [HttpPost("{identificadorLocacao}/devolucao")]
    [ProducesResponseType(typeof(DevolucaoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DevolverMoto(string identificadorLocacao, [FromBody] DateTime dataDevolucao)
    {
        try
        {
            var devolucaoOutput = await _devolucaoService.ProcessarDevolucaoAsync(identificadorLocacao, dataDevolucao);

            var locacaoResponse = _mapper.Map<DevolucaoResponse>(devolucaoOutput);

            return Ok(locacaoResponse);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ErrorResponse { Message = ex.Message });
        }
    }
}
