using CartaoSeguro.Domain.Card;
using CartaoSeguro.Domain.User;
using MongoDB.Driver;

namespace CartaoSeguro.Infrastructure.Persistence.DbContext;

public interface IApplicationDbContext
{
    IMongoCollection<Card> Cards { get; }
    IMongoCollection<User> Users { get; }
}
