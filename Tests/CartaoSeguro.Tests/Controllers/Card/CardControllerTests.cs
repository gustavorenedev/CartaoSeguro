using CartaoSeguro.API.Controllers.Card;
using CartaoSeguro.Application.Card.Request;
using CartaoSeguro.Application.Card.Response;
using CartaoSeguro.Application.Card.Service.Interface;
using CartaoSeguro.Domain.Enum;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CartaoSeguro.Tests.Controllers.Card;

public class CardControllerTests
{
    private readonly Mock<ICardService> _mockCardService;
    private readonly CardController _cardController;

    public CardControllerTests()
    {
        _mockCardService = new Mock<ICardService>();

        _cardController = new CardController(_mockCardService.Object);
    }

    [Fact(DisplayName = "CreateCard - Should create card successfully")]
    [Trait("Category", "CardController")]
    public async Task CreateCard_ShouldCreateCardSuccessfully_WhenValidData()
    {
        // Arrange
        var cardRequest = new CreateCardRequest
        {
            UserId = "user123",
            Number = "1234567890123456",
            ExpirationDate = DateTime.Now.AddYears(1),
            CVV = 123
        };
        var cardResponse = new CreateCardResponse
        {
            Number = "1234567890123456"
        };

        _mockCardService.Setup(service => service.CreateCard(It.IsAny<CreateCardRequest>()))
            .ReturnsAsync(cardResponse);

        // Act
        var result = await _cardController.CreateCard(cardRequest);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        var response = Assert.IsType<CreateCardResponse>(createdAtActionResult.Value);
        Assert.Equal("1234567890123456", response.Number);
    }

    [Fact(DisplayName = "CreateCard - Should return BadRequest when invalid data is provided")]
    [Trait("Category", "CardController")]
    public async Task CreateCard_ShouldReturnBadRequest_WhenInvalidData()
    {
        // Arrange
        var cardRequest = new CreateCardRequest
        {
            UserId = "",
            Number = "",
            ExpirationDate = DateTime.Now.AddYears(1),
            CVV = 123
        };

        _cardController.ModelState.AddModelError("UserId", "O campo 'UserId' é obrigatório.");
        _cardController.ModelState.AddModelError("CardFlag", "O campo 'CardFlag' é obrigatório.");

        // Act
        var result = await _cardController.CreateCard(cardRequest);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Dados inválidos.", badRequestResult.Value);
    }

    [Fact(DisplayName = "FindCardsByUser - Should return user's cards successfully")]
    [Trait("Category", "CardController")]
    public async Task FindCardsByUser_ShouldReturnUserCardsSuccessfully_WhenValidRequest()
    {
        // Arrange
        var userRequest = new CardsByUserRequest { Email = "user@example.com" };
        var cardsResponse = new CardsByUserResponse
        {
            Cards = new List<CardResponse>
            {
                new CardResponse { Number = "1234567890123456", Type = TypeCard.Credit }
            }
        };

        _mockCardService.Setup(service => service.FindCardsByUser(It.IsAny<CardsByUserRequest>()))
            .ReturnsAsync(cardsResponse);

        // Act
        var result = await _cardController.FindCardsByUser(userRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<CardsByUserResponse>(okResult.Value);
        Assert.Single(response.Cards);
    }

    [Fact(DisplayName = "FindCardsByUser - Should return BadRequest when invalid data is provided")]
    [Trait("Category", "CardController")]
    public async Task FindCardsByUser_ShouldReturnBadRequest_WhenInvalidData()
    {
        // Arrange
        var userRequest = new CardsByUserRequest { Email = "" };

        _cardController.ModelState.AddModelError("Email", "O campo 'Email' é obrigatório.");

        // Act
        var result = await _cardController.FindCardsByUser(userRequest);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Dados inválidos.", badRequestResult.Value);
    }

    [Fact(DisplayName = "FindCardById - Should return BadRequest when invalid id is provided")]
    [Trait("Category", "CardController")]
    public async Task FindCardById_ShouldReturnBadRequest_WhenInvalidId()
    {
        // Arrange
        var cardRequest = new CardByIdRequest { Id = "" };

        _cardController.ModelState.AddModelError("Id", "O campo 'Id' é obrigatório.");

        // Act
        var result = await _cardController.FindCardById(cardRequest);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Dados inválidos.", badRequestResult.Value);
    }

    [Fact(DisplayName = "BlockOrActiveUserCard - Should block or activate card successfully")]
    [Trait("Category", "CardController")]
    public async Task BlockOrActiveUserCard_ShouldBlockOrActivateCardSuccessfully_WhenValidData()
    {
        // Arrange
        var request = new BlockOrActiveUserCardRequest
        {
            Number = "1234567890123456",
            Email = "user@example.com",
            Act = Status.Active
        };
        var response = new BlockOrActiveUserCardResponse { Message = "Card successfully blocked." };

        _mockCardService.Setup(service => service.BlockOrActiveUserCard(It.IsAny<BlockOrActiveUserCardRequest>()))
            .ReturnsAsync(response);

        // Act
        var result = await _cardController.BlockOrActiveUserCard(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var resultResponse = Assert.IsType<BlockOrActiveUserCardResponse>(okResult.Value);
        Assert.Equal("Card successfully blocked.", resultResponse.Message);
    }

    [Fact(DisplayName = "BlockOrActiveUserCard - Should return BadRequest when invalid data is provided")]
    [Trait("Category", "CardController")]
    public async Task BlockOrActiveUserCard_ShouldReturnBadRequest_WhenInvalidData()
    {
        // Arrange
        var request = new BlockOrActiveUserCardRequest
        {
            Number = "",
            Email = "fasdfasdfa",
            Act = Status.Inactive
        };

        _cardController.ModelState.AddModelError("Number", "O campo 'Number' é obrigatório.");
        _cardController.ModelState.AddModelError("Email", "O e-mail não é válido.");

        // Act
        var result = await _cardController.BlockOrActiveUserCard(request);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Dados inválidos.", badRequestResult.Value);
    }

    [Fact(DisplayName = "ConfirmedToken - Should block or activate card based on token")]
    [Trait("Category", "CardController")]
    public async Task ConfirmedToken_ShouldBlockOrActivateCardSuccessfully_WhenValidToken()
    {
        // Arrange
        var token = "1234";
        var response = "Card successfully blocked.";

        _mockCardService.Setup(service => service.ConfirmedToken(It.IsAny<string>()))
            .ReturnsAsync(response);

        // Act
        var result = await _cardController.ConfirmedToken(token);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var resultResponse = Assert.IsType<string>(okResult.Value);
        Assert.Equal("Card successfully blocked.", resultResponse);
    }

    [Fact(DisplayName = "ConfirmedToken - Should return BadRequest when invalid token is provided")]
    [Trait("Category", "CardController")]
    public async Task ConfirmedToken_ShouldReturnBadRequest_WhenInvalidToken()
    {
        // Arrange
        var token = "";

        _cardController.ModelState.AddModelError("Token", "O token é inválido.");

        // Act
        var result = await _cardController.ConfirmedToken(token);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Dados inválidos.", badRequestResult.Value);
    }
}