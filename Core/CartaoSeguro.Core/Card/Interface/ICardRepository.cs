using CartaoSeguro.Domain.Enum;

namespace CartaoSeguro.Domain.Card.Interface;

public interface ICardRepository
{
    Task<Card> CreateCardAsync(Card card);
    Task<List<Card>> GetCardsByUserAsync(string email);
    Task<string> DoesCardNumberExistAsync(string cardNumber);
    Task<Card> GetCardByIdAsync(string id);
    Task<Card> GetCardByNumberAsync(string cardNumber);
    Task AlterStatusCard(Status? act, string cardNumber);
}