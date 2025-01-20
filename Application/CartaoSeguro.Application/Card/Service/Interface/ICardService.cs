using CartaoSeguro.Application.Card.Request;
using CartaoSeguro.Application.Card.Response;

namespace CartaoSeguro.Application.Card.Service.Interface;

public interface ICardService
{
    Task<CreateCardResponse> CreateCard(CreateCardRequest card);
}
