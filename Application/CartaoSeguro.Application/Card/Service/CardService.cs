using AutoMapper;
using CartaoSeguro.Application.Card.Request;
using CartaoSeguro.Application.Card.Response;
using CartaoSeguro.Application.Card.Service.Interface;
using CartaoSeguro.Domain.Card.Interface;
using CartaoSeguro.Domain.User.Interface;
using System.Text;

namespace CartaoSeguro.Application.Card.Service;

public class CardService : ICardService
{
    private readonly ICardRepository _cardRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public CardService(ICardRepository cardRepository, IMapper mapper, IUserRepository userRepository)
    {
        _cardRepository = cardRepository;
        _mapper = mapper;
        _userRepository = userRepository;
    }

    public async Task<CardsByUserResponse> FindCardsByUser(CardsByUserRequest userRequest)
    {
        if (string.IsNullOrEmpty(userRequest.Email))
            throw new InvalidOperationException("Email is required.");

        var user = await _userRepository.GetByEmailAsync(userRequest.Email);

        if (user != null)
        {
            var cards = await _cardRepository.GetCardsByUserAsync(userRequest.Email) ?? new List<Domain.Card.Card>();

            return _mapper.Map<CardsByUserResponse>(cards);
        }

        throw new KeyNotFoundException("User not found.");
    }

    public async Task<CreateCardResponse> CreateCard(CreateCardRequest card)
    {
        var newCard = new CreateCardRequest
        {
            Id = Guid.NewGuid(),
            Number = await GenerateCardNumber(),
            Type = card.Type,
            CVV = GenerateCVV(),
            CardFlag = card.CardFlag,
            ExpirationDate = GenerateExpirationDate(),
            Status = Domain.Enum.Status.Inactive,
            UserId = card.UserId
        };

        var createCard = await _cardRepository.CreateCardAsync(_mapper.Map<Domain.Card.Card>(newCard));

        return _mapper.Map<CreateCardResponse>(createCard);
    }

    public async Task<CardByIdResponse> FindCardById(CardByIdRequest cardRequest)
    {
        if (string.IsNullOrEmpty(cardRequest.Id))
            throw new InvalidOperationException("Id is required.");

        var card = await _cardRepository.GetCardByIdAsync(cardRequest.Id);

        if (card == null)
            throw new KeyNotFoundException("Card not found.");

        return _mapper.Map<CardByIdResponse>(card);
    }

    private async Task<string> GenerateCardNumber()
    {
        var random = new Random();
        string cardNumber;

        var cardNumberBuilder = new StringBuilder();
        for (var i = 0; i < 16; i++)
        {
            cardNumberBuilder.Append(random.Next(0, 10));
        }
        cardNumber = cardNumberBuilder.ToString();

        while (await DoesCardNumberExist(cardNumber))
        {
            cardNumberBuilder.Clear();
            for (var i = 0; i < 16; i++)
            {
                cardNumberBuilder.Append(random.Next(0, 10));
            }
            cardNumber = cardNumberBuilder.ToString();
        }

        return cardNumber;
    }

    private async Task<bool> DoesCardNumberExist(string cardNumber)
    {
        var card = await _cardRepository.DoesCardNumberExistAsync(cardNumber);
        if (card != null && !string.IsNullOrEmpty(card.ToString()))
        {
            return true;
        }
        return false;
    }

    private static int GenerateCVV()
    {
        var random = new Random();
        return random.Next(100, 999);
    }

    private static DateTime GenerateExpirationDate()
    {
        return DateTime.Now.AddYears(5);
    }
}
