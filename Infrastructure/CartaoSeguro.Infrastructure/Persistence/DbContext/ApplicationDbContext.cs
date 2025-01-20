using CartaoSeguro.Domain.Card;
using CartaoSeguro.Domain.User;
using MongoDB.Driver;

namespace CartaoSeguro.Infrastructure.Persistence.DbContext;

public class ApplicationDbContext : IApplicationDbContext
{
    private readonly IMongoDatabase _database;

    public ApplicationDbContext(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);

        _database = client.GetDatabase(databaseName);

        Cards = _database.GetCollection<Card>("Cards");
        Users = _database.GetCollection<User>("Users");
    }

    public IMongoCollection<Card> Cards { get; }
    public IMongoCollection<User> Users { get; }
}