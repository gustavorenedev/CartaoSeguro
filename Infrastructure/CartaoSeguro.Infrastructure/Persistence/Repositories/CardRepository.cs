using CartaoSeguro.Domain.Card;
using CartaoSeguro.Domain.Card.Interface;
using CartaoSeguro.Domain.Enum;
using CartaoSeguro.Domain.User;
using CartaoSeguro.Infrastructure.Persistence.DbContext;
using MongoDB.Driver;

namespace CartaoSeguro.Infrastructure.Persistence.Repositories;

public class CardRepository : ICardRepository
{
    private readonly IApplicationDbContext _context;

    public CardRepository(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AlterStatusCard(Status? act, string cardNumber)
    {
        var update = Builders<Card>.Update.Set(c => c.Status, act);
        await _context.Cards.UpdateOneAsync(c => c.Number == cardNumber, update);
    }

    public async Task<Card> CreateCardAsync(Card card)
    {
        var user = await _context.Users.Find(u => u.Id == Guid.Parse(card.UserId!)).FirstOrDefaultAsync();

        card.Id = Guid.NewGuid();
        card.UserId = user.Id.ToString();

        await _context.Cards.InsertOneAsync(card);

        var initializeCardsField = Builders<User>.Update.Set(u => u.Cards, user.Cards ?? new List<string>());
        await _context.Users.UpdateOneAsync(u => u.Id == user.Id, initializeCardsField);

        var update = Builders<User>.Update.Push(u => u.Cards, card.Id.ToString());
        await _context.Users.UpdateOneAsync(u => u.Id == user.Id, update);

        return card;
    }

    public async Task<string> DoesCardNumberExistAsync(string cardNumber)
    {
        var existCard = await _context.Cards.Find(c => c.Number == cardNumber).FirstOrDefaultAsync();

        return existCard != null ? existCard.Number : null;
    }

    public async Task<Card> GetCardByIdAsync(string id)
    {
        var existCard = await _context.Cards.Find(c => c.Id == Guid.Parse(id)).FirstOrDefaultAsync();

        return existCard;
    }

    public async Task<Card> GetCardByNumberAsync(string cardNumber)
    {
        var existCard = await _context.Cards.Find(c => c.Number == cardNumber).FirstOrDefaultAsync();

        return existCard;
    }

    public async Task<List<Card>> GetCardsByUserAsync(string email)
    {
        var user = await _context.Users.Find(u => u.Email == email).FirstOrDefaultAsync();

        var cards = await _context.Cards.Find(c => user.Cards!.Contains(c.Id.ToString())).ToListAsync();

        return cards;
    }
}
