using CartaoSeguro.Domain.User;
using CartaoSeguro.Domain.User.Interface;
using CartaoSeguro.Infrastructure.Persistence.DbContext;
using MongoDB.Driver;

namespace CartaoSeguro.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IApplicationDbContext _context;

    public UserRepository(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<User> AddAsync(User user)
    {
        await _context.Users.InsertOneAsync(user);
        return user;
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Email, email);
        return await _context.Users.Find(filter).FirstOrDefaultAsync();
    }
}
