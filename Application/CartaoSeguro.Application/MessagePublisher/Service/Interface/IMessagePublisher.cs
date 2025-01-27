using CartaoSeguro.Application.Card.Request;

namespace CartaoSeguro.Application.MessagePublisher.Service.Interface;

public interface IMessagePublisher
{
    Task PublishAsync(CardAndUserRequest request);
}
