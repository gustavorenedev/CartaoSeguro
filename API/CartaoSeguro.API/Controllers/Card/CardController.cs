using CartaoSeguro.Application.Card.Request;
using CartaoSeguro.Application.Card.Response;
using CartaoSeguro.Application.Card.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CartaoSeguro.API.Controllers.Card;

/// <summary>
/// Controlador para registro, bloqueio, desbloqueio de cartões.
/// </summary>
[Route("api/[controller]")]
[Authorize]
[ApiController]
public class CardController : ControllerBase
{
    private readonly ICardService _cardService;

    /// <summary>
    /// Construtor do CardController.
    /// </summary>
    /// <param name="cardService">Serviço de registro, bloqueio, desbloqueio de cartões.</param>
    public CardController(ICardService cardService)
    {
        _cardService = cardService;
    }

    /// <summary>
    /// Endpoint para registrar um novo cartão.
    /// </summary>
    /// <param name="cardRequest">Dados do cartão e do usuário a serem registrados.</param>
    /// <returns>O Cartão criado com o ID gerado.</returns>
    /// <response code="201">Cartão criado com sucesso.</response>
    /// <response code="400">Erro de validação nos dados enviados.</response>
    [HttpPost("CreateCard")]
    [ProducesResponseType(typeof(CreateCardResponse), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CreateCard([FromBody] CreateCardRequest cardRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest("Dados inválidos.");

        var response = await _cardService.CreateCard(cardRequest);

        return CreatedAtAction(nameof(CreateCard), new { id = response.Number }, response);
    }

    /// <summary>
    /// Endpoint para buscar cartões do usuário.
    /// </summary>
    /// <param name="userRequest">Email do usuário a ser buscado.</param>
    /// <returns>Lista dos cartões vinculados ao usuário.</returns>
    /// <response code="200">Consulta realizada com sucesso.</response>
    /// <response code="400">Erro de validação nos dados enviados.</response>
    [HttpGet("FindCardsByUser")]
    [ProducesResponseType(typeof(CardsByUserResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> FindCardsByUser([FromQuery] CardsByUserRequest userRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest("Dados inválidos.");

        var response = await _cardService.FindCardsByUser(userRequest);

        return Ok(response);
    }

    /// <summary>
    /// Endpoint para buscar cartão pelo id.
    /// </summary>
    /// <param name="cardRequest">Id do cartão a ser buscado.</param>
    /// <returns>Cartão pelo id.</returns>
    /// <response code="200">Consulta realizada com sucesso.</response>
    /// <response code="400">Erro de validação nos dados enviados.</response>
    [HttpGet("FindCardById")]
    [ProducesResponseType(typeof(CardByIdResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> FindCardById([FromBody] CardByIdRequest cardRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest("Dados inválidos.");

        var response = await _cardService.FindCardById(cardRequest);

        return Ok(response);
    }

    /// <summary>
    /// Endpoint para solicitar o bloqueio ou ativação do cartão do usuário.
    /// </summary>
    /// <param name="request">Número do cartão e e-mail do usuário.</param>
    /// <returns>Bloqueio ou Ativação do cartão.</returns>
    /// <response code="200">Bloqueio ou Ativação realizado com sucesso.</response>
    /// <response code="400">Erro de validação nos dados enviados.</response>
    [HttpPost("BlockOrActiveUserCard")]
    [ProducesResponseType(typeof(BlockOrActiveUserCardResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> BlockOrActiveUserCard([FromBody] BlockOrActiveUserCardRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Dados inválidos.");

        var response = await _cardService.BlockOrActiveUserCard(request);

        return Ok(response);
    }

    /// <summary>
    /// Endpoint para confirmar o token recebido.
    /// </summary>
    /// <param name="token">Número do token.</param>
    /// <returns>Bloqueio ou Desbloqueio do cartão.</returns>
    /// <response code="200">Bloqueio ou Desbloqueio realizado com sucesso.</response>
    /// <response code="400">Erro de validação nos dados enviados.</response>
    [HttpPost("ConfirmedToken")]
    [ProducesResponseType(typeof(BlockOrActiveUserCardResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> ConfirmedToken([FromQuery] string token)
    {
        if (!ModelState.IsValid)
            return BadRequest("Dados inválidos.");

        var response = await _cardService.ConfirmedToken(token);

        return Ok(response);
    }
}
