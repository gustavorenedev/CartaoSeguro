using CartaoSeguro.Application.Card.Request;
using CartaoSeguro.Application.Card.Response;
using CartaoSeguro.Application.Card.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CartaoSeguro.API.Controllers.Card;

/// <summary>
/// Controlador para registro, bloqueio, desbloqueio de cartões.
/// </summary>
[Route("api/[controller]")]
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
}
