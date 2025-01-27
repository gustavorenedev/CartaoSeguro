using AutoMapper;
using CartaoSeguro.Application.Card.Request;
using CartaoSeguro.Application.Card.Response;
using CartaoSeguro.Application.Card.Service;
using CartaoSeguro.Application.MessagePublisher.Service.Interface;
using CartaoSeguro.Domain.Card.Interface;
using CartaoSeguro.Domain.Enum;
using CartaoSeguro.Domain.User;
using CartaoSeguro.Domain.User.Interface;
using Moq;

namespace CartaoSeguro.Tests.Application.Services.Card;

public class CardServiceTests
{
    private readonly Mock<ICardRepository> _mockCardRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IMessagePublisher> _mockMessagePublisher;
    private readonly CardService _cardService;

    public CardServiceTests()
    {
        _mockCardRepository = new Mock<ICardRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockMessagePublisher = new Mock<IMessagePublisher>();

        _cardService = new CardService(_mockCardRepository.Object, _mockMapper.Object, _mockUserRepository.Object, _mockMessagePublisher.Object);
    }

    [Fact(DisplayName = "FindCardsByUser - Should return cards when user exists")]
    [Trait("Category", "FindCardsByUser")]
    public async Task FindCardsByUser_ShouldReturnCards_WhenUserExists()
    {
        // Arrange
        var userRequest = new CardsByUserRequest { Email = "test@example.com" };
        var user = new Domain.User.User { Email = "test@example.com" };
        var cards = new List<Domain.Card.Card>
        {
            new Domain.Card.Card { Id = Guid.NewGuid(), Number = "1234-5678-9012-3456" }
        };

        _mockUserRepository
            .Setup(repo => repo.GetByEmailAsync(userRequest.Email))
            .ReturnsAsync(user);

        _mockCardRepository
            .Setup(repo => repo.GetCardsByUserAsync(userRequest.Email))
            .ReturnsAsync(cards);

        _mockMapper
            .Setup(mapper => mapper.Map<CardsByUserResponse>(cards))
            .Returns(new CardsByUserResponse { Cards = new List<CardResponse>
            {
                new CardResponse { Id = cards[0].Id, Number = cards[0].Number }
            }
            });

        // Act
        var result = await _cardService.FindCardsByUser(userRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(cards.Count, result.Cards!.Count);
    }

    [Fact(DisplayName = "FindCardsByUser - Should throw exception when user does not exist")]
    [Trait("Category", "FindCardsByUser")]
    public async Task FindCardsByUser_ShouldThrowKeyNotFoundException_WhenUserDoesNotExist()
    {
        // Arrange
        var userRequest = new CardsByUserRequest { Email = "notfound@example.com" };

        _mockUserRepository
            .Setup(repo => repo.GetByEmailAsync(userRequest.Email))
            .ReturnsAsync((Domain.User.User)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _cardService.FindCardsByUser(userRequest));

        Assert.Equal("User not found.", exception.Message);
        _mockUserRepository.Verify(repo => repo.GetByEmailAsync(userRequest.Email), Times.Once);
        _mockCardRepository.Verify(repo => repo.GetCardsByUserAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact(DisplayName = "BlockOrActiveUserCard - Should send email when card and user exist")]
    [Trait("Category", "BlockOrActiveUserCard")]
    public async Task BlockOrActiveUserCard_ShouldSendEmail_WhenCardAndUserExist()
    {
        // Arrange
        var request = new BlockOrActiveUserCardRequest
        {
            Email = "test@example.com",
            Number = "1234-5678-9012-3456",
            Act = Status.Blocked
        };

        var user = new Domain.User.User { Email = "test@example.com" };
        var card = new Domain.Card.Card { Number = "1234-5678-9012-3456", Status = Status.Active };

        _mockCardRepository
            .Setup(repo => repo.GetCardByNumberAsync(request.Number))
            .ReturnsAsync(card);

        _mockUserRepository
            .Setup(repo => repo.GetByEmailAsync(request.Email))
            .ReturnsAsync(user);

        _mockMessagePublisher
            .Setup(pub => pub.PublishAsync(It.IsAny<CardAndUserRequest>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _cardService.BlockOrActiveUserCard(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("An email has been sent confirming your request with a token that you will use to confirm the act.", result.Message);
        _mockCardRepository.Verify(repo => repo.GetCardByNumberAsync(request.Number), Times.Once);
        _mockMessagePublisher.Verify(pub => pub.PublishAsync(It.IsAny<CardAndUserRequest>()), Times.Once);
    }


    [Fact(DisplayName = "BlockOrActiveUserCard - Should throw exception when card does not exist")]
    [Trait("Category", "BlockOrActiveUserCard")]
    public async Task BlockOrActiveUserCard_ShouldThrowKeyNotFoundException_WhenCardDoesNotExist()
    {
        // Arrange
        var request = new BlockOrActiveUserCardRequest { Email = "test@example.com", Number = "1234-5678-9012-3456", Act = Status.Blocked };

        _mockCardRepository
            .Setup(repo => repo.GetCardByNumberAsync(request.Number))
            .ReturnsAsync((Domain.Card.Card)null!);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _cardService.BlockOrActiveUserCard(request));

        Assert.Equal("Card not found.", exception.Message);
    }

    [Fact(DisplayName = "FindCardById - Should return card details when card exists")]
    [Trait("Category", "FindCardById")]
    public async Task FindCardById_ShouldReturnCardDetails_WhenCardExists()
    {
        // Arrange
        var cardRequest = new CardByIdRequest { Id = "1234" };
        var card = new Domain.Card.Card
        {
            Id = Guid.NewGuid(),
            Number = "1234-5678-9012-3456",
            Type = TypeCard.Debit,
            CVV = 123,
            CardFlag = "VISA",
            ExpirationDate = DateTime.Now.AddYears(1),
            Status = Status.Inactive,
            UserId = "user123"
        };

        var cardResponse = new CardByIdResponse
        {
            Id = card.Id,
            Number = card.Number,
            Type = card.Type,
            CVV = card.CVV,
            CardFlag = card.CardFlag,
            ExpirationDate = card.ExpirationDate,
            Status = card.Status,
            UserId = card.UserId
        };

        _mockCardRepository
            .Setup(repo => repo.GetCardByIdAsync(cardRequest.Id))
            .ReturnsAsync(card);

        _mockMapper
            .Setup(mapper => mapper.Map<CardByIdResponse>(It.IsAny<Domain.Card.Card>()))
            .Returns(cardResponse);

        // Act
        var result = await _cardService.FindCardById(cardRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(cardResponse.Id, result.Id);
        Assert.Equal(cardResponse.Number, result.Number);
        Assert.Equal(cardResponse.Status, result.Status);
    }

    [Fact(DisplayName = "FindCardById - Should throw exception when card not found")]
    [Trait("Category", "FindCardById")]
    public async Task FindCardById_ShouldThrowException_WhenCardNotFound()
    {
        // Arrange
        var cardRequest = new CardByIdRequest { Id = "1234" };

        _mockCardRepository
            .Setup(repo => repo.GetCardByIdAsync(cardRequest.Id))
            .ReturnsAsync((Domain.Card.Card?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            await _cardService.FindCardById(cardRequest)
        );
        _mockCardRepository.Verify(repo => repo.GetCardByIdAsync(cardRequest.Id), Times.Once);
    }

    [Fact(DisplayName = "ConfirmedToken - Should block card when token is valid and action is Blocked")]
    [Trait("Category", "ConfirmedToken")]
    public async Task ConfirmedToken_ShouldBlockCard_WhenTokenIsValidAndActionIsBlocked()
    {
        // Arrange
        var validToken = 1234;

        typeof(CardService).GetField("tokenToUser", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
            ?.SetValue(null, validToken);

        typeof(CardService).GetField("actUser", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
            ?.SetValue(null, Domain.Enum.Status.Blocked);

        typeof(CardService).GetField("cardNumberToUser", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
            ?.SetValue(null, "1234-5678-9012-3456");

        _mockCardRepository
            .Setup(repo => repo.AlterStatusCard(It.IsAny<Domain.Enum.Status>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _cardService.ConfirmedToken(validToken.ToString());

        // Assert
        Assert.Equal("Card successfully blocked.", result);
        _mockCardRepository.Verify(repo => repo.AlterStatusCard(Domain.Enum.Status.Blocked, "1234-5678-9012-3456"), Times.Once);
    }

    [Fact(DisplayName = "ConfirmedToken - Should return 'Invalid token' when token is invalid")]
    [Trait("Category", "ConfirmedToken")]
    public async Task ConfirmedToken_ShouldReturnInvalidToken_WhenTokenIsInvalid()
    {
        // Arrange
        var invalidToken = "invalidToken";

        // Act
        var result = await _cardService.ConfirmedToken(invalidToken);

        // Assert
        Assert.Equal("Invalid token.", result);
        _mockCardRepository.Verify(repo => repo.AlterStatusCard(It.IsAny<Domain.Enum.Status>(), It.IsAny<string>()), Times.Never);
    }
}
