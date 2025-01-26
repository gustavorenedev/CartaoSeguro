using AutoMapper;
using CartaoSeguro.Application.Card.Request;
using CartaoSeguro.Application.Card.Response;
using CartaoSeguro.Application.Card.Service.Interface;
using CartaoSeguro.Domain.Card.Interface;
using CartaoSeguro.Domain.User.Interface;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;

namespace CartaoSeguro.Application.Card.Service;

public class CardService : ICardService
{
    private readonly ICardRepository _cardRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly string _cardTopicName;
    private readonly string _kafkaBootstrapServers;

    public CardService(ICardRepository cardRepository, IMapper mapper, IUserRepository userRepository, IConfiguration configuration)
    {
        _cardRepository = cardRepository;
        _mapper = mapper;
        _userRepository = userRepository;
        _configuration = configuration;

        _cardTopicName = _configuration["CardSettings:CardTopicName"];
        _kafkaBootstrapServers = _configuration["CardSettings:KafkaBootstrapServer"];
    }

    public async Task<CardsByUserResponse> FindCardsByUser(CardsByUserRequest userRequest)
    {
        var user = await _userRepository.GetByEmailAsync(userRequest.Email!);

        if (user != null)
        {
            var cards = await _cardRepository.GetCardsByUserAsync(userRequest.Email!) ?? new List<Domain.Card.Card>();

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
        var card = await _cardRepository.GetCardByIdAsync(cardRequest.Id!);

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

    public async Task<BlockUserCardResponse> BlockUserCard(BlockUserCardRequest request)
    {
        var card = await _cardRepository.GetCardByNumberAsync(request.Number!);
        var user = await _userRepository.GetByEmailAsync(request.Email!);

        if (card == null)
            throw new KeyNotFoundException("Card not found.");

        if (user == null)
            throw new KeyNotFoundException("User not found.");

        var cardAndUser = new CardAndUserRequest
        {
            Card = card,
            User = user
        };

        await PublishMessageToTopic(cardAndUser);
        return new BlockUserCardResponse { Message = "An email has been sent confirming your request with a token that you will use to confirm the block." };
    }

    private async Task PublishMessageToTopic(CardAndUserRequest request)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = _kafkaBootstrapServers
        };

        string cardAndUserConvertedToJson = JsonSerializer.Serialize(request);

        using (var producer = new ProducerBuilder<Null, string>(config).Build())
        {
            var cardReport = await producer.ProduceAsync(_cardTopicName, new Message<Null, string> { Value = cardAndUserConvertedToJson });
        }
    }
}
