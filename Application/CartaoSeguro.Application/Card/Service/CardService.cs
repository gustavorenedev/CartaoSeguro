using AutoMapper;
using CartaoSeguro.Application.Card.Request;
using CartaoSeguro.Application.Card.Response;
using CartaoSeguro.Application.Card.Service.Interface;
using CartaoSeguro.Domain.Card.Interface;
using System.Text;

namespace CartaoSeguro.Application.Card.Service;

public class CardService : ICardService
{
    private readonly ICardRepository _cardRepository;
    private readonly IMapper _mapper;

    public CardService(ICardRepository cardRepository, IMapper mapper)
    {
        _cardRepository = cardRepository;
        _mapper = mapper;
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
