namespace CartaoSeguro.Domain.Card.Interface;

public interface ICardRepository
{
    Task<Card> CreateCardAsync(Card card);
    Task<List<Card>> GetCardsByUserAsync(string email);
    Task<string> DoesCardNumberExistAsync(string cardNumber);
}